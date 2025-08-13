using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Diagrams;

namespace Exercises.Controllers
{
    [Route("diagramEvaluation")]
    public class DiagramEvaluationController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            LoadDiagramAssets();
            return View("DiagramEvaluation");
        }

        private void LoadDiagramAssets()
        {
            var ports = new List<DiagramPort>
            {
                new DiagramPort
                {
                    Id = "portTop",
                    Shape = PortShapes.Circle,
                    Offset = new DiagramPoint { X = 0.5, Y = 0 },
                    Visibility = PortVisibility.Visible
                },
                new DiagramPort
                {
                    Id = "portRight",
                    Shape = PortShapes.Circle,
                    Offset = new DiagramPoint { X = 1, Y = 0.5 },
                    Visibility = PortVisibility.Visible
                },
                new DiagramPort
                {
                    Id = "portBottom",
                    Shape = PortShapes.Circle,
                    Offset = new DiagramPoint { X = 0.5, Y = 1 },
                    Visibility = PortVisibility.Visible
                },
                new DiagramPort
                {
                    Id = "portLeft",
                    Shape = PortShapes.Circle,
                    Offset = new DiagramPoint { X = 0, Y = 0.5 },
                    Visibility = PortVisibility.Visible
                }
            };

                    var basicNodes = new List<DiagramNode>
            {
                new DiagramNode
                {
                    Id = "Ellipse",
                    Shape = new DiagramBasicShape { Type = Shapes.Basic, Shape = BasicShapes.Ellipse },
                    Ports = ports
                },
                new DiagramNode
                {
                    Id = "Star",
                    Shape = new DiagramBasicShape { Type = Shapes.Basic, Shape = BasicShapes.Star },
                    Ports = ports
                }
            };

            ViewBag.BasicShapes = basicNodes;

            var palettes = new List<SymbolPalettePalette>
    {
        new SymbolPalettePalette { Id = "basic", Symbols = basicNodes, Title = "Symbols" }
    };
            ViewBag.palettes = palettes;

            var margin = new DiagramMargin { Left = 15, Bottom = 15, Right = 15, Top = 15 };
            ViewBag.margin = margin;

            ViewBag.getNodeDefaults = "getNodeDefaults";
            ViewBag.getSymbolInfo = "getSymbolInfo";
            ViewBag.valueChanged = "valueChanged";
        }

       /* private void LoadDiagramAssets()
        {
            var basicNodes = new List<DiagramNode>();
            basicNodes.Add(new DiagramNode
                {Id = "Ellipse", Shape = new DiagramBasicShape {Type = Shapes.Basic, Shape = BasicShapes.Ellipse}});
            basicNodes.Add(new DiagramNode
                {Id = "Star", Shape = new DiagramBasicShape {Type = Shapes.Basic, Shape = BasicShapes.Star}});
            ViewBag.BasicShapes = basicNodes;

            var palettes = new List<SymbolPalettePalette>();
            palettes.Add(new SymbolPalettePalette {Id = "basic", Symbols = basicNodes, Title = "Symbols"});
            ViewBag.palettes = palettes;


            var margin = new DiagramMargin {Left = 15, Bottom = 15, Right = 15, Top = 15};
            ViewBag.margin = margin;

            ViewBag.getNodeDefaults = "getNodeDefaults";
            ViewBag.getSymbolInfo = "getSymbolInfo";
            ViewBag.valueChanged = "valueChanged";
        }*/
    }
}