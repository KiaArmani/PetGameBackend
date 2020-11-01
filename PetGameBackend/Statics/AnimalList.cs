using System.Collections.Generic;
using PetGameBackend.Models.Data;

namespace PetGameBackend.Statics
{
    public static class AnimalList
    {
        public static List<Animal> Animals = new List<Animal>
        {
            new Animal
            {
                AnimalTypeEnum = AnimalTypeEnum.AnimalType.Dog,
                HappinessTickRate = new Tickable {BaseTickRate = 10000},
                HungerTickRate = new Tickable {BaseTickRate = 10000}
            },
            new Animal
            {
                AnimalTypeEnum = AnimalTypeEnum.AnimalType.Cat,
                HappinessTickRate = new Tickable {BaseTickRate = 10000},
                HungerTickRate = new Tickable {BaseTickRate = 10000}
            },
            new Animal
            {
                AnimalTypeEnum = AnimalTypeEnum.AnimalType.FallGuy,
                HappinessTickRate = new Tickable {BaseTickRate = short.MaxValue},
                HungerTickRate = new Tickable {BaseTickRate = 10000}
            }
        };
    }
}