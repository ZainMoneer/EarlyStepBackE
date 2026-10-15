using EarlyStep.BLL.Helper;
using EarlyStep.BLL.Interfaces;
using EarlyStep.Contracts.DTOS.Requests.ActivityRequests;
using EarlyStep.Contracts.DTOS.Responses.ActivityResponses;
using EarlyStep.Contracts.DTOS.Responses.CategoryResponses;
using EarlyStep.DAL.Data;
using EarlyStep.DAL.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EarlyStep.BLL.Repository;

public class ActivityRepository : IActivityRepository
    {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ActivityRepository(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
        {
        _context = context;
        _userManager = userManager;
        }
    public async Task<List<ActivityResponse>> GetActivitiesAsync(
       ActivityType type,
       Guid? categoryId = null,
       CancellationToken cancellationToken = default)
        {
        var query = _context.Activities
            .AsNoTracking()
            .Include(a => a.Category)
            .Where(a =>
                !a.IsDeleted &&
                a.Type == type);

        if (categoryId.HasValue)
            {
            query = query.Where(a =>
                a.CategoryId == categoryId.Value);
            }

        var activities = await query
            .OrderBy(a => a.Title)
            .ToListAsync(cancellationToken);

        return activities.Select(a => new ActivityResponse
            {
            Id = a.Id,
            Title = a.Title,
            Description = a.Description,
            Slug = a.Slug,
            Type = a.Type.ToString(),
            CategoryId = a.CategoryId,
            CategoryName = a.Category?.Name,
            QuestionCount = (int)a.QuestionCount,
            CreatedById = a.CreatedById,
            IconUrl = a.IconUrl,
            }).ToList();
        }


    public async Task<ActivityResponse?> GetActivityByIdAsync(
        Guid id,
        ActivityType type,
        CancellationToken cancellationToken = default)
        {
        var activity = await _context.Activities
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.CreatedBy)
            .Include(x => x.Questions
                .Where(q => !q.IsDeleted))
                .ThenInclude(q => q.Options
                    .Where(o => !o.IsDeleted))
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.Type == type &&
                     !x.IsDeleted,
                cancellationToken);

        if (activity is null)
            return null;

        return MapToAdminResponse(activity);
        }

    public async Task<List<CategoryResponse>> GetActivityCategoriesAsync(
        ActivityType type,
        CancellationToken cancellationToken = default)
        {
        return await _context.Categories
            .Select(category => new CategoryResponse
                {
                Id = category.Id,
                Name = category.Name,
                Count = _context.Activities.Count(activity =>
                    activity.CategoryId == category.Id &&
                    activity.Type == type &&
                    !activity.IsDeleted)
                })
            .Where(x => x.Count > 0)
            .ToListAsync(cancellationToken);
        }

    public async Task<ActivityResponse> CreateActivityAsync(
        CreateActivityContentRequest request,
        ActivityType type,
        Guid adminId,
        CancellationToken cancellationToken = default)
        {
        var categoryExists = await _context.Categories
            .AnyAsync(
                x => x.Id == request.CategoryId && !x.IsDeleted,
                cancellationToken);

        if (!categoryExists)
            throw new KeyNotFoundException("التصنيف غير موجود.");

        var admin = await _userManager.FindByIdAsync(adminId.ToString());

        if (admin == null)
            throw new KeyNotFoundException("Admin not found.");

        var roles = await _userManager.GetRolesAsync(admin);

        if (!roles.Contains("Admin"))
            throw new UnauthorizedAccessException(
                "الادمن فقط بامكانه اضافة نشاط.");

        string? iconUrl = null;

        if (request.Icon != null)
            {
            iconUrl = await FileSettings.UploadFileAsync(
                request.Icon,
                "activities",
                cancellationToken);
            }


        var activity = new Activity
            {
            Id = Guid.NewGuid(),

            Title = request.Title,

            Description = request.Description,

            Slug = request.Slug,

            IconUrl = iconUrl,

            Type = type,

            CategoryId = request.CategoryId,

            QuestionCount = 0,

            CreatedById = adminId,

            IsDeleted = false
            };

        _context.Activities.Add(activity);

        await _context.SaveChangesAsync(cancellationToken);

        return new ActivityResponse
            {
            Id = activity.Id,

            Title = activity.Title,

            Description = activity.Description,

            Slug = activity.Slug,

            IconUrl = activity.IconUrl,

            Type = activity.Type.ToString(),

            CategoryId = activity.CategoryId,

            QuestionCount = 0,

            CreatedById = activity.CreatedById
            };
        }
    public async Task<ActivityResponse?> UpdateActivityAsync(
    Guid id,
    ActivityType type,
    UpdateActivityRequest request,
    CancellationToken cancellationToken = default)
        {
        var activity = await _context.Activities
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.Type == type &&
                     !x.IsDeleted,
                cancellationToken);

        if (activity == null)
            return null;



        if (!string.IsNullOrWhiteSpace(request.Title))
            activity.Title = request.Title;

        if (request.Description != null)
            activity.Description = request.Description;

        if (!string.IsNullOrWhiteSpace(request.Slug))
            activity.Slug = request.Slug;



        if (request.CategoryId.HasValue)
            {
            var categoryExists = await _context.Categories
                .AnyAsync(
                    x => x.Id == request.CategoryId.Value &&
                         !x.IsDeleted,
                    cancellationToken);

            if (!categoryExists)
                throw new KeyNotFoundException("التصنيف غير موجود.");

            activity.CategoryId = request.CategoryId.Value;
            }


        if (request.Icon != null)
            {
            // حذف الصورة القديمة
            if (!string.IsNullOrWhiteSpace(activity.IconUrl))
                {
                var oldFileName = Path.GetFileName(activity.IconUrl);

                FileSettings.RemoveFile(
                    oldFileName,
                    "activities");
                }

            // رفع الصورة الجديدة
            var newIconUrl = await FileSettings.UploadFileAsync(
                request.Icon,
                "activities",
                cancellationToken);

            activity.IconUrl = newIconUrl;
            }

        await _context.SaveChangesAsync(cancellationToken);

        return await GetActivityByIdAsync(
            activity.Id,
            activity.Type,
            cancellationToken);
        }
    public async Task<bool> DeleteActivityAsync(
        Guid id,
        ActivityType type,
        CancellationToken cancellationToken = default)
        {
        var activity = await _context.Activities
            .FirstOrDefaultAsync(
                x => x.Id == id && x.Type == type && !x.IsDeleted,
                cancellationToken);

        if (activity == null)
            return false;

        activity.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
        }

    public async Task<ActivityQuestionResponse?> GetQuestionByIdAsync(
        Guid questionId,
        CancellationToken cancellationToken = default)
        {
        var question = await _context.ActivityQuestions
            .AsNoTracking()
            .Include(q => q.Options)
            .FirstOrDefaultAsync(
                q => q.Id == questionId && !q.IsDeleted,
                cancellationToken);

        if (question == null)
            return null;

        return new ActivityQuestionResponse
            {
            Id = question.Id,
            QuestionText = question.QuestionText,
            QuestionImageUrl = question.QuestionImageUrl,
            Type = question.Type.ToString(),
            Order = question.Order,

            Options = question.Options
                .OrderBy(o => o.Order)
                .Select(o => new ActivityOptionResponse
                    {
                    Id = o.Id,
                    OptionText = o.OptionText,
                    OptionImageUrl = o.OptionImageUrl,
                    IsCorrect = o.IsCorrect,
                    CorrectOrder = o.CorrectOrder,
                    Order = o.Order
                    })
                .ToList()
            };
        }
    public async Task<ActivityQuestionResponse> CreateQuestionAsync(
        Guid activityId,
        CreateQuestionRequest request,
        CancellationToken cancellationToken = default)
        {
        var activity = await _context.Activities
            .FirstOrDefaultAsync(
                x => x.Id == activityId && !x.IsDeleted,
                cancellationToken);

        if (activity == null)
            throw new KeyNotFoundException("النشاط غير موجود.");

        var lastOrder = await _context.ActivityQuestions
            .Where(q => q.ActivityId == activityId)
            .Select(q => (int?)q.Order)
            .MaxAsync(cancellationToken) ?? 0;

        var question = await BuildQuestionAsync(
            activityId,
            request,
            lastOrder + 1,
            cancellationToken);

        _context.ActivityQuestions.Add(question);

        // تحديث عدد الأسئلة
        activity.QuestionCount =
            (activity.QuestionCount ?? 0) + 1;

        await _context.SaveChangesAsync(cancellationToken);

        return new ActivityQuestionResponse
            {
            Id = question.Id,

            QuestionText = question.QuestionText,

            QuestionImageUrl = question.QuestionImageUrl,

            Type = question.Type.ToString(),

            Order = question.Order,

            Options = question.Options
    .OrderBy(o => o.Order)
    .Select(o => new ActivityOptionResponse
        {
        Id = o.Id,
        OptionText = o.OptionText,
        OptionImageUrl = o.OptionImageUrl,

        Order = o.Order, // أضف هذا

        IsCorrect = o.IsCorrect,
        CorrectOrder = o.CorrectOrder
        })
    .ToList()
            };
        }
    public async Task<ActivityQuestionResponse?> UpdateQuestionAsync(
    Guid questionId,
    UpdateQuestionRequest request,
    CancellationToken cancellationToken = default)
        {
        var question = await _context.ActivityQuestions
            .Include(q => q.Options)
            .FirstOrDefaultAsync(
                q => q.Id == questionId && !q.IsDeleted,
                cancellationToken);

        if (question == null)
            return null;


        if (!string.IsNullOrWhiteSpace(request.QuestionText))
            {
            question.QuestionText = request.QuestionText;
            }

        if (request.Type.HasValue &&
            request.Type.Value != question.Type)
            {
            var options = question.Options.ToList();

            if (options.Count == 0)
                {
                throw new ArgumentException(
                    "لا يمكن تغيير نوع السؤال قبل إضافة الخيارات.");
                }

            switch (request.Type.Value)
                {


                case QuestionType.SingleChoice:

                    if (options.Count(x => x.IsCorrect) != 1)
                        {
                        throw new ArgumentException(
                            "لا يمكن تغيير النوع إلى SingleChoice. عدّل الخيارات أولاً بحيث توجد إجابة صحيحة واحدة فقط.");
                        }

                    break;


                case QuestionType.MultiSelect:

                    if (options.Count(x => x.IsCorrect) < 2)
                        {
                        throw new ArgumentException(
                            "لا يمكن تغيير النوع إلى MultiSelect. عدّل الخيارات أولاً بحيث توجد إجابتان صحيحتان على الأقل.");
                        }

                    break;

                case QuestionType.Sequence:

                    if (options.Any(x => x.CorrectOrder == null))
                        {
                        throw new ArgumentException(
                            "لا يمكن تغيير النوع إلى Sequence. عدّل الخيارات أولاً وحدد CorrectOrder لكل خيار.");
                        }

                    if (options
                        .Select(x => x.CorrectOrder)
                        .Distinct()
                        .Count() != options.Count)
                        {
                        throw new ArgumentException(
                            "لا يمكن تغيير النوع إلى Sequence لأن CorrectOrder مكرر. عدّل الخيارات أولاً.");
                        }

                    break;
                }
            question.Type = request.Type.Value;
            }

        if (request.QuestionImage != null &&
            request.QuestionImage.Length > 0)
            {
            if (!string.IsNullOrWhiteSpace(question.QuestionImageUrl))
                {
                var oldFileName =
                    Path.GetFileName(question.QuestionImageUrl);

                FileSettings.RemoveFile(
                    oldFileName,
                    "questions");
                }

            question.QuestionImageUrl =
                await FileSettings.UploadFileAsync(
                    request.QuestionImage,
                    "questions",
                    cancellationToken);
            }

        await _context.SaveChangesAsync(cancellationToken);

        return new ActivityQuestionResponse
            {
            Id = question.Id,
            QuestionText = question.QuestionText,
            QuestionImageUrl = question.QuestionImageUrl,
            Type = question.Type.ToString(),
            Order = question.Order,

            Options = question.Options
                .OrderBy(o => o.Order)
                .Select(o => new ActivityOptionResponse
                    {
                    Id = o.Id,
                    OptionText = o.OptionText,
                    OptionImageUrl = o.OptionImageUrl,
                    IsCorrect = o.IsCorrect,
                    CorrectOrder = o.CorrectOrder
                    })
                .ToList()
            };
        }


    public async Task<bool> DeleteQuestionAsync(
        Guid questionId,
        CancellationToken cancellationToken = default)
        {
        var question = await _context.ActivityQuestions
            .FirstOrDefaultAsync(
                q => q.Id == questionId &&
                     !q.IsDeleted,
                cancellationToken);

        if (question is null)
            return false;

        question.IsDeleted = true;

        var activity = await _context.Activities
            .FirstOrDefaultAsync(
                a => a.Id == question.ActivityId,
                cancellationToken);

        if (activity is not null)
            {
            activity.QuestionCount = await _context.ActivityQuestions
                .CountAsync(q =>
                    q.ActivityId == activity.Id &&
                    !q.IsDeleted,
                    cancellationToken);
            }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
        }


    public async Task<ActivityOptionResponse?> UpdateQuestionOptionAsync(
        Guid optionId,
        UpdateOptionRequest request,
        CancellationToken cancellationToken = default)
        {
        // 1. Get option with its question
        var option = await _context.QuestionOptions
            .Include(o => o.Question)
            .FirstOrDefaultAsync(
                o => o.Id == optionId && !o.IsDeleted,
                cancellationToken);

        if (option is null)
            return null;

        if (request.Order.HasValue)
            {
            var duplicateOrder = await _context.QuestionOptions
                .AnyAsync(o =>
                    o.QuestionId == option.QuestionId &&
                    o.Id != optionId &&
                    !o.IsDeleted &&
                    o.Order == request.Order.Value,
                    cancellationToken);

            if (duplicateOrder)
                {
                throw new ArgumentException(
                    "لا يمكن تكرار ترتيب الخيار (Order) داخل نفس السؤال.");
                }
            }

        if (option.Question.Type == QuestionType.SingleChoice &&
            request.IsCorrect == true)
            {
            var anotherCorrectOption = await _context.QuestionOptions
                .AnyAsync(o =>
                    o.QuestionId == option.QuestionId &&
                    o.Id != optionId &&
                    !o.IsDeleted &&
                    o.IsCorrect,
                    cancellationToken);

            if (anotherCorrectOption)
                {
                throw new ArgumentException(
                    "سؤال SingleChoice يسمح بإجابة صحيحة واحدة فقط. قم بإلغاء الإجابة الصحيحة القديمة أولاً.");
                }
            }

        if (request.OptionText != null)
            option.OptionText = request.OptionText;

        if (request.Order.HasValue)
            option.Order = request.Order.Value;

        if (request.IsCorrect.HasValue)
            option.IsCorrect = request.IsCorrect.Value;

        if (request.CorrectOrder.HasValue)
            option.CorrectOrder = request.CorrectOrder.Value;

        if (request.OptionImage != null &&
            request.OptionImage.Length > 0)
            {
            var newImageUrl = await FileSettings.UploadFileAsync(
                request.OptionImage,
                "question-options",
                cancellationToken);

            var oldImageUrl = option.OptionImageUrl;

            option.OptionImageUrl = newImageUrl;

            await _context.SaveChangesAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(oldImageUrl))
                {
                FileSettings.RemoveFile(
                    Path.GetFileName(oldImageUrl),
                    "question-options");
                }
            }
        else
            {
            if (string.IsNullOrWhiteSpace(option.OptionText) &&
                string.IsNullOrWhiteSpace(option.OptionImageUrl))
                {
                throw new ArgumentException(
                    "يجب أن يحتوي الخيار على نص أو صورة على الأقل.");
                }

            await _context.SaveChangesAsync(cancellationToken);
            }

        return new ActivityOptionResponse
            {
            Id = option.Id,
            OptionText = option.OptionText,
            OptionImageUrl = option.OptionImageUrl,
            Order = option.Order,
            IsCorrect = option.IsCorrect,
            CorrectOrder = option.CorrectOrder
            };
        }

    public async Task<bool> DeleteAllQuestionOptionsAsync(
        Guid questionId,
        CancellationToken cancellationToken = default)
        {
        var options = await _context.QuestionOptions
            .Where(o => o.QuestionId == questionId)
            .ToListAsync(cancellationToken);

        if (!options.Any())
            return false;

        _context.QuestionOptions.RemoveRange(options);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
        }


    public async Task<bool> DeleteQuestionOptionAsync(
    Guid optionId,
    CancellationToken cancellationToken = default)
        {
        var option = await _context.QuestionOptions
            .FirstOrDefaultAsync(
                x => x.Id == optionId,
                cancellationToken);

        if (option == null)
            return false;

        // حذف صورة الخيار إذا موجودة
        if (!string.IsNullOrWhiteSpace(option.OptionImageUrl))
            {
            var oldFileName =
                Path.GetFileName(option.OptionImageUrl);

            FileSettings.RemoveFile(
                oldFileName,
                "question-options");
            }

        _context.QuestionOptions.Remove(option);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
        }

    private static ActivityResponse MapToAdminResponse(Activity activity)
        {
        return new ActivityResponse
            {
            Id = activity.Id,
            Title = activity.Title,
            Description = activity.Description,
            Slug = activity.Slug,
            Type = activity.Type.ToString(),
            CategoryId = activity.CategoryId,
            CategoryName = activity.Category?.Name ?? "",
            QuestionCount = activity.QuestionCount ?? 0,
            CreatedById = activity.CreatedById,
            IconUrl = activity.IconUrl,
            CreatedByName = activity.CreatedBy?.FullName ?? "",
            Questions = activity.Questions
    .OrderBy(q => q.Order)
    .Select(q => new ActivityQuestionResponse
        {
        Id = q.Id,
        QuestionText = q.QuestionText,
        QuestionImageUrl = q.QuestionImageUrl,
        Type = q.Type.ToString(),
        Order = q.Order,
        Options = q.Options.Select(o => new ActivityOptionResponse
            {
            Id = o.Id,
            OptionText = o.OptionText,
            OptionImageUrl = o.OptionImageUrl,
            IsCorrect = o.IsCorrect,
            CorrectOrder = o.CorrectOrder
            }).ToList()
        }).ToList()
            };
        }


    private async Task<ActivityQuestion> BuildQuestionAsync(
       Guid activityId,
       CreateQuestionRequest request,
       int order,
       CancellationToken cancellationToken)
        {


        if (string.IsNullOrWhiteSpace(request.QuestionText) &&
            request.QuestionImage == null)
            {
            throw new ArgumentException(
                "يجب أن يحتوي السؤال على نص أو صورة على الأقل.");
            }

        if (request.Options == null ||
            request.Options.Count == 0)
            {
            throw new ArgumentException(
                "يجب أن يحتوي السؤال على خيار واحد على الأقل.");
            }

        string? questionImageUrl = null;

        if (request.QuestionImage != null)
            {
            questionImageUrl = await FileSettings.UploadFileAsync(
                request.QuestionImage,
                "questions",
                cancellationToken);
            }
        // إنشاء السؤال

        var question = new ActivityQuestion
            {
            Id = Guid.NewGuid(),

            ActivityId = activityId,

            QuestionText = request.QuestionText,

            QuestionImageUrl = questionImageUrl,

            Type = request.Type,

            Order = order
            };


        foreach (var optionRequest in request.Options)
            {
            if (string.IsNullOrWhiteSpace(optionRequest.OptionText) &&
                optionRequest.OptionImage == null)
                {
                throw new ArgumentException(
                    "يجب أن يحتوي كل خيار على نص أو صورة على الأقل.");
                }

            string? optionImageUrl = null;

            if (optionRequest.OptionImage != null)
                {
                optionImageUrl = await FileSettings.UploadFileAsync(
                    optionRequest.OptionImage,
                    "question-options",
                    cancellationToken);
                }

            question.Options.Add(new QuestionOption
                {
                Id = Guid.NewGuid(),

                QuestionId = question.Id,

                OptionText = optionRequest.OptionText,

                OptionImageUrl = optionImageUrl,

                IsCorrect = optionRequest.IsCorrect,

                CorrectOrder = optionRequest.CorrectOrder,

                Order = optionRequest.Order
                });
            }

        ValidateOptionsForType(
            request.Type,
            request.Options);

        return question;
        }
    private static void ValidateOptionsForType(
        QuestionType type,
        List<QuestionOptionRequest> options)
        {

        if (options == null || options.Count == 0)
            {
            throw new ArgumentException(
                "يجب أن يحتوي السؤال على خيار واحد على الأقل.");
            }

        foreach (var option in options)
            {
            if (string.IsNullOrWhiteSpace(option.OptionText) &&
                option.OptionImage == null)
                {
                throw new ArgumentException(
                    "يجب أن يحتوي كل خيار على نص أو صورة على الأقل.");
                }
            }
        if (options
            .GroupBy(x => x.Order)
            .Any(g => g.Count() > 1))
            {
            throw new ArgumentException(
                "لا يمكن تكرار Order بين الخيارات.");
            }
        switch (type)
            {
            case QuestionType.SingleChoice:
                    {
                    if (options.Count < 2)
                        {
                        throw new ArgumentException(
                            "سؤال SingleChoice يجب أن يحتوي على خيارين على الأقل.");
                        }

                    var correctCount =
                        options.Count(x => x.IsCorrect);

                    if (correctCount != 1)
                        {
                        throw new ArgumentException(
                            "سؤال SingleChoice يجب أن يحتوي على إجابة صحيحة واحدة فقط.");
                        }

                    break;
                    }


            case QuestionType.MultiSelect:
                    {
                    if (options.Count < 2)
                        {
                        throw new ArgumentException(
                            "سؤال MultiSelect يجب أن يحتوي على خيارين على الأقل.");
                        }

                    var correctCount =
                        options.Count(x => x.IsCorrect);

                    if (correctCount < 2)
                        {
                        throw new ArgumentException(
                            "سؤال MultiSelect يجب أن يحتوي على إجابتين صحيحتين على الأقل.");
                        }

                    break;
                    }


            case QuestionType.Sequence:
                    {
                    if (options.Count < 2)
                        {
                        throw new ArgumentException(
                            "سؤال Sequence يجب أن يحتوي على خيارين على الأقل.");
                        }
                    if (options.Any(x => x.CorrectOrder == null))
                        {
                        throw new ArgumentException(
                            "سؤال Sequence يجب أن يحتوي كل خيار فيه على CorrectOrder.");
                        }

                    if (options.Any(x => x.CorrectOrder <= 0))
                        {
                        throw new ArgumentException(
                            "CorrectOrder يجب أن يبدأ من 1.");
                        }
                    if (options
                        .GroupBy(x => x.CorrectOrder)
                        .Any(g => g.Count() > 1))
                        {
                        throw new ArgumentException(
                            "لا يمكن تكرار CorrectOrder بين خيارات Sequence.");
                        }
                    var correctOrders = options
                        .Select(x => x.CorrectOrder!.Value)
                        .OrderBy(x => x)
                        .ToList();

                    for (int i = 0; i < correctOrders.Count; i++)
                        {
                        if (correctOrders[i] != i + 1)
                            {
                            throw new ArgumentException(
                                "CorrectOrder في Sequence يجب أن يكون متسلسلاً من 1 بدون أرقام مفقودة.");
                            }
                        }

                    break;
                    }
            default:
                throw new ArgumentException(
                    "نوع السؤال غير صحيح.");
            }
        }
    }