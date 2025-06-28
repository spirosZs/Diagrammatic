using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Exercises.Common;
using Exercises.Common.Abstractions;
using Exercises.Core.Helpers;
using Exercises.Data.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Exercises.Controllers
{
    [ApiController]
    public abstract class EntityController<TResource, TFilter>
        : ControllerBase
        where TFilter : FilterBase
    {
        protected readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _accessor;
        private readonly LinkGenerator _generator;
        private readonly IPropertyMappingService _propertyMappingService;
        protected readonly dynamic _entityService;

        protected EntityController(IServiceProvider serviceProvider, IHttpContextAccessor accessor, LinkGenerator generator,
            IPropertyMappingService propertyMappingService)
        {
            _serviceProvider = serviceProvider;
            _accessor = accessor;
            _generator = generator;
            _propertyMappingService = propertyMappingService;
            _entityService = _serviceProvider.GetCrudService(typeof(TResource));
        }

        protected async Task<IActionResult> GetAsync<TResponseDto>(TFilter filter, CancellationToken token = default)
//            where TDto : LinkedResourceBaseDto, IEntity
        {
            if (!_propertyMappingService.ValidMappingExistsFor<TResource>(filter.OrderBy))
            {
                return BadRequest();
            }

            PagedList<TResource> entitiesRepo = await _entityService.GetAsync(filter, token);
            var controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            var paginationMeta =
                PaginationMeta.Create(filter, entitiesRepo, $"GetMultiple{controllerName}", _accessor, _generator);

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMeta));

            var entities = Mapper.Map<IEnumerable<TResponseDto>>(entitiesRepo);

//            entities = entities.Select(entity =>
//            {
//                entity = CreateLinks(entity);
//                return entity;
//            });
//            
            return Ok(entities);
        }

        protected async Task<IActionResult> GetAsync<TResponseDto>(Guid id, CancellationToken token = default)
        {
            var entityRepo = await _entityService.GetAsync(id, token);
            if (entityRepo == null)
            {
                return NotFound();
            }

            var entity = Mapper.Map<TResponseDto>(entityRepo);
            return Ok(entity);
        }

        protected async Task<IActionResult> CreateAsync<TResponseDto>(JObject payload, CancellationToken token = default)
        {
            var assembly = typeof(IExerciseService).Assembly;
            var entityType = GetEntityCreateType(payload);
            var entityName = typeof(TResource).Name;

            var dtoType =
                Type.GetType(
                    $"Exercises.Common.{entityName}.{entityType}{entityName}{CrudOperationType.Create}Dto, {assembly}");
            dynamic dto = payload.ToObject(dtoType);

            if (!OnOperationValidation(dto, dtoType))
            {
                return BadRequest(ModelState);
            }

            dynamic service = _serviceProvider.GetCrudService($"{entityType}{typeof(TResource).Name}");
            dynamic entityRepo = await service.CreateAsync(dto, token);
            dynamic entity = Mapper.Map<TResponseDto>(entityRepo);

            string controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            return CreatedAtRoute($"Get{controllerName}", new {id = entity.Id, token}, entity);
        }


        protected async Task<IActionResult> UpdateAsync<TResponseDto>(Guid id, JArray payload,
            CancellationToken token = default)
        {
            var entityRepo = await _entityService.GetAsync(id, token);
            if (entityRepo == null)
            {
                return NotFound();
            }

            var assembly = typeof(IExerciseService).Assembly;
            var entityType = GetEntityUpdateType(entityRepo);
            var entityName = typeof(TResource).Name;

            var dtoType =
                Type.GetType(
                    $"Exercises.Common.{entityName}.{entityType}{entityName}{CrudOperationType.Update}Dto, {assembly}");

            dynamic entityDto = Mapper.Map(entityRepo, typeof(TResource), dtoType);
            dynamic patchDocument = payload.ToObject(typeof(JsonPatchDocument<>).MakeGenericType(dtoType));

            patchDocument.ApplyTo(entityDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(entityDto, entityRepo);

            dynamic service = _serviceProvider.GetCrudService($"{entityType}{typeof(TResource).Name}");

            entityRepo = await service.UpdateAsync(entityRepo, token);
            dynamic entity = Mapper.Map<TResponseDto>(entityRepo);

            return Ok(entity);
        }

        protected async Task<IActionResult> DeleteAsync<TResponseDto>(Guid id, CancellationToken token = default)
        {
            var entityRepo = await _entityService.GetAsync(id, token);
            if (entityRepo == null)
            {
                return NotFound();
            }

            bool success = await _entityService.DeleteAsync(id, token);
            if (!success)
            {
                throw new Exception($"There was a problem while deleting entity with id {id}");
            }

            return NoContent();
        }

        //HELPERS
        protected virtual string GetEntityCreateType(JObject payload)
        {
            return String.Empty;
        }

        protected virtual string GetEntityUpdateType(TResource resource)
        {
            return String.Empty;
        }

        protected virtual bool OnOperationValidation(dynamic dto, Type dtoType)
        {
            return TryValidateModel(dto);
        }


//        protected TDto CreateLinks<TDto>(TDto dto)
//            where TDto : LinkedResourceBaseDto, IEntity
//        {
//            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
//
//
//            dto.Links.Add(
//                new LinkDto(
//                    _urlHelper.Link($"Get{controllerName}", new {id = dto.Id}),
//                    "self",
//                    "GET"
//                )
//            );
//
//            dto.Links.Add(
//                new LinkDto(
//                    _urlHelper.Link($"Delete{controllerName}", new {id = dto.Id}),
//                    "delete",
//                    "DELETE"
//                )
//            );
//
//            dto.Links.Add(
//                new LinkDto(
//                    _urlHelper.Link($"Update{controllerName}", new {id = dto.Id}),
//                    "update",
//                    "PUT"
//                )
//            );
//
//            return dto;
//        }
    }
}