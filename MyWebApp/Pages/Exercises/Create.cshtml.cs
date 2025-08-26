using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Pages.ViewModels;

namespace MyWebApp.Pages.Exercises
{
    public class CreateModel : PageModel
    {
        private readonly DBContext _context;

        public CreateModel(DBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CreateExerciseViewModel ExerciseViewModel { get; set; } = new CreateExerciseViewModel();

        public async Task<IActionResult> OnGetAsync()
        {
            //await LoadAvailableExercisesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Add this debugging
            Console.WriteLine($"Raw MaxWeight from form: {Request.Form["ExerciseViewModel.MaxWeight"]}");
            Console.WriteLine($"Parsed MaxWeight: {ExerciseViewModel.MaxWeight}");
            Console.WriteLine($"Current Culture: {Thread.CurrentThread.CurrentCulture.Name}");

            if (!ModelState.IsValid)
            {
                // Print validation errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
                }
                return Page();
            }

            try
            {
                // Create new Exercise
                var exercise = new Exercise
                {
                    Name = ExerciseViewModel.Name
                    //MaxReps = ExerciseViewModel.MaxReps,
                    //MaxWeight = ExerciseViewModel.MaxWeight
                };
                if (ExerciseViewModel.MaxReps.HasValue)
                {
                    exercise.MaxReps = ExerciseViewModel.MaxReps.Value;
                }
                else
                {
                    exercise.MaxReps = 0; // Default value if not provided
                }

                if (ExerciseViewModel.MaxWeight.HasValue)
                {
                    exercise.MaxWeight = ExerciseViewModel.MaxWeight.Value;
                }
                else
                {
                    exercise.MaxWeight = 0; // Default value if not provided
                }
                Console.WriteLine($"Creating exercise: {exercise.Name}, MaxReps: {exercise.MaxReps}, MaxWeight: {exercise.MaxWeight}");
                _context.Exercises.Add(exercise);
                await _context.SaveChangesAsync();

                //// Add selected exercises to the workout
                //foreach (var exerciseId in WorkoutViewModel.SelectedExerciseIds)
                //{
                //    var workoutExercise = new WorkoutExercise
                //    {
                //        WorkoutId = workout.Id,
                //        ExerciseId = exerciseId
                //    };
                //    _context.WorkoutExercises.Add(workoutExercise);
                //}
                //await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Exercise '{exercise.Name}' created successfully!";
                return RedirectToPage("/Exercises/Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the exercise. Please try again.");
                //await LoadAvailableExercisesAsync();
                return Page();
            }
        }


    }
}
