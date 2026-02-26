using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using otrs_backend.Data;
using otrs_backend.Services;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<System.Net.Mail.SmtpClient>(sp =>
{
    return new System.Net.Mail.SmtpClient(Environment.GetEnvironmentVariable("MAILTRAP_HOST")!)
    {
        Port = int.Parse(Environment.GetEnvironmentVariable("MAILTRAP_PORT")!),
        Credentials = new System.Net.NetworkCredential(
            Environment.GetEnvironmentVariable("MAILTRAP_USERNAME"), 
            Environment.GetEnvironmentVariable("MAILTRAP_PASSWORD")),
        EnableSsl = true
    };
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration.GetSection("JwtSettings:SecretKey").Value!)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:8080")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Services
builder.Services.AddScoped<TicketService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated(); 
    otrs_backend.Data.DataSeeder.SeedRoles(context); 
}

app.Run();