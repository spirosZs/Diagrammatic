using Exercises.Common.Submission;

namespace Exercises.Common.Abstractions
{
    public interface ISubmissionService : ICrudService<Data.Submission, SubmissionFilter, SubmissionCreateDto>
    {
        
    }
}