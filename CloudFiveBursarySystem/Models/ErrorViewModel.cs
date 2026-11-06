namespace CloudFiveBursarySystem.Models
{
    // Provides information used by the error page when an application error occurs.
    public class ErrorViewModel
    {
        // Stores a unique identifier for the request that caused the error.
        // This can help developers trace and investigate the problem.
        public string? RequestId { get; set; }

        // Determines whether the request ID should be displayed on the error page.
        // Returns true when a request ID is available; otherwise, returns false.
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}