using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Pages.ViewModels;

namespace MyWebApp.Pages.Workouts
{
    public class CreateModel : PageModel
    {
        private readonly DBContext _context;

        public CreateModel(DBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CreateWorkoutViewModel WorkoutViewModel { get; set; } = new CreateWorkoutViewModel();

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadAvailableExercisesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadAvailableExercisesAsync();
                return Page();
            }

            try
            {
                // Create new workout
                var workout = new Workout
                {
                    Title = WorkoutViewModel.Title
                };

                _context.Workouts.Add(workout);
                await _context.SaveChangesAsync();

                // Add selected exercises to the workout
                foreach (var exerciseId in WorkoutViewModel.SelectedExerciseIds)
                {
                    var workoutExercise = new WorkoutExercise
                    {
                        WorkoutId = workout.Id,
                        ExerciseId = exerciseId
                    };
                    _context.WorkoutExercises.Add(workoutExercise);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Workout '{workout.Title}' created successfully!";
                return RedirectToPage("/Workouts/Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the workout. Please try again.");
                await LoadAvailableExercisesAsync();
                return Page();
            }
        }

        private async Task LoadAvailableExercisesAsync()
        {
            var exercises = await _context.Exercises
                .OrderBy(e => e.Name)
                .Select(e => new ExerciseSelectItem
                {
                    Id = e.Id,
                    Name = e.Name,
                    MaxReps = e.MaxReps,
                    MaxWeight = e.MaxWeight,
                    IsSelected = WorkoutViewModel.SelectedExerciseIds.Contains(e.Id)
                })
                .ToListAsync();

            WorkoutViewModel.AvailableExercises = exercises;
        }
    }
}
