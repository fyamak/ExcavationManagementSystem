using Business.Mediator.Behaviours.Requests;
using FluentValidation;
using Infrastructure.Data.Postgres;
using Infrastructure.Data.Postgres.Entities;
using MediatR;
using Serilog;
using Serilog.Events;
using Shared.Extensions;
using Shared.Models.Results;

namespace Business.RequestHandlers.Vehicle;

public abstract class CreateVehicle
{
    public class CreateVehicleRequest : IRequest<DataResult<CreateVehicleResponse>>, IRequestToValidate
    {
        public string Title { get; set; }
        public VehicleType VehicleType { get; set; }
    }
    
    public class CreateVehicleResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public VehicleType VehicleType { get; set; }
    }

    public class CreateVehicleRequestValidator : AbstractValidator<CreateVehicleRequest>
    {
        public CreateVehicleRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Araç ismi boş bırakalamaz.");
            RuleFor(x => x.VehicleType)
                .NotEmpty().WithMessage("Araç tipi boş bırakalamaz.")
                .Must(vehicleType => Enum.IsDefined(typeof(VehicleType), vehicleType))
                .WithMessage("Böyle bir araç tipi bulunmamaktadır.");
        }
    }

    public class CreateVehicleRequestHandler : IRequestHandler<CreateVehicleRequest, DataResult<CreateVehicleResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public CreateVehicleRequestHandler(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<DataResult<CreateVehicleResponse>> Handle(CreateVehicleRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var vehicleValidation = await _unitOfWork.Vehicles.FirstOrDefaultAsync(x => x.Title == request.Title);
                if (vehicleValidation is not null)
                {
                    return DataResult<CreateVehicleResponse>.Invalid("Aynı isme sahip başka bir araç bulunamaz.");
                }

                
                var vehicle = new Infrastructure.Data.Postgres.Entities.Vehicle
                {
                    Title = request.Title,
                    VehicleType = request.VehicleType
                };

                await _unitOfWork.Vehicles.AddAsync(vehicle);
                await _unitOfWork.CommitAsync();

                var result = new CreateVehicleResponse
                {
                    Id = vehicle.Id,
                    Title = vehicle.Title,
                    VehicleType = vehicle.VehicleType
                };
                return DataResult<CreateVehicleResponse>.Success(result);

            }
            catch(Exception ex)
            {
                _logger.LogExtended(LogEventLevel.Error, $"Error on {GetType().Name}", ex);
                return DataResult<CreateVehicleResponse>.Error(ex.Message);
            }
        }
    }
}
