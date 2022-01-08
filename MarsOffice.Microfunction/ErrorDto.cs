using System.Collections.Generic;

namespace MarsOffice.Microfunction
{
    public class ErrorDto
    {
        public string Message { get; set; }
        public Dictionary<string, string> PlaceholderValues { get; set; }
    }
}