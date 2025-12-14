window.diagramInterop = (function () {
    let dotNetRef = null;
    let iframeId = null;
    let iframeReady = false;

    function init(dotNetObject, targetIframeId) {
        dotNetRef = dotNetObject;
        iframeId = targetIframeId;

        // Listen for messages from the iframe
        window.addEventListener("message", onMessageFromIframe);

        // Hook iframe load to know when it's ready
        const frame = document.getElementById(iframeId);
        if (frame) {
            // If iframe already loaded, mark ready; otherwise wait for load event.
            if (frame.contentWindow && frame.contentDocument.readyState !== 'complete') {
                frame.addEventListener("load", () => {
                    iframeReady = true;
                }, { once: true });
            } else {
                iframeReady = true;
            }
        }
    }

    function onMessageFromIframe(event) {
        // Optional: check event.origin for security if you know the expected origin
        const data = event.data;
        if (!data) return;

        if (data.action === "diagramData" && dotNetRef) {
            // call into Blazor
            dotNetRef.invokeMethodAsync("ReceiveDiagramData", data.data)
                .catch(err => console.error("Error invoking ReceiveDiagramData:", err));
        }
    }

    function requestDiagramData() {
        const frame = document.getElementById(iframeId);
        if (!frame || !frame.contentWindow) {
            console.warn("diagramInterop: iframe not found or no contentWindow");
            return false;
        }

        // If iframe not yet flagged ready, still attempt to postMessage — but return status
        frame.contentWindow.postMessage({ action: "saveDiagram" }, "*");
        return true;
    }

    function dispose() {
        window.removeEventListener("message", onMessageFromIframe);
        dotNetRef = null;
        iframeId = null;
        iframeReady = false;
    }

    return {
        init,
        requestDiagramData,
        dispose
    };
})();
