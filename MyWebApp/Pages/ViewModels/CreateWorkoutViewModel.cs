
// ViewModels/CreateWorkoutViewModel.cs
using System.ComponentModel.DataAnnotations;


namespace MyWebApp.Pages.ViewModels
{
    public class CreateWorkoutViewModel
    {
        [Required(ErrorMessage = "Workout title is required")]
        [Display(Name = "Workout Title")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Selected Exercises")]
        public List<int> SelectedExerciseIds { get; set; } = new List<int>();

        // For displaying available exercises
        public List<ExerciseSelectItem> AvailableExercises { get; set; } = new List<ExerciseSelectItem>();
    }

    public class ExerciseSelectItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? MaxReps { get; set; }
        public double? MaxWeight { get; set; }
        public bool IsSelected { get; set; }
    }
}
