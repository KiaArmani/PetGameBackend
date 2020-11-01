using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using PetGameBackend.Models.Data;
using PetGameBackend.Models.Requests.Pet;
using PetGameBackend.Models.Requests.PetAction;
using PetGameBackend.Statics;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace PetGameBackend.XTests
{
    public class PetActionControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        public PetActionControllerTests(WebApplicationFactory<Startup> fixture)
        {
            Setup.SetupEnvironment();
            Client = fixture.CreateClient();
        }

        public HttpClient Client { get; }

        [Fact]
        public async Task ShouldStrokePet()
        {
            // Arrange
            var userIdentifier = await Shared.CreateUser(Client);
            var petIdentifier = await Shared.CreatePet(Client, userIdentifier);

            var payload = new PetActionControllerStrokePatch
            {
                PetIdentifier = petIdentifier,
                HappinessImprovement = 2
            };

            // Act
            var createResponse = await Shared.StrokePet(Client, payload);

            // Get Pet to see if value has increased
            var petResponse = await Shared.GetPet(Client, new PetControllerRootGet {PetIdentifier = petIdentifier});
            var pet = JsonConvert.DeserializeObject<Pet>(await petResponse.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
            Assert.Equal(pet.Happiness, PetDefaults.DefaultHappiness + 2);
        }

        [Fact]
        public async Task ShouldFeedPet()
        {
            // Arrange
            var userIdentifier = await Shared.CreateUser(Client);
            var petIdentifier = await Shared.CreatePet(Client, userIdentifier);

            var payload = new PetActionControllerFeedPatch
            {
                PetIdentifier = petIdentifier,
                HungerImprovement = 2
            };

            // Act
            var createResponse = await Shared.FeedPet(Client, payload);

            // Get Pet to see if value has increased
            var petResponse = await Shared.GetPet(Client, new PetControllerRootGet {PetIdentifier = petIdentifier});
            var pet = JsonConvert.DeserializeObject<Pet>(await petResponse.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
            Assert.Equal(pet.Hunger, PetDefaults.DefaultHunger + 2);
        }
    }
}