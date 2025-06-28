using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Exercises.Data.DbContext
{
    public class ExerciseCollectionConfiguration : BaseEntityConfiguration<ExerciseCollection>
    {
        public override void Configure(EntityTypeBuilder<ExerciseCollection> builder)
        {
            base.Configure(builder);
            
        }
    }
}