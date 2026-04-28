using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using otrs_backend.Data;
using otrs_backend.Services;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// --- Serwisy ---

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<System.Net.Mail.SmtpClient>(sp =>
{
    var host = Environment.GetEnvironmentVariable("MAILTRAP_HOST");
    var portRaw = Environment.GetEnvironmentVariable("MAILTRAP_PORT");
    var username = Environment.GetEnvironmentVariable("MAILTRAP_USERNAME");
    var password = Environment.GetEnvironmentVariable("MAILTRAP_PASSWORD");

    var port = 2525;
    if (!string.IsNullOrWhiteSpace(portRaw) && int.TryParse(portRaw, out var parsedPort))
    {
        port = parsedPort;
    }

    var smtpClient = new System.Net.Mail.SmtpClient(
        string.IsNullOrWhiteSpace(host) ? "localhost" : host)
    {
        Port = port,
        EnableSsl = true
    };

    if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
    {
        smtpClient.Credentials = new System.Net.NetworkCredential(username, password);
    }

    return smtpClient;
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

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:8080")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<UserService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- Middleware Pipeline ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    // OBSŁUGA BŁĘDÓW DLA PRODUKCJI
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { message = "Błąd serwera. Spróbuj później." });
        });
    });
}

app.UseHttpsRedirection();

// CORS musi być przed Auth!
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    otrs_backend.Data.DataSeeder.SeedRoles(context); 
}

app.Run();