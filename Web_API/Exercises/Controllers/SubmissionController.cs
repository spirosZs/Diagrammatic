using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Exercises.Common;
using Exercises.Common.Abstractions;
using Exercises.Common.Submission;
using Exercises.Data;
using Exercises.Swagger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json.Linq;

namespace Exercises.Controllers
{
    [Route(Router.Submission)]
    public class SubmissionController : EntityController<Submission, SubmissionFilter>
    {
        public SubmissionController(IServiceProvider serviceProvider, IHttpContextAccessor accessor,
            LinkGenerator generator, IPropertyMappingService propertyMappingService)
            : base(serviceProvider, accessor, generator, propertyMappingService)
        {
        }

        /// <summary>
        /// Get a collection of Submissions.
        /// </summary>
        /// <param name="filter">Query parameters to send to filter the returned results.</param>
        /// <param name="token"></param>
        /// <response code="200">Successful response.</response>
        /// <response code="400">Bad request.</response>
        [HttpGet(Name = "GetMultiple[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<SubmissionDto>))]
        [ProducesResponseType(400)]
//        [SwaggerResponseHeader(200, "X-Pagination", "string", "Pagination metadata for this request.")]
        public async Task<IActionResult> GetAsync([FromQuery] SubmissionFilter filter,
            CancellationToken token = default)
        {
            return await GetAsync<SubmissionDto>(filter, token);
        }

        /// <summary>
        /// Get a single Submission.
        /// </summary>
        /// <param name="id">The id of the Submission to return.</param>
        /// <param name="token"></param>
        /// <response code="200">Successful response.</response>
        /// <response code="404">Not found.</response>  
        [HttpGet("{id}", Name = "Get[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(SubmissionDto))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid id, CancellationToken token = default)
        {
            return await GetAsync<SubmissionDto>(id, token);
        }


        /// <summary>
        /// Create a Submission.
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="token"></param>
        /// <response code="201">Returns the newly created Submission.</response>
        /// <response code="400">Bad request.</response>  
        [HttpPost(Name = "Create[controller]")]
        [Produces("application/json")]
        [ProducesResponseType(201, Type = typeof(SubmissionDto))]
        [ProducesResponseType(400)]
        [Consumes("application/json")]
        [ConsumesType(typeof(SubmissionCreateDto))]
        public async Task<IActionResult> CreateAsync([FromBody] JObject payload,
            CancellationToken token = default)
        {
            return await CreateAsync<SubmissionDto>(payload, token);
        }

        /// <summary>
        /// Delete a Submission.
        /// </summary>
        /// <param name="id">The id of the Submission to delete.</param>
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
            return await DeleteAsync<SubmissionDto>(id, token);
        }
    }
}