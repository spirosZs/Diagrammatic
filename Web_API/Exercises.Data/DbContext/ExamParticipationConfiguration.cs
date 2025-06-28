using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Exercises.Data.DbContext
{
    public class ExamParticipationConfiguration : BaseEntityConfiguration<ExamParticipation>
    {
        public override void Configure(EntityTypeBuilder<ExamParticipation> builder)
        {
            base.Configure(builder);
            builder
                .HasKey(p=> new { p.ExamId, p.UserId });
            builder
                .HasOne(p => p.Exam)
                .WithMany(e => e.Participations)
                .HasForeignKey(p => p.ExamId);
            builder
                .HasOne(p => p.User)
                .WithMany(u => u.Participations)
                .HasForeignKey(p => p.UserId);
        }
    }
}