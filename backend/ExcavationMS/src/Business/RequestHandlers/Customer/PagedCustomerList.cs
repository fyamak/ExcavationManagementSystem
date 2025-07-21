using Business.Mediator.Behaviours.Requests;
using FluentValidation;
using Infrastructure.Data.Postgres;
using MediatR;
using Serilog;
using Serilog.Events;
using Shared.Extensions;
using Shared.Models.Results;

namespace Business.RequestHandlers.Customer;

public abstract class PagedCustomerList
{
    public class PagedCustomerListRequest: IRequest<PagedResult<PagedCustomerListResponse>>, IRequestToValidate
    {
        public int PageNumber;
        public int PageSize;
        public string? Search;
    }

    public class PagedCustomerListResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Detail { get; set; }
    }

    public class PagedCustomerListValidator : AbstractValidator<PagedCustomerListRequest>
    {
        public PagedCustomerListValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("Sayfa numarası sıfırdan küçük olamaz.");
            RuleFor(x => x.PageSize).GreaterThan(0).WithMessage("Sayfa boyutu sıfırdan küçük olamaz.");
        }
    }


    public class PagedCustomerListRequestHandler : IRequestHandler<PagedCustomerListRequest, PagedResult<PagedCustomerListResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public PagedCustomerListRequestHandler(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PagedResult<PagedCustomerListResponse>> Handle(PagedCustomerListRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var (pagedCustomers, totalCount) = await _unitOfWork.Customers.GetPagedAsync(pageNumber: request.PageNumber, pageSize: request.PageSize, search: request.Search);

                var result = pagedCustomers.Select(x => new PagedCustomerListResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Phone = x.Phone,
                    Detail = x.Detail
                });

                return PagedResult<PagedCustomerListResponse>.Success(
                    result, 
                    request.PageNumber, 
                    request.PageSize, 
                    totalCount);

            }
            catch(Exception ex)
            {
                _logger.LogExtended(LogEventLevel.Error, $"Error on {GetType().Name}", ex);
                return PagedResult<PagedCustomerListResponse>.Error(ex.Message); 
            }
        }
    }
}
