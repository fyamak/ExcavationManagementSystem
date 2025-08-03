using Business.RequestHandlers.Vehicle;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Results;
using Web.Controllers.Base;
using Web.Filters;

namespace Web.Controllers
{
    public class VehicleController(IMediator mediator) : BaseController(mediator)
    {

        [HttpPost]
        //[Authorize]
        public async Task<DataResult<CreateVehicle.CreateVehicleResponse>> CreateVehicle(CreateVehicle.CreateVehicleRequest request)
        {
            return await Mediator.Send(request);
        }


        [HttpGet]
        //[Authorize]
        public async Task<DataResult<List<VehicleList.VehicleListResponse>>> VehicleList()
        {
            return await Mediator.Send(new VehicleList.VehicleListRequest());
        }


        [HttpPatch("{id}")]
        //[Authorize]
        public async Task<DataResult<EditVehicle.EditVehicleResponse>> EditVehicle(int id, EditVehicle.EditVehicleRequest request)
        {
            request.Id = id;
            return await Mediator.Send(request);
        }


        [HttpDelete("{id}")]
        //[Authorize]
        public async Task<Result> DeleteVehicle(int id)
        {
            var request = new DeleteVehicle.DeleteVehicleRequest { Id = id };
            return await Mediator.Send(request);
        }

    }
}
