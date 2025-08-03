using Business.Mediator.Behaviours.Requests;
using FluentValidation;
using Infrastructure.Data.Postgres;
using MediatR;
using Serilog;
using Serilog.Events;
using Shared.Extensions;
using Shared.Models.Results;

namespace Business.RequestHandlers.Job;

public abstract class PagedJobList
{
    public class PagedJobListRequest : IRequest<PagedResult<PagedJobListResponse>>, IRequestToValidate
    {
        public int VehicleId;
        public int PageNumber;
        public int PageSize;
        public string? Search;
    }

    public class PagedJobListResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CustomerName { get; set; }
        public int TotalIncome { get; set; }
        public int TotalExpense { get; set; }
    }

    public class PagedJobListRequestValidator : AbstractValidator<PagedJobListRequest>
    {
        public PagedJobListRequestValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("Sayfa numarası sıfırdan küçük olamaz.");
            RuleFor(x => x.PageSize).GreaterThan(0).WithMessage("Sayfa boyutu sıfırdan küçük olamaz.");
        }
    }
    public class PagedJobListRequestHandler : IRequestHandler<PagedJobListRequest, PagedResult<PagedJobListResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public PagedJobListRequestHandler(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PagedResult<PagedJobListResponse>> Handle(PagedJobListRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var vehicle = await _unitOfWork.Vehicles.FirstOrDefaultAsync(x => x.Id == request.VehicleId);
                if(vehicle is null)
                {
                    return PagedResult<PagedJobListResponse>.Invalid("Belirtilen araç bulunamadı.");
                }

                var (pagedJobs, totalCount) = await _unitOfWork.Jobs.GetPagedAsync(
                    vehicleId: request.VehicleId,
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    search: request.Search);

                var result = pagedJobs.Select(x => new PagedJobListResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    CustomerName = x.Customer.Name,
                    TotalExpense = x.Expenses.Sum(e => e.Price),
                    TotalIncome = x.Incomes.Sum(i => i.Price)
                });

                return PagedResult<PagedJobListResponse>.Success(
                    result,
                    request.PageNumber,
                    request.PageSize,
                    totalCount);

            }
            catch(Exception ex)
            {
                _logger.LogExtended(LogEventLevel.Error, $"Error on {GetType().Name}", ex);
                return PagedResult<PagedJobListResponse>.Error(ex.Message);
            }
        }
    }
}
