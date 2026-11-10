using System.Net;
using Defra.PTS.User.ApiServices.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Defra.PTS.User.Functions.Functions.User;

/// <summary>
/// Check whether a user is suspended
/// </summary>
public class IsUserSuspended
{
    private readonly IUserService _userService;
    private readonly ILogger<IsUserSuspended> _logger;

    /// <summary>
    /// Check whether a user is suspended
    /// </summary>
    /// <param name="userService">The user service</param>
    /// <param name="log">The log</param>
    public IsUserSuspended(IUserService userService, ILogger<IsUserSuspended> log)
    {
        _userService = userService;
        _logger = log;
    }

    [Function(nameof(IsUserSuspended))]
    [OpenApiOperation(operationId: "IsUserSuspended", tags: new[] { "User" }, Summary = "Check if a user is suspended", Description = "Returns whether the user identified by their contact ID is suspended")]
    [OpenApiParameter(name: "contactId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The contact ID (GUID)")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(bool), Description = "Suspension status retrieved successfully")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid contact ID format")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "IsUserSuspended/{contactId}")] HttpRequestData req, string contactId)
    {
        _logger.LogInformation($"{nameof(IsUserSuspended)} HTTP trigger function processed a request.");

        if (!Guid.TryParse(contactId, out Guid contactGuid))
        {
            var badResponse = req.CreateResponse();
            badResponse.StatusCode = HttpStatusCode.BadRequest;
            await badResponse.WriteStringAsync("You must provide a valid value for contactId");
            return badResponse;
        }

        var result = await _userService.IsUserSuspended(contactGuid);

        var response = req.CreateResponse();
        response.StatusCode = HttpStatusCode.OK;
        await response.WriteAsJsonAsync(result);
        return response;
    }
}
