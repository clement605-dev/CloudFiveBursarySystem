using System.ComponentModel.DataAnnotations;

namespace CloudFiveBursarySystem.ViewModels
{
    // Holds the applicant's personal and academic information
    // displayed and updated on the My Profile page.
    public class ApplicantProfileViewModel
    {
        // Personal Information

        [Required]
        [Display(Name = "First Name")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "ID Number")]
        [StringLength(20)]
        public string IDNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(250)]
        [Display(Name = "Residential Address")]
        public string? Address { get; set; }

        // Academic Information

        [StringLength(150)]
        [Display(Name = "Institution Name")]
        public string? InstitutionName { get; set; }

        [StringLength(150)]
        [Display(Name = "Course Name")]
        public string? CourseName { get; set; }

        [StringLength(30)]
        [Display(Name = "Year of Study")]
        public string? YearOfStudy { get; set; }

        // Stores the calendar year in which the applicant is studying.
        [Range(2020, 2100)]
        [Display(Name = "Academic Year")]
        public int? AcademicYear { get; set; }
    }
}