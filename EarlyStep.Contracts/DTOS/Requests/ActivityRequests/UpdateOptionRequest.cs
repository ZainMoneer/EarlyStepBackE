using Microsoft.AspNetCore.Http;

namespace EarlyStep.Contracts.DTOS.Requests.ActivityRequests;

public class UpdateOptionRequest
    {

    public string? OptionText { get; set; }

    public IFormFile? OptionImage { get; set; }

    public int? Order { get; set; }

    public bool? IsCorrect { get; set; }

    public int? CorrectOrder { get; set; }

    }