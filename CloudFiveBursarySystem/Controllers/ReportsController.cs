using ClosedXML.Excel;
using CloudFiveBursarySystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudFiveBursarySystem.Controllers
{
    // Only administrators can access reports and export information.
    [Authorize(Roles = "Administrator")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReportsController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Displays the main reports dashboard.
        public IActionResult Index()
        {
            return View();
        }

        // Displays registered applicants, excluding administrator accounts.
        public async Task<IActionResult> Applicants()
        {
            var administrators = await _userManager
                .GetUsersInRoleAsync("Administrator");

            var administratorIds = administrators
                .Select(user => user.Id)
                .ToList();

            var applicants = await _context.Applicants
                .Where(a => !administratorIds.Contains(a.UserId))
                .OrderBy(a => a.LastName)
                .ThenBy(a => a.FirstName)
                .ToListAsync();

            return View(applicants);
        }

        // Displays submitted bursary applications.
        public async Task<IActionResult> Applications()
        {
            var applications = await _context.BursaryApplications
                .Include(a => a.Applicant)
                .Include(a => a.BursaryProgramme)
                .OrderByDescending(a => a.DateSubmitted)
                .ToListAsync();

            return View(applications);
        }

        // Displays recorded bursary payments.
        public async Task<IActionResult> Payments()
        {
            var payments = await _context.Payments
                .Include(p => p.Beneficiary)
                    .ThenInclude(b => b!.BursaryApplication)
                        .ThenInclude(a => a!.Applicant)
                .Include(p => p.Beneficiary)
                    .ThenInclude(b => b!.BursaryApplication)
                        .ThenInclude(a => a!.BursaryProgramme)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return View(payments);
        }

        // Exports registered applicants to an Excel spreadsheet.
        public async Task<IActionResult> ExportApplicants()
        {
            var administrators = await _userManager
                .GetUsersInRoleAsync("Administrator");

            var administratorIds = administrators
                .Select(user => user.Id)
                .ToList();

            var applicants = await _context.Applicants
                .Where(a => !administratorIds.Contains(a.UserId))
                .OrderBy(a => a.LastName)
                .ThenBy(a => a.FirstName)
                .ToListAsync();

            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Applicant Report");

            worksheet.Cell(1, 1).Value = "Applicant Name";
            worksheet.Cell(1, 2).Value = "ID Number";
            worksheet.Cell(1, 3).Value = "Email Address";
            worksheet.Cell(1, 4).Value = "Phone Number";
            worksheet.Cell(1, 5).Value = "Date Registered";

            int row = 2;

            foreach (var applicant in applicants)
            {
                worksheet.Cell(row, 1).Value =
                    $"{applicant.FirstName} {applicant.LastName}";

                worksheet.Cell(row, 2).Value = applicant.IDNumber;
                worksheet.Cell(row, 3).Value = applicant.Email;
                worksheet.Cell(row, 4).Value = applicant.PhoneNumber;
                worksheet.Cell(row, 5).Value = applicant.DateCreated;

                row++;
            }

            StyleWorksheet(worksheet, 5);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "CLOUD5IVE_Applicant_Report.xlsx");
        }

        // Exports bursary applications to an Excel spreadsheet.
        public async Task<IActionResult> ExportApplications()
        {
            var applications = await _context.BursaryApplications
                .Include(a => a.Applicant)
                .Include(a => a.BursaryProgramme)
                .OrderByDescending(a => a.DateSubmitted)
                .ToListAsync();

            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Application Report");

            worksheet.Cell(1, 1).Value = "Reference Number";
            worksheet.Cell(1, 2).Value = "Applicant";
            worksheet.Cell(1, 3).Value = "Bursary Programme";
            worksheet.Cell(1, 4).Value = "Submission Date";
            worksheet.Cell(1, 5).Value = "Application Status";
            worksheet.Cell(1, 6).Value = "Administrator Comments";

            int row = 2;

            foreach (var application in applications)
            {
                worksheet.Cell(row, 1).Value = application.ReferenceNumber;

                worksheet.Cell(row, 2).Value =
                    $"{application.Applicant?.FirstName} {application.Applicant?.LastName}";

                worksheet.Cell(row, 3).Value =
                    application.BursaryProgramme?.ProgrammeName ?? "N/A";

                worksheet.Cell(row, 4).Value = application.DateSubmitted;
                worksheet.Cell(row, 5).Value = application.ApplicationStatus;

                worksheet.Cell(row, 6).Value =
                    application.AdministratorComments ?? "No comments";

                row++;
            }

            StyleWorksheet(worksheet, 6);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "CLOUD5IVE_Application_Report.xlsx");
        }

        // Exports bursary payment records to an Excel spreadsheet.
        public async Task<IActionResult> ExportPayments()
        {
            var payments = await _context.Payments
                .Include(p => p.Beneficiary)
                    .ThenInclude(b => b!.BursaryApplication)
                        .ThenInclude(a => a!.Applicant)
                .Include(p => p.Beneficiary)
                    .ThenInclude(b => b!.BursaryApplication)
                        .ThenInclude(a => a!.BursaryProgramme)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Payment Report");

            worksheet.Cell(1, 1).Value = "Payment Reference";
            worksheet.Cell(1, 2).Value = "Beneficiary";
            worksheet.Cell(1, 3).Value = "Bursary Programme";
            worksheet.Cell(1, 4).Value = "Payment Amount";
            worksheet.Cell(1, 5).Value = "Payment Date";
            worksheet.Cell(1, 6).Value = "Payment Status";
            worksheet.Cell(1, 7).Value = "Description";

            int row = 2;

            foreach (var payment in payments)
            {
                worksheet.Cell(row, 1).Value = payment.PaymentReference;

                worksheet.Cell(row, 2).Value =
                    $"{payment.Beneficiary?.BursaryApplication?.Applicant?.FirstName} " +
                    $"{payment.Beneficiary?.BursaryApplication?.Applicant?.LastName}";

                worksheet.Cell(row, 3).Value =
                    payment.Beneficiary?.BursaryApplication?.BursaryProgramme?.ProgrammeName ?? "N/A";

                worksheet.Cell(row, 4).Value = payment.PaymentAmount;
                worksheet.Cell(row, 5).Value = payment.PaymentDate;
                worksheet.Cell(row, 6).Value = payment.PaymentStatus;
                worksheet.Cell(row, 7).Value = payment.Description ?? "No description";

                row++;
            }

            worksheet.Column(4).Style.NumberFormat.Format = "\"R\" #,##0.00";

            StyleWorksheet(worksheet, 7);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "CLOUD5IVE_Payment_Report.xlsx");
        }

        // Applies consistent formatting to exported Excel worksheets.
        private void StyleWorksheet(IXLWorksheet worksheet, int columnCount)
        {
            var header = worksheet.Range(1, 1, 1, columnCount);

            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.LightBlue;
            header.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            worksheet.Columns().AdjustToContents();

            worksheet.SheetView.FreezeRows(1);
        }
    }
}