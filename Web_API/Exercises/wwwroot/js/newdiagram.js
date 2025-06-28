var diagram;
var editor = ace.edit("editor");
var annotation_ellipse_counter = 0;
var annotation_star_counter = 0;
var deleted_annotations_ellipse = [];
var deleted_annotations_star = [];

$('.connectors').click(function (args) {
    var target = args.currentTarget;
    switch (target.id) 
    {
        case 'straight':
            drawingObject = { type: 'Straight' };
            break;
        case 'cubic':
            drawingObject = { type: 'Bezier' };
            break;
    }
    if (drawingObject) 
    {
        diagram.drawingObject = drawingObject;
        var checkBoxObj = document.getElementById("checked").ej2_instances[0];
        diagram.tool = checkBoxObj.checked ? ej.diagrams.DiagramTools.ContinuousDraw : ej.diagrams.DiagramTools.DrawOnce;
        diagram.dataBind();
    }
});

$('.tools_select').click(function () {
    $('.active').each(function () {
        $(this).removeClass('active');
    });
    $(this).addClass('active');
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
    diagram.tool = ej.diagrams.DiagramTools.ContinuousDraw;
    diagram.dataBind();
}

function onChangeData(args) 
{
    console.log(diagram);
    diagram.tool = args.checked ? ej.diagrams.DiagramTools.ContinuousDraw : ej.diagrams.DiagramTools.DrawOnce;
}

function getNodeDefaults(symbol) 
{
    symbol.width  = 60;
    symbol.height = 60;
    if(symbol.id=='Star')
    {
        symbol.style  = { strokeWidth: 4 , fill:'red' };
    }
    else
    {
        symbol.style  = { strokeWidth: 4 , fill:'#5FB4DC' };
    }
    
    symbol.constraints &= ~(ej.diagrams.NodeConstraints.Resize | ej.diagrams.NodeConstraints.Rotate);
}

function getSymbolInfo(symbol) 
{
    
    return { fit: true };
}

function toolbarClick(element) 
{
    var option=$(element).attr('id');

    switch (option) {
        case "Delete_Tool":
            diagram.clear();
            annotation_ellipse_counter = 0;
            annotation_star_counter = 0;
            deleted_annotations_ellipse = [];
            deleted_annotations_star = [];
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
        case "Pan_Tool":
        case "MoveTool_Tool":
            var updateTool;
            //Enable The Tool Bar Visibility
            console.log(ej);
            if (option == "Pan_Tool") {
                updateTool = ej.diagrams.DiagramTools.ZoomPan;
            }
            else if (option == "MoveTool_Tool") {
                updateTool = ej.diagrams.DiagramTools.SingleSelect | ej.diagrams.DiagramTools.MultipleSelect;
            }
            diagram.tool = updateTool;
            break;
    }
}


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
    console.log(args);
    console.log('drop function');
    if (args.element) 
    {  
        var annotation;
        if(args.element.oldProperties.id == 'Ellipse')
        {
            if(deleted_annotations_ellipse.length == 0)
            {
                annotation_ellipse_counter = annotation_ellipse_counter+1;
                annotation                 = annotation_ellipse_counter;
            }
            else
            {
                annotation = deleted_annotations_ellipse.pop();
            }
        }
        else //Star
        {
            if(deleted_annotations_star.length == 0)
            {
                annotation_star_counter = annotation_star_counter+1;
                annotation              = annotation_star_counter;
            }
            else
            {
                annotation = deleted_annotations_star.pop();
            }
        }
        args.element.annotations = [{ content: annotation.toString(), constraints: ej.diagrams.AnnotationConstraints.ReadOnly}]; 
    }  
}

function collectionChange(args) 
{
    console.log('collectionChange');
    console.log(args.state);
    console.log(args.type);

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
    console.log(deleted_annotations_star);
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


