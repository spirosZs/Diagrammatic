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
            Plugins = new List<IProblemPlugin>();

            var pluginDir = Path.Combine(
                AppContext.BaseDirectory,
                "Exercises.Core",
                "Plugins",
                "Problem");

            if (Directory.Exists(pluginDir))
            {
                foreach (var file in Directory.GetFiles(pluginDir, "*.dll"))
                {
                    Assembly.LoadFrom(Path.GetFullPath(file));
                }
            }

            var interfaceType = typeof(IProblemPlugin);

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null); }
                })
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