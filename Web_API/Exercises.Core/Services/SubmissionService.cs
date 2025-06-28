using System;
using System.Linq;
using Exercises.Common.Abstractions;
using Exercises.Common.Submission;
using Exercises.Core.Abstractions;
using Exercises.Data;
using Exercises.Data.DbContext;
using Microsoft.AspNetCore.Http;

namespace Exercises.Core.Services
{
    public class SubmissionService
        : ResourceBase<Submission, SubmissionFilter, SubmissionCreateDto>, ISubmissionService
    {
        private readonly IProblemPluginManager _problemPluginManager;

        public SubmissionService(
            ExercisesContext context,
            IPropertyMappingService propertyMappingService,
            IProblemPluginManager problemPluginManager,
            IHttpContextAccessor httpContextAccessor
        )
            : base(context, propertyMappingService, httpContextAccessor)
        {
            _problemPluginManager = problemPluginManager;
        }

        protected override void OnBeforeCreate(Submission submission)
        {
            base.OnBeforeCreate(submission);

            var exam = _context.Exams.FirstOrDefault(e => e.Id == submission.ExamId);
            if (exam == null)
            {
                throw new Exception("Exam not found.");
            }

            var exercise = _context.Exercises.FirstOrDefault(e => e.Id == submission.ExerciseId);
            if (exercise == null)
            {
                throw new Exception("Exercise not found.");
            }

            if (!exam.Exercises.Contains(exercise))
            {
                throw new Exception("The specified exercise doesnt belong to this exam.");
            }

            submission.Exercise = exercise;
            submission.Exam = exam;

            var plugin = _problemPluginManager.GetPlugin(submission.Exercise.ProblemType);
            if (plugin == null)
            {
                throw new Exception("Cannot evaluate this problem type.");
            }

            var score = plugin.Evaluate(submission);
            submission.Score = score;

            exam.Submissions.Add(submission);
        }
    }
}