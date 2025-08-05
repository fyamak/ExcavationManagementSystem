using Business.Mediator.Behaviours.Requests;
using FluentValidation;
using Infrastructure.Data.Postgres;
using MediatR;
using Serilog;
using Serilog.Events;
using Shared.Extensions;
using Shared.Models.Results;

namespace Business.RequestHandlers.Job;

public abstract class EditJob
{
    public class EditJobRequest : IRequest<DataResult<EditJobResponse>>, IRequestToValidate
    {
        public int Id;
        public string? Title { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int? StartHour { get; set; }
        public int? EndHour { get; set; }
        public int? AgreementAmount { get; set; }
        public string? Description { get; set; }
    }

    public class EditJobResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int? StartHour { get; set; }
        public int? EndHour { get; set; }
        public int? AgreementAmount { get; set; }
        public string? Description { get; set; }

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

            RuleFor(x => x.StartHour)
                .GreaterThan(0).WithMessage("Başlangıç saati sıfırdan küçük olamaz.");
            RuleFor(x => x.EndHour)
                .GreaterThan(0).WithMessage("Bitiş saati sıfırdan küçük olamaz.");

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

                if(request.StartHour is not null)
                {
                    job.StartHour = request.StartHour;
                }

                if(request.EndHour is not null)
                {
                    job.EndHour = request.EndHour;
                }

                if(request.AgreementAmount is not null)
                {
                    job.AgreementAmount = request.AgreementAmount;
                }

                if(request.Description is not null)
                {
                    job.Description = request.Description;
                }


                await _unitOfWork.CommitAsync();

                var result = new EditJobResponse
                {
                    Id = job.Id,
                    Title = job.Title,
                    StartDate = job.StartDate,
                    EndDate = job.EndDate,
                    StartHour = job.StartHour,
                    EndHour = job.EndHour,
                    AgreementAmount = job.AgreementAmount,
                    Description = job.Description,
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
