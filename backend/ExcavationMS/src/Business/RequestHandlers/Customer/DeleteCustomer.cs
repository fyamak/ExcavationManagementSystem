using Business.Mediator.Behaviours.Requests;
using FluentValidation;
using Infrastructure.Data.Postgres;
using MediatR;
using Serilog;
using Serilog.Events;
using Shared.Extensions;
using Shared.Models.Results;

namespace Business.RequestHandlers.Customer;

public abstract class DeleteCustomer
{
    public class DeleteCustomerRequest : IRequest<Result>, IRequestToValidate
    {
        public int Id;
    }

    public class DeleteCustomerRequestValidator : AbstractValidator<DeleteCustomerRequest>
    {
        public DeleteCustomerRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Müşteri Id boş bırakılamaz.");
        }
    }

    public class DeleteCustomerRequestHandler : IRequestHandler<DeleteCustomerRequest, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public DeleteCustomerRequestHandler(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteCustomerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await _unitOfWork.Customers.FirstOrDefaultAsync(x => x.Id == request.Id, tracked: true);
                if(customer is null)
                {
                    return Result.Invalid("Müşteri bulunamadı");
                }

                customer.IsDeleted = true;
                await _unitOfWork.CommitAsync();
                return Result.Success("Müşteri başarılı bir şekilde silindi.");
            }
            catch(Exception ex)
            {
                _logger.LogExtended(LogEventLevel.Error, $"Error on {GetType().Name}", ex);
                return Result.Error(ex.Message);
            }
        }
    }
}
