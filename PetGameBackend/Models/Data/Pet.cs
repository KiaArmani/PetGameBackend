using System;
using System.Linq;
using PetGameBackend.Attributes;
using PetGameBackend.Calculators;
using PetGameBackend.Services;
using PetGameBackend.Statics;
using MongoDB.Bson.Serialization.Attributes;

namespace PetGameBackend.Models.Data
{
    public class Pet
    {
        /// <summary>
        /// </summary>
        public AnimalTypeEnum.AnimalType AnimalType { get; set; }

        /// <summary>
        ///     <see cref="Guid" /> of the Pet as string
        /// </summary>
        public string PetIdentifier { get; set; }

        /// <summary>
        ///     Associated Value at the given <see cref="LastHappinessUpdate" />
        /// </summary>
        public int LastHappiness { get; set; }

        /// <summary>
        ///     Associated DateTime value for <see cref="LastHappiness" />
        /// </summary>
        public DateTime LastHappinessUpdate { get; set; }

        /// <summary>
        ///     Associated Value at the given <see cref="LastHungerUpdate" />
        /// </summary>
        public int LastHunger { get; set; }

        /// <summary>
        ///     Associated DateTime value for <see cref="LastHunger" />
        /// </summary>
        public DateTime LastHungerUpdate { get; set; }

        /// <summary>
        ///     Returns the current Happiness under consideration of <see cref="LastHappiness" /> and
        ///     <see cref="LastHappinessUpdate" />
        ///     <para>
        ///         Has Attribute PropParserIgnore - Will not be considered at evaluation of properties in
        ///         <see cref="StorageService.UpdatePet" />
        ///     </para>
        ///     <para>Has Attribute BsonIgnore - Will not be saved in database</para>
        /// </summary>
        [PropParserIgnore]
        [BsonIgnore]
        public int Happiness =>
            ValueByTickRateCalculator.GetValueByTickRateAndDateTime(LastHappiness, LastHappinessUpdate,
                AnimalList.Animals.Single(x => x.AnimalTypeEnum == AnimalType).HappinessTickRate.BaseTickRate);

        /// <summary>
        ///     Returns the current Hunger under consideration of <see cref="LastHunger" /> and <see cref="LastHungerUpdate" />
        ///     <para>
        ///         Has Attribute PropParserIgnore - Will not be considered at evaluation of properties in
        ///         <see cref="StorageService.UpdatePet" />
        ///     </para>
        ///     <para>Has Attribute BsonIgnore - Will not be saved in database</para>
        /// </summary>
        [PropParserIgnore]
        [BsonIgnore]
        public int Hunger => ValueByTickRateCalculator.GetValueByTickRateAndDateTime(LastHunger, LastHungerUpdate,
            AnimalList.Animals.Single(x => x.AnimalTypeEnum == AnimalType).HungerTickRate.BaseTickRate);
    }
}