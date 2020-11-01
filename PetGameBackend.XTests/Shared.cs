using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PetGameBackend.Models.Data;
using PetGameBackend.Models.Requests.Pet;
using PetGameBackend.Models.Requests.PetAction;
using PetGameBackend.Models.Requests.User;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PetGameBackend.XTests
{
    public static class Shared
    {
        /// <summary>
        ///     Send a POST Request to the endpoint for <see cref="UserControllerRootPost" />
        /// </summary>
        /// <param name="client">
        ///     HTTP Client which sends the request (Client of <see cref="WebApplicationFactory{TEntryPoint}" />
        ///     and type <see cref="Startup" />
        /// </param>
        /// <param name="payload">Payload containing data to create a <see cref="UserControllerRootPost" /></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> CreateUser(HttpClient client, object payload)
        {
            var userIdentifier = GetPropertyFromGenericClass(payload, nameof(UserControllerRootPost.UserIdentifier))
                .ToString();
            var createUserRequest = new HttpRequestMessage(HttpMethod.Post, "api/User")
            {
                Content = FormatContent<UserControllerRootPost>(new UserControllerRootPost
                    {UserIdentifier = userIdentifier})
            };
            var createResponse = await client.SendAsync(createUserRequest);
            return createResponse;
        }

        /// <summary>
        ///     Send a GET Request to the endpoint for <see cref="UserControllerRootGet" />
        /// </summary>
        /// <param name="client">
        ///     HTTP Client which sends the request (Client of <see cref="WebApplicationFactory{TEntryPoint}" />
        ///     and type <see cref="Startup" />
        /// </param>
        /// <param name="payload">Payload containing data to create a <see cref="UserControllerRootGet" /></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> GetUser(HttpClient client, object payload)
        {
            var userIdentifier = GetPropertyFromGenericClass(payload, nameof(UserControllerRootGet.UserIdentifier))
                .ToString();
            var createUserRequest = new HttpRequestMessage(HttpMethod.Get, "api/User")
            {
                Content = FormatContent<UserControllerRootGet>(new UserControllerRootGet
                    {UserIdentifier = userIdentifier})
            };
            var createResponse = await client.SendAsync(createUserRequest);
            return createResponse;
        }

        /// <summary>
        ///     Send a GET Request to the endpoint for <see cref="UserControllerRootPost" />
        ///     <para>Creates a user identifier itself and does not need a payload.</para>
        /// </summary>
        /// <param name="client">
        ///     HTTP Client which sends the request (Client of <see cref="WebApplicationFactory{TEntryPoint}" />
        ///     and type <see cref="Startup" />
        /// </param>
        /// <returns></returns>
        public static async Task<string> CreateUser(HttpClient client)
        {
            var userIdentifier = Guid.NewGuid().ToString();
            await CreateUser(client, new UserControllerRootPost
            {
                UserIdentifier = userIdentifier
            });

            return userIdentifier;
        }

        /// <summary>
        ///     Send a GET Request to the endpoint for <see cref="PetControllerRootPost" />
        ///     <para>Creates a user identifier itself and does not need a payload.</para>
        /// </summary>
        /// <param name="client">
        ///     HTTP Client which sends the request (Client of <see cref="WebApplicationFactory{TEntryPoint}" />
        ///     and type <see cref="Startup" />
        /// </param>
        /// <param name="userIdentifier">GUID of the user as string</param>
        /// <returns></returns>
        public static async Task<string> CreatePet(HttpClient client, string userIdentifier)
        {
            var responseMessage = await CreatePet(client, new PetControllerRootPost
            {
                UserIdentifier = userIdentifier,
                AnimalType = AnimalTypeEnum.AnimalType.FallGuy
            });

            return await responseMessage.Content.ReadAsStringAsync();
        }

        /// <summary>
        ///     Send a POST Request to the endpoint for <see cref="PetControllerRootPost" />
        /// </summary>
        /// <param name="client">
        ///     HTTP Client which sends the request (Client of <see cref="WebApplicationFactory{TEntryPoint}" />
        ///     and type <see cref="Startup" />
        /// </param>
        /// <param name="payload">Payload containing data to create a <see cref="PetControllerRootPost" /></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> CreatePet(HttpClient client, object payload)
        {
            var userIdentifier = GetPropertyFromGenericClass(payload, nameof(PetControllerRootPost.UserIdentifier))
                .ToString();
            var animalType =
                (AnimalTypeEnum.AnimalType) GetPropertyFromGenericClass(payload,
                    nameof(PetControllerRootPost.AnimalType));

            var createPetRequest = new HttpRequestMessage(HttpMethod.Post, "api/Pet")
            {
                Content = FormatContent<PetControllerRootPost>(new PetControllerRootPost
                    {UserIdentifier = userIdentifier, AnimalType = animalType})
            };
            var createResponse = await client.SendAsync(createPetRequest);
            return createResponse;
        }

        /// <summary>
        ///     Send a GET Request to the endpoint for <see cref="PetControllerRootGet" />
        /// </summary>
        /// <param name="client">
        ///     HTTP Client which sends the request (Client of <see cref="WebApplicationFactory{TEntryPoint}" />
        ///     and type <see cref="Startup" />
        /// </param>
        /// <param name="payload">Payload containing data to create a <see cref="PetControllerRootGet" /></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> GetPet(HttpClient client, object payload)
        {
            var petIdentifier = GetPropertyFromGenericClass(payload, nameof(PetControllerRootGet.PetIdentifier))
                .ToString();
            var createPetRequest = new HttpRequestMessage(HttpMethod.Get, "api/Pet")
            {
                Content = FormatContent<PetControllerRootGet>(new PetControllerRootGet {PetIdentifier = petIdentifier})
            };
            var createResponse = await client.SendAsync(createPetRequest);
            return createResponse;
        }

        /// <summary>
        ///     Send a PATCH Request to the endpoint for <see cref="PetActionControllerStrokePatch" />
        /// </summary>
        /// <param name="client">
        ///     HTTP Client which sends the request (Client of <see cref="WebApplicationFactory{TEntryPoint}" />
        ///     and type <see cref="Startup" />
        /// </param>
        /// <param name="payload">Payload containing data to create a <see cref="PetActionControllerStrokePatch" /></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> StrokePet(HttpClient client, object payload)
        {
            var petIdentifier =
                GetPropertyFromGenericClass(payload, nameof(PetActionControllerStrokePatch.PetIdentifier)).ToString();
            var happinessImprovement = Convert.ToInt32(GetPropertyFromGenericClass(payload,
                nameof(PetActionControllerStrokePatch.HappinessImprovement)));

            var strokePetRequest = new HttpRequestMessage(HttpMethod.Patch, "api/PetAction/stroke")
            {
                Content = FormatContent<PetActionControllerStrokePatch>(new PetActionControllerStrokePatch
                    {PetIdentifier = petIdentifier, HappinessImprovement = happinessImprovement})
            };
            var strokeResponse = await client.SendAsync(strokePetRequest);
            return strokeResponse;
        }

        /// <summary>
        ///     Send a PATCH Request to the endpoint for <see cref="PetActionControllerFeedPatch" />
        /// </summary>
        /// <param name="client">
        ///     HTTP Client which sends the request (Client of <see cref="WebApplicationFactory{TEntryPoint}" />
        ///     and type <see cref="Startup" />
        /// </param>
        /// <param name="payload">Payload containing data to create a <see cref="PetActionControllerFeedPatch" /></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> FeedPet(HttpClient client, object payload)
        {
            var petIdentifier = GetPropertyFromGenericClass(payload, nameof(PetActionControllerFeedPatch.PetIdentifier))
                .ToString();
            var happinessImprovement =
                Convert.ToInt32(GetPropertyFromGenericClass(payload,
                    nameof(PetActionControllerFeedPatch.HungerImprovement)));

            var strokePetRequest = new HttpRequestMessage(HttpMethod.Patch, "api/PetAction/feed")
            {
                Content = FormatContent<PetActionControllerFeedPatch>(new PetActionControllerFeedPatch
                    {PetIdentifier = petIdentifier, HungerImprovement = happinessImprovement})
            };
            var strokeResponse = await client.SendAsync(strokePetRequest);
            return strokeResponse;
        }

        /// <summary>
        ///     Serializes the given payload object as Type T and returns it as <see cref="StringContent" />
        /// </summary>
        /// <typeparam name="T">Type to cast the payload to</typeparam>
        /// <param name="payload">Payload object</param>
        /// <returns></returns>
        public static StringContent FormatContent<T>(object payload)
        {
            return new StringContent(JsonSerializer.Serialize((T) payload),
                Encoding.UTF8, "application/json");
        }

        /// <summary>
        ///     Returns a requested property value of a generic object
        /// </summary>
        /// <param name="payload">Payload object</param>
        /// <param name="propertyName">Name of property which' value should be returned</param>
        /// <returns></returns>
        public static object GetPropertyFromGenericClass(object payload, string propertyName)
        {
            var t = payload.GetType();
            return t.GetProperty(propertyName)?.GetValue(payload);
        }
    }
}