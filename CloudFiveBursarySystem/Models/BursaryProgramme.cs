using System.ComponentModel.DataAnnotations;

namespace CloudFiveBursarySystem.Models
{
    // Represents a bursary opportunity created and managed by an administrator.
    public class BursaryProgramme
    {
        // Stores the unique ID used to identify each bursary programme.
        public int BursaryProgrammeId { get; set; }

        // Stores the name or title of the bursary programme.
        // This field is compulsory and cannot exceed 150 characters.
        [Required]
        [StringLength(150)]
        public string ProgrammeName { get; set; } = string.Empty;

        // Provides a description explaining what the bursary programme is about.
        // This field is compulsory and allows up to 1,000 characters.
        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        // Explains the conditions and requirements applicants must meet
        // before they can qualify for the bursary.
        [Required]
        [StringLength(1000)]
        public string EligibilityCriteria { get; set; } = string.Empty;

        // Stores the total funding amount allocated to the bursary programme.
        // The Range attribute prevents an amount below 0.01 or above 100 million.
        [Range(0.01, 100000000)]
        public decimal FundingAmount { get; set; }

        // Records the date when applications for the programme become available.
        [Required]
        public DateTime ApplicationOpeningDate { get; set; }

        // Records the final date on which applicants can submit their applications.
        [Required]
        public DateTime ApplicationClosingDate { get; set; }

        // Indicates the current availability of the bursary programme.
        // A programme can be Open, Closed or Suspended.
        // New programmes are automatically marked as Open.
        [Required]
        [StringLength(20)]
        public string ProgrammeStatus { get; set; } = "Open";

        // Automatically records the date and time when the programme is created.
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}