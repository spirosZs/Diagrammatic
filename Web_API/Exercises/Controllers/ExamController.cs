using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Exercises.Common;
using Exercises.Common.Abstractions;
using Exercises.Common.Exam;
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
    /// An Exam is based on an Exercise Collection that was previously created by the teacher
    /// and can receive user submissions.
    /// </summary>
    [Route(Router.Exam)]
    [Authorize]
    public sealed class ExamController
        : EntityController<Exam, ExamFilter>
    {
        private readonly IExerciseCollectionService _exerciseCollectionService;
        
        public ExamController(
            IServiceProvider serviceProvider,
            IHttpContextAccessor accessor, LinkGenerator generator,
            IPropertyMappingService propertyMappingService
        )
            : base(serviceProvider, accessor,generator, propertyMappingService)
        {
            _exerciseCollectionService =
                (IExerciseCollectionService) serviceProvider.GetService(typeof(IExerciseCollectionService));
        }

        /// <summary>
        /// Get a collection of Exams.
        /// </summary>
        /// <param name="filter">Query parameters to send to filter the returned results.</param>
        /// <param name="token"></param>
        /// <response code="200">Successful response.</response>
        /// <response code="400">Bad request.</response>
        [HttpGet(Name = "GetMultiple[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ExamDto>))]
        [ProducesResponseType(400)]
        //        [SwaggerResponseHeader(200, "X-Pagination", "string", "Pagination metadata for this request.")]
        public async Task<IActionResult> GetAsync([FromQuery] ExamFilter filter,
            CancellationToken token = default)
        {
            return await GetAsync<ExamDto>(filter, token);
        }

        /// <summary>
        /// Get a single Exam.
        /// </summary>
        /// <param name="id">The id of the Exam to return.</param>
        /// <param name="token"></param>
        /// <response code="200">Successful response.</response>
        /// <response code="404">Not found.</response>  
        [HttpGet("{id}", Name = "Get[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(ExamDto))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid id, CancellationToken token = default)
        {
            return await GetAsync<ExamWithExercisesDto>(id, token);
        }

        /// <summary>
        /// Create an Exam.
        /// </summary>
        /// <remarks>
        /// An Exam is based on an Exercise Collection. This means that when creating an Exam you actually copy an Exercise Collection
        /// and convert it to an Exam. Please note that while you will be able to modify the values and create further exams
        /// from the same Exercise Collection that this Exam was based of, you will not be able to alter the definitions that exist in
        /// this Exam apart from the basic entity definitions like the name, etc.
        /// </remarks>
        /// <param name="payload"></param>
        /// <param name="token"></param>
        /// <response code="201">Returns the newly created Exam.</response>
        /// <response code="400">Bad request.</response>  
        [HttpPost(Name = "Create[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(201, Type = typeof(ExamDto))]
        [ProducesResponseType(400)]
        [Consumes("application/json")]
        [ConsumesType(typeof(ExamCreateDto))]
        public async Task<IActionResult> CreateAsync([FromBody] JObject payload,
            CancellationToken token = default)
        {
            return await CreateAsync<ExamDto>(payload, token);
        }

        /// <summary>
        /// Update an Exam.
        /// </summary>
        /// <param name="id">The id of the Exam to update.</param>
        /// <param name="payload"></param>
        /// <param name="token"></param>
        /// <remarks>
        /// This endpoint exists so that the teacher can modify basic entity definitions like the name, published etc.
        /// Notice that you cannot add/remove exercises from an Exam.
        /// </remarks>
        /// <response code="200">Returns the updated Exam.</response>
        /// <response code="400">Bad request.</response>  
        /// <response code="404">Not found.</response>  
        [HttpPatch("{id}", Name = "Update[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(ExamDto))]
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
            return await UpdateAsync<ExamDto>(id, payload, token);
        }

        /// <summary>
        /// Delete an Exam
        /// </summary>
        /// <param name="id">The id of the Exam to delete.</param>
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
            return await DeleteAsync<ExamDto>(id, token);
        }

        protected override bool OnOperationValidation(dynamic dto, Type dtoType)
        {
            if (dtoType == typeof(ExamCreateDto) && base.OnOperationValidation((ExamCreateDto) dto, dtoType))
            {
                ExamCreateDto dtoObject = (ExamCreateDto) dto;

                if (dtoObject.ExerciseCollectionId != Guid.Empty)
                {
                    var exerciseCollectionRepo =
                        _exerciseCollectionService.GetAsync(dtoObject.ExerciseCollectionId).Result;
                    if (exerciseCollectionRepo == null)
                    {
                        ModelState.AddModelError("ExerciseCollectionId",
                            $"The requested exercise collection with id {dtoObject.ExerciseCollectionId} doesn't exist.");
                    }
                }
                else
                {
                    ModelState.AddModelError("ExerciseCollectionId",
                        "You need to specify the id of the exercise collection that will be used to create this exam.");
                }
            }

            return ModelState.IsValid;
        }
    }
}