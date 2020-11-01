using Newtonsoft.Json;

namespace PetGameBackend.Models.Requests.Pet
{
    public class PetControllerRootDelete
    {
        /// <summary>
        ///     Guid of the Pet that gets deleted
        /// </summary>
        [JsonProperty(nameof(PetIdentifier))]
        public string PetIdentifier { get; set; }
    }
}