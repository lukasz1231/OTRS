using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using otrs_backend.Data;
using otrs_backend.Tests.Helpers;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // ✔ ENV TU
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // ❌ usuń wszystkie DbContexty
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

            // ✔ tylko INMEMORY
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // ❌ NIE ROBIMY BuildServiceProvider()

            // ✔ AUTH
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.Scheme;
                options.DefaultChallengeScheme = TestAuthHandler.Scheme;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.Scheme, _ => { });
        });
    }
}