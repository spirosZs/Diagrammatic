using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Exercises.Core.Abstractions;
using Exercises.Data.Types;

namespace Exercises.Core
{
    public class ProblemPluginManager : IProblemPluginManager
    {
        private List<IProblemPlugin> Plugins { get; set; }

        public ProblemPluginManager()
        {
            var location = Path.GetDirectoryName(Environment.CurrentDirectory);
            var root = Path.Combine(location, @"Exercises.Core\Plugins\Problem");

            string folderName = root;

            Plugins = new List<IProblemPlugin>();

            if (Directory.Exists(folderName))
            {
                var files = Directory.GetFiles(folderName);
                foreach (var file in files)
                {
                    if (file.EndsWith(".dll"))
                    {
                        Assembly.LoadFile(Path.GetFullPath(file));
                    }
                }
            }
            else
            {
                throw new Exception("Could not load plugin directory.");
            }

            var interfaceType = typeof(IProblemPlugin);

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(p => interfaceType.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
                .ToArray();
            foreach (var type in types)
            {
                Plugins.Add((IProblemPlugin) Activator.CreateInstance(type));
            }
        }


        public List<IProblemPlugin> GetPlugins()
        {
            return Plugins;
        }


        public IProblemPlugin GetPlugin(ProblemType type)
        {
            return Plugins.FirstOrDefault(plugin => plugin.Type == type);
        }
    }
}