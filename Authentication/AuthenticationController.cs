using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace C_Sharp_Web_API.Authentication;

/// <summary>
/// Handles user authentication operations for the API, including login and token generation.
/// </summary>
[Route("workout-api/authentication")]
[ApiController]
public class AuthenticationController(
    UserManager<ApiUser> userManager,
    SignInManager<ApiUser> signInManager,
    IConfiguration configuration
    ) : ControllerBase
{
    private readonly UserManager<ApiUser> _userManager =
        userManager ?? throw new ArgumentNullException(nameof(userManager));

    private readonly SignInManager<ApiUser> _signInManager =
        signInManager ?? throw new ArgumentNullException(nameof(signInManager));

    private readonly IConfiguration _configuration =
        configuration ?? throw new ArgumentNullException(nameof(configuration));

    /// <summary>
    /// Represents a request model for user login, containing the user's identifier (such as username or email)
    /// and password used for authentication.
    /// </summary>
    public class LoginRequest
    {
        public string? Identifier { get; set; }
        public string? Password { get; set; }
    }

    private class LoginResponse(string tokenString, DateTime expiresAt)
    {
        public string TokenString { get; set; } = tokenString;
        public DateTime ExpiresAtUtc { get; set; } = expiresAt;
    }

    /// <summary>
    /// Authenticates a user with provided login credentials and generates a JWT token on successful authentication.
    /// </summary>
    /// <param name="loginRequest">
    /// The login request containing the user's identifier (e.g., username or email) and password.
    /// </param>
    /// <returns>
    /// Returns an ActionResult containing a generated JWT token and its expiration time on successful authentication,
    /// or an error response for invalid credentials or missing parameters.
    /// </returns>
    [HttpPost("login")]
    public async Task<ActionResult<string>> Authenticate(
        [FromBody] LoginRequest loginRequest)
    {
        if (string.IsNullOrWhiteSpace(loginRequest.Identifier) || string.IsNullOrWhiteSpace(loginRequest.Password))
        {
            return BadRequest("Missing identifier or password");
        }

        var user = await _userManager.FindByNameAsync(loginRequest.Identifier)
                   ?? await _userManager.FindByEmailAsync(loginRequest.Identifier);

        if (user is null) return Unauthorized("User not found");

        var checkPassword = await _signInManager.CheckPasswordSignInAsync(
            user, loginRequest.Password, lockoutOnFailure: true);

        if (!checkPassword.Succeeded)
        {
            return Unauthorized("Password is wrong");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var (tokenString, expiresAt) = GenerateJwt(user, roles);

        var loginResponse = new LoginResponse(tokenString, expiresAt);

        return Ok(loginResponse);
    }

    private (string tokenString, DateTime expiresAtUtc) GenerateJwt(
        ApiUser user,
        IList<string> roles
    )
    {
        var key = _configuration["Authentication:SecretForKey"] ?? 
                  throw new InvalidOperationException("Missing Authentication:SecretForKey");
        var issuer = _configuration["Authentication:Issuer"];
        var audience = _configuration["Authentication:Audience"];
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>()
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };


        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var now = DateTime.UtcNow;
        var expiresAt = now.AddHours(1);

        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(jwt);
        return (tokenString, expiresAt);

    }

}