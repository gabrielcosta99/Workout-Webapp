using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Progress
{
    public class ViewWorkoutProgressModel : PageModel
    {
        [BindProperty]
        public int WorkoutId { get; set; }

        public string WorkoutTitle { get; set; }

        private readonly DBContext _context;

        public ViewWorkoutProgressModel(DBContext context)
        {
            _context = context;
        }
        public Dictionary<string, List<ExerciseProgressViewModel>> Progress { get; set; } = new Dictionary<string, List<ExerciseProgressViewModel>>();

        public void OnGet(int id)
        {
            WorkoutId = id;
            WorkoutTitle = _context.Workouts
                .Where(w => w.Id == id)
                .Select(w => w.Title)
                .FirstOrDefault() ?? string.Empty;

            // Use a projection to select only the necessary data and group it
            // into a dictionary where the key is the exercise name.
            Progress = _context.ExerciseProgresses
                .Include(ep => ep.Exercise)
                .Include(ep => ep.WorkoutProgress)
                .Where(ep => ep.WorkoutProgress.WorkoutId == id)
                .ToList()
                .GroupBy(ep => ep.Exercise.Name)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(ep => new ExerciseProgressViewModel
                    {
                        Date = ep.WorkoutProgress.Date,
                        Set = ep.Set,
                        Reps = ep.Reps,
                        Weight = ep.Weight,
                        ExerciseName = ep.Exercise.Name
                    }).ToList()
                );
        }

    }

    public class ExerciseProgressViewModel
    {
        public DateTime Date { get; set; }
        public int Set { get; set; }
        public int Reps { get; set; }
        public double? Weight { get; set; }
        public string ExerciseName { get; set; } = string.Empty;
    }
}
