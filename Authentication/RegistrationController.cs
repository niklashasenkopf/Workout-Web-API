using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace C_Sharp_Web_API.Authentication;

/// <summary>
/// Handles user registration-related features (e.g., registering new users) within the application.
/// </summary>
/// <remarks>
/// This controller provides endpoints for managing user registration processes. It uses
/// dependency injection to interact with the application's user management systems and
/// configuration settings. The primary responsibilities include capturing registration
/// data, creating new users, and returning appropriate responses based on the outcome
/// of the registration attempt.
/// </remarks>
[Route("workout-api/authentication")]
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

    /// <summary>
    /// Represents the request model for user registration.
    /// </summary>
    /// <remarks>
    /// This class is used to capture the necessary information for registering a new user
    /// within the application. It includes properties for email, username, and password,
    /// which are required for creating a new user account.
    /// </remarks>
    public class RegisterRequest
    {
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    /// <summary>
    /// Registers a new user with the provided email, username, and password.
    /// </summary>
    /// <param name="request">
    /// An object containing the user registration details, including email, username, and password.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains:
    /// - A 200 OK response if registration succeeded.
    /// - A 400 Bad Request response if the input is invalid or registration fails.
    /// </returns>
    [HttpPost("register")]
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