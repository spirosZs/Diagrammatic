using System;
using Exercises.Common.Diagram;
using Exercises.Core.Abstractions;
using Exercises.Core.Helpers;
using Exercises.Data;
using Exercises.Data.Types;

namespace Exercises.Core.Plugins.Problem
{
    public class PathProblemPlugin : IProblemPlugin
    {
        public ProblemType Type { get; }

        public PathProblemPlugin()
        {
            Type = ProblemType.Path;
        }
        
        public int Evaluate(Submission submission)
        {
            var answersGiven = submission.GetDataObject<string[]>();
            if (answersGiven == null)
            {
                throw new Exception("Cannot parse the answers given.");
            }
            
            var exercise = (PathExercise) submission.Exercise;

            // Scored against the teacher's per-path credits: an exercise weighted 50/30/20
            // pays 80 for the first two paths, not an even share of 100.
            return Graph.ComparePaths(answersGiven, exercise.Paths, exercise.Credits);
        }
    }
}