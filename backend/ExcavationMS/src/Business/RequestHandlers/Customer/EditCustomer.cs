using Business.Mediator.Behaviours.Requests;
using FluentValidation;
using Infrastructure.Data.Postgres;
using MediatR;
using Serilog;
using Serilog.Events;
using Shared.Extensions;
using Shared.Models.Results;

namespace Business.RequestHandlers.Customer;

public abstract class EditCustomer
{
    public class EditCustomerRequest : IRequest<DataResult<EditCustomerResponse>>, IRequestToValidate
    {
        public int Id;
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Detail { get; set; }
    }

    public class EditCustomerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Detail { get; set; }
    }

    public class EditCustomerRequestValidator : AbstractValidator<EditCustomerRequest>
    {
        public EditCustomerRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Müşteri Id boş bırakılamaz.");

            RuleFor(x => x.Name)
                .MaximumLength(256).WithMessage("İsim boyutu 256 karakterden fazla olamaz.")
                .When(x => !string.IsNullOrWhiteSpace(x.Name));


            RuleFor(x => x.Phone)
                .MaximumLength(64).WithMessage("Numara boyutu 64 karakterden fazla olamaz.")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone));

            RuleFor(x => x.Detail)
                .MaximumLength(512).WithMessage("Detay boyutu 512 karakterden fazla olamaz.")
                .When(x => !string.IsNullOrWhiteSpace(x.Detail));
        }
    }

    public class EditCustomerRequestHandler : IRequestHandler<EditCustomerRequest, DataResult<EditCustomerResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public EditCustomerRequestHandler(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<DataResult<EditCustomerResponse>> Handle(EditCustomerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await _unitOfWork.Customers.FirstOrDefaultAsync(x => x.Id == request.Id, tracked: true);
                if(customer is null)
                {
                    return DataResult<EditCustomerResponse>.Invalid("Müşteri bulunamadı.");
                }

                if(request.Name is not null)
                {
                    customer.Name = request.Name;
                }

                if (request.Phone is not null)
                {
                    customer.Phone = request.Phone;
                }

                if (request.Detail is not null)
                {
                    customer.Detail = request.Detail;
                }

                await _unitOfWork.CommitAsync();

                var result = new EditCustomerResponse
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Phone = customer.Phone,
                    Detail = customer.Detail
                };
                return DataResult<EditCustomerResponse>.Success(result);


            }
            catch (Exception ex)
            {
                _logger.LogExtended(LogEventLevel.Error, $"Error on {GetType().Name}", ex);
                return DataResult<EditCustomerResponse>.Error(ex.Message);
            }
        }
    }
}
