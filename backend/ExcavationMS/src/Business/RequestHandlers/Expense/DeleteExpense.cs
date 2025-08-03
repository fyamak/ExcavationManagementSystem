using Infrastructure.Data.Postgres;
using MediatR;
using Serilog;
using Serilog.Events;
using Shared.Extensions;
using Shared.Models.Results;

namespace Business.RequestHandlers.Expense;

public abstract class DeleteExpense
{
    public class DeleteExpenseRequest : IRequest<Result>
    {
        public int Id;
    }

    public class DeleteExpenseRequestHandler : IRequestHandler<DeleteExpenseRequest, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public DeleteExpenseRequestHandler(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteExpenseRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var expense = await _unitOfWork.Expenses.FirstOrDefaultAsync(x => x.Id == request.Id);
                if(expense is null)
                {
                    return Result.Invalid("Gider bulunamadı.");
                }

                await _unitOfWork.Expenses.RemoveById(expense.Id);
                await _unitOfWork.CommitAsync();

                return Result.Success("Gider başarıyla silindi");
            }
            catch(Exception ex)
            {
                _logger.LogExtended(LogEventLevel.Error, $"Error on {GetType().Name}", ex);
                return Result.Error(ex.Message);
            }
        }
    }
}
