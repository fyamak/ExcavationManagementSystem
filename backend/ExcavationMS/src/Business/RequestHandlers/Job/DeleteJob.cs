using Business.Mediator.Behaviours.Requests;
using FluentValidation;
using Infrastructure.Data.Postgres;
using MediatR;
using Serilog;
using Serilog.Events;
using Shared.Extensions;
using Shared.Models.Results;

namespace Business.RequestHandlers.Job;

public abstract class DeleteJob
{
    public class DeleteJobRequest : IRequest<Result>
    {
        public int Id;
    }

    public class DeleteJobRequestHandler : IRequestHandler<DeleteJobRequest, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        public DeleteJobRequestHandler(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteJobRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var job = await _unitOfWork.Jobs.FirstOrDefaultAsync(x => x.Id == request.Id, tracked: true);
                if(job is null)
                {
                    return Result.Invalid("İş bulunamadı.");
                }

                job.IsDeleted = true;
                await _unitOfWork.CommitAsync();
                return Result.Success("İş başarılı bir şekilde silindi");
            }
            catch(Exception ex)
            {
                _logger.LogExtended(LogEventLevel.Error, $"Error on {GetType().Name}", ex);
                return Result.Error(ex.Message);
            }
        }
    }
}
