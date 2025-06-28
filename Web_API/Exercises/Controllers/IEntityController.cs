using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Exercises.Controllers
{
    public interface IEntityController<TCreatePayload, TUpdatePayload, TFilter>
    {
        Task<IActionResult> GetAsync(TFilter filter, CancellationToken token = default);
        Task<IActionResult> GetAsync(Guid id, CancellationToken token = default);
        Task<IActionResult> CreateAsync(TCreatePayload payload, CancellationToken token = default);
        Task<IActionResult> UpdateAsync(Guid id, TUpdatePayload payload, CancellationToken token = default);
        Task<IActionResult> DeleteAsync(Guid id, CancellationToken token = default);
    }
}