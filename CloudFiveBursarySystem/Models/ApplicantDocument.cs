using System.ComponentModel.DataAnnotations;

namespace CloudFiveBursarySystem.Models
{
    // Represents a supporting document uploaded by an applicant.
    public class ApplicantDocument
    {
        public int ApplicantDocumentId { get; set; }

        [Required]
        public int ApplicantId { get; set; }

        [Required]
        [StringLength(255)]
        public string DocumentName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string StoredFileName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime DateUploaded { get; set; } = DateTime.Now;

        [Required]
        [StringLength(30)]
        public string DocumentStatus { get; set; } = "Pending";

        // Stores feedback entered by the administrator.
        // This can explain why a document was rejected.
        [StringLength(1000)]
        public string? AdminComment { get; set; }

        public Applicant? Applicant { get; set; }
    }
}