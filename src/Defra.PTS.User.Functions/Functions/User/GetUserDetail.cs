using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

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

    [FunctionName(nameof(GetUserDetail))]
    [OpenApiOperation(operationId: nameof(GetUserDetail), tags: new[] { TagName })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "userId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **UserId** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<UserDetail>), Description = "OK")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "BAD REQUEST")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetUserDetail/{userId}")] HttpRequest req, string userId)
    {
        _logger.LogInformation($"{nameof(GetUserDetail)} HTTP trigger function processed a request.");

        if (!Guid.TryParse(userId, out Guid userGuid))
        {
            return new BadRequestObjectResult("You must provide a valid value for userId");
        }

        var result = await _userService.GetUserDetail(userGuid);

        return new OkObjectResult(result);
    }
}

