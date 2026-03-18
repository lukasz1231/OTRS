// Services/UserService.cs
using Microsoft.EntityFrameworkCore;
using otrs_backend.Data;
using otrs_backend.Responses;

public class UserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfileResponse?> GetUserProfileAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .Include(u => u.Ques)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return null;

        return new UserProfileResponse
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            BirthDate = user.BirthDate,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            Roles = user.Roles.Select(r => r.Name).ToList(),
            Queues = user.Ques.Select(q => q.Name).ToList()
        };
    }
}