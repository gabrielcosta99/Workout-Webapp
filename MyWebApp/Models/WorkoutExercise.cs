using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    public class WorkoutExercise
    {
        [Key]
        [Column("workout_id")]
        public int WorkoutId { get; set; }

        [Key]
        [Column("exercise_id")]
        public int ExerciseId { get; set; }

        // Navigation properties
        [ForeignKey("WorkoutId")]
        public virtual Workout Workout { get; set; } = null!;

        [ForeignKey("ExerciseId")]
        public virtual Exercise Exercise { get; set; } = null!;
    }
}
