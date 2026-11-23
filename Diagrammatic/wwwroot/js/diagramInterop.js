// Bridge to the Syncfusion diagram editor embedded as a cross-origin iframe
// (Web_API /diagramEvaluation, message listener in newdiagram.js).
window.diagramInterop = (function () {

    // Asks the iframe for its current diagram (Syncfusion saveDiagram() JSON).
    // Resolves with the JSON string, or null if the iframe is missing or does
    // not answer within timeoutMs — callers treat null as "capture failed".
    function getDiagram(iframeId, timeoutMs) {
        return new Promise(function (resolve) {
            const frame = document.getElementById(iframeId);
            if (!frame || !frame.contentWindow) {
                resolve(null);
                return;
            }

            const timer = setTimeout(function () {
                window.removeEventListener("message", onMessage);
                resolve(null);
            }, timeoutMs || 3000);

            function onMessage(event) {
                const data = event.data;
                if (!data || data.action !== "diagramData") return;

                clearTimeout(timer);
                window.removeEventListener("message", onMessage);
                resolve(typeof data.data === "string" ? data.data : null);
            }

            window.addEventListener("message", onMessage);

            // targetOrigin "*": the editor's host/port differs per environment
            // and the request carries no sensitive data.
            frame.contentWindow.postMessage({ action: "saveDiagram" }, "*");
        });
    }

    // Wipes the editor canvas; used when a new round starts, because the
    // iframe element survives re-renders and keeps the previous drawing.
    function clearDiagram(iframeId) {
        const frame = document.getElementById(iframeId);
        if (frame && frame.contentWindow) {
            frame.contentWindow.postMessage({ action: "clearDiagram" }, "*");
        }
    }

    // Puts a saved drawing (Syncfusion saveDiagram() JSON) onto the canvas — how
    // a path exercise shows the class the teacher's diagram.
    //
    // The message is repeated until the editor acknowledges it, because there is
    // no way from out here to know when the page inside the iframe has finished
    // starting up: the frame's load event fires before Syncfusion has created the
    // diagram, and a message that arrives before then is simply dropped. Resolves
    // true once the editor confirms it loaded, false if it never answers or said
    // it could not parse the drawing.
    // Drawings waiting to go into a frame, by iframe id. An entry lives here from the
    // moment a load is asked for until that frame confirms it, so whichever side gets
    // there first, the diagram lands: the editor announces itself when it is ready
    // (below) and is answered from here, and until then the payload is re-sent on a
    // timer in case the announcement was made before this page was listening.
    const pending = new Map();

    // The editor inside the frame cannot be timed from out here — an iframe fires its
    // load event long before Syncfusion has built the canvas — so it tells us instead.
    window.addEventListener("message", function (event) {
        const data = event.data;
        if (!data || data.action !== "diagramViewerReady") return;

        pending.forEach(function (entry, iframeId) {
            const frame = document.getElementById(iframeId);
            if (frame && frame.contentWindow === event.source) entry.send();
        });
    });

    // `token` identifies the drawing (the round's exercise id). It is remembered on
    // the iframe element itself once the load succeeds, so calling this again for the
    // same round returns immediately — and a frame the browser has re-created (a new
    // round, or a Blazor circuit reconnect rebuilding the DOM) comes back without the
    // mark and is filled in again, which is what keeps a reconnected student from
    // staring at an empty canvas.
    //
    // Resolves true once the editor confirms it loaded, false if it never answers or
    // said it could not parse the drawing.
    function loadDiagram(iframeId, json, token, timeoutMs) {
        return new Promise(function (resolve) {
            if (!json) {
                resolve(false);
                return;
            }

            const already = document.getElementById(iframeId);
            if (already && token && already.dataset.dgLoadedToken === token) {
                resolve(true);
                return;
            }

            // Supersede anything still being sent to this frame — a new round's
            // drawing must not race the previous one onto the same canvas.
            const previous = pending.get(iframeId);
            if (previous) previous.cancel();

            let settled = false;

            function finish(ok) {
                if (settled) return;
                settled = true;
                clearInterval(ticker);
                clearTimeout(deadline);
                window.removeEventListener("message", onMessage);
                if (pending.get(iframeId) === entry) pending.delete(iframeId);

                const frame = document.getElementById(iframeId);
                if (ok && token && frame) frame.dataset.dgLoadedToken = token;

                // One line per round, so the console says which half of this went wrong
                // when a canvas comes up empty: nothing at all here means the page never
                // asked for the diagram.
                console.info("diagramInterop: '" + iframeId + "' load " +
                    (ok ? "confirmed" : "failed") + " (" + json.length + " chars).");

                resolve(ok);
            }

            function onMessage(event) {
                const data = event.data;
                if (!data || data.action !== "diagramLoaded") return;

                const frame = document.getElementById(iframeId);
                if (frame && frame.contentWindow !== event.source) return;

                finish(data.ok === true);
            }

            // The element is looked up on every attempt rather than captured once:
            // the host may ask for the load before the frame is in the page, and a
            // frame the browser re-creates is a different element with the same id.
            function send() {
                const frame = document.getElementById(iframeId);
                if (frame && frame.contentWindow) {
                    frame.contentWindow.postMessage({ action: "loadDiagram", data: json }, "*");
                }
            }

            const entry = { send: send, cancel: function () { finish(false); } };
            pending.set(iframeId, entry);

            window.addEventListener("message", onMessage);

            const ticker = setInterval(send, 300);
            const deadline = setTimeout(function () {
                console.error("diagramInterop: '" + iframeId + "' never confirmed the diagram.",
                    { frameInPage: !!document.getElementById(iframeId), chars: json.length });
                finish(false);
            }, timeoutMs || 15000);

            send();
        });
    }

    return { getDiagram, clearDiagram, loadDiagram };
})();
