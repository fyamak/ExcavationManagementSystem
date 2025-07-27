using FluentValidation;
using Infrastructure.Data.Postgres;
using MediatR;
using Serilog;
using Serilog.Events;
using Shared.Extensions;
using Shared.Models.Results;

namespace Business.RequestHandlers.Vehicle;

public abstract class DeleteVehicle
{
    public class DeleteVehicleRequest : IRequest<Result>
    {
        public int Id;
    }

    public class DeleteVehicleRequestValidator : AbstractValidator<DeleteVehicleRequest>
    {
        public DeleteVehicleRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Araç Id boş bırakılamaz");
        }
    }

    public class DeleteVehicleRequestHandler : IRequestHandler<DeleteVehicleRequest, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public DeleteVehicleRequestHandler(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteVehicleRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var vehicle = await _unitOfWork.Vehicles.FirstOrDefaultAsync(x => x.Id == request.Id, tracked: true);
                if(vehicle is null)
                {
                    Result.Invalid("Araç bulunamadı.");
                }

                vehicle.IsDeleted = true;
                await _unitOfWork.CommitAsync();
                return Result.Success("Araç başarılı bir şekilde silindi.");

            }
            catch (Exception ex)
            {
                _logger.LogExtended(LogEventLevel.Error, $"Error on {GetType().Name}", ex);
                return Result.Error(ex.Message);

            }
        }
    }

}
