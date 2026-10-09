using Microsoft.AspNetCore.Http;

namespace EarlyStep.Contracts.DTOS.Requests.ActivityRequests;

public class UpdateActivityRequest
    {
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Slug { get; set; }
    public Guid? CategoryId { get; set; }
    public IFormFile? Icon { get; set; }

    }