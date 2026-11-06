using System.ComponentModel.DataAnnotations;

namespace CloudFiveBursarySystem.Models
{
    // Represents an applicant who has been approved
    // and registered as a bursary beneficiary.
    public class Beneficiary
    {
        // Unique database ID for each beneficiary record.
        public int BeneficiaryId { get; set; }

        // Connects the beneficiary to the approved application.
        [Required]
        public int BursaryApplicationId { get; set; }

        // Records when the applicant was registered as a beneficiary.
        public DateTime DateAdded { get; set; } = DateTime.Now;

        // Stores the beneficiary's current funding status.
        [Required]
        [StringLength(30)]
        public string BeneficiaryStatus { get; set; } = "Active";

        // Records the funding amount allocated to the beneficiary.
        [Range(0.01, 100000000)]
        public decimal FundingAmount { get; set; }

        // Connects the beneficiary to the related application.
        public BursaryApplication? BursaryApplication { get; set; }
    }
}