using System;

namespace Reserveer.Models
{
    public class ErrorViewModel // Model used to show error messages
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}