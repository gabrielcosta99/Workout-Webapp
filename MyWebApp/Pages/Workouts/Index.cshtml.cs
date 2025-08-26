using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Workouts
{
    public class IndexModel : PageModel
    {
        private readonly DBContext _context;

        public IndexModel(DBContext context)
        {
            _context = context;
        }

        public IList<Workout> Workouts { get; set; } = new List<Workout>();

        public async Task OnGetAsync()
        {
            Workouts = await _context.Workouts
                .Include(w => w.WorkoutExercises)
                    .ThenInclude(we => we.Exercise)
                .OrderBy(w => w.Title)
                .ToListAsync();
        }

        // AJAX endpoint for updating exercises
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnGetGetExercisesAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid data");
                }

                var workout = await _context.Workouts.FindAsync(id);
                if (workout == null)
                {
                    return NotFound("Workout not found");
                }
                var exercises = await _context.WorkoutExercises
                    .Where(e => e.WorkoutId == id)
                    .Select(e => new
                    {
                        e.ExerciseId,
                        e.Exercise.Name,
                        e.Exercise.MaxReps,
                        e.Exercise.MaxWeight
                    })
                    .ToListAsync();


                return new JsonResult(new { success = true, exercises });
                //return new JsonResult(new { success = true, message = "Exercise updated successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception here if you have logging set up
                return BadRequest($"Error updating exercise: {ex.Message}");
            }
        }

        public async Task<IActionResult> OnGetAllExercisesAsync()
        {
            var allExercises = await _context.Exercises
                .OrderBy(e => e.Name)
                .Select(e => new { e.Id, e.Name })
                .ToListAsync();

            return new JsonResult(new { success = true, exercises = allExercises });
        }


        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostUpdateExercisesAsync(int id, List<int> ExerciseIds)
        {
            // Add debugging
            Console.WriteLine($"Updating workout {id} with exercises: {string.Join(",", ExerciseIds ?? new List<int>())}");

            if (ExerciseIds == null)
            {
                Console.WriteLine("ExerciseIds is null!");
                return BadRequest("No exercises provided");
            }

            var workout = await _context.Workouts
                .Include(w => w.WorkoutExercises)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workout == null)
            {
                Console.WriteLine($"Workout {id} not found!");
                return NotFound();
            }

            Console.WriteLine($"Found workout: {workout.Title}, current exercises: {workout.WorkoutExercises.Count}");

            // Remove old
            _context.WorkoutExercises.RemoveRange(workout.WorkoutExercises);
            Console.WriteLine("Removed old exercises");

            // Add new
            foreach (var exerciseId in ExerciseIds)
            {
                workout.WorkoutExercises.Add(new WorkoutExercise
                {
                    WorkoutId = workout.Id,
                    ExerciseId = exerciseId
                });
                Console.WriteLine($"Added exercise {exerciseId}");
            }

            await _context.SaveChangesAsync();
            Console.WriteLine("Saved changes");

            return new JsonResult(new { success = true });
        }

        // AJAX endpoint for deleting exercises
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteWorkoutAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid data");
                }

                var workout = await _context.Workouts.FindAsync(id);
                if (workout == null)
                {
                    return NotFound("Exercise not found");
                }


                var progresses = _context.WorkoutProgresses.Where(ep => ep.WorkoutId == id).ToList();
                if (progresses == null)
                {
                    return NotFound("Workout progresses not found");
                }
                foreach(var progress in progresses)
                {
                    var exerciseProgresses = _context.ExerciseProgresses.Where(ep => ep.WorkoutProgId == progress.Id);
                    _context.ExerciseProgresses.RemoveRange(exerciseProgresses);

                }

                _context.WorkoutProgresses.RemoveRange(progresses);


           
                _context.Workouts.Remove(workout);

                await _context.SaveChangesAsync();

                return new JsonResult(new { success = true, message = "Exercise updated successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception here if you have logging set up
                return BadRequest($"Error deleting workout: {ex.Message}");
            }
        }

    }    
}
