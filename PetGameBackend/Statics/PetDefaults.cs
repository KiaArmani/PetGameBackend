using PetGameBackend.Models.Data;

namespace PetGameBackend.Statics
{
    public static class PetDefaults
    {
        /// <summary>
        ///     Default value for <see cref="Pet.LastHunger" /> when a <see cref="Pet" /> is created
        /// </summary>
        public static int DefaultHunger = 10;

        /// <summary>
        ///     Default value for <see cref="Pet.LastHappiness" /> when a <see cref="Pet" /> is created
        /// </summary>
        public static int DefaultHappiness = 10;
    }
}