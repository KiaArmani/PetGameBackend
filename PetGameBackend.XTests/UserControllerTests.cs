using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using PetGameBackend.Models.Data;
using PetGameBackend.Models.Requests.User;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace PetGameBackend.XTests
{
    public class UserControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        public UserControllerTests(WebApplicationFactory<Startup> fixture)
        {
            Setup.SetupEnvironment();
            Client = fixture.CreateClient();
        }

        public HttpClient Client { get; }

        [Fact]
        public async Task ShouldCreateUser()
        {
            // Arrange
            var userIdentifier = Guid.NewGuid().ToString();
            var payload = new UserControllerRootPost
            {
                UserIdentifier = userIdentifier
            };

            // Act
            var createResponse = await Shared.CreateUser(Client, payload);

            // Assert
            Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        }

        [Fact]
        public async Task ShouldGetUser()
        {
            // Arrange
            var userIdentifier = Guid.NewGuid().ToString();
            var payload = new UserControllerRootGet
            {
                UserIdentifier = userIdentifier
            };

            // Act

            // Create User first
            await Shared.CreateUser(Client, payload);

            // Get User
            var getUserResponse = await Shared.GetUser(Client, payload);

            // Assert
            var user = JsonConvert.DeserializeObject<User>(await getUserResponse.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, getUserResponse.StatusCode);
            Assert.Equal(user.UserIdentifier, payload.UserIdentifier);
        }

        [Fact]
        public async Task ShouldUpdateUser()
        {
            // Arrange
            var userIdentifier = Guid.NewGuid().ToString();
            const string userName = "PeterGriffin";
            var payload = new UserControllerRootPut
            {
                UserIdentifier = userIdentifier,
                UserName = userName
            };

            // Prepare PUT Request
            var getUserRequest = new HttpRequestMessage(HttpMethod.Put, "api/User")
            {
                Content = Shared.FormatContent<UserControllerRootPut>(payload)
            };

            // Act

            // Create User first
            await Shared.CreateUser(Client, payload);

            // Update User
            var updateUserResponse = await Client.SendAsync(getUserRequest);

            // Get User
            var getUserResponse = await Shared.GetUser(Client, payload);

            // Assert
            var user = JsonConvert.DeserializeObject<User>(await getUserResponse.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, updateUserResponse.StatusCode);
            Assert.Equal(user.UserIdentifier, payload.UserIdentifier);
            Assert.Equal(userName, user.UserName);
        }

        [Fact]
        public async Task ShouldDeleteUser()
        {
            // Arrange
            var payload = new UserControllerRootDelete
            {
                UserIdentifier = "197231f1-166d-4a3e-8a83-99dce27ff68c"
            };

            // Prepare DELETE Request
            var deleteUserRequest = new HttpRequestMessage(HttpMethod.Delete, "api/User")
            {
                Content = Shared.FormatContent<UserControllerRootDelete>(payload)
            };

            // Act
            // Create User first
            await Shared.CreateUser(Client, payload);

            // Delete User
            var deleteUserResponse = await Client.SendAsync(deleteUserRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, deleteUserResponse.StatusCode);
        }
    }
}