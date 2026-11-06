using System.ComponentModel.DataAnnotations;

namespace CloudFiveBursarySystem.Models
{
    // Represents a notification sent to an applicant.
    public class Notification
    {
        // Unique ID for every notification.
        public int NotificationId { get; set; }

        // Identifies the applicant receiving the notification.
        [Required]
        public int ApplicantId { get; set; }

        // Short heading displayed in the notification centre.
        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        // Full notification message.
        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        // Identifies the notification category.
        // Examples: Application, Payment, Bursary, Document.
        [Required]
        [StringLength(50)]
        public string NotificationType { get; set; } = string.Empty;

        // Indicates whether the applicant has read the notification.
        public bool IsRead { get; set; } = false;

        // Records when the notification was created.
        public DateTime DateCreated { get; set; } = DateTime.Now;

        // Optional link to the relevant page in the system.
        [StringLength(255)]
        public string? ActionUrl { get; set; }

        // Connects the notification to its applicant.
        public Applicant? Applicant { get; set; }
    }
}