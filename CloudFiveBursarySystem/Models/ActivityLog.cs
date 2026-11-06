using System.ComponentModel.DataAnnotations;

namespace CloudFiveBursarySystem.Models
{
    // Stores important activities performed by applicants and administrators.
    public class ActivityLog
    {
        // Unique identifier for each activity.
        public int ActivityLogId { get; set; }

        // ID of the user who performed the action.
        [Required]
        public string UserId { get; set; } = string.Empty;

        // Email address of the user.
        [Required]
        [StringLength(256)]
        public string UserEmail { get; set; } = string.Empty;

        // Identifies whether the user is an Applicant or Administrator.
        [Required]
        [StringLength(50)]
        public string UserRole { get; set; } = string.Empty;

        // Type of action performed.
        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;

        // Explains what happened.
        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        // Identifies the part of the system affected.
        [StringLength(100)]
        public string? EntityType { get; set; }

        // Identifies the specific database record affected.
        public int? EntityId { get; set; }

        // Records when the activity took place.
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}