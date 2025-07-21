using Business.RequestHandlers.Customer;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Results;
using Web.Controllers.Base;
using Web.Filters;

namespace Web.Controllers
{
    public class CustomerController(IMediator mediator) : BaseController(mediator)
    {

        [HttpPost]
        //[Authorize]
        public async Task<DataResult<CreateCustomer.CreateCustomerResponse>> CreateCustomer(CreateCustomer.CreateCustomerRequest request)
        {
            return await Mediator.Send(request);
        }


        [HttpGet]
        //[Authorize]
        public async Task<DataResult<List<CustomerList.CustomerListResponse>>> CustomerList()
        {
            return await Mediator.Send(new CustomerList.CustomerListRequest());
        }


        [HttpDelete("{id}")]
        //[Authorize]
        public async Task<Result> DeleteCustomer(int id)
        {
            var request = new DeleteCustomer.DeleteCustomerRequest{ Id = id };
            return await Mediator.Send(request);
        }


        [HttpPatch("{id}")]
        //[Authorize]
        public async Task<DataResult<EditCustomer.EditCustomerResponse>> EditCustomer(int id, EditCustomer.EditCustomerRequest request)
        {
            request.Id = id;
            return await Mediator.Send(request);
        }

        [HttpGet]
        //[Authorize]
        public async Task<PagedResult<PagedCustomerList.PagedCustomerListResponse>> PagedCustomerList(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            return await Mediator.Send(new PagedCustomerList.PagedCustomerListRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = search
            });
        }
    }
}
