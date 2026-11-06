using CloudFiveBursarySystem.Data;
using CloudFiveBursarySystem.Models;
using CloudFiveBursarySystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudFiveBursarySystem.Controllers
{
    // Only administrators are allowed to manage applicant documents.
    [Authorize(Roles = "Administrator")]
    public class AdminDocumentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly NotificationService _notificationService;

        // Constructor injects the database, file storage and notification service.
        public AdminDocumentsController(
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            NotificationService notificationService)
        {
            _context = context;
            _environment = environment;
            _notificationService = notificationService;
        }

        // Displays all documents uploaded by applicants.
        public async Task<IActionResult> Index()
        {
            var documents = await _context.ApplicantDocuments
                .Include(d => d.Applicant)
                .OrderByDescending(d => d.DateUploaded)
                .ToListAsync();

            return View(documents);
        }

        // Allows an administrator to download an applicant's document.
        public async Task<IActionResult> Download(int id)
        {
            var document = await _context.ApplicantDocuments
                .FirstOrDefaultAsync(d => d.ApplicantDocumentId == id);

            if (document == null)
                return NotFound("Document not found.");

            var storageFolder = Path.Combine(
                _environment.ContentRootPath,
                "App_Data",
                "ApplicantDocuments",
                document.ApplicantId.ToString());

            var filePath = Path.GetFullPath(
                Path.Combine(storageFolder, document.StoredFileName));

            // Ensure the requested file remains inside the applicant's folder.
            var fullStorageFolder = Path.GetFullPath(storageFolder)
                + Path.DirectorySeparatorChar;

            if (!filePath.StartsWith(
                fullStorageFolder,
                StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Invalid file location.");
            }

            if (!System.IO.File.Exists(filePath))
                return NotFound("The stored document could not be found.");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(
                fileBytes,
                document.ContentType,
                document.DocumentName);
        }

        // Marks an applicant's document as verified and sends a notification.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(int id)
        {
            var document = await _context.ApplicantDocuments
                .FirstOrDefaultAsync(d => d.ApplicantDocumentId == id);

            if (document == null)
                return NotFound("Document not found.");

            document.DocumentStatus = "Verified";
            document.AdminComment = null;

            await _context.SaveChangesAsync();

            // Notify the applicant that their document has been verified.
            await _notificationService.CreateNotificationAsync(
                document.ApplicantId,
                "Document Verified",
                $"Your {document.DocumentType} document has been successfully verified.",
                "Document",
                "/ApplicantDocuments");

            TempData["Success"] = "Document successfully verified. The applicant has been notified.";

            return RedirectToAction(nameof(Index));
        }

        // Rejects a document and records the administrator's reason.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string adminComment)
        {
            var document = await _context.ApplicantDocuments
                .FirstOrDefaultAsync(d => d.ApplicantDocumentId == id);

            if (document == null)
                return NotFound("Document not found.");

            // A rejection must include an explanation for the applicant.
            if (string.IsNullOrWhiteSpace(adminComment))
            {
                TempData["Error"] = "Please provide a reason for rejecting the document.";
                return RedirectToAction(nameof(Index));
            }

            if (adminComment.Trim().Length > 1000)
            {
                TempData["Error"] = "The rejection reason cannot exceed 1000 characters.";
                return RedirectToAction(nameof(Index));
            }

            document.DocumentStatus = "Rejected";
            document.AdminComment = adminComment.Trim();

            await _context.SaveChangesAsync();

            // Notify the applicant and explain why the document was rejected.
            await _notificationService.CreateNotificationAsync(
                document.ApplicantId,
                "Document Rejected",
                $"Your {document.DocumentType} document was rejected. Reason: {document.AdminComment}",
                "Document",
                "/ApplicantDocuments");

            TempData["Success"] = "Document rejected. The applicant has been notified.";

            return RedirectToAction(nameof(Index));
        }
    }
}