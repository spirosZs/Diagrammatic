using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Exercises.Common;
using Microsoft.AspNetCore.Mvc;
using Exercises.Common.Abstractions;
using Exercises.Common.Diagram;
using Exercises.Core.Helpers;
using Exercises.Data.DiagramDefinitions;

namespace Exercises.Controllers
{
    [Route(Router.Diagram)]
    public class DiagramsController : ControllerBase
    {
        private readonly IDiagramService _diagramService;

        public DiagramsController(IDiagramService diagramService)
        {
            _diagramService = diagramService;
        }

        /// <summary>
        /// Get Suggested Paths
        /// </summary>
        /// <remarks>
        /// Get a collection of paths that can be exported from a diagram definition.
        /// </remarks>
        /// <param name="diagramDefinitionDto">A json definition of a diagram.</param>
        /// <response code="200">Returns an array of all the paths paths.</response>
        /// <response code="400">Bad request.</response>  
        [HttpPost("suggested")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(ICollection<string>))]
        [ProducesResponseType(400)]
        [Consumes("application/json")]
        public async Task<IActionResult> GetSuggestedPaths(
            [FromBody] DiagramDefinitionDto diagramDefinitionDto
            )
        {
            var diagramDefinition = Mapper.Map<DiagramDefinition>(diagramDefinitionDto);
            var paths = _diagramService.GetSuggestedPaths(diagramDefinition);
            return Ok(paths);
        }

        /// <summary>
        /// Get diagram evaluation
        /// </summary>
        /// <remarks>
        /// This endpoint is used to test the evaluation algorithm.
        /// </remarks>
        /// <param name="compareDto">A json definition of a diagram.</param>
        /// <response code="200">Returns the score for this evaluation.</response>
        /// <response code="400">Bad request.</response>  
        [HttpPost("compare")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400)]
        [Consumes("application/json")]
        public async Task<IActionResult> Compare([FromBody] DiagramCompareDto compareDto)
        {
            var correct = Mapper.Map<DiagramDefinition>(compareDto.Correct);
            var input = Mapper.Map<DiagramDefinition>(compareDto.Input);


            var inputGraph = input.ToGraph();
            var correctGraph = correct.ToGraph();
            
            var result = Graph.CompareGraphs(inputGraph, correctGraph);
            return Ok(result);
        }
    }
}