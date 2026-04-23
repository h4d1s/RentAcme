using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using User.Domain.AggregatesModel.ApplicationUserAggregate;
using Userproto;

namespace User.Infrastructure.Grpc;

[Authorize]
public class UserGrpcServerService : UserProtoService.UserProtoServiceBase
{
    private readonly ILogger<UserGrpcServerService> _logger;
    private readonly IApplicationUserRepository _applicationUserRepository;

    public UserGrpcServerService(
        ILogger<UserGrpcServerService> logger,
        IApplicationUserRepository applicationUserRepository)
    {
        _logger = logger;
        _applicationUserRepository = applicationUserRepository;
    }

    public override async Task<UserStatusResponse> CheckUser(CheckUserRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Checking user {Id}", request.UserId);
        var isExists = await _applicationUserRepository.GetByIdAsync(Guid.Parse(request.UserId)) is not null;

        var response = new UserStatusResponse
        {
            IsExists = isExists
        };

        return response;
    }

    public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Getting user {Id}", request.UserId);
        ApplicationUser? user;

        try
        {
            var id = Guid.Parse(request.UserId);
            user = await _applicationUserRepository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        return new GetUserResponse
        {
            Id = user?.Id.ToString(),
            ExternalId = user?.ExternalId,
            Email = user?.Email,
            FirstName = user?.FirstName,
            LastName = user?.LastName,
        };
    }

    public override async Task<GetUserByExternalIdResponse> GetUserByExternalId(GetUserByExternalIdRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Getting user by external id: {Id}", request.UserExternalId);
        ApplicationUser? user;

        try
        {
            user = await _applicationUserRepository.GetByExternalIdAsync(request.UserExternalId);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        return new GetUserByExternalIdResponse
        {
            Id = user?.Id.ToString(),
            ExternalId = user?.ExternalId,
            Email = user?.Email,
            FirstName = user?.FirstName,
            LastName = user?.LastName,
        };
    }
}
