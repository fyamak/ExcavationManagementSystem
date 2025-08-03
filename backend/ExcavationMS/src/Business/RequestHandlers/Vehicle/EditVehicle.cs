using Business.Mediator.Behaviours.Requests;
using FluentValidation;
using Infrastructure.Data.Postgres;
using MediatR;
using Serilog;
using Serilog.Events;
using Shared.Extensions;
using Shared.Models.Results;

namespace Business.RequestHandlers.Vehicle;

public abstract class EditVehicle
{
    public class EditVehicleRequest : IRequest<DataResult<EditVehicleResponse>>, IRequestToValidate
    {
        public int Id;
        public string Title { get; set; }
    }

    public class EditVehicleResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class EditVehicleRequestValidator : AbstractValidator<EditVehicleRequest>
    {
        public EditVehicleRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Araç ismi boş bırakılamaz.");
        }
    }

    public class EditVehicleRequestHandler : IRequestHandler<EditVehicleRequest, DataResult<EditVehicleResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public EditVehicleRequestHandler(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<DataResult<EditVehicleResponse>> Handle(EditVehicleRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var vehicle = await _unitOfWork.Vehicles.FirstOrDefaultAsync(x => x.Id == request.Id, tracked: true);
                if(vehicle is null)
                {
                    return DataResult<EditVehicleResponse>.Invalid("Araç bulunamadı.");
                }

                vehicle.Title = request.Title;
                await _unitOfWork.CommitAsync();

                var result = new EditVehicleResponse
                {
                    Id = vehicle.Id,
                    Title = vehicle.Title
                };
                return DataResult<EditVehicleResponse>.Success(result);
            }
            catch(Exception ex)
            {
                _logger.LogExtended(LogEventLevel.Error, $"Error on {GetType().Name}", ex);
                return DataResult<EditVehicleResponse>.Error(ex.Message);
            }
        }
    }
}
