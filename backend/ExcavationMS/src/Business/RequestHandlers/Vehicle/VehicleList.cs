using Infrastructure.Data.Postgres;
using Infrastructure.Data.Postgres.Entities;
using MediatR;
using Serilog;
using Serilog.Events;
using Shared.Extensions;
using Shared.Models.Results;

namespace Business.RequestHandlers.Vehicle;

public abstract class VehicleList
{
    public class VehicleListRequest : IRequest<DataResult<List<VehicleListResponse>>>
    {

    }

    public class VehicleListResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string VehicleType { get; set; }
    }

    public class VehicleListRequestHandler : IRequestHandler<VehicleListRequest, DataResult<List<VehicleListResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public VehicleListRequestHandler(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<DataResult<List<VehicleListResponse>>> Handle(VehicleListRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var vehicles = await _unitOfWork.Vehicles.GetAllAsync();

                var result = vehicles.Select(x => new VehicleListResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    VehicleType = x.VehicleType.ToString()
                }).ToList();

                return DataResult<List<VehicleListResponse>>.Success(result);

            }
            catch(Exception ex)
            {
                _logger.LogExtended(LogEventLevel.Error, $"Error on {GetType().Name}", ex);
                return DataResult<List<VehicleListResponse>>.Error(ex.Message);
            }
        }
    }
}
