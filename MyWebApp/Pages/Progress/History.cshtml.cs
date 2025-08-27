using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MyWebApp.Pages.Progress
{
    public class HistoryModel : PageModel
    {

        private readonly DBContext _context;
        public HistoryModel(DBContext context)
        {
            _context = context;
        }

        public IList<WorkoutProgress> Progresses { get; set; } = new List<WorkoutProgress>();
        public async Task OnGetAsync()
        {
            Progresses = await _context.WorkoutProgresses
                .Include(w => w.Workout)
              .Include(w => w.ExerciseProgresses)
              .ToListAsync();

         
        }


        // AJAX endpoint for updating exercises
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnGetGetProgExercisesAsync(int id)
        {
            Console.WriteLine("getprogex");
            try
            {
                Console.WriteLine($"WorkoutId: {id}");
                if (id <= 0)
                {
                    return BadRequest("Invalid data");
                }

                var workoutProg = await _context.WorkoutProgresses.FindAsync(id);
                if (workoutProg == null)
                {
                    return NotFound("Workout not found");
                }
                var exercisesProg = await _context.ExerciseProgresses
                    //.Include(ep => ep.Exercise)
                    .Where(ep => ep.WorkoutProgId == id)
                    .GroupBy(ep => new { ep.ExerciseId, ep.Exercise.Name})
                    .Select(g => new
                    {
                        ExerciseId = g.Key.ExerciseId,
                        ExerciseName = g.Key.Name,
                        Sets = g.Select(ep => new{
                            ep.Set,
                            ep.Reps,
                            ep.Weight
                        })
                        
                    })
                    .ToListAsync();
                Console.WriteLine(exercisesProg.Count);
                return new JsonResult(new { success = true, exercisesProg });
                //return new JsonResult(new { success = true, message = "Exercise updated successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception here if you have logging set up
                return BadRequest($"Error getting progress exercises: {ex.Message}");

            }
        }


        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostUpdateProgressAsync([FromBody] UpdateProgressRequest updateProgressRequest)
        {
            var workoutProgId = updateProgressRequest.WorkoutProgId;
            var progressData = updateProgressRequest.ProgressData;
            // Add debugging
            Console.WriteLine($"Updating workout {workoutProgId} with exercise: {progressData[0]}");

            if (progressData == null)
            {
                Console.WriteLine("No progress data provided!");
                return BadRequest("No data provided");
            }

            var workoutProg = await _context.WorkoutProgresses
                .Include(w => w.ExerciseProgresses)
                .FirstOrDefaultAsync(w => w.Id == workoutProgId);

            if (workoutProg == null)
            {
                Console.WriteLine($"Workout progress {workoutProgId} not found!");
                return NotFound();
            }
            //var test = _context.ExerciseProgresses
            //    .OrderBy(e => e.Id)
            //    .LastOrDefault(e => e.ExerciseId == progressData[0].ExerciseId);
            //Console.WriteLine($"QUERY: ------{test.Id}");


            // Add new
            foreach (var progress in progressData)
            {
                var exercise = await _context.Exercises.FindAsync(progress.ExerciseId);
                if (exercise == null)
                {
                    return BadRequest($"Exercise with ID {progress.ExerciseId} not found");
                }

                var existingProgresses = _context.ExerciseProgresses.Where(ep => ep.WorkoutProgId == workoutProgId && ep.ExerciseId == progress.ExerciseId);

                var data = progress.data;
                var set1 = existingProgresses.FirstOrDefault(ep => ep.Set == 1);
                if (set1 != null)
                {
                    set1.Reps = data.reps1;
                    set1.Weight = data.weight1;
                    _context.ExerciseProgresses.Update(set1);
                }
                if (progress.data.weight1 > exercise.MaxWeight || (progress.data.weight1 == exercise.MaxWeight && progress.data.reps1 > exercise.MaxReps))
                {
                    exercise.MaxWeight = progress.data.weight1;
                    exercise.MaxReps = progress.data.reps1;
                }

                var set2 = existingProgresses.FirstOrDefault(ep => ep.Set == 2);
                if (set2 != null)
                {
                    set2.Reps = data.reps2;
                    set2.Weight = data.weight2;
                    _context.ExerciseProgresses.Update(set2);
                }
                if (progress.data.weight2 > exercise.MaxWeight || (progress.data.weight2 == exercise.MaxWeight && progress.data.reps2 > exercise.MaxReps))
                {
                    exercise.MaxWeight = progress.data.weight2;
                    exercise.MaxReps = progress.data.reps2;
                }


                var set3 = existingProgresses.FirstOrDefault(ep => ep.Set == 3);
                if (set3 != null)
                {
                    set3.Reps = data.reps3;
                    set3.Weight = data.weight3;
                    _context.ExerciseProgresses.Update(set3);
                }
                if (progress.data.weight3 > exercise.MaxWeight || (progress.data.weight3 == exercise.MaxWeight && progress.data.reps3 > exercise.MaxReps))
                {
                    exercise.MaxWeight = progress.data.weight3;
                    exercise.MaxReps = progress.data.reps3;
                }

                _context.Exercises.Update(exercise);

            }
            ;
            await _context.SaveChangesAsync();
            Console.WriteLine("Saved changes");

            return new JsonResult(new { success = true });
        }



        // AJAX endpoint for deleting exercises
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteProgressAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid data");
                }

                var progress = await _context.WorkoutProgresses.FindAsync(id);
                if (progress == null)
                {
                    return NotFound("Workout progress not found");
                }

                var exerciseProgresses = _context.ExerciseProgresses.Where(ep => ep.WorkoutProgId == id);

                _context.ExerciseProgresses.RemoveRange(exerciseProgresses);
                _context.WorkoutProgresses.Remove(progress);

                await _context.SaveChangesAsync();

                return new JsonResult(new { success = true, message = "Exercise updated successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception here if you have logging set up
                return BadRequest($"Error updating exercise: {ex.Message}");
            }
        }


        public class UpdateProgressRequest
        {
            public int WorkoutProgId { get; set; }
            public List<WorkoutProgDto> ProgressData { get; set; }
        }
        public class WorkoutProgDto
        {
            public int ExerciseId { get; set; }
            public SetData data { get; set; }
        }

        public class SetData
        {
            
            public int reps1 { get; set; }
            public double weight1 { get; set; }
            public int reps2 { get; set; }
            public double weight2 { get; set; }
            public int reps3 { get; set; }
            public double weight3 { get; set; }
        }
    }
}
