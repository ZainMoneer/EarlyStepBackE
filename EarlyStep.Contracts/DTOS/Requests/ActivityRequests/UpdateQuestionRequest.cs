using EarlyStep.DAL.Model;
using Microsoft.AspNetCore.Http;

namespace EarlyStep.Contracts.DTOS.Requests.ActivityRequests;

public class UpdateQuestionRequest
    {
    public string? QuestionText { get; set; }

    public IFormFile? QuestionImage { get; set; }

    public QuestionType? Type { get; set; }
    }