using CloudFiveBursarySystem.Data;
using CloudFiveBursarySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CloudFiveBursarySystem.Controllers
{
    // Only administrators can manage bursary payment records.
    [Authorize(Roles = "Administrator")]
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Receives the database context through dependency injection.
        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Displays all payment records.
        public async Task<IActionResult> Index()
        {
            var payments = await _context.Payments
                .Include(p => p.Beneficiary)
                    .ThenInclude(b => b!.BursaryApplication)
                        .ThenInclude(a => a!.Applicant)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return View(payments);
        }

        // Displays the form for recording a new payment.
        public async Task<IActionResult> Create()
        {
            await LoadBeneficiariesAsync();

            return View();
        }

        // Saves a new payment record.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment payment)
        {
            // Confirms that the selected beneficiary exists.
            var beneficiaryExists = await _context.Beneficiaries
                .AnyAsync(b => b.BeneficiaryId == payment.BeneficiaryId);

            if (!beneficiaryExists)
            {
                ModelState.AddModelError(
                    nameof(payment.BeneficiaryId),
                    "Please select a valid beneficiary.");
            }

            // Only supported payment statuses are accepted.
            string[] allowedStatuses = { "Pending", "Paid", "Failed" };

            if (!allowedStatuses.Contains(payment.PaymentStatus))
            {
                ModelState.AddModelError(
                    nameof(payment.PaymentStatus),
                    "Please select a valid payment status.");
            }

            // Prevents duplicate payment reference numbers.
            bool referenceExists = await _context.Payments
                .AnyAsync(p => p.PaymentReference == payment.PaymentReference);

            if (referenceExists)
            {
                ModelState.AddModelError(
                    nameof(payment.PaymentReference),
                    "This payment reference already exists.");
            }

            // Redisplays the form if validation fails.
            if (!ModelState.IsValid)
            {
                await LoadBeneficiariesAsync(payment.BeneficiaryId);

                return View(payment);
            }

            // Adds the payment record to the database.
            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Payment record successfully created.";

            return RedirectToAction(nameof(Index));
        }

        // Displays the details of a selected payment.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Beneficiary)
                    .ThenInclude(b => b!.BursaryApplication)
                        .ThenInclude(a => a!.Applicant)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // Displays the form for updating an existing payment.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null)
            {
                return NotFound();
            }

            await LoadBeneficiariesAsync(payment.BeneficiaryId);

            return View(payment);
        }

        // Saves changes made to an existing payment.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Payment payment)
        {
            if (id != payment.PaymentId)
            {
                return NotFound();
            }

            bool beneficiaryExists = await _context.Beneficiaries
                .AnyAsync(b => b.BeneficiaryId == payment.BeneficiaryId);

            if (!beneficiaryExists)
            {
                ModelState.AddModelError(
                    nameof(payment.BeneficiaryId),
                    "Please select a valid beneficiary.");
            }

            string[] allowedStatuses = { "Pending", "Paid", "Failed" };

            if (!allowedStatuses.Contains(payment.PaymentStatus))
            {
                ModelState.AddModelError(
                    nameof(payment.PaymentStatus),
                    "Please select a valid payment status.");
            }

            bool referenceExists = await _context.Payments
                .AnyAsync(p =>
                    p.PaymentReference == payment.PaymentReference &&
                    p.PaymentId != payment.PaymentId);

            if (referenceExists)
            {
                ModelState.AddModelError(
                    nameof(payment.PaymentReference),
                    "This payment reference already exists.");
            }

            if (!ModelState.IsValid)
            {
                await LoadBeneficiariesAsync(payment.BeneficiaryId);

                return View(payment);
            }

            _context.Update(payment);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Payment record successfully updated.";

            return RedirectToAction(nameof(Index));
        }

        // Loads registered beneficiaries into a dropdown list.
        private async Task LoadBeneficiariesAsync(int? selectedBeneficiaryId = null)
        {
            var beneficiaries = await _context.Beneficiaries
                .Include(b => b.BursaryApplication)
                    .ThenInclude(a => a!.Applicant)
                .OrderBy(b => b.BeneficiaryId)
                .ToListAsync();

            var beneficiaryOptions = beneficiaries.Select(b => new
            {
                b.BeneficiaryId,
                DisplayName =
                    $"{b.BursaryApplication?.Applicant?.FirstName} " +
                    $"{b.BursaryApplication?.Applicant?.LastName} " +
                    $"- Beneficiary #{b.BeneficiaryId}"
            });

            ViewBag.BeneficiaryId = new SelectList(
                beneficiaryOptions,
                "BeneficiaryId",
                "DisplayName",
                selectedBeneficiaryId);
        }
    }
}