using Microsoft.AspNetCore.Mvc;

namespace APIs.Responses
{
    public class ApiProblemDetails : ProblemDetails
    {
        public string? TraceId { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
