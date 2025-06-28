using Exercises.Core.Abstractions;
using Exercises.Data;

namespace Exercises.Core.Services
{
    public class SolutionService
    {
        private readonly IProblemPluginManager _problemPluginManager;

        public SolutionService(IProblemPluginManager problemPluginManager)
        {
            _problemPluginManager = problemPluginManager;
        }


        public void EvaluateCollection(ExerciseCollection exerciseCollection)
        {
//            var exercises = exerciseCollection.Exercises;
//            foreach (var exercise in exercises)
//            {
//                var submissions = exerciseCollection
//                    .Submissions
//                    .Where(submission => submission.ExerciseId == exercise.Id);
//                foreach (var submission in submissions)
//                {
//                    EvaluateSubmission(submission, exercise);
//                }
//            }
        }

        public void EvaluateSubmission(Submission submission, Exercise exercise)
        {
//            var problems = exercise.Problems;
//
//            foreach (var problem in problems)
//            {
//                var solution = problem.Solution;
//                var answer = submission.Answer;
//                int score = GetScore(exercise.Diagram, solution, answer);
//                submission.Score = score;
//            }
        }

        protected int GetScore(Diagram diagram, object solution, object answer)
        {
            return 100;//TODO: return score
        }
        
    }
}