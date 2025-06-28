using System;
using System.Threading;
using System.Threading.Tasks;
using Exercises.Common;
using Exercises.Common.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json.Linq;

namespace Exercises.Controllers
{
    [ApiController]
    public abstract class EntityControllerBase<TResource, TFilter, TResourceDto> :
        EntityController<TResource, TFilter>, IEntityController<JObject, JArray, TFilter>
        where TFilter : FilterBase
    {
        protected EntityControllerBase(IServiceProvider serviceProvider, IHttpContextAccessor accessor,
            LinkGenerator generator, IPropertyMappingService propertyMappingService)
            : base(serviceProvider, accessor, generator, propertyMappingService)
        {
        }

        [HttpGet(Name = "GetMultiple[controller]")]
        public virtual async Task<IActionResult> GetAsync([FromQuery] TFilter filter, CancellationToken token = default)
        {
            return await GetAsync<TResourceDto>(filter, token);
        }

        [HttpGet("{id}", Name = "Get[controller]")]
        public virtual async Task<IActionResult> GetAsync([FromRoute] Guid id, CancellationToken token = default)
        {
            return await GetAsync<TResourceDto>(id, token);
        }

        [HttpPost(Name = "Create[controller]")]
        public virtual async Task<IActionResult> CreateAsync([FromBody] JObject payload,
            CancellationToken token = default)
        {
            return await CreateAsync<TResourceDto>(payload, token);
        }

        [HttpPatch("{id}", Name = "Update[controller]")]
        public virtual async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            [FromBody] JArray payload,
            CancellationToken token = default
        )
        {
            return await UpdateAsync<TResourceDto>(id, payload, token);
        }

        [HttpDelete("{id}", Name = "Delete[controller]")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken token = default)
        {
            return await DeleteAsync<TResourceDto>(id, token);
        }
    }
}