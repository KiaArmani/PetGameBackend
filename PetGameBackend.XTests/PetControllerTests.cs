using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using PetGameBackend.Models.Data;
using PetGameBackend.Models.Requests.Pet;
using PetGameBackend.Statics;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace PetGameBackend.XTests
{
    public class PetControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        public PetControllerTests(WebApplicationFactory<Startup> fixture)
        {
            Setup.SetupEnvironment();
            Client = fixture.CreateClient();
        }

        public HttpClient Client { get; }

        [Fact]
        public async Task ShouldCreatePet()
        {
            // Arrange
            var userIdentifier = await Shared.CreateUser(Client);

            var payload = new PetControllerRootPost
            {
                UserIdentifier = userIdentifier,
                AnimalType = AnimalTypeEnum.AnimalType.FallGuy
            };

            // Act
            var createResponse = await Shared.CreatePet(Client, payload);
            var createResponseBody = await createResponse.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
            Assert.True(Guid.TryParse(createResponseBody, out _));
        }

        [Fact]
        public async Task ShouldGetPet()
        {
            // Arrange
            var userIdentifier = await Shared.CreateUser(Client);
            var petIdentifier = await Shared.CreatePet(Client, userIdentifier);

            var payload = new PetControllerRootGet
            {
                PetIdentifier = petIdentifier
            };

            // Act

            // Get Pet
            var getPetResponse = await Shared.GetPet(Client, payload);

            // Assert
            var pet = JsonConvert.DeserializeObject<Pet>(await getPetResponse.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, getPetResponse.StatusCode);
            Assert.Equal(pet.PetIdentifier, payload.PetIdentifier);
            Assert.Equal(pet.Happiness, PetDefaults.DefaultHappiness);
            Assert.Equal(pet.LastHappiness, PetDefaults.DefaultHappiness);
            Assert.Equal(pet.Hunger, PetDefaults.DefaultHunger);
            Assert.Equal(pet.LastHunger, PetDefaults.DefaultHunger);
        }

        [Fact]
        public async Task ShouldDeletePet()
        {
            // Arrange
            var userIdentifier = await Shared.CreateUser(Client);
            var petIdentifier = await Shared.CreatePet(Client, userIdentifier);

            var payload = new PetControllerRootDelete
            {
                PetIdentifier = petIdentifier
            };

            // Prepare DELETE Request
            var deletePetRequest = new HttpRequestMessage(HttpMethod.Delete, "api/Pet")
            {
                Content = Shared.FormatContent<PetControllerRootDelete>(payload)
            };

            // Act

            // Delete Pet
            var deletePetResponse = await Client.SendAsync(deletePetRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, deletePetResponse.StatusCode);
        }
    }
}