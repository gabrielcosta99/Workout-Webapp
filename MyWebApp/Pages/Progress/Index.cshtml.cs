using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages.Progress
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
    }
}
