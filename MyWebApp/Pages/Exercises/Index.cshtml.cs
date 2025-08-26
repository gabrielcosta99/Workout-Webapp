using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using System.Text.Json;

namespace MyWebApp.Pages.Exercises
{
    public class IndexModel : PageModel
    {
        private readonly DBContext _context;

        public IndexModel(DBContext context)
        {
            _context = context;
        }

        public IList<Exercise> Exercises { get; set; } = new List<Exercise>();

        public async Task OnGetAsync()
        {
            Exercises = await _context.Exercises
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        // AJAX endpoint for updating exercises
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostUpdateExerciseAsync(int id, string name, int? maxReps, double? maxWeight)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid data");
                }

                var exercise = await _context.Exercises.FindAsync(id);
                if (exercise == null)
                {
                    return NotFound("Exercise not found");
                }

                // Update properties if provided
                if (!string.IsNullOrEmpty(name))
                {
                    exercise.Name = name;
                }

                if (maxReps.HasValue)
                {
                    exercise.MaxReps = maxReps.Value;
                }

                if (maxWeight.HasValue)
                {
                    exercise.MaxWeight = maxWeight.Value;
                }

                await _context.SaveChangesAsync();

                return new JsonResult(new { success = true, message = "Exercise updated successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception here if you have logging set up
                return BadRequest($"Error updating exercise: {ex.Message}");
            }
        }


        // AJAX endpoint for deleting exercises
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteExerciseAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid data");
                }

                var exercise = await _context.Exercises.FindAsync(id);
                if (exercise == null)
                {
                    return NotFound("Exercise not found");
                }

                _context.Exercises.Remove(exercise);

                await _context.SaveChangesAsync();

                return new JsonResult(new { success = true, message = "Exercise updated successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception here if you have logging set up
                return BadRequest($"Error updating exercise: {ex.Message}");
            }
        }

    }


    // DTO for the update request
    public class ExerciseUpdateDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? MaxReps { get; set; }
        public double? MaxWeight { get; set; }
    }
}