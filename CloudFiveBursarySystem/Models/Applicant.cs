using System.ComponentModel.DataAnnotations;

namespace CloudFiveBursarySystem.Models
{
    // Represents an applicant who registers for the organisation's bursary programme.
    public class Applicant
    {
        // Unique ID used to identify each applicant in the database.
        public int ApplicantId { get; set; }

        // Connects the applicant's profile to their ASP.NET Identity account.
        [Required]
        public string UserId { get; set; } = string.Empty;

        // Applicant's first name.
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        // Applicant's surname.
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        // Applicant's identification number.
        [Required]
        [StringLength(20)]
        public string IDNumber { get; set; } = string.Empty;

        // Applicant's email address.
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Applicant's contact telephone number.
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        // Applicant's residential address.
        [StringLength(250)]
        public string? Address { get; set; }

        // Name of the university or college where the applicant studies.
        [StringLength(150)]
        public string? InstitutionName { get; set; }

        // Qualification or course currently being studied.
        [StringLength(150)]
        public string? CourseName { get; set; }

        // Applicant's current academic year, for example First Year or Second Year.
        [StringLength(30)]
        public string? YearOfStudy { get; set; }


        // Records when the applicant's profile was created.
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}