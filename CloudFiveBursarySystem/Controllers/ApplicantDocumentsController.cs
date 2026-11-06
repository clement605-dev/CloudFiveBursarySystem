using CloudFiveBursarySystem.Data;
using CloudFiveBursarySystem.Models;
using CloudFiveBursarySystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CloudFiveBursarySystem.Controllers
{
    // Only authenticated applicants can manage their own documents.
    [Authorize(Roles = "Applicant")]
    public class ApplicantDocumentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ActivityLogService _activityLogService;

        private const long MaximumFileSize = 5 * 1024 * 1024; // 5 MB

        // Documents applicants are allowed to upload.
        private static readonly string[] AllowedDocumentTypes =
        {
            "Certified ID Document / Passport",
            "Student Card",
            "Matric Results",
            "Academic Transcript",
            "Acceptance Letter",
            "Proof of Registration",
            "Latest Academic Record",
            "Proof of Residence",
            "Parent/Guardian ID",
            "Parent/Guardian Proof of Income",
            "Affidavit of Unemployment",
            "University Fee Statement",
            "Motivation Letter"
        };

        public ApplicantDocumentsController(
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            ActivityLogService activityLogService)
        {
            _context = context;
            _environment = environment;
            _activityLogService = activityLogService;
        }

        // Displays all documents belonging to the logged-in applicant.
        public async Task<IActionResult> Index()
        {
            var applicantId = await GetCurrentApplicantIdAsync();

            if (applicantId == null)
                return NotFound("Applicant profile not found.");

            var documents = await _context.ApplicantDocuments
                .Where(d => d.ApplicantId == applicantId.Value)
                .OrderByDescending(d => d.DateUploaded)
                .ToListAsync();

            ViewBag.AllowedDocumentTypes = AllowedDocumentTypes;

            return View(documents);
        }

        // Uploads a new supporting document.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(
            string documentType,
            IFormFile uploadedFile)
        {
            var applicantId = await GetCurrentApplicantIdAsync();

            if (applicantId == null)
                return NotFound("Applicant profile not found.");

            if (!AllowedDocumentTypes.Contains(documentType))
            {
                TempData["Error"] = "Please select a valid document category.";
                return RedirectToAction(nameof(Index));
            }

            if (uploadedFile == null || uploadedFile.Length == 0)
            {
                TempData["Error"] = "Please select a file to upload.";
                return RedirectToAction(nameof(Index));
            }

            if (uploadedFile.Length > MaximumFileSize)
            {
                TempData["Error"] = "The maximum allowed file size is 5 MB.";
                return RedirectToAction(nameof(Index));
            }

            var extension = Path.GetExtension(uploadedFile.FileName)
                .ToLowerInvariant();

            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(extension))
            {
                TempData["Error"] =
                    "Only PDF, JPG, JPEG and PNG files are allowed.";

                return RedirectToAction(nameof(Index));
            }

            var allowedContentTypes = new[]
            {
                "application/pdf",
                "image/jpeg",
                "image/png"
            };

            if (!allowedContentTypes.Contains(uploadedFile.ContentType))
            {
                TempData["Error"] = "The uploaded file format is not supported.";
                return RedirectToAction(nameof(Index));
            }

            // Create a private storage folder outside wwwroot.
            var storageFolder = Path.Combine(
                _environment.ContentRootPath,
                "App_Data",
                "ApplicantDocuments",
                applicantId.Value.ToString());

            Directory.CreateDirectory(storageFolder);

            // Generate a unique filename to prevent file overwriting.
            var storedFileName = Guid.NewGuid().ToString("N") + extension;

            var filePath = Path.Combine(storageFolder, storedFileName);

            try
            {
                // Save the file to private server storage.
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(stream);
                }

                var document = new ApplicantDocument
                {
                    ApplicantId = applicantId.Value,
                    DocumentName = Path.GetFileName(uploadedFile.FileName),
                    StoredFileName = storedFileName,
                    DocumentType = documentType,
                    ContentType = uploadedFile.ContentType,
                    FileSize = uploadedFile.Length,
                    DateUploaded = DateTime.Now,
                    DocumentStatus = "Pending"
                };

                _context.ApplicantDocuments.Add(document);
                await _context.SaveChangesAsync();

                // Records the successful document upload.
                await _activityLogService.LogActivityAsync(
                    "Document Uploaded",
                    $"Applicant uploaded a {documentType} document.",
                    "ApplicantDocument",
                    document.ApplicantDocumentId);

                TempData["Success"] = "Your document was uploaded successfully.";
            }
            catch
            {
                // Remove an incomplete file if saving or processing fails.
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                TempData["Error"] =
                    "Something went wrong while uploading your document.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Allows an applicant to view their own uploaded document.
        public async Task<IActionResult> Download(int id)
        {
            var applicantId = await GetCurrentApplicantIdAsync();

            if (applicantId == null)
                return NotFound();

            var document = await _context.ApplicantDocuments
                .FirstOrDefaultAsync(d =>
                    d.ApplicantDocumentId == id &&
                    d.ApplicantId == applicantId.Value);

            if (document == null)
                return NotFound();

            var filePath = Path.Combine(
                _environment.ContentRootPath,
                "App_Data",
                "ApplicantDocuments",
                applicantId.Value.ToString(),
                document.StoredFileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("The stored document could not be found.");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            // Records the successful document download.
            await _activityLogService.LogActivityAsync(
                "Document Downloaded",
                $"Applicant downloaded a {document.DocumentType} document.",
                "ApplicantDocument",
                document.ApplicantDocumentId);

            return File(
                fileBytes,
                document.ContentType,
                document.DocumentName);
        }

        // Allows an applicant to delete their own uploaded document.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var applicantId = await GetCurrentApplicantIdAsync();

            if (applicantId == null)
                return NotFound();

            var document = await _context.ApplicantDocuments
                .FirstOrDefaultAsync(d =>
                    d.ApplicantDocumentId == id &&
                    d.ApplicantId == applicantId.Value);

            if (document == null)
                return NotFound();

            var filePath = Path.Combine(
                _environment.ContentRootPath,
                "App_Data",
                "ApplicantDocuments",
                applicantId.Value.ToString(),
                document.StoredFileName);

            // Store the document type before deleting the database record.
            var deletedDocumentType = document.DocumentType;
            var deletedDocumentId = document.ApplicantDocumentId;

            _context.ApplicantDocuments.Remove(document);
            await _context.SaveChangesAsync();

            // Remove the physical file after removing its database record.
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            // Records the successful document deletion.
            await _activityLogService.LogActivityAsync(
                "Document Deleted",
                $"Applicant deleted a {deletedDocumentType} document.",
                "ApplicantDocument",
                deletedDocumentId);

            TempData["Success"] = "Document deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Finds the applicant profile belonging to the logged-in user.
        private async Task<int?> GetCurrentApplicantIdAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return null;

            var applicant = await _context.Applicants
                .FirstOrDefaultAsync(a => a.UserId == userId);

            return applicant?.ApplicantId;
        }
    }
}
