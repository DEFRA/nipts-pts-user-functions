using System.Net;
using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Defra.PTS.User.Functions.Functions.User;

/// <summary>
/// Get user detail
/// </summary>
public class GetUserDetail
{
    private readonly IUserService _userService;
    private readonly ILogger<GetUserDetail> _logger;

    /// <summary>
    /// Get user detail
    /// </summary>
    /// <param name="userService">The user service</param>
    /// <param name="log">The log</param>
    public GetUserDetail(IUserService userService, ILogger<GetUserDetail> log)
    {
        _userService = userService;
        _logger = log;
    }

    [Function(nameof(GetUserDetail))]
    [OpenApiOperation(operationId: "GetUserDetail", tags: new[] { "User" }, Summary = "Get user details", Description = "Retrieves detailed information about a user by their ID")]
    [OpenApiParameter(name: "userId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The user ID (GUID)")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserDetail), Description = "User details retrieved successfully")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid user ID format")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetUserDetail/{userId}")] HttpRequestData req, string userId)
    {
        _logger.LogInformation($"{nameof(GetUserDetail)} HTTP trigger function processed a request.");

        if (!Guid.TryParse(userId, out Guid userGuid))
        {
            var badResponse = req.CreateResponse();
            badResponse.StatusCode = HttpStatusCode.BadRequest;
            await badResponse.WriteStringAsync("You must provide a valid value for userId");
            return badResponse;
        }

        var result = await _userService.GetUserDetail(userGuid);

        var response = req.CreateResponse();
        response.StatusCode = HttpStatusCode.OK;
        await response.WriteAsJsonAsync(result);
        return response;
    }
}

