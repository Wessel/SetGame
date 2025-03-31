using Microsoft.AspNetCore.Mvc;
using Backend.Interfaces;

namespace Backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase {
  private readonly IAuthService _authService = authService;

  [HttpPost("login")]
  public IActionResult Login([FromBody] UserLogin credentials) {
    var user = _authService.ValidateUser(credentials);

    if (user != null) {
      var token = _authService.GenerateJwtToken(user.Id.ToString());
      return Ok(new { token });
    }

    return Unauthorized();
  }
}