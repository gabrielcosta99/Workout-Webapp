using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using MyWebApp.Data;
using MyWebApp.Models;
using System;

namespace MyWebApp.Pages.Progress
{
    public class WorkoutProgressModel : PageModel
    {

        [BindProperty]
        public int WorkoutId { get; set; }

        public string WorkoutTitle { get; set; }

        private readonly DBContext _context;
        
        public WorkoutProgressModel(DBContext context)
        {
            _context = context;
        }
        public IList<Exercise> Exercises { get; set; } = new List<Exercise>();

        public void OnGet(int id)
        {
            WorkoutId = id;
            Exercises = _context.Exercises
                .Include(e => e.WorkoutExercises)
                .Where(e => e.WorkoutExercises.Any(we => we.WorkoutId == id))
                .ToList();

            WorkoutTitle = _context.Workouts
                .Where(w => w.Id == id)
                .Select(w => w.Title)
                .FirstOrDefault() ?? "";

        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostSubmitProgressAsync([FromBody] List<WorkoutProgDto> progressData)
        {

            Console.WriteLine($"WorkoutId: {progressData[0].WorkoutId}");
            Console.WriteLine(progressData.Count);
            if (progressData == null || !progressData.Any())
            {
                return BadRequest("No progress data provided");
            }
            var workoutProg = new WorkoutProgress
            {
                Date = DateTime.Now,
                WorkoutId = progressData[0].WorkoutId
            };
            _context.WorkoutProgresses.Add(workoutProg);
            await _context.SaveChangesAsync(); // Save to get the generated ID
            foreach (var progress in progressData)
            {
                

                // Now add ExerciseProgress entries
                Console.WriteLine($"Processing Workout progress id: {workoutProg.Id}");
                Console.WriteLine($"Processing ExerciseId: {progress.ExerciseId}, Reps1: {progress.data.reps1}, Weight1: {progress.data.weight1}");
                var exercise = await _context.Exercises.FindAsync(progress.ExerciseId);
                if (exercise == null)
                {
                    return BadRequest($"Exercise with ID {progress.ExerciseId} not found");
                }
                var entry1 = new ExerciseProgress
                {
                    WorkoutProgId = workoutProg.Id,
                    ExerciseId = progress.ExerciseId,
                    Set = 1,
                    Reps = progress.data.reps1,
                    Weight = progress.data.weight1
                };
                if(progress.data.weight1 > exercise.MaxWeight || (progress.data.weight1 == exercise.MaxWeight && progress.data.reps1 > exercise.MaxReps))
                {
                    exercise.MaxWeight = progress.data.weight1;
                    exercise.MaxReps = progress.data.reps1;
                }

                var entry2 = new ExerciseProgress
                {
                    WorkoutProgId = workoutProg.Id,
                    ExerciseId = progress.ExerciseId,
                    Set = 2,
                    Reps = progress.data.reps2,
                    Weight = progress.data.weight2
                };
                if (progress.data.weight2 > exercise.MaxWeight || (progress.data.weight2 == exercise.MaxWeight && progress.data.reps2 > exercise.MaxReps))
                {
                    exercise.MaxWeight = progress.data.weight2;
                    exercise.MaxReps = progress.data.reps2;
                }

                var entry3 = new ExerciseProgress
                {
                    WorkoutProgId = workoutProg.Id,
                    ExerciseId = progress.ExerciseId,
                    Set = 3,
                    Reps = progress.data.reps3,
                    Weight = progress.data.weight3
                };
                if (progress.data.weight3 > exercise.MaxWeight || (progress.data.weight3 == exercise.MaxWeight && progress.data.reps3 > exercise.MaxReps))
                {
                    exercise.MaxWeight = progress.data.weight3;
                    exercise.MaxReps = progress.data.reps3;
                }

                _context.Exercises.Update(exercise);

                _context.ExerciseProgresses.Add(entry1);
                _context.ExerciseProgresses.Add(entry2);
                _context.ExerciseProgresses.Add(entry3);
            }
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }

    }
    public class WorkoutProgDto
    {
        public int WorkoutId { get; set; }
        public int ExerciseId { get; set; }
        public ExerciseDto data { get; set; }
    }

    public class ExerciseDto
    {
        public double weight1 { get; set; }
        public double weight2 { get; set; }
        public double weight3 { get; set; }
        public int reps1 { get; set; }
        public int reps2 { get; set; }
        public int reps3 { get; set; }

    }
}
