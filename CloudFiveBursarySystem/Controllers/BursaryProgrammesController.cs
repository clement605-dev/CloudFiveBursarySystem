using CloudFiveBursarySystem.Data;
using CloudFiveBursarySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudFiveBursarySystem.Controllers
{
    // Restricts access to this controller to users assigned the Administrator role.
    // Applicants cannot create, edit or delete bursary programmes.
    [Authorize(Roles = "Administrator")]
    public class BursaryProgrammesController : Controller
    {
        // Provides access to the database tables through Entity Framework Core.
        private readonly ApplicationDbContext _context;

        // Receives the database context through dependency injection.
        public BursaryProgrammesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Displays all bursary programmes stored in the database.
        public async Task<IActionResult> Index()
        {
            // Retrieves all programmes and displays the newest ones first.
            var programmes = await _context.BursaryProgrammes
                .OrderByDescending(p => p.DateCreated)
                .ToListAsync();

            // Sends the retrieved programmes to the Index view.
            return View(programmes);
        }

        // Displays the details of a specific bursary programme.
        public async Task<IActionResult> Details(int? id)
        {
            // Checks whether a programme ID was provided.
            if (id == null)
            {
                return NotFound();
            }

            // Searches the database for the selected programme.
            var programme = await _context.BursaryProgrammes
                .FirstOrDefaultAsync(p =>
                    p.BursaryProgrammeId == id);

            // Returns a not-found response if the programme does not exist.
            if (programme == null)
            {
                return NotFound();
            }

            // Displays the programme information.
            return View(programme);
        }

        // Displays an empty form where the administrator can create a programme.
        public IActionResult Create()
        {
            return View();
        }

        // Receives and saves a new bursary programme submitted through the form.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BursaryProgramme programme)
        {
            // Ensures that the closing date is not earlier than the opening date.
            if (programme.ApplicationClosingDate <
                programme.ApplicationOpeningDate)
            {
                ModelState.AddModelError(
                    "ApplicationClosingDate",
                    "The closing date cannot be earlier than the opening date.");
            }

            // Checks whether the submitted programme information is valid.
            if (ModelState.IsValid)
            {
                // Adds the new programme to the database.
                _context.BursaryProgrammes.Add(programme);

                // Saves the new programme permanently in SQL Server.
                await _context.SaveChangesAsync();

                // Returns the administrator to the programme list.
                return RedirectToAction(nameof(Index));
            }

            // If validation fails, redisplays the form with the submitted information.
            return View(programme);
        }

        // Retrieves an existing programme so the administrator can edit it.
        public async Task<IActionResult> Edit(int? id)
        {
            // Checks whether a programme ID was provided.
            if (id == null)
            {
                return NotFound();
            }

            // Searches for the programme using its primary key.
            var programme = await _context.BursaryProgrammes
                .FindAsync(id);

            // Checks whether the selected programme exists.
            if (programme == null)
            {
                return NotFound();
            }

            // Displays the edit form with the existing programme information.
            return View(programme);
        }

        // Receives and saves changes made to an existing bursary programme.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, BursaryProgramme programme)
        {
            // Confirms that the URL ID matches the programme being edited.
            if (id != programme.BursaryProgrammeId)
            {
                return NotFound();
            }

            // Checks that the closing date is not earlier than the opening date.
            if (programme.ApplicationClosingDate <
                programme.ApplicationOpeningDate)
            {
                ModelState.AddModelError(
                    "ApplicationClosingDate",
                    "The closing date cannot be earlier than the opening date.");
            }

            // Saves the changes only if all validation checks pass.
            if (ModelState.IsValid)
            {
                // Marks the programme as updated in the database.
                _context.Update(programme);

                // Saves the updated information in SQL Server.
                await _context.SaveChangesAsync();

                // Returns to the list of bursary programmes.
                return RedirectToAction(nameof(Index));
            }

            // Redisplays the edit form if validation fails.
            return View(programme);
        }

        // Displays a confirmation page before deleting a bursary programme.
        public async Task<IActionResult> Delete(int? id)
        {
            // Checks whether a programme ID was provided.
            if (id == null)
            {
                return NotFound();
            }

            // Retrieves the programme selected for deletion.
            var programme = await _context.BursaryProgrammes
                .FirstOrDefaultAsync(p =>
                    p.BursaryProgrammeId == id);

            // Returns a not-found response if the programme does not exist.
            if (programme == null)
            {
                return NotFound();
            }

            // Displays the deletion confirmation page.
            return View(programme);
        }

        // Deletes the selected programme after the administrator confirms.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Searches for the programme that must be deleted.
            var programme = await _context.BursaryProgrammes
                .FindAsync(id);

            // Checks whether the programme still exists.
            if (programme != null)
            {
                // Marks the programme for deletion.
                _context.BursaryProgrammes.Remove(programme);

                // Applies the deletion to the SQL Server database.
                await _context.SaveChangesAsync();
            }

            // Returns to the bursary programme list.
            return RedirectToAction(nameof(Index));
        }
    }
}