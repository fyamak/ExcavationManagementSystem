
using Business.RequestHandlers.Income;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Results;
using Web.Controllers.Base;
using Web.Filters;

namespace Web.Controllers;

public class IncomeController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost]
    //[Authorize]
    public async Task<DataResult<CreateIncome.CreateIncomeResponse>> CreateIncome(CreateIncome.CreateIncomeRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpDelete("{id}")]
    //[Authorize]
    public async Task<Result> DeleteIncome(int id)
    {
        var request = new DeleteIncome.DeleteIncomeRequest
        {
            Id = id
        };
        return await Mediator.Send(request);
    }
    
}
