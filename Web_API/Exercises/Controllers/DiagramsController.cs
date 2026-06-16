using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Exercises.Common;
using Exercises.Common.Abstractions;
using Exercises.Common.Diagram;
using Exercises.Core.Helpers;
using Exercises.Data;
using Exercises.Data.DiagramDefinitions;
using Exercises.Swagger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json.Linq;

namespace Exercises.Controllers
{
    [Route(Router.Diagram)]
    public class DiagramsController : EntityController<Diagram, DiagramFilter>
    {
        private readonly IDiagramService _diagramService;

        public DiagramsController(IServiceProvider serviceProvider, IHttpContextAccessor accessor,
            LinkGenerator generator, IPropertyMappingService propertyMappingService,
            IDiagramService diagramService)
            : base(serviceProvider, accessor, generator, propertyMappingService)
        {
            _diagramService = diagramService;
        }

        /// <summary>
        /// Get a single Diagram.
        /// </summary>
        /// <param name="id">The id of the Diagram to return.</param>
        /// <param name="token"></param>
        /// <response code="200">Successful response.</response>
        /// <response code="404">Not found.</response>
        [HttpGet("{id}", Name = "Get[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(DiagramDto))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid id, CancellationToken token = default)
        {
            return await GetAsync<DiagramDto>(id, token);
        }

        /// <summary>
        /// Create a Diagram.
        /// </summary>
        /// <remarks>
        /// Create a new diagram by providing its Name, Url and Definition.
        /// </remarks>
        /// <param name="payload"></param>
        /// <param name="token"></param>
        /// <response code="201">Returns the newly created Diagram.</response>
        /// <response code="400">Bad request.</response>
        [HttpPost(Name = "Create[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(201, Type = typeof(DiagramDto))]
        [ProducesResponseType(400)]
        [Consumes("application/json")]
        [ConsumesType(typeof(DiagramCreateDto))]
        public async Task<IActionResult> CreateAsync([FromBody] JObject payload,
            CancellationToken token = default)
        {
            return await CreateAsync<DiagramDto>(payload, token);
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
