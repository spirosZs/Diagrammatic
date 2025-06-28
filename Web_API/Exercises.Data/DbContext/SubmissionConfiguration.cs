using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Exercises.Data.DbContext
{
    public class SubmissionConfiguration : BaseEntityConfiguration<Submission>
    {
        public override void Configure(EntityTypeBuilder<Submission> builder)
        {
            base.Configure(builder);
            
        }
    }
}