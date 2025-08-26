using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class Workout
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
        public virtual ICollection<WorkoutProgress> WorkoutProgresses { get; set; } = new List<WorkoutProgress>();
    }
}
