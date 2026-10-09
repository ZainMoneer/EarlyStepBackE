using EarlyStep.Contracts.DTOS.Requests.ActivityRequests;
using EarlyStep.Contracts.DTOS.Responses.ActivityResponses;
using EarlyStep.Contracts.DTOS.Responses.CategoryResponses;
using EarlyStep.DAL.Model;

namespace EarlyStep.BLL.Interfaces;

public interface IActivityRepository
    {
    Task<List<ActivityResponse>> GetActivitiesAsync(
        ActivityType type,
        Guid? categoryId = null,
        CancellationToken cancellationToken = default);

    Task<List<CategoryResponse>> GetActivityCategoriesAsync(
    ActivityType type,
    CancellationToken cancellationToken = default);

    Task<ActivityResponse?> GetActivityByIdAsync(
        Guid id,
        ActivityType type,
        CancellationToken cancellationToken = default);


    Task<ActivityResponse> CreateActivityAsync(
        CreateActivityContentRequest request,
        ActivityType type,
        Guid adminId,
        CancellationToken cancellationToken = default);
    Task<ActivityResponse?> UpdateActivityAsync(
    Guid id,
    ActivityType type,
    UpdateActivityRequest request,
    CancellationToken cancellationToken = default);

    Task<bool> DeleteActivityAsync(
        Guid id,
        ActivityType type,
        CancellationToken cancellationToken = default);

    //CRUD QUstion
    Task<ActivityQuestionResponse?> GetQuestionByIdAsync(
      Guid questionId,
      CancellationToken cancellationToken = default);
    Task<ActivityQuestionResponse> CreateQuestionAsync(
    Guid activityId,
    CreateQuestionRequest request,
    CancellationToken cancellationToken = default);

    Task<ActivityQuestionResponse?> UpdateQuestionAsync(
    Guid questionId,
    UpdateQuestionRequest request,
    CancellationToken cancellationToken = default);
    Task<bool> DeleteQuestionAsync(
    Guid questionId,
    CancellationToken cancellationToken = default);

    Task<ActivityOptionResponse?> UpdateQuestionOptionAsync(
        Guid optionId,
        UpdateOptionRequest request,
        CancellationToken cancellationToken = default);
    Task<bool> DeleteAllQuestionOptionsAsync(
    Guid questionId,
    CancellationToken cancellationToken = default);

    Task<bool> DeleteQuestionOptionAsync(
        Guid optionId,
        CancellationToken cancellationToken = default);


    }