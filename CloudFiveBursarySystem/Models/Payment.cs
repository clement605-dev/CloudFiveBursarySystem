using System.ComponentModel.DataAnnotations;

namespace CloudFiveBursarySystem.Models
{
    // Represents a payment record associated with a bursary beneficiary.
    public class Payment
    {
        // Unique identifier for each payment record.
        public int PaymentId { get; set; }

        // Links the payment to an existing beneficiary.
        [Required]
        public int BeneficiaryId { get; set; }

        // Unique reference used to identify the payment.
        [Required]
        [StringLength(50)]
        public string PaymentReference { get; set; } = string.Empty;

        // Amount recorded as paid or scheduled for payment.
        [Required]
        [Range(0.01, 100000000)]
        public decimal PaymentAmount { get; set; }

        // Date associated with the payment.
        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        // Current status of the payment.
        [Required]
        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Pending";

        // Optional notes about the payment.
        [StringLength(500)]
        public string? Description { get; set; }

        // Navigation property connecting the payment to its beneficiary.
        public Beneficiary? Beneficiary { get; set; }
    }
}