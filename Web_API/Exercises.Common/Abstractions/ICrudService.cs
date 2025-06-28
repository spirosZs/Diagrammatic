using System;
using System.Threading;
using System.Threading.Tasks;

namespace Exercises.Common.Abstractions
{
    public interface ICrudService<TResource, TFilter, TCreatePayload>
        where TFilter : FilterBase
    {
        Task<PagedList<TResource>> GetAsync(TFilter filter, CancellationToken token = default);
        Task<TResource> GetAsync(Guid id, CancellationToken token = default);
        Task<TResource> CreateAsync(TCreatePayload payload, CancellationToken token = default);
        Task<TResource> UpdateAsync(TResource payload, CancellationToken token = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken token = default);
    }
}