using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

[ApiController, Authorize, Route("api/[controller]")]
[SwaggerResponse((int) System.Net.HttpStatusCode.NotFound)]
[SwaggerResponse((int) System.Net.HttpStatusCode.BadRequest)]
[SwaggerResponse((int) System.Net.HttpStatusCode.InternalServerError)]
public class ApiController : Controller
{
    protected readonly ILogger<ApiController> Logger;

    public ApiController(ILogger<ApiController> logger)
        => Logger = logger;
}