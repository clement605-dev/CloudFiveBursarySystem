using System.ComponentModel.DataAnnotations;

namespace CloudFiveBursarySystem.ViewModels
{
    // Holds the information an administrator needs
    // when reviewing an applicant's bursary application.
    public class ReviewApplicationViewModel
    {
        // Identifies the application being reviewed.
        public int BursaryApplicationId { get; set; }

        // Displays the unique application reference number.
        public string ReferenceNumber { get; set; } = string.Empty;

        // Displays the applicant's full name.
        public string ApplicantName { get; set; } = string.Empty;

        // Displays the applicant's email address.
        public string ApplicantEmail { get; set; } = string.Empty;

        // Displays the bursary programme name.
        public string ProgrammeName { get; set; } = string.Empty;

        // Displays when the application was submitted.
        public DateTime DateSubmitted { get; set; }

        // Allows the administrator to select a new application status.
        [Required]
        public string ApplicationStatus { get; set; } = "Pending";

        // Allows the administrator to provide feedback to the applicant.
        [StringLength(1000)]
        public string? AdministratorComments { get; set; }
    }
}