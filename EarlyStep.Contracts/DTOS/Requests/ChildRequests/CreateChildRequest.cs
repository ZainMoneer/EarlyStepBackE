using EarlyStep.DAL.Model;

namespace EarlyStep.Contracts.DTOS.Requests.ChildRequests;

public class CreateChildRequest
{
    public string FullName { get; set; }

    public int Age { get; set; }

    public Gender? Gender { get; set; }

}
