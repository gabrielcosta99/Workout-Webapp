using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    public class ExerciseProgress
    {
        [Key]
        public int Id { get; set; }

        [Column("workout_prog_id")]
        public int WorkoutProgId { get; set; }

        [Column("exercise_id")]
        public int ExerciseId { get; set; }

        public int Set { get; set; }

        public int Reps { get; set; }

        public double? Weight { get; set; }

        // Navigation properties
        [ForeignKey("WorkoutProgId")]
        public virtual WorkoutProgress WorkoutProgress { get; set; } = null!;

        [ForeignKey("ExerciseId")]
        public virtual Exercise Exercise { get; set; } = null!;
    }
}
