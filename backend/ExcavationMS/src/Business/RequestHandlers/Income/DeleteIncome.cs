using Infrastructure.Data.Postgres;
using MediatR;
using Serilog;
using Serilog.Events;
using Shared.Extensions;
using Shared.Models.Results;

namespace Business.RequestHandlers.Income;

public abstract class DeleteIncome
{
    public class DeleteIncomeRequest : IRequest<Result>
    {
        public int Id;
    }

    public class DeleteIncomeRequestHandler : IRequestHandler<DeleteIncomeRequest, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public DeleteIncomeRequestHandler(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteIncomeRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var income = await _unitOfWork.Incomes.FirstOrDefaultAsync(x => x.Id == request.Id);
                if (income is null)
                {
                    return Result.Invalid("Gelir bulunamadı");
                }

                await _unitOfWork.Incomes.RemoveById(income.Id);
                await _unitOfWork.CommitAsync();

                return Result.Success("Gelir başarıyla silindi");
            }
            catch(Exception ex)
            {
                _logger.LogExtended(LogEventLevel.Error, $"Error on {GetType().Name}", ex);
                return Result.Error(ex.Message);
            }
        }
    }
}
