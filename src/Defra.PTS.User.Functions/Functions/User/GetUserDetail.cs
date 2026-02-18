using System.Net;
using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace Defra.PTS.User.Functions.Functions.User;

/// <summary>
/// Get user detail
/// </summary>
public class GetUserDetail
{
    private readonly IUserService _userService;
    private readonly ILogger<GetUserDetail> _logger;
    private const string TagName = "UserDetail";

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
    [OpenApiOperation(operationId: nameof(GetUserDetail), tags: TagName)]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "userId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **UserId** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<UserDetail>), Description = "OK")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "BAD REQUEST")]
    public async Task<HttpResponseData> Run(
   [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetUserDetail/{userId}")] HttpRequestData req, string userId)
    {
        _logger.LogInformation($"{nameof(GetUserDetail)} HTTP trigger function processed a request.");

  if (!Guid.TryParse(userId, out Guid userGuid))
   {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
    await badResponse.WriteAsJsonAsync("You must provide a valid value for userId");
   return badResponse;
        }

     var result = await _userService.GetUserDetail(userGuid);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(result);
        return response;
    }
}

