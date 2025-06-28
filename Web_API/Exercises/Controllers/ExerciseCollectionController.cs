using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Exercises.Common;
using Exercises.Common.Abstractions;
using Exercises.Common.ExerciseCollection;
using Exercises.Data;
using Exercises.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json.Linq;


namespace Exercises.Controllers
{
    /// <summary>
    /// An Exercise Collection consists of a set of set of Exercises. This is used to make draft Exams by the teacher
    /// that will be later published.
    /// </summary>
    [Route(Router.ExerciseCollection)]
    [Authorize]
    public sealed class ExerciseCollectionController
        : EntityController<ExerciseCollection, ExerciseCollectionFilter>
    {
        
        public ExerciseCollectionController(IServiceProvider serviceProvider, IHttpContextAccessor accessor, LinkGenerator generator,
            IPropertyMappingService propertyMappingService)
            : base(serviceProvider, accessor, generator , propertyMappingService)
        {
        }

        /// <summary>
        /// Get a collection of Exercise Collections.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="filter">Query parameters to send to filter the returned results.</param>
        /// <param name="token"></param>
        /// <response code="200">Successful response.</response>
        /// <response code="400">Bad request.</response>
        [HttpGet(Name = "GetMultiple[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ExerciseCollectionFilter>))]
        [ProducesResponseType(400)]
//        [SwaggerResponseHeader(200, "X-Pagination", "string", "Pagination metadata for this request.")]
        public async Task<IActionResult> GetAsync([FromQuery] ExerciseCollectionFilter filter,
            CancellationToken token = default)
        {
            return await GetAsync<ExerciseCollectionDto>(filter, token);
        }


        /// <summary>
        /// Get a single Exercise Collection.
        /// </summary>
        /// <param name="id">The id of the Exercise Collection to return.</param>
        /// <param name="token"></param>
        /// <response code="200">Successful response.</response>
        /// <response code="404">Not found.</response>  
        [HttpGet("{id}", Name = "Get[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(ExerciseCollectionWithExercisesDto))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid id, CancellationToken token = default)
        {
            return await GetAsync<ExerciseCollectionWithExercisesDto>(id, token);
        }

        /// <summary>
        /// Create an Exercise Collection.
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="token"></param>
        /// <response code="201">Returns the newly created Exercise Collection.</response>
        /// <response code="400">Bad request.</response>  
        [HttpPost(Name = "Create[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(201, Type = typeof(ExerciseCollectionDto))]
        [ProducesResponseType(400)]
        [Consumes("application/json")]
        [ConsumesType(typeof(ExerciseCollectionCreateDto))]
        public async Task<IActionResult> CreateAsync([FromBody] JObject payload,
            CancellationToken token = default)
        {
            return await CreateAsync<ExerciseCollectionDto>(payload, token);
        }

        /// <summary>
        /// Add/Remove Exercise from Exercise Collection
        /// </summary>
        /// <param name="id">The id of the Exercise Collection to perform this operation.</param>
        /// <param name="exercisesOperation"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPatch("{id}/exercises")]
        [Produces("application/json")]
        [ProducesResponseType(201, Type = typeof(ExerciseCollectionWithExercisesDto))]
        [ProducesResponseType(400)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateAsync(
            [FromRoute] Guid id,
            [FromBody] ExerciseCollectionExercisesOperationDto exercisesOperation,
            CancellationToken token = default
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var collectionRepo = await _entityService.GetAsync(id, token);
            if (collectionRepo == null)
            {
                return NotFound();
            }

            switch (exercisesOperation.Operation)
            {
                case ExerciseCollectionOperation.AddExercise:
                    await _entityService.AddExerciseToCollection(id, exercisesOperation.ExerciseId);
                    break;
                case ExerciseCollectionOperation.RemoveExercise:
                    await _entityService.RemoveExerciseFromCollection(id, exercisesOperation.ExerciseId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(exercisesOperation));
            }

            var collection = Mapper.Map<ExerciseCollectionWithExercisesDto>(collectionRepo);
            return Ok(collection);
        }
        
        /// <summary>
        /// Update an Exercise Collection.
        /// </summary>
        /// <param name="id">The id of the Exercise Collection to update.</param>
        /// <param name="payload"></param>
        /// <param name="token"></param>
        /// <response code="200">Returns the updated Exercise Collection.</response>
        /// <response code="400">Bad request.</response>  
        /// <response code="404">Not found.</response>  
        [HttpPatch("{id}", Name = "Update[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(ExerciseCollectionWithExercisesDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Consumes("application/json")]
        [ConsumesType(typeof(JsonPatchDocumentSchema))]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            [FromBody] JArray payload,
            CancellationToken token = default
        )
        {
            return await UpdateAsync<ExerciseCollectionWithExercisesDto>(id, payload, token);
        }

        /// <summary>
        /// Delete an Exercise Collection.
        /// </summary>
        /// <param name="id">The id of the Exercise Collection to delete.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <response code="204">Successful response.</response>
        /// <response code="404">Not found.</response>  
        [HttpDelete("{id}", Name = "Delete[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken token = default)
        {
            return await DeleteAsync<ExerciseCollectionDto>(id, token);
        }
    }
    
    
}