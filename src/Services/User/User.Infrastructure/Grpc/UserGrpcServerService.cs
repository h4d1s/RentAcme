using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using User.Application.Infrastructure.Services;
using Userproto;
using User.Infrastructure.Services;
using User.Domain.AggregatesModel.ApplicationUser;

namespace User.Infrastructure.Grpc;

public class UserGrpcServerService : UserProtoService.UserProtoServiceBase
{
    private readonly ILogger<UserGrpcServerService> _logger;
    private readonly IUserService _userService;

    public UserGrpcServerService(
        ILogger<UserGrpcServerService> logger,
        IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    public override async Task<UserStatusResponse> CheckUser(CheckUserRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Checking user {Id}", request.UserId);
        var isExists = await _userService.ExistsAsync(Guid.Parse(request.UserId));

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
            user = await _userService.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        return new GetUserResponse
        {
            Id = user?.Id,
            Email = user?.Email,
            FirstName = user?.FirstName,
            LastName = user?.LastName,
        };
    }
}
