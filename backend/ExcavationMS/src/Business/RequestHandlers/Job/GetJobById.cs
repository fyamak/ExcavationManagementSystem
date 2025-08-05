using Infrastructure.Data.Postgres;
using Infrastructure.Data.Postgres.Entities;
using MediatR;
using Serilog;
using Serilog.Events;
using Shared.Extensions;
using Shared.Models.Results;

namespace Business.RequestHandlers.Job;

public abstract class GetJobById
{
    public class GetJobByIdRequest : IRequest<DataResult<GetJobByIdResponse>>
    {
        public int Id;
    }

    public class GetJobByIdResponse
    {
        public string Title { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int? StartHour { get; set; }
        public int? EndHour { get; set; }
        public int? AgreementAmount { get; set; }
        public string? Description { get; set; }
        public int VehicleId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public List<ExpenseIncomeDto>  Expenses { get; set; }
        public List<ExpenseIncomeDto> Incomes { get; set; }
    }

    public class ExpenseIncomeDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Price { get; set; }
        public DateOnly? Date { get; set; }
    }

    public class GetJobByIdRequestHandler : IRequestHandler<GetJobByIdRequest, DataResult<GetJobByIdResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public GetJobByIdRequestHandler(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<DataResult<GetJobByIdResponse>> Handle(GetJobByIdRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var job = await _unitOfWork.Jobs.GetJobByIdAsync(request.Id);
                if (job is null)
                {
                    return DataResult<GetJobByIdResponse>.Invalid("İş bulunamadı.");
                }

                var expenses = job.Expenses.Select(x => new ExpenseIncomeDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Price = x.Price,
                    Date = x.Date
                }).ToList();

                var incomes = job.Incomes.Select(x => new ExpenseIncomeDto()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Price = x.Price,
                    Date = x.Date
                }).ToList();

                var result = new GetJobByIdResponse
                {
                    Title = job.Title,
                    StartDate = job.StartDate,
                    EndDate = job.EndDate,
                    StartHour = job.StartHour,
                    EndHour = job.EndHour,
                    AgreementAmount = job.AgreementAmount,
                    Description = job.Description,
                    VehicleId = job.VehicleId,
                    CustomerId = job.CustomerId,
                    CustomerName = job.Customer.Name,
                    Expenses = expenses,
                    Incomes = incomes
                };

                return DataResult<GetJobByIdResponse>.Success(result);

            }
            catch (Exception ex)
            {
                _logger.LogExtended(LogEventLevel.Error, $"Error on {GetType().Name}", ex);
                return DataResult<GetJobByIdResponse>.Error(ex.Message);
            }
        }
    }


}
