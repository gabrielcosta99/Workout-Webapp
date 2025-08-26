
using System.ComponentModel.DataAnnotations;


namespace MyWebApp.Pages.ViewModels
{
    public class CreateExerciseViewModel
    {
        [Required(ErrorMessage = "Exercise name is required")]
        [Display(Name = "Exercise Name")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        //[Display(Name = "Selected Exercises")]
        public int? MaxReps { get; set; }

        public float? MaxWeight { get; set; }

    }

    //public class ExerciseSelectItem
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public int? MaxReps { get; set; }
    //    public double? MaxWeight { get; set; }
    //    public bool IsSelected { get; set; }
    //}
}
