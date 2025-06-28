using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Exercises.Common;
using Exercises.Common.Abstractions;
using Exercises.Core.Helpers;
using Exercises.Data;
using Exercises.Data.DbContext;
using Exercises.Data.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Exercises.Core
{
    public abstract class ResourceBase<TResource, TFilter, TCreatePayload>
        : ICrudService<TResource, TFilter, TCreatePayload>
        where TResource : Entity, new()
        where TFilter : FilterBase
    {
        protected readonly ExercisesContext _context;
        protected readonly IPropertyMappingService _propertyMappingService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected DbSet<TResource> _dbset;

        protected ResourceBase(ExercisesContext context, IPropertyMappingService propertyMappingService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            string className = this.GetType().Name;
            string propertyName = className.Substring(0, className.Length - "Service".Length);
            var property = _context.GetType().GetProperty($"{propertyName}s");
            _dbset = (DbSet<TResource>) property?.GetValue(_context, null);
            _propertyMappingService = propertyMappingService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedList<TResource>> GetAsync(TFilter filter, CancellationToken token = default)
        {
            var query = _dbset
                .AsQueryable()
                .ApplyResourceFilters(filter)
                .ApplySort(filter.OrderBy, _propertyMappingService.GetPropertyMapping<TResource>());

            OnGet(filter, ref query);
            return await PagedList<TResource>.Create(query, filter.PageNumber, filter.PageSize);
        }

        public async Task<TResource> GetAsync(Guid id, CancellationToken token = default)
        {
            var query = _dbset.AsQueryable();
            OnGetSingle(ref query);
            return await query.FirstOrDefaultAsync(c => c.Id == id, cancellationToken: token);
        }

        public async Task<TResource> CreateAsync(TCreatePayload payload, CancellationToken token = default)
        {
            var entity = Mapper.Map<TCreatePayload, TResource>(payload);
            OnBeforeCreate(entity);
            _dbset.Add(entity);
            await _context.SaveChangesAsync(token);
            OnAfterCreate(entity);
            return entity;
        }

        public async Task<TResource> UpdateAsync(TResource entity, CancellationToken token = default)
        {
            var entityToUpdate = await _dbset.FindAsync(entity.Id);
            if (entityToUpdate == null)
            {
                return null;
            }

            OnBeforeUpdate(entityToUpdate, entity);
            await _context.SaveChangesAsync(token);
            OnAfterUpdate(entityToUpdate);
            return entityToUpdate;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
        {
            var entity = await _dbset.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            var clone = GetResourceClone(entity);

            OnBeforeDelete(entity);
            _context.Remove(entity);
            var status = await _context.SaveChangesAsync(token);
            if (status >= 0)
            {
                OnAfterDelete(clone);
            }

            return status >= 0;
        }


        //HELPER HOOKS
        protected virtual void OnGetSingle(ref IQueryable<TResource> query)
        {
            var userRoles = _httpContextAccessor.HttpContext.GetUserRoles();

            var canViewAny = Permissions.OperationAllowed<TResource>(EntityOperationType.ViewAny, userRoles);
            if (canViewAny == AccessResultType.Neutral || canViewAny == AccessResultType.Allowed)
            {
                return;
            }

            var userId = _httpContextAccessor.HttpContext.GetUserId();
            var canViewOwn = Permissions.OperationAllowed<TResource>(EntityOperationType.ViewOwn, userRoles);
            if (canViewOwn == AccessResultType.Neutral || canViewOwn == AccessResultType.Allowed)
            {
                query = query.Where(r => r.UserId == userId);
                return;
            }

            throw new Exception($"Current user is not allowed to view {typeof(TResource)}s");
        }

        protected virtual void OnGet(TFilter filter, ref IQueryable<TResource> query)
        {
            var userRoles = _httpContextAccessor.HttpContext.GetUserRoles();

            var canViewAny = Permissions.OperationAllowed<TResource>(EntityOperationType.ViewAny, userRoles);
            if (canViewAny == AccessResultType.Neutral || canViewAny == AccessResultType.Allowed)
            {
                return;
            }

            var userId = _httpContextAccessor.HttpContext.GetUserId();
            var canViewOwn = Permissions.OperationAllowed<TResource>(EntityOperationType.ViewOwn, userRoles);
            if (canViewOwn == AccessResultType.Neutral || canViewOwn == AccessResultType.Allowed)
            {
                query = query.Where(r => r.UserId == userId);
                return;
            }
        }

        protected virtual void OnBeforeCreate(TResource resource)
        {
            var userId = _httpContextAccessor.HttpContext.GetUserId();
            resource.UserId = userId;
            resource.Created = DateTime.Now;
        }

        protected virtual void OnAfterCreate(TResource resource)
        {
        }

        protected virtual void OnBeforeUpdate(TResource entityToUpdate, TResource entity)
        {
        }

        protected virtual void OnAfterUpdate(TResource updatedEntity)
        {
        }

        protected virtual void OnBeforeDelete(TResource entity)
        {
        }

        protected virtual void OnAfterDelete(TResource entity)
        {
        }

        protected virtual void OnAfterDelete()
        {
        }

        protected virtual TResource GetResourceClone(TResource resource)
        {
            return new TResource
            {
                Id = resource.Id,
                Name = resource.Name,
                Created = resource.Created
            };
        }
    }
}