using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NetWebApiKC.Controllers;


[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpGet("secure-endpoint")]
    [Authorize] // simple authorization 
    public IActionResult GetSecureData()
    {
        return Ok("This is a secure endpoint!");
    }

    [HttpGet("admin-endpoint")]
    [Authorize(Policy = "RequireWritedataRole")]
    public IActionResult GetAdminData()
    {
        return Ok("This is an admin-only endpoint!");
    }

    [HttpGet("user-endpoint")]
    [Authorize(Roles = "administrador")]
    public IActionResult GetUserData()
    {
        return Ok("This is a user-only endpoint!");
    }
    
}
