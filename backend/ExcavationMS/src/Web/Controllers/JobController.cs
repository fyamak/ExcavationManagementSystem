using Business.RequestHandlers.Job;
using Infrastructure.Data.Postgres.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Results;
using Web.Controllers.Base;
using Web.Filters;

namespace Web.Controllers
{
    public class JobController(IMediator mediator) : BaseController(mediator)
    {
        [HttpPost]
        //[Authorize]
        public async Task<DataResult<CreateJob.CreateJobResponse>> CreateJob(CreateJob.CreateJobRequest request)
        {
            return await Mediator.Send(request);
        }

        [HttpDelete("{id}")]
        //[Authorize]
        public async Task<Result> DeleteJob(int id)
        {
            var request = new DeleteJob.DeleteJobRequest { Id = id };
            return await Mediator.Send(request);
        }


        [HttpPatch("{id}")]
        //[Authorize]
        public async Task<DataResult<EditJob.EditJobResponse>> EditJob(int id, EditJob.EditJobRequest request)
        {
            request.Id = id;
            return await Mediator.Send(request);
        }


        [HttpGet("{id}")]
        //[Authorize]
        public async Task<DataResult<GetJobById.GetJobByIdResponse>> GetJobByID(int id)
        {
            var request = new GetJobById.GetJobByIdRequest
            {
                Id = id
            };
            return await Mediator.Send(request);
        }

        [HttpGet]
        //[Authorize]
        public async Task<PagedResult<PagedJobList.PagedJobListResponse>> PagedJobList(
            [FromQuery] int vehicleId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            return await Mediator.Send(new PagedJobList.PagedJobListRequest
            {
                VehicleId = vehicleId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = search
            });
        }

        


    }
}
