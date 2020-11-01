using System;
using System.Data;
using PetGameBackend.Models.Data;
using PetGameBackend.Models.Requests.Pet;
using PetGameBackend.Models.Requests.PetAction;
using PetGameBackend.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PetGameBackend.Services
{
    public class PetService
    {
        private readonly StorageService _storageService;

        public PetService(ILogger<PetService> logger, IServiceProvider services)
        {
            _storageService = services.GetRequiredService<StorageService>();
        }

        public Pet GetPet(PetControllerRootGet payload)
        {
            // Validate Payload
            DataValidator.ValidateGuid(payload.PetIdentifier);

            // Get User
            return _storageService.GetPet(payload.PetIdentifier);
        }

        public string CreatePet(PetControllerRootPost payload)
        {
            // Validate Payload
            DataValidator.ValidateGuid(payload.UserIdentifier);
            DataValidator.ValidateEnumField(typeof(AnimalTypeEnum.AnimalType), payload.AnimalType);

            // Create User and return new Pet Id
            return _storageService.CreatePet(payload.UserIdentifier, payload.AnimalType);
        }

        public void UpdatePet(Pet payload)
        {
            // Validate Payload
            DataValidator.ValidateGuid(payload.PetIdentifier);
            DataValidator.ValidateField(nameof(payload.LastHunger), payload.LastHunger);
            DataValidator.ValidateField(nameof(payload.LastHappiness), payload.LastHappiness);

            // Update User
            var updateResult = _storageService.UpdatePet(payload);

            // Check Update Result
            if (updateResult.ModifiedCount == 0)
                throw new NoNullAllowedException("UserService (UpdateUser) - No updates were performed.");
            if (updateResult.ModifiedCount > 1)
                throw new InvalidOperationException("UserService (UpdateUser) - More than one user was updated.");
        }

        public void DeletePet(PetControllerRootDelete payload)
        {
            // Validate Payload
            DataValidator.ValidateGuid(payload.PetIdentifier);

            // Delete User
            var deletionResult = _storageService.DeletePet(payload.PetIdentifier);

            // Check Deletion Result
            if (deletionResult.ModifiedCount == 0)
                throw new NoNullAllowedException("UserService (DeleteUser) - No deletions were performed.");
            if (deletionResult.ModifiedCount > 1)
                throw new InvalidOperationException("UserService (DeleteUser) - More than one user was deleted.");
        }

        public void StrokePet(PetActionControllerStrokePatch payload)
        {
            // Validate Payload
            DataValidator.ValidateGuid(payload.PetIdentifier);

            // Get Pet Information
            var pet = GetPet(new PetControllerRootGet
            {
                PetIdentifier = payload.PetIdentifier
            });

            if (pet == null)
                throw new NoNullAllowedException("PetService (StrokePet) - The requested pet does not exist.");

            // Update Pet
            var now = DateTime.Now;
            UpdatePet(new Pet
                {
                    PetIdentifier = payload.PetIdentifier,
                    LastHappiness = GetUpdatedMetric(pet.Happiness, payload.HappinessImprovement),
                    LastHappinessUpdate = now
                }
            );
        }

        public void FeedPet(PetActionControllerFeedPatch payload)
        {
            // Validate Payload
            DataValidator.ValidateGuid(payload.PetIdentifier);

            // Get Pet Information
            var pet = GetPet(new PetControllerRootGet
            {
                PetIdentifier = payload.PetIdentifier
            });

            if (pet == null)
                throw new NoNullAllowedException("PetService (StrokePet) - The requested pet does not exist.");

            // Update Pet
            var now = DateTime.Now;
            UpdatePet(new Pet
                {
                    PetIdentifier = payload.PetIdentifier,
                    LastHunger = GetUpdatedMetric(pet.Hunger, payload.HungerImprovement),
                    LastHungerUpdate = now
                }
            );
        }

        private static int GetUpdatedMetric(int metricValue, int improvementAmount, bool decrease = false)
        {
            return decrease ? metricValue - improvementAmount : metricValue + improvementAmount;
        }
    }
}