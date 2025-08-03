using Infrastructure.Data.Postgres;
using MediatR;
using Serilog;
using Serilog.Events;
using Shared.Extensions;
using Shared.Models.Results;

namespace Business.RequestHandlers.Customer;

public abstract class CustomerList
{
    public class CustomerListRequest : IRequest<DataResult<List<CustomerListResponse>>>
    {
    }
    
    public class CustomerListResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Detail { get; set; }
    }

    public class CustomerListRequestHandler : IRequestHandler<CustomerListRequest, DataResult<List<CustomerListResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public CustomerListRequestHandler(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<DataResult<List<CustomerListResponse>>> Handle(CustomerListRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var customers = await _unitOfWork.Customers.GetAllAsync();

                var result = customers.Select(x => new CustomerListResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Phone = x.Phone,
                    Detail = x.Detail
                }).ToList();

                return DataResult<List<CustomerListResponse>>.Success(result);
            }
            catch(Exception ex)
            {
                _logger.LogExtended(LogEventLevel.Error, $"Error on {GetType().Name}", ex);
                return DataResult<List<CustomerListResponse>>.Error(ex.Message);
            }
        }
    }
}
