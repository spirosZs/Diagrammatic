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

    return { getDiagram, clearDiagram };
})();
