using System;
using System.Linq;
using Exercises.Common.Abstractions;
using Exercises.Common.Submission;
using Exercises.Core.Abstractions;
using Exercises.Data;
using Exercises.Data.DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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

        protected override void OnGetSingle(ref IQueryable<Submission> query)
        {
            base.OnGetSingle(ref query);
            query = query.Include(s => s.User);
        }

        protected override void OnGet(SubmissionFilter filter, ref IQueryable<Submission> query)
        {
            base.OnGet(filter, ref query);
            query = query.Include(s => s.User);
        }

        protected override void OnBeforeCreate(Submission submission)
        {
            base.OnBeforeCreate(submission);

            var exam = _context.Exams.FirstOrDefault(e => e.Id == submission.ExamId);
            if (exam == null)
            {
                throw new Exception("Exam not found.");
            }

            var exercise = _context.Exercises
                .Include(e => (e as DiagramExercise).Diagram)
                .FirstOrDefault(e => e.Id == submission.ExerciseId);
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