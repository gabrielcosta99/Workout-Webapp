using Microsoft.EntityFrameworkCore;
using MyWebApp.Models;


namespace MyWebApp.Data
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
        public DbSet<WorkoutProgress> WorkoutProgresses { get; set; }
        public DbSet<ExerciseProgress> ExerciseProgresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure table names to match your schema
            modelBuilder.Entity<Exercise>().ToTable("Exercise");
            modelBuilder.Entity<Workout>().ToTable("Workout");
            modelBuilder.Entity<WorkoutExercise>().ToTable("Workout_exercises");
            modelBuilder.Entity<WorkoutProgress>().ToTable("Workout_progress");
            modelBuilder.Entity<ExerciseProgress>().ToTable("Exercise_progress");

            // Configure composite key for WorkoutExercise
            modelBuilder.Entity<WorkoutExercise>()
                .HasKey(we => new { we.WorkoutId, we.ExerciseId });

            // Configure relationships
            modelBuilder.Entity<WorkoutExercise>()
                .HasOne(we => we.Workout)
                .WithMany(w => w.WorkoutExercises)
                .HasForeignKey(we => we.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkoutExercise>()
                .HasOne(we => we.Exercise)
                .WithMany(e => e.WorkoutExercises)
                .HasForeignKey(we => we.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkoutProgress>()
                .HasOne(wp => wp.Workout)
                .WithMany(w => w.WorkoutProgresses)
                .HasForeignKey(wp => wp.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExerciseProgress>()
                .HasOne(ep => ep.WorkoutProgress)
                .WithMany(wp => wp.ExerciseProgresses)
                .HasForeignKey(ep => ep.WorkoutProgId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExerciseProgress>()
                .HasOne(ep => ep.Exercise)
                .WithMany(e => e.ExerciseProgresses)
                .HasForeignKey(ep => ep.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed some sample data
            modelBuilder.Entity<Exercise>().HasData(
                new Exercise { Id = 1, Name = "Push-ups", MaxReps = 50 },
                new Exercise { Id = 2, Name = "Bench Press", MaxWeight = 100.0 },
                new Exercise { Id = 3, Name = "Squats", MaxWeight = 150.0 }
            );

            modelBuilder.Entity<Workout>().HasData(
                new Workout { Id = 1, Title = "Upper Body" },
                new Workout { Id = 2, Title = "Lower Body" }
            );
        }
    }
}