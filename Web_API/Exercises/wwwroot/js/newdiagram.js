var diagram;
var editor = ace.edit("editor");
var annotation_ellipse_counter = 0;
var annotation_star_counter = 0;
var deleted_annotations_ellipse = [];
var deleted_annotations_star = [];

// Shared look of the drawing surface. Kept in one place so the palette symbols,
// the nodes dropped from it and the ones added by tapping all match.
var DG_NODE_SIZE = 64;
var DG_NODE_STYLE = { strokeWidth: 2.5, fill: '#eef2ff', strokeColor: '#4f46e5' };
var DG_STAR_STYLE = { strokeWidth: 2.5, fill: '#fef3c7', strokeColor: '#d97706' };
var DG_ANNOTATION_STYLE = { fontSize: 15, bold: true, color: '#1e293b', fontFamily: 'Inter, Segoe UI, sans-serif' };
var DG_CONNECTOR_COLOR = '#334155';
var DG_LOOSE_COLOR = '#dc2626';

function dgDiagram() {
    if (typeof diagram !== 'undefined' && diagram) return diagram;
    var el = document.getElementById('diagram');
    return (el && el.ej2_instances && el.ej2_instances[0]) ? el.ej2_instances[0] : null;
}

// Highlights the tool that is currently armed (the toolbar is the only place
// that shows which mode the canvas is in).
function dgSetActiveTool(id) {
    $('.dg-toolbar .active').removeClass('active');
    if (id) $('#' + id).addClass('active');
}

// Arms the connector tools. Draws one arrow and falls back to Select, which is
// what you want on a touch screen: staying in draw mode turns every attempt to
// nudge a shape into a stray arrow.
$('.connectors').click(function (args) {
    var d = dgDiagram();
    if (!d) return;

    var drawingObject;
    switch (args.currentTarget.id) {
        case 'straight':
            drawingObject = { type: 'Straight' };
            break;
        case 'cubic':
            drawingObject = { type: 'Bezier' };
            break;
    }

    if (drawingObject) {
        d.drawingObject = drawingObject;
        d.tool = ej.diagrams.DiagramTools.DrawOnce;
        d.dataBind();
    }
});

$('.tools_select').click(function () {
    dgSetActiveTool($(this).attr('id'));
});

$('.toolbarClick').click(function(){
    toolbarClick($(this));
});

$('#next_button').click(function () {

    var check = checkConnectivity();
    if (check) {
        getPaths();
        console.log('next page');
        $('#next_button').hide();
        $('#step3_buttons').show();        
        $('.step1_2').hide();
        $('.step3').show();
        $('#diagram-column-div').removeClass('col-md-8').addClass('col-md-4');
        diagram.height = 400;
        var DiagramConstraints = ej.diagrams.DiagramConstraints;
        diagram.constraints = DiagramConstraints.Default & ~DiagramConstraints.PageEditable;
        diagram.dataBind();
        diagram.clearSelection();
        diagram.fitToPage();
    }
    else
    {
        $('#warningModal').modal('show');
    }

    
});
$('#back_button').click(function () {

        console.log('back page');
        $('#next_button').show();
        $('#step3_buttons').hide();
        $('.step1_2').show();
        $('.step3').hide();
        $('#diagram-column-div').removeClass('col-md-4').addClass('col-md-8');
        diagram.height = 540;
        var DiagramConstraints = ej.diagrams.DiagramConstraints;
        diagram.constraints = DiagramConstraints.Default | DiagramConstraints.PageEditable;
        diagram.dataBind();
        diagram.fitToPage();
});
function diagramCreated()
{
    diagram = document.getElementById("diagram").ej2_instances[0];

    // Soft grid; the old settings painted a solid grey wash over the canvas.
    diagram.snapSettings = {
        constraints: ej.diagrams.SnapConstraints.All,
        snapAngle: 5,
        horizontalGridlines: { lineColor: '#e8edf4', lineIntervals: [1, 19, 0.5, 19.5] },
        verticalGridlines: { lineColor: '#e8edf4', lineIntervals: [1, 19, 0.5, 19.5] }
    };

    diagram.connectorDefaults = function (connector) {
        connector.sourceDecorator = { shape: 'None' };
        connector.targetDecorator = { shape: 'Arrow', style: { fill: DG_CONNECTOR_COLOR, strokeColor: DG_CONNECTOR_COLOR } };
        connector.style = { strokeWidth: 2, strokeColor: DG_CONNECTOR_COLOR };
        return connector;
    };

    // Start in select mode (the toolbar shows Select as active). Previously the
    // canvas started in draw mode without anything to draw.
    diagram.tool = ej.diagrams.DiagramTools.SingleSelect | ej.diagrams.DiagramTools.MultipleSelect;
    diagram.dataBind();

    // Deferred: 'created' fires inside the render pass, and dataBind() must not
    // re-enter it.
    dgRequestSize();
    dgWatchContainerSize();
    dgEnableTapToAdd();
}

function onChangeData(args)
{
    var d = dgDiagram();
    if (!d) return;
    d.tool = args.checked ? ej.diagrams.DiagramTools.ContinuousDraw : ej.diagrams.DiagramTools.DrawOnce;
    d.dataBind();
}

function getNodeDefaults(symbol)
{
    symbol.width  = DG_NODE_SIZE;
    symbol.height = DG_NODE_SIZE;
    symbol.style  = (symbol.id == 'Star') ? DG_STAR_STYLE : DG_NODE_STYLE;

    symbol.constraints &= ~(ej.diagrams.NodeConstraints.Resize | ej.diagrams.NodeConstraints.Rotate);
}

function getSymbolInfo(symbol)
{

    return { fit: true };
}

// ---------------------------------------------------------------------------
// Responsive canvas
// ---------------------------------------------------------------------------
// The editor is embedded in an iframe whose height follows the phone's
// viewport, so the diagram has to be measured from its container rather than
// pinned to a fixed height.
var _dgSizeFrame = null;

function dgSizeToContainer()
{
    var host = document.getElementById('diagram-canvas');
    var d = dgDiagram();
    if (!host || !d) return;

    var w = Math.round(host.clientWidth);
    var h = Math.round(host.clientHeight);
    if (w < 40 || h < 40) return;          // hidden / still laying out
    if (d.width === w && d.height === h) return;

    d.width = w;
    d.height = h;
    d.dataBind();
}

function dgRequestSize()
{
    if (_dgSizeFrame) cancelAnimationFrame(_dgSizeFrame);
    _dgSizeFrame = requestAnimationFrame(function () {
        _dgSizeFrame = null;
        dgSizeToContainer();
    });
}

function dgWatchContainerSize()
{
    var host = document.getElementById('diagram-canvas');

    if (host && typeof ResizeObserver !== 'undefined') {
        new ResizeObserver(dgRequestSize).observe(host);
    }

    window.addEventListener('resize', dgRequestSize);
    window.addEventListener('orientationchange', function () {
        // iOS reports the new viewport a beat after the event.
        setTimeout(dgRequestSize, 250);
    });
}

// ---------------------------------------------------------------------------
// Shape numbering
// ---------------------------------------------------------------------------
// Every node carries a number (its annotation) and the score is computed from
// those, so numbers freed by a deletion are handed back out before new ones.
// Single source of truth for both ways of adding a shape: drag and tap.
function dgNextAnnotation(kind)
{
    if (kind === 'Star') {
        if (deleted_annotations_star.length > 0) return deleted_annotations_star.pop();
        annotation_star_counter = annotation_star_counter + 1;
        return annotation_star_counter;
    }

    if (deleted_annotations_ellipse.length > 0) return deleted_annotations_ellipse.pop();
    annotation_ellipse_counter = annotation_ellipse_counter + 1;
    return annotation_ellipse_counter;
}

// Dragging a 64px symbol onto an exact spot is fiddly on a phone, so tapping a
// symbol drops it in the middle of the visible canvas instead.
var _dgTapCount = 0;
var _dgLastDrop = 0;

function dgEnableTapToAdd()
{
    var palette = document.getElementById('symbolpalette');
    if (!palette) return;

    var startX = 0, startY = 0, tracking = false, moved = false;

    palette.addEventListener('pointerdown', function (e) {
        startX = e.clientX; startY = e.clientY; tracking = true; moved = false;
    }, true);

    palette.addEventListener('pointermove', function (e) {
        if (!tracking) return;
        if (Math.abs(e.clientX - startX) > 8 || Math.abs(e.clientY - startY) > 8) moved = true;
    }, true);

    palette.addEventListener('click', function (e) {
        var wasDrag = moved;
        tracking = false;
        moved = false;

        // A real drag ends with a drop on the canvas; don't add a second shape.
        if (wasDrag || (Date.now() - _dgLastDrop) < 500) return;

        var symbolId = dgFindSymbolId(e.target);
        if (symbolId) dgAddSymbolAtCenter(symbolId);
    });
}

function dgFindSymbolId(el)
{
    while (el && el !== document.body) {
        var id = el.id || '';
        if (/^Ellipse($|[_-])/.test(id)) return 'Ellipse';
        if (/^Star($|[_-])/.test(id)) return 'Star';
        el = el.parentElement;
    }
    return null;
}

function dgAddSymbolAtCenter(symbolId)
{
    var d = dgDiagram();
    if (!d) return;

    try {
        var isStar = symbolId === 'Star';

        // Middle of what the student can actually see, in diagram coordinates.
        var scroll = d.scrollSettings || {};
        var zoom = scroll.currentZoom || 1;
        var vw = scroll.viewPortWidth || d.element.clientWidth;
        var vh = scroll.viewPortHeight || d.element.clientHeight;
        var cx = (-(scroll.horizontalOffset || 0) + vw / 2) / zoom;
        var cy = (-(scroll.verticalOffset || 0) + vh / 2) / zoom;

        // Fan successive taps out so they don't stack on one another.
        var step = _dgTapCount++ % 6;
        cx += (step % 3) * (DG_NODE_SIZE + 16) - (DG_NODE_SIZE + 16);
        cy += Math.floor(step / 3) * (DG_NODE_SIZE + 16) - (DG_NODE_SIZE + 16) / 2;

        var ports = [
            { id: 'portTop', shape: 'Circle', offset: { x: 0.5, y: 0 }, visibility: ej.diagrams.PortVisibility.Visible },
            { id: 'portRight', shape: 'Circle', offset: { x: 1, y: 0.5 }, visibility: ej.diagrams.PortVisibility.Visible },
            { id: 'portBottom', shape: 'Circle', offset: { x: 0.5, y: 1 }, visibility: ej.diagrams.PortVisibility.Visible },
            { id: 'portLeft', shape: 'Circle', offset: { x: 0, y: 0.5 }, visibility: ej.diagrams.PortVisibility.Visible }
        ];

        d.add({
            id: symbolId + '_' + Date.now() + '_' + Math.floor(Math.random() * 1000),
            width: DG_NODE_SIZE,
            height: DG_NODE_SIZE,
            offsetX: cx,
            offsetY: cy,
            shape: { type: 'Basic', shape: isStar ? 'Star' : 'Ellipse' },
            style: isStar ? DG_STAR_STYLE : DG_NODE_STYLE,
            ports: ports,
            annotations: [{
                content: String(dgNextAnnotation(isStar ? 'Star' : 'Ellipse')),
                constraints: ej.diagrams.AnnotationConstraints.ReadOnly,
                style: DG_ANNOTATION_STYLE
            }],
            constraints: ej.diagrams.NodeConstraints.Default &
                ~(ej.diagrams.NodeConstraints.Resize | ej.diagrams.NodeConstraints.Rotate)
        });
    }
    catch (err) {
        // Tapping is a convenience; dragging still works if this ever fails.
        console.warn('Could not add symbol by tap:', err);
    }
}

function toolbarClick(element)
{
    var option=$(element).attr('id');
    var diagram = dgDiagram();
    if (!diagram) return;

    switch (option) {
        case "Delete_Tool":
            // Wiping the board with one tap is easy to do by accident on a
            // phone, and the drawing is the student's answer.
            if (diagram.nodes.length + diagram.connectors.length > 0 &&
                !window.confirm('Clear the whole canvas? This removes every shape and arrow.')) {
                break;
            }
            diagram.clear();
            annotation_ellipse_counter = 0;
            annotation_star_counter = 0;
            deleted_annotations_ellipse = [];
            deleted_annotations_star = [];
            break;
        case "DeleteSelection_Tool":
            // Touch devices have no Delete key, so removing one shape needs a button.
            var selected = (diagram.selectedItems.nodes || []).slice()
                .concat((diagram.selectedItems.connectors || []).slice());
            for (var s = 0; s < selected.length; s++) {
                diagram.remove(selected[s]);
            }
            break;
        case "Upload_Tool":
            document.getElementsByClassName('e-file-select-wrap')[0].querySelector('button').click();
            break;
        case "Download_Tool":
            download(diagram.saveDiagram());
            break;
        case "Redo_Tool":
            diagram.redo();
            break;
        case "Undo_Tool":
            diagram.undo();
            break;
        case "FitToPage_Tool":
            diagram.fitToPage();
            break;
        case "ZoomIn_Tool":
            diagram.zoomTo({ type: 'ZoomIn', zoomFactor: 0.2 });
            break;
        case "ZoomOut_Tool":
            diagram.zoomTo({ type: 'ZoomOut', zoomFactor: 0.2 });
            break;
        case "Pan_Tool":
        case "MoveTool_Tool":
            var updateTool;
            if (option == "Pan_Tool") {
                updateTool = ej.diagrams.DiagramTools.ZoomPan;
            }
            else if (option == "MoveTool_Tool") {
                updateTool = ej.diagrams.DiagramTools.SingleSelect | ej.diagrams.DiagramTools.MultipleSelect;
            }
            diagram.drawingObject = undefined;
            diagram.tool = updateTool;
            diagram.dataBind();
            break;
    }
}


// Bridge for the Blazor app that embeds this editor in an iframe
// (diagramInterop.js on the parent side). 'saveDiagram' is answered with a
// 'diagramData' message carrying the serialized diagram; 'clearDiagram'
// resets the canvas between game rounds, since the iframe is not reloaded.
window.addEventListener('message', function (event)
{
    var msg = event.data;
    if (!msg || !msg.action) return;

    if (msg.action === 'saveDiagram')
    {
        if (diagram && event.source)
        {
            event.source.postMessage(
                { action: 'diagramData', data: diagram.saveDiagram() },
                event.origin);
        }
    }
    else if (msg.action === 'clearDiagram')
    {
        if (diagram)
        {
            diagram.clear();
            annotation_ellipse_counter = 0;
            annotation_star_counter = 0;
            deleted_annotations_ellipse = [];
            deleted_annotations_star = [];
        }
    }
});

function download(data)
{
    if (window.navigator.msSaveBlob) 
    {
        var blob = new Blob([data], { type: 'data:text/json;charset=utf-8,' });
        window.navigator.msSaveOrOpenBlob(blob, 'Diagram.json');
    } 
    else 
    {
        var dataStr = 'data:text/json;charset=utf-8,' + encodeURIComponent(data);
        var a = document.createElement('a');
        a.href = dataStr;
        a.download = 'Diagram.json';
        document.body.appendChild(a);
        a.click();
        a.remove();
    } 
}

function onUploadSuccess(args) 
{
    var file1 = args.file;
    var file = file1.rawFile;
    let reader = new FileReader();
    reader.readAsText(file);
    reader.onloadend = loadDiagram;
}

function loadDiagram(event) 
{
    diagram.loadDiagram((event.target).result);
}

function checkConnectivity(args) 
{    
    var all_connected = true;
    var default_color = "black";
    var alert_color   = "red";

    for (var i = 0; i < diagram.connectors.length; i++) {
        console.log(diagram.connectors[i].sourceID);
        if(diagram.connectors[i].sourceID=="" || diagram.connectors[i].targetID=="")
        {
            diagram.connectors[i].style.strokeColor                 = alert_color;
            diagram.connectors[i].targetDecorator.style.fill        = alert_color;
            diagram.connectors[i].targetDecorator.style.strokeColor = alert_color;
            all_connected = false;
        }
        else
        {
            diagram.connectors[i].style.strokeColor                 = default_color;
            diagram.connectors[i].targetDecorator.style.fill        = default_color;
            diagram.connectors[i].targetDecorator.style.strokeColor = default_color;
        }
    }

    /*for (var i = 0; i < diagram.nodes.length; i++) {
        console.log(diagram.nodes[i]);
        if (diagram.nodes[i].inEdges.length == 0 && diagram.nodes[i].outEdges.length == 0) {
            diagram.nodes[i].style.strokeColor = alert_color;
            all_connected = false;
        }
        else {
            diagram.nodes[i].style.strokeColor = default_color;
        }
        
    }*/
    diagram.dataBind();
    return all_connected;

}

function drop(args)
{
    if (args.element)
    {
        _dgLastDrop = Date.now();

        var droppedId = (args.element.oldProperties && args.element.oldProperties.id) ||
                        (args.element.shape && args.element.shape.shape) || '';
        var kind = (droppedId.indexOf('Star') === 0 || droppedId === 'Star') ? 'Star' : 'Ellipse';

        args.element.annotations = [{
            content: dgNextAnnotation(kind).toString(),
            constraints: ej.diagrams.AnnotationConstraints.ReadOnly,
            style: DG_ANNOTATION_STYLE
        }];
    }
}

function collectionChange(args)
{
    // A connector drawn with DrawOnce puts the canvas back into select mode;
    // keep the toolbar's highlight in step with it.
    if (args.state === 'Changed' && args.type === 'Addition' &&
        args.element && args.element.sourcePoint && args.element.targetPoint) {
        dgSetActiveTool('MoveTool_Tool');
    }

    // Shapes always carry their number; anything without one (a connector, or a
    // node still mid-drop) has nothing to bookkeep.
    if (!args.element || !args.element.shape ||
        !args.element.annotations || args.element.annotations.length === 0) {
        return;
    }

    if (args.state === 'Changed' && args.type === 'Removal')
    {
        if(args.element.shape.shape=='Ellipse')
        {
            deleted_annotations_ellipse.push(args.element.annotations[0].content);
            deleted_annotations_ellipse.sort(function(a, b){return b - a});
        }
        else if (args.element.shape.shape=='Star')
        {
            deleted_annotations_star.push(args.element.annotations[0].content);
            deleted_annotations_star.sort(function(a, b){return b - a});
        }

        
    }
    else if (args.state === 'Changed' && args.type === 'Addition')
    {
        if (args.element.shape.shape == 'Ellipse')
        {
            //Check if a node's annotation that is added exists in delete annotation array ,this will happen only on redo that adds a node
            //Otherwise adding a node is handled by drop function
            var added_annotation = args.element.annotations[0].content;
            if (deleted_annotations_ellipse.find(function (annotation) { return annotation == added_annotation }) == added_annotation)
            {
                var index = deleted_annotations_ellipse.indexOf(added_annotation);

                if (index > -1) {
                    deleted_annotations_ellipse.splice(index, 1);
                }
            }
            
            deleted_annotations_ellipse.sort(function (a, b) { return b - a });
        }
        else if (args.element.shape.shape == 'Star')
        {
            var added_annotation = args.element.annotations[0].content;
            if (deleted_annotations_star.find(function (annotation) { return annotation == added_annotation }) == added_annotation) {
                var index = deleted_annotations_star.indexOf(added_annotation);

                if (index > -1) {
                    deleted_annotations_star.splice(index, 1);
                }
            }

            deleted_annotations_star.sort(function (a, b) { return b - a });
        }
    }
}

var paths = [];
function getPaths()
{
    console.log(editor.getValue());
    console.log(diagram);
    $.ajax({
        type: "POST",
        url: '/Exercise/GetPaths/',
        //data: '{diagram:"' + diagram.saveDiagram() + '"}',//creates error
        data: '{diagram:"' + '' + '"}',
        success: function (response) {
            console.log(response);
            if (response != null && response != null)
            {
                var data = response;
                //we need to parse it to JSON 
                data = JSON.parse(data);
                console.log(data);
                paths = data;
                show_paths(data);
            }
        }
    }); 
}

$('#save_new_path').click(function () {
    var new_path = $('#new_path').val();
    console.log(new_path);
    var regex = /^(\d+,)+(\d+){1}$/;
    console.log(regex.test(new_path));
    if (regex.test(new_path))
    {
        var found = paths.find(function (element) {
            return element == new_path;
        });
        if (found)
        {
            $('#notvalid').text('Already exists!');
            $('#notvalid').show();
            $('#notvalid').delay(1500).fadeOut('fast');
        }
        else
        {
            console.log('valid');
            $('#new_path').val('');
            paths.push(new_path);
            $('#addPathModal').modal('hide');
            show_paths(paths);
        } 
    }
    else
    {
        console.log('NOT valid');
        $('#notvalid').text('Not valid!');
        $('#notvalid').show();
        $('#notvalid').delay(1500).fadeOut('fast');
    }
    
    console.log(paths);
});

function show_paths(data)
{
    var path;
    var blueprint = '<table class="table table-hover">\
                                    <thead>\
                                        <th> Path</th>\
                                        <th> Credits(%)</th>\
                                        <th></th>\
                                    </thead>\
                                    <tbody>';
    var value = 100 / data.length;
    for (var i = 0; i < data.length; i++) {
        path = data[i].replace(/,/g, ' <i class="fas fa-arrow-right"></i> ');

        blueprint += '<tr id="'+data[i]+'" class="path_row">\
                                    <td>' + path + '</td>\
                                    <td><input class="form-control" type="number" value="'+ value + '" style="width: 50%;"></td>\
                                    <td class="delete_path" data-path="'+ data[i] + '"><i class="far fa-trash-alt" style="color:red;"></i></td>\
                                  </tr > ';
    }

    blueprint += '<tr>\
                                    <td><button class="btn btn-secondary btn-sm btn-block" id="add_path"><i class="fas fa-plus"></i> Add new path </button></td>\
                                    <td></td>\
                                    <td></td>\
                                  </tr>';
    blueprint += '</table>';
    $('#diagram_paths').html(blueprint);

    $('.delete_path').click(function () {
        //Delete the path from the array with the paths
        var index = paths.indexOf($(this).data('path').toString());

        if (index > -1) {
            paths.splice(index, 1);
        }
        show_paths(paths);
    });

    $('#add_path').click(function () {
        $('#addPathModal').modal('show');
    });

    var highlight_color = 'orange';
    var default_color   = 'black';
    $('.path_row').mouseenter(function () {

        var path       = $(this).prop('id');
        var array_path = path.split(',');

        //Search for an Ellipse node by their annotatio
        for (var i=0; i < diagram.nodes.length; i++)
        {
            if (diagram.nodes[i].id.search('Ellipse')!=-1) // Paths are made only from Ellipse nodes
            {
                var found = array_path.find(function (element) {
                    return element == diagram.nodes[i].annotations[0].content;
                });
                if (found) {
                    diagram.nodes[i].style.strokeColor = highlight_color;
                }
            }
            
        }
    });

    $('tr').mouseleave(function () {
        for (var i = 0; i < diagram.nodes.length; i++)
        {
            diagram.nodes[i].style.strokeColor = default_color;
        }

    });
}

$('#editor-mode').select2();
$('#editor-mode').change(function(){
    var mode=$(this).val();
    console.log(editor);
    console.log(mode);
    editor.session.setMode(mode);
});

$('#editor').change(function () {
    console.log(editor.getValue());
});


//POINTS IN THE CODE
var code_points_counter = 0;
$('#new_point').click(function () {
    code_points_counter++;

    //Gets the selected text
    var range = editor.selection.getRange();
    var selectedContent = editor.getSelectedText();
    //Replace it with the same text including the start and end tags of the point
    editor.session.replace(range, '__start__' + code_points_counter + '__' + selectedContent + '__end__' + code_points_counter + '__');
});


$('#reset_points').click(function () {
    code_points_counter = 0;
    //Get the range of all the code
    editor.selectAll();
    var range = editor.selection.getRange();

    //replace/delete the start and end tags
    var code = editor.getValue();
    code = code.replace(/__\w*__/g, '');
    editor.session.replace(range,code);
});


// --- Live "loose connector" warning (game / diagram evaluation) -------------
// A connector whose source or target isn't snapped to a node is dropped when
// the answer is scored (the node it should link disappears from the graph),
// so the student silently loses points. While the round is live we highlight
// such connectors red and show a banner, giving them a chance to reattach
// before time runs out. Self-contained so it can't disturb the existing
// diagram event wiring; it only reacts when the set of loose connectors
// actually changes, so it never fights an in-progress drag.
(function () {
    var lastSignature = null;

    function getDiagram() {
        return dgDiagram();
    }

    function ensureBanner() {
        var el = document.getElementById('connectivityWarning');
        if (el) return el;
        el = document.createElement('div');
        el.id = 'connectivityWarning';
        el.className = 'dg-warning';           // styling lives in newdiagram.css
        el.setAttribute('role', 'status');
        document.body.appendChild(el);
        return el;
    }

    function refresh() {
        var d = getDiagram();
        if (!d || !d.connectors) return;

        var loose = [];
        for (var i = 0; i < d.connectors.length; i++) {
            var c = d.connectors[i];
            if (!c.sourceID || !c.targetID) loose.push(c.id);
        }

        // Only touch the diagram when the set of connectors changes, so we
        // don't re-bind (and disrupt drawing/selection) on every tick. The
        // count is part of the signature so a newly drawn arrow also picks up
        // the default styling below.
        var signature = d.connectors.length + '|' + loose.slice().sort().join(',');
        if (signature === lastSignature) return;
        lastSignature = signature;

        for (var j = 0; j < d.connectors.length; j++) {
            var conn = d.connectors[j];
            var color = (!conn.sourceID || !conn.targetID) ? DG_LOOSE_COLOR : DG_CONNECTOR_COLOR;
            if (conn.style) conn.style.strokeColor = color;
            if (conn.targetDecorator && conn.targetDecorator.style) {
                conn.targetDecorator.style.fill = color;
                conn.targetDecorator.style.strokeColor = color;
            }
        }
        d.dataBind();

        var banner = ensureBanner();
        if (loose.length > 0) {
            banner.textContent = '⚠ ' + loose.length +
                ' arrow(s) are not attached to a shape. Reconnect them or they will be ' +
                'ignored when your answer is scored.';
            banner.classList.add('is-visible');
        } else {
            banner.classList.remove('is-visible');
        }
    }

    setInterval(refresh, 800);
})();


