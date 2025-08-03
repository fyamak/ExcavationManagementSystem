using Business.RequestHandlers.Expense;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Results;
using Web.Controllers.Base;
using Web.Filters;

namespace Web.Controllers
{
    public class ExpenseController(IMediator mediator) : BaseController(mediator)
    {
        [HttpPost]
        //[Authorize]
        public async Task<DataResult<CreateExpense.CreateExpenseResponse>> CreateExpense(CreateExpense.CreateExpenseRequest request)
        {
            return await Mediator.Send(request);
        }


        [HttpDelete("{id}")]
        //[Authorize]
        public async Task<Result> DeleteExpense(int id)
        {
            var request = new DeleteExpense.DeleteExpenseRequest
            {
                Id = id
            };
            return await Mediator.Send(request);
        }

    }
}
