using System;
using Exercises.Common.Diagram;
using Exercises.Core.Abstractions;
using Exercises.Core.Helpers;
using Exercises.Data;
using Exercises.Data.DiagramDefinitions;
using Exercises.Data.Types;

namespace Exercises.Core.Plugins.Problem
{
    public class DiagramProblemPlugin : IProblemPlugin
    {
        public DiagramProblemPlugin()
        {
            Type = ProblemType.Diagram;
        }

        public ProblemType Type { get; }
        
        public int Evaluate(Submission submission)
        {
            var answersGiven = submission.GetDataObject<DiagramDefinition>();
            if (answersGiven == null)
            {
                throw new Exception("Cannot parse the answers given.");
            }
            var exercise = (DiagramExercise) submission.Exercise;
            var inputGraph = answersGiven.ToGraph();
            var correctGraph = exercise.Diagram.ToGraph();
            return Graph.CompareGraphs(inputGraph, correctGraph);
        }
    }
}