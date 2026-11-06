using System.ComponentModel.DataAnnotations;

namespace CloudFiveBursarySystem.Models
{
    // Represents an application submitted by an applicant
    // for a particular bursary programme.
    public class BursaryApplication
    {
        // Unique database ID for each application.
        public int BursaryApplicationId { get; set; }

        // Unique reference number used to track the application.
        [Required]
        [StringLength(30)]
        public string ReferenceNumber { get; set; } = string.Empty;

        // Identifies the applicant who submitted the application.
        [Required]
        public int ApplicantId { get; set; }

        // Identifies the bursary programme being applied for.
        [Required]
        public int BursaryProgrammeId { get; set; }

        // Records when the application was submitted.
        public DateTime DateSubmitted { get; set; } = DateTime.Now;

        // Tracks the application's current review status.
        [Required]
        [StringLength(30)]
        public string ApplicationStatus { get; set; } = "Pending";

        // Stores feedback provided by the administrator.
        [StringLength(1000)]
        public string? AdministratorComments { get; set; }

        // Links the application to the applicant.
        public Applicant? Applicant { get; set; }

        // Links the application to its bursary programme.
        public BursaryProgramme? BursaryProgramme { get; set; }
    }
}