using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Exercises.Data.DbContext
{
    public class ExerciseConfiguration : BaseEntityConfiguration<Exercise>
    {
        public override void Configure(EntityTypeBuilder<Exercise> builder)
        {
            base.Configure(builder);
        }
    }
}