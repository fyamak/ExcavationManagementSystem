using Business.Mediator.Behaviours.Requests;
using FluentValidation;
using Infrastructure.Data.Postgres;
using MediatR;
using Serilog;
using Serilog.Events;
using Shared.Extensions;
using Shared.Models.Results;

namespace Business.RequestHandlers.Customer;

public abstract class CreateCustomer
{
    public class CreateCustomerRequest : IRequest<DataResult<CreateCustomerResponse>>, IRequestToValidate
    {
        public string Name { get; set; }
        public string? Phone { get; set; }
        public string? Detail { get; set; }
    }

    public class CreateCustomerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }

    public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
    {
        public CreateCustomerRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("İsim boş bırakılamaz.")
                .MaximumLength(256).WithMessage("İsim boyutu 256 karakterden fazla olamaz.");

            RuleFor(x => x.Phone)
                .MaximumLength(64).WithMessage("Numara boyutu 64 karakterden fazla olamaz.")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone));

            RuleFor(x => x.Detail)
                .MaximumLength(512).WithMessage("Detay boyutu 512 karakterden fazla olamaz.")
                .When(x => !string.IsNullOrWhiteSpace(x.Detail));
        }
    }

    public class CreateCustomerRequestHandler : IRequestHandler<CreateCustomerRequest, DataResult<CreateCustomerResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public CreateCustomerRequestHandler(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<DataResult<CreateCustomerResponse>> Handle(CreateCustomerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var customerValidation = await _unitOfWork.Customers.FirstOrDefaultAsync(x => x.Name.ToLower() == request.Name.ToLower());
                if(customerValidation is not null)
                {
                    return DataResult<CreateCustomerResponse>.Invalid("Bu müşteri zaten kayıtlı.");
                }

                var customer = new Infrastructure.Data.Postgres.Entities.Customer
                {
                    Name = request.Name,
                    Phone = request.Phone,
                    Detail = request.Detail
                };

                await _unitOfWork.Customers.AddAsync(customer);
                await _unitOfWork.CommitAsync();

                var result = new CreateCustomerResponse
                {
                    Id = customer.Id,
                    Name = customer.Name
                };

                return DataResult<CreateCustomerResponse>.Success(result);
            }
            catch(Exception ex)
            {
                _logger.LogExtended(LogEventLevel.Error, $"Error on {GetType().Name}", ex);

                return DataResult<CreateCustomerResponse>.Error(ex.Message);
            }
        }
    }
}
