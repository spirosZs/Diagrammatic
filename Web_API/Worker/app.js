const fetch = require('node-fetch');

const HOST_NAME = 'web';
const HOST_URL = `http://${HOST_NAME}:8080`;

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

    let connection = new signalR.HubConnectionBuilder()
        .withUrl(HOST_URL + "/gameHub")
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

    // Reconnect loop
    function start() {
        connection.start()
            .catch(function (err) {
                setTimeout(function () {
                    start();
                }, 10);
            })
            .then(() => connection.invoke("ReceiveHello"));
    }

    connection.onclose(function () {
        start();
    });

    start();

    let timeouts = {};

    function updateTimers(examId) {
        fetchGameInfo(examId)
            .then((gameInfo) => {
                const {dateTimeToEnd, dateTimeToNextExercise, hasStarted} = gameInfo;
                if (!hasStarted) return;
                const a = new Date(dateTimeToEnd);
                const b = new Date(dateTimeToNextExercise);
                const c = new Date();
                if (typeof timeouts[examId] !== "undefined") {
                    clearTimeout(timeouts[examId].end);
                    clearTimeout(timeouts[examId].nextExercise);
                }
                const endsIn = a - c;
                const nextExerciseIn = b - c;

                timeouts[examId] = {};

                if (endsIn !== nextExerciseIn) {
                    console.log(`Game with id ${examId} will proceed to next exercise ${nextExerciseIn / 1000} seconds.`);
                    timeouts[examId].nextExercise = setTimeout(() => {
                        return connection.invoke("GoToNextExercise", examId)
                    }, nextExerciseIn);
                }

                console.log(`Game with id ${examId} will end in ${endsIn / 1000} seconds.`);
                timeouts[examId].end = setTimeout(() => {
                    return connection.invoke("EndGame", examId)
                }, endsIn);
            })
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
