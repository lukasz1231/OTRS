using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using otrs_backend.Requests;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly UserService _userService;

    public ProfileController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        // Ponownie wyciągamy ID z tokena - to najbezpieczniejsza metoda
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
        {
            return Unauthorized();
        }

        var profile = await _userService.GetUserProfileAsync(userId);
        if (profile == null) return NotFound("Użytkownik nie istnieje.");

        return Ok(profile);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        // Wyciągamy ID z tokena
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
        {
            return Unauthorized();
        }

        var success = await _userService.UpdateUserProfileAsync(userId, request);

        if (!success)
        {
            return NotFound("Użytkownik nie istnieje.");
        }

        return Ok(new { message = "Profil został zaktualizowany pomyślnie." });
    }
}