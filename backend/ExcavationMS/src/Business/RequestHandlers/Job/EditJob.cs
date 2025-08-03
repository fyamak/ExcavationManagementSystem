using Business.Mediator.Behaviours.Requests;
using FluentValidation;
using Infrastructure.Data.Postgres;
using Infrastructure.Data.Postgres.Entities;
using MediatR;
using Serilog;
using Serilog.Events;
using Shared.Extensions;
using Shared.Models.Results;
using static Business.RequestHandlers.Customer.EditCustomer;

namespace Business.RequestHandlers.Job;

public abstract class EditJob
{
    public class EditJobRequest : IRequest<DataResult<EditJobResponse>>, IRequestToValidate
    {
        public int Id;
        public string? Title { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int? AgreementAmount { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
    }

    public class EditJobResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int? AgreementAmount { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }

    }

    public class EditJobRequestValidator : AbstractValidator<EditJobRequest>
    {
        public EditJobRequestValidator()
        {
            RuleFor(x => x.Title)
               .MaximumLength(256).WithMessage("İş başlığı boyutu 256 karakterden fazla olamaz.")
               .When(x => !string.IsNullOrWhiteSpace(x.Title));

            RuleFor(x => x.Description)
                .MaximumLength(4096).WithMessage("Açıklama boyutu 4096 karakterden fazla olamaz.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Location)
                .MaximumLength(256).WithMessage("Konum bilgisi boyutu 256 karakterden fazla olamaz.")
                .When(x => !string.IsNullOrWhiteSpace(x.Location));

        }
    }

    public class EditJobRequestHandler : IRequestHandler<EditJobRequest, DataResult<EditJobResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public EditJobRequestHandler(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<DataResult<EditJobResponse>> Handle(EditJobRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var job = await _unitOfWork.Jobs.FirstOrDefaultAsync(x => x.Id == request.Id, tracked: true);
                if (job is null)
                {
                    return DataResult<EditJobResponse>.Invalid("İş bulunamadı.");
                }

                if (request.Title is not null)
                {
                    job.Title = request.Title;
                }

                if(request.StartDate is not null)
                {
                    job.StartDate = request.StartDate;
                }

                if(request.EndDate is not null)
                {
                    job.EndDate = request.EndDate;
                }

                if(request.AgreementAmount is not null)
                {
                    job.AgreementAmount = request.AgreementAmount;
                }

                if(request.Description is not null)
                {
                    job.Description = request.Description;
                }

                if(request.Location is not null)
                {
                    job.Location = request.Location;
                }

                await _unitOfWork.CommitAsync();

                var result = new EditJobResponse
                {
                    Id = job.Id,
                    Title = job.Title,
                    StartDate = job.StartDate,
                    EndDate = job.EndDate,
                    AgreementAmount = job.AgreementAmount,
                    Description = job.Description,
                    Location = job.Location
                };

                return DataResult<EditJobResponse>.Success(result);
            }
            catch(Exception ex)
            {
                _logger.LogExtended(LogEventLevel.Error, $"Error on {GetType().Name}", ex);
                return DataResult<EditJobResponse>.Error(ex.Message);
            }
        }
    }
}
