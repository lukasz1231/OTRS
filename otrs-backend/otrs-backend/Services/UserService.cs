using Microsoft.EntityFrameworkCore;
using otrs_backend.Data;
using otrs_backend.Requests;
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
    public async Task<bool> UpdateUserProfileAsync(int userId, UpdateProfileRequest request)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null) return false;

        user.Name = request.Name;
        user.Surname = request.Surname;
        user.BirthDate = request.BirthDate;
        user.Bio = request.Bio ?? user.Bio;
        user.AvatarUrl = request.AvatarUrl ?? user.AvatarUrl;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return true;
    }
}