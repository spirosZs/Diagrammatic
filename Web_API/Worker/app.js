const fetch = require('node-fetch');

const HOST_NAME = 'web';
const HOST_URL = `http://${HOST_NAME}:8080`;

// The game hub is authenticated: its methods change the round for every student in a
// game, so it no longer accepts anonymous callers. The worker owns no exam, so it
// identifies itself with the shared service key instead of a user token.
const SERVICE_API_KEY = process.env.SERVICE_API_KEY || '';
const HUB_URL = HOST_URL + '/gameHub?serviceKey=' + encodeURIComponent(SERVICE_API_KEY);

const EVENT_GAME_STARTED = 'Started';
const EVENT_GAME_UPDATED = 'Updated';
const EVENT_GAME_RESTARTED = 'Restarted';
const EVENT_GAME_ENDED = 'Ended';
const EVENT_EXERCISE_COMPLETED = 'ExerciseCompleted';
const EVENT_EXERCISE_SKIPPED = 'ExerciseSkipped';
const EVENT_EXERCISE_TIME_CHANGED = 'ExerciseTimeChanged';

ping();

function ping() {
    fetch(HOST_URL)
        .then(res => res.ok ? init() : repeat())
        .catch(repeat);
}

function fetchGameInfo(id) {
    function parseJSON(response) {
        return response.text().then(function (text) {
            console.log(text);
            return text ? JSON.parse(text) : {}
        })
    }

    return fetch(HOST_URL + '/api/game/' + id, {
        method: 'get',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
        }
    })
        .then(res => res.json());
}

function repeat() {
    console.log(`Cannot ping ${HOST_NAME}...`);
    setTimeout(ping, 2000);
}

function init() {
    const signalR = require("@aspnet/signalr");

    if (!SERVICE_API_KEY) {
        console.log('WARNING: SERVICE_API_KEY is not set. The game hub will reject this ' +
            'worker and rounds will only advance when a client reads the game state.');
    }

    let connection = new signalR.HubConnectionBuilder()
        .withUrl(HUB_URL)
        .build();

    connection.on("SayHello", message => {
        console.log(message);
    });

    connection.on("Notify", (event, payload) => {
        console.log('------------------------------------');
        console.log('Received event "' + event + '", with parameters:');
        console.log(payload);
        if (typeof handleEvent[event] !== "undefined") {
            handleEvent[event](JSON.parse(payload));
        }
        console.log('------------------------------------');

    });

    // Reconnect loop. The retry backs off instead of the old flat 10ms: a connection
    // that fails for a persistent reason (a rejected service key, say) would otherwise
    // spin as fast as the event loop allows and bury the API in negotiate requests.
    // Chaining .then() off .catch() also used to invoke ReceiveHello on a connection
    // that had just failed to start, so every retry raised an unhandled rejection too.
    let retryDelay = 1000;
    const MAX_RETRY_DELAY_MS = 30000;

    function start() {
        connection.start()
            .then(() => {
                retryDelay = 1000;
                return connection.invoke("ReceiveHello");
            })
            .catch(function (err) {
                console.log(`Cannot connect to the game hub (${err}); retrying in ${retryDelay / 1000}s.`);
                setTimeout(start, retryDelay);
                retryDelay = Math.min(retryDelay * 2, MAX_RETRY_DELAY_MS);
            });
    }

    connection.onclose(function () {
        start();
    });

    start();

    let timeouts = {};

    // A deadline that is already in the past must still be acted on, but never
    // faster than this. setTimeout treats a negative or NaN delay as "run now", so
    // an unreachable deadline used to turn the event -> updateTimers -> invoke ->
    // event cycle into a tight busy loop that hammered the API and flooded every
    // connected student with round-change events.
    const MIN_DELAY_MS = 250;

    function scheduleIn(ms) {
        return Number.isFinite(ms) ? Math.max(ms, MIN_DELAY_MS) : null;
    }

    function updateTimers(examId) {
        fetchGameInfo(examId)
            .then((gameInfo) => {
                const {dateTimeToEnd, dateTimeToNextExercise, hasStarted, hasEnded} = gameInfo;
                if (!hasStarted) return;

                if (typeof timeouts[examId] !== "undefined") {
                    clearTimeout(timeouts[examId].end);
                    clearTimeout(timeouts[examId].nextExercise);
                    delete timeouts[examId];
                }

                // A finished game has no deadlines left to watch (the API reports
                // dateTimeToEnd/dateTimeToNextExercise as null once it is over, which
                // would otherwise parse to Invalid Date and schedule NaN timers).
                if (hasEnded) {
                    console.log(`Game with id ${examId} has ended; no timers scheduled.`);
                    return;
                }

                const a = new Date(dateTimeToEnd);
                const b = new Date(dateTimeToNextExercise);
                const c = new Date();

                const endsIn = a - c;
                const nextExerciseIn = b - c;

                if (!Number.isFinite(endsIn) || !Number.isFinite(nextExerciseIn)) {
                    console.log(`Game with id ${examId} returned unusable deadlines; no timers scheduled.`);
                    return;
                }

                timeouts[examId] = {};

                // Equal deadlines mean the current round is the last one: it is the end
                // of the game that closes it out, not a round change.
                if (endsIn !== nextExerciseIn) {
                    const delay = scheduleIn(nextExerciseIn);
                    console.log(`Game with id ${examId} will proceed to next exercise in ${delay / 1000} seconds.`);
                    timeouts[examId].nextExercise = setTimeout(() => {
                        return connection.invoke("GoToNextExercise", examId)
                    }, delay);
                }

                const endDelay = scheduleIn(endsIn);
                console.log(`Game with id ${examId} will end in ${endDelay / 1000} seconds.`);
                timeouts[examId].end = setTimeout(() => {
                    return connection.invoke("EndGame", examId)
                }, endDelay);
            })
            .catch(err => console.log(`Could not update timers for game ${examId}: ${err}`))
    }

    const onGameStarted = (payload) => {
        const {examId} = payload;
        updateTimers(examId);
    };

    const onGameUpdated = (payload) => {
        const {examId} = payload;
        updateTimers(examId);
    };

    const onGameRestarted = (payload) => {
        const {examId} = payload;
        updateTimers(examId);
    };

    const onGameEnded = (payload) => {
        const {examId} = payload;
        if (typeof timeouts[examId] !== "undefined") {
            clearTimeout(timeouts[examId].end);
            clearTimeout(timeouts[examId].nextExercise);
            delete timeouts[examId];
            console.log(`Removed timeouts for game with id ${examId}.`);
        }
    };

    const onExerciseCompleted = (payload) => {
        const {examId} = payload;
        updateTimers(examId);
    };

    const onExerciseSkipped = (payload) => {
        const {examId} = payload;
        updateTimers(examId);
    };

    const onExerciseTimeChanged = (payload) => {
        const {examId} = payload;
        updateTimers(examId);
    };

    const handleEvent = {
        [EVENT_GAME_STARTED]: onGameStarted,
        [EVENT_GAME_UPDATED]: onGameUpdated,
        [EVENT_GAME_RESTARTED]: onGameRestarted,
        [EVENT_GAME_ENDED]: onGameEnded,
        [EVENT_EXERCISE_COMPLETED]: onExerciseCompleted,
        [EVENT_EXERCISE_SKIPPED]: onExerciseSkipped,
        [EVENT_EXERCISE_TIME_CHANGED]: onExerciseTimeChanged
    };
}
