using Microsoft.AspNetCore.Http;

namespace EarlyStep.Contracts.DTOS.Requests.ActivityRequests;

public class QuestionOptionRequest
    {
    // إذا موجود = تعديل خيار موجود
    // إذا null = إضافة خيار جديد
    public Guid? Id { get; set; }

    public string? OptionText { get; set; }

    public IFormFile? OptionImage { get; set; }

    public int Order { get; set; }

    public bool IsCorrect { get; set; }

    // يستخدم مع Sequence
    public int? CorrectOrder { get; set; }

    }