using System.Collections.Generic;
using Exercises.Data.Types;

namespace Exercises.Core.Abstractions
{
    public interface IProblemPluginManager
    {
        List<IProblemPlugin> GetPlugins();
        IProblemPlugin GetPlugin(ProblemType type);
    }
}