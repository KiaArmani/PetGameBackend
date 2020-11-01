using PetGameBackend.Models.Data;
using Newtonsoft.Json;

namespace PetGameBackend.Models.Requests.Pet
{
    public class PetControllerRootPost
    {
        /// <summary>
        ///     Guid of the user that should be assigned the new pet
        /// </summary>
        [JsonProperty(nameof(UserIdentifier))]
        public string UserIdentifier { get; set; }

        /// <summary>
        ///     Type of Animal the new pet should be
        /// </summary>
        [JsonProperty(nameof(AnimalType))]
        public AnimalTypeEnum.AnimalType AnimalType { get; set; }
    }
}