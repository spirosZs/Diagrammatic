// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
//
// namespace Exercises.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     [Consumes("multipart/form-data")]
//     public class ImagesController : ControllerBase
//     {
//         private readonly IHostingEnvironment _hostingEnvironment;
//
//         public ImagesController(IHostingEnvironment hostingEnvironment)
//         {
//             _hostingEnvironment = hostingEnvironment;
//         }
//
//         [HttpPost("")]
//         public async Task<IActionResult> SaveDiagramImageAsync(IFormFile formFile, CancellationToken token = default)
//         {
//             Directory.CreateDirectory("Data");
//             var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Data", formFile.FileName);
//             using (var s = new FileStream(filePath, FileMode.Create))
//             {
//                 await formFile.CopyToAsync(s);
//             }
//
//             return Ok(formFile.FileName);
//         }
//
//         [HttpGet("{name}")]
//         public IActionResult GetDiagramImage(string name, CancellationToken token = default)
//         {
//             var path = Path.Combine("Data", name);
//             var file = _hostingEnvironment.ContentRootFileProvider.GetFileInfo(path);
//             if (!file.Exists)
//             {
//                 return NotFound($"No image found with name {name}");
//             }
//
//             var fileStream = file.CreateReadStream();
//             return File(fileStream, "image/jpeg");
//         }
//     }
// }