using Business.Mediator.Behaviours.Requests;
using FluentValidation;
using Infrastructure.Data.Postgres;
using MediatR;
using Serilog;
using Serilog.Events;
using Shared.Extensions;
using Shared.Models.Results;

namespace Business.RequestHandlers.Job;

public abstract class CreateJob
{
    public class CreateJobRequest : IRequest<DataResult<CreateJobResponse>>, IRequestToValidate
    {
        public string Title { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int? AgreementAmount { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public int VehicleId { get; set; }
        public int CustomerId { get; set; }
    }

    public class CreateJobResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int VehicleId { get; set; }
        public int CustomerId { get; set; }
    }

    public class CreateJobRequestValidator : AbstractValidator<CreateJobRequest>
    {
        public CreateJobRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("İş başlığı boş bırakalamaz.")
                .MaximumLength(256).WithMessage("İş başlığı boyutu 256 karakterden fazla olamaz.");
            RuleFor(x => x.Description)
                .MaximumLength(4096).WithMessage("Açıklama boyutu 4096 karakterden fazla olamaz.");
            RuleFor(x => x.Location)
                .MaximumLength(256).WithMessage("Konum bilgisi boyutu 256 karakterden fazla olamaz.");
        }
    }

    public class CreateJobRequestHandler : IRequestHandler<CreateJobRequest, DataResult<CreateJobResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public CreateJobRequestHandler(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<DataResult<CreateJobResponse>> Handle(CreateJobRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var vehicle = await _unitOfWork.Vehicles.FirstOrDefaultAsync(x => x.Id == request.VehicleId);
                if(vehicle is null)
                {
                    return DataResult<CreateJobResponse>.Invalid("Geçerli bir araç bulunamadı.");
                }

                var customer = await _unitOfWork.Customers.FirstOrDefaultAsync(x => x.Id == request.CustomerId);
                if(customer is null)
                {
                    return DataResult<CreateJobResponse>.Invalid("Geçerli bir müşteri bulunamadı.");
                }

                var job = new Infrastructure.Data.Postgres.Entities.Job
                {
                    Title = request.Title,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    AgreementAmount = request.AgreementAmount,
                    Description = request.Description,
                    Location = request.Location,
                    VehicleId = request.VehicleId,
                    CustomerId = request.CustomerId
                };

                await _unitOfWork.Jobs.AddAsync(job);
                await _unitOfWork.CommitAsync();

                var result = new CreateJobResponse
                {
                    Id = job.Id,
                    Title = job.Title,
                    VehicleId = job.VehicleId,
                    CustomerId = job.CustomerId
                };
                return DataResult<CreateJobResponse>.Success(result);
            }
            catch(Exception ex)
            {
                _logger.LogExtended(LogEventLevel.Error, $"Error on {GetType().Name}", ex);
                return DataResult<CreateJobResponse>.Error(ex.Message);
            }
        }
    }
}
