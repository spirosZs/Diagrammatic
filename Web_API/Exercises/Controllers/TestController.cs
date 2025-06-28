using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exercises.Core;
using Exercises.Core.Abstractions;
using Exercises.Data;
using Exercises.Data.Types;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Exercises.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IProblemPluginManager _pluginManager;

        public TestController(IProblemPluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

//        [HttpGet]
//        public IActionResult GetTest()
//        {
//            var plugins = _pluginManager.GetPlugins();
//
//            var diagramPlugin = _pluginManager.GetPlugin(ProblemType.Diagram);
//          
//            
//            
//            var result = "";
//            foreach (var plugin in plugins)
//            {
//                result = $"{result} {plugin.Type}";
//            }
//            return Ok(result);
//        }
    }
}