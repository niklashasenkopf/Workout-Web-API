using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace C_Sharp_Web_API.Authentication;

[Route("api/register")]
[ApiController]
public class RegistrationController(
    UserManager<ApiUser> userManager,
    IConfiguration configuration
    ) : ControllerBase
{
    private readonly UserManager<ApiUser> _userManager =
        userManager ?? throw new ArgumentNullException(nameof(userManager));

    private readonly IConfiguration _configuration =
        configuration ?? throw new ArgumentNullException(nameof(configuration));

    public class RegisterRequest
    {
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email)
            || string.IsNullOrWhiteSpace(request.Username)
            || string.IsNullOrWhiteSpace(request.Password)
           )
        {
            return BadRequest("Requirements: Email, Username and password");
        }

        var newUser = new ApiUser()
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Username
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { Errors = errors }); 
        }

        return Ok("Registration succeeded"); 
    }
}