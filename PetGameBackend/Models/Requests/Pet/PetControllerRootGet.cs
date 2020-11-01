using Newtonsoft.Json;

namespace PetGameBackend.Models.Requests.Pet
{
    public class PetControllerRootGet
    {
        /// <summary>
        ///     Guid of the Pet that is being requested
        /// </summary>
        [JsonProperty(nameof(PetIdentifier))]
        public string PetIdentifier { get; set; }
    }
}