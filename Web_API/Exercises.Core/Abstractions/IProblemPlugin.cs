using Exercises.Data;
using Exercises.Data.Types;

namespace Exercises.Core.Abstractions
{
    public interface IProblemPlugin
    {
        ProblemType Type { get; }
        
        int Evaluate(Submission submission);
    }
}