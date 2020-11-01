namespace PetGameBackend.Models.Data
{
    public class Animal
    {
        /// <summary>
        ///     In what rate happiness changes
        /// </summary>
        public Tickable HappinessTickRate { get; set; }

        /// <summary>
        ///     In what rate hunger changes
        /// </summary>
        public Tickable HungerTickRate { get; set; }

        /// <summary>
        ///     What kind of creature / animal, this animal is
        /// </summary>
        public AnimalTypeEnum.AnimalType AnimalTypeEnum { get; set; }
    }
}