using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Payment.API.Models;

public class CustomProblemDetails : ProblemDetails
{
    [JsonPropertyName("errors")]
    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
}
