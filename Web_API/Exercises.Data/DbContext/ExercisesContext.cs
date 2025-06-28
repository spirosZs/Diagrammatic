using System;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Exercises.Data.DbContext
{
    public class ExercisesContext : IdentityDbContext<User, Role, Guid>
    {
        public ExercisesContext(DbContextOptions<ExercisesContext> options)
            : base(options)
        {
        }
        
        public DbSet<Diagram> Diagrams { get; set; }
        public DbSet<ExerciseCollection> ExerciseCollections { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<DiagramExercise> DiagramExercises { get; set; }
        public DbSet<PathExercise> PathExercises { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<DiagramExerciseSubmission> DiagramExerciseSubmissions { get; set; }
        public DbSet<PathExerciseSubmission> PathExerciseSubmissions { get; set; }
        public DbSet<ExamParticipation> ExamParticipations { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PathExercise>()
                .Property(e => e.Paths)
                .HasConversion(v => string.Join(";", v),
                    v => v.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries));

            modelBuilder.Entity<PathExercise>()
                .Property(e => e.Credits)
                .HasConversion(v => string.Join(";", v),
                    v =>
                        Array.ConvertAll(v.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries), Double.Parse)
                );


            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            
            modelBuilder.ApplyConfiguration(new ExerciseConfiguration());
            modelBuilder.ApplyConfiguration(new ExerciseCollectionConfiguration());
            modelBuilder.ApplyConfiguration(new ExamParticipationConfiguration());
            modelBuilder.ApplyConfiguration(new SubmissionConfiguration());
        }
    }
}