using Business.Mediator.Behaviours.Requests;
using FluentValidation;
using Infrastructure.Data.Postgres;
using MediatR;
using Serilog;
using Serilog.Events;
using Shared.Extensions;
using Shared.Models.Results;

namespace Business.RequestHandlers.Expense;

public abstract class CreateExpense
{
    public class CreateExpenseRequest : IRequest<DataResult<CreateExpenseResponse>>, IRequestToValidate
    {
        public int JobId { get; set; }
        public string Title { get; set; }
        public int Price { get; set; }
        public DateOnly? Date { get; set; }
    }

    public class CreateExpenseResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Price { get; set; }
        public DateOnly? Date { get; set; }
        public int JobId { get; set; }
    }

    public class CreateExpenseRequestValidator : AbstractValidator<CreateExpenseRequest>
    {
        public CreateExpenseRequestValidator()
        {
            RuleFor(x => x.JobId).NotEmpty().WithMessage("İş Id boş bırakılamaz.");
            RuleFor(x => x.Title).NotEmpty().WithMessage("Başlık boş bırakılamaz.");
            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Fiyat boş bırakılamaz.")
                .GreaterThan(0).WithMessage("Fiyat sıfırdan küçük olamaz.");
        }
    }

    public class CreateExpenseRequestHandler : IRequestHandler<CreateExpenseRequest, DataResult<CreateExpenseResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public CreateExpenseRequestHandler(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<DataResult<CreateExpenseResponse>> Handle(CreateExpenseRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var job = await _unitOfWork.Jobs.FirstOrDefaultAsync(x => x.Id == request.JobId);
                if (job is null)
                {
                    return DataResult<CreateExpenseResponse>.Invalid("Geçerli bir iş bulunamadı.");
                }

                var expense = new Infrastructure.Data.Postgres.Entities.Expense
                {
                    Title = request.Title,
                    Price = request.Price,
                    Date = request.Date,
                    JobId = request.JobId
                };
                await _unitOfWork.Expenses.AddAsync(expense);
                await _unitOfWork.CommitAsync();

                var result = new CreateExpenseResponse
                {
                    Id = expense.Id,
                    Title = expense.Title,
                    Price = expense.Price,
                    Date = expense.Date,
                    JobId = expense.JobId
                };

                return DataResult<CreateExpenseResponse>.Success(result);

            }
            catch(Exception ex)
            {
                _logger.LogExtended(LogEventLevel.Error, $"Error on {GetType().Name}", ex);
                return DataResult<CreateExpenseResponse>.Error(ex.Message);
                
            }
        }
    }

}
