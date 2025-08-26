using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class Exercise
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int? MaxReps { get; set; }

        public double? MaxWeight { get; set; }

        // Navigation properties
        public virtual ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
        public virtual ICollection<ExerciseProgress> ExerciseProgresses { get; set; } = new List<ExerciseProgress>();

    }
}
