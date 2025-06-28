var srcIframe = document.getElementById('srcIframe');
var destIframe = document.getElementById('destIframe');


$("#cloneBtn").click(function () {
    destIframe.contentWindow.diagram.loadDiagram(
        srcIframe.contentWindow.diagram.saveDiagram()
    );
});


$("#updateBtn").click(function () {
    const src = srcIframe.contentWindow.diagram.saveDiagram();
    const dest = destIframe.contentWindow.diagram.saveDiagram();

    $.ajax({
        type: "POST",
        contentType: 'application/json',
        url: '/api/diagram/compare',
        data: JSON.stringify({
            correct: JSON.parse(src),
            input: JSON.parse(dest)
        }),
        success: function (response) {
            $('#score').html(response);
        }
    });
});


