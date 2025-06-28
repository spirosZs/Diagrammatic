using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Exercises.Data.DbContext
{
    public class ExamConfiguration : BaseEntityConfiguration<Exam>
    {
        public override void Configure(EntityTypeBuilder<Exam> builder)
        {
            base.Configure(builder);
        }
    }
}