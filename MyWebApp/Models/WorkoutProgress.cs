using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    public class WorkoutProgress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Column("workout_id")]
        public int WorkoutId { get; set; }

        // Navigation properties
        [ForeignKey("WorkoutId")]
        public virtual Workout Workout { get; set; } = null!;
        public virtual ICollection<ExerciseProgress> ExerciseProgresses { get; set; } = new List<ExerciseProgress>();

    }
}
