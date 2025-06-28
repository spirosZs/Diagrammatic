using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Exercises.Common;
using Exercises.Common.Abstractions;
using Exercises.Common.Exercise;
using Exercises.Data;
using Exercises.Data.Types;
using Exercises.Swagger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json.Linq;


namespace Exercises.Controllers
{
    /// <summary>
    /// An exercise is a part of an exercise collection. Each exercise contributes independently
    /// to the evaluation score of a students submission in an exam. An exercise can be identified by its type
    /// and for every Problem Type there is a specific exercise type.
    /// </summary>
    [Route(Router.Exercise)]
    public class ExerciseController : EntityController<Exercise, ExerciseFilter>
    {
        public ExerciseController(IServiceProvider serviceProvider, IHttpContextAccessor accessor,
            LinkGenerator generator,
            IPropertyMappingService propertyMappingService)
            : base(serviceProvider, accessor, generator, propertyMappingService)
        {
        }

        /// <summary>
        /// Get a collection of Exercises.
        /// </summary>
        /// <param name="filter">Query parameters to send to filter the returned results.</param>
        /// <param name="token"></param>
        /// <response code="200">Successful response.</response>
        /// <response code="400">Bad request.</response>
        [HttpGet(Name = "GetMultiple[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ExerciseDto>))]
        [ProducesResponseType(400)]
//        [SwaggerResponseHeader(200, "X-Pagination", "string", "Pagination metadata for this request.")]
        public virtual async Task<IActionResult> GetAsync([FromQuery] ExerciseFilter filter,
            CancellationToken token = default)
        {
            return await GetAsync<ExerciseDto>(filter, token);
        }

        /// <summary>
        /// Get a single Exercise.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <response code="200">Successful response.</response>
        /// <response code="404">Not found.</response>  
        [HttpGet("{id}", Name = "Get[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(ExerciseDto))]
        [ProducesResponseType(404)]
        public virtual async Task<IActionResult> GetAsync([FromRoute] Guid id, CancellationToken token = default)
        {
            return await GetAsync<ExerciseDto>(id, token);
        }

        /// <summary>
        /// Create an Exercise
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="token"></param>
        /// <response code="201">Returns the newly created Exercise.</response>
        /// <response code="400">Bad request.</response>  
        [HttpPost(Name = "Create[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(201, Type = typeof(ExerciseDto))]
        [ProducesResponseType(400)]
        [Consumes("application/json")]
        [ConsumesType(typeof(ExerciseCreateDto))]
        public virtual async Task<IActionResult> CreateAsync([FromBody] JObject payload,
            CancellationToken token = default)
        {
            return await CreateAsync<ExerciseDto>(payload, token);
        }

        /// <summary>
        /// Update an Exercise.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="payload"></param>
        /// <param name="token"></param>
        /// <response code="200">Returns the updated Exercise.</response>
        /// <response code="400">Bad request.</response>  
        /// <response code="404">Not found.</response>  
        [HttpPatch("{id}", Name = "Update[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(ExerciseDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Consumes("application/json")]
        [ConsumesType(typeof(JsonPatchDocumentSchema))]
        public virtual async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            [FromBody] JArray payload,
            CancellationToken token = default
        )
        {
            return await UpdateAsync<Exercise>(id, payload, token);
        }

        /// <summary>
        /// Delete an Exercise
        /// </summary>
        /// <param name="id">The id of the Exercise to delete.</param>
        /// <param name="token"></param>
        /// <response code="204">Successful response.</response>
        /// <response code="404">Not found.</response>  
        [HttpDelete("{id}", Name = "Delete[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken token = default)
        {
            return await DeleteAsync<ExerciseDto>(id, token);
        }


        protected override string GetEntityCreateType(JObject payload)
        {
            try
            {
                var problemType = payload.ContainsKey("problemType")
                    ? payload.GetValue("problemType").ToObject<ProblemType>()
                    : payload.GetValue("ProblemType").ToObject<ProblemType>();
                return problemType.ToString();
            }
            catch (Exception e)
            {
                ModelState
                    .AddModelError("problemType", "Problem type can not be empty.");
                return base.GetEntityCreateType(payload);
            }
        }

        protected override string GetEntityUpdateType(Exercise exercise)
        {
            return exercise.ProblemType.ToString();
        }
    }
}