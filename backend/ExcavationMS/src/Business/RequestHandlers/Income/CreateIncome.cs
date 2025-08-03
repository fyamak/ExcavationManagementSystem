using FluentValidation;
using Infrastructure.Data.Postgres;
using MediatR;
using Serilog;
using Serilog.Events;
using Shared.Extensions;
using Shared.Models.Results;

namespace Business.RequestHandlers.Income;

public abstract class CreateIncome
{
    public class CreateIncomeRequest : IRequest<DataResult<CreateIncomeResponse>>
    {
        public int JobId { get; set; }
        public string Title { get; set; }
        public int Price { get; set; }
        public DateOnly? Date { get; set; }
    }

    public class CreateIncomeResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Price { get; set; }
        public DateOnly? Date { get; set; }
        public int JobId { get; set; }
    }

    public class CreateIncomeRequestValidator : AbstractValidator<CreateIncomeRequest>
    {
        public CreateIncomeRequestValidator()
        {
            RuleFor(x => x.JobId).NotEmpty().WithMessage("İş Id boş bırakılamaz.");
            RuleFor(x => x.Title).NotEmpty().WithMessage("Başlık boş bırakılamaz.");
            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Fiyat boş bırakılamaz.")
                .GreaterThan(0).WithMessage("Fiyat sıfırdan küçük olamaz.");
        }
    }

    public class CreateIncomeRequestHandler : IRequestHandler<CreateIncomeRequest, DataResult<CreateIncomeResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public CreateIncomeRequestHandler(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<DataResult<CreateIncomeResponse>> Handle(CreateIncomeRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var job = await _unitOfWork.Jobs.FirstOrDefaultAsync(x => x.Id == request.JobId);
                if (job is null)
                {
                    return DataResult<CreateIncomeResponse>.Invalid("Geçerli bir iş bulunamadı.");
                }

                var income = new Infrastructure.Data.Postgres.Entities.Income
                {
                    Title = request.Title,
                    Price = request.Price,
                    Date = request.Date,
                    JobId = request.JobId
                };
                await _unitOfWork.Incomes.AddAsync(income);
                await _unitOfWork.CommitAsync();

                var result = new CreateIncomeResponse
                {
                    Id = income.Id,
                    Title = income.Title,
                    Price = income.Price,
                    Date = income.Date,
                    JobId = income.JobId
                };

                return DataResult<CreateIncomeResponse>.Success(result);
            }
            catch(Exception ex)
            {
                _logger.LogExtended(LogEventLevel.Error, $"Error on {GetType().Name}", ex);
                return DataResult<CreateIncomeResponse>.Error(ex.Message);
            }
        }
    }
}
