namespace otrs_backend.Tests.Controllers
{
    using FluentAssertions;
    using otrs_backend.Tests.Helpers;
    using System.Net;
    using System.Net.Http.Headers;
    using Xunit;

    public class AdminControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AdminControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");
        }

        [Fact]
        public async Task GetUsers_AsRegularUser_ShouldReturnForbidden()
        {
            var response = await _client.GetAsync("/api/admin/users");

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task DeleteUser_AsRegularUser_ShouldReturnForbidden()
        {
            var response = await _client.DeleteAsync("/api/admin/users/1");

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task CreateQueue_AsRegularUser_ShouldReturnForbidden()
        {
            var content = new StringContent(
                "{\"name\":\"Test Queue\"}",
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("/api/admin/queues", content);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
