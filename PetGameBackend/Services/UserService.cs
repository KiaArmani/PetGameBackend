using System;
using System.Data;
using PetGameBackend.Models.Data;
using PetGameBackend.Models.Requests.User;
using PetGameBackend.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PetGameBackend.Services
{
    public class UserService
    {
        private readonly StorageService _storageService;

        public UserService(ILogger<UserService> logger, IServiceProvider services)
        {
            _storageService = services.GetRequiredService<StorageService>();
        }

        public User GetUser(UserControllerRootGet payload)
        {
            // Validate Payload
            DataValidator.ValidateField(nameof(payload.UserIdentifier), payload.UserIdentifier);
            DataValidator.ValidateGuid(payload.UserIdentifier);

            // Get User
            return _storageService.GetUser(payload.UserIdentifier);
        }

        public void CreateUser(UserControllerRootPost payload)
        {
            // Validate Payload
            DataValidator.ValidateField(nameof(payload.UserIdentifier), payload.UserIdentifier);
            DataValidator.ValidateGuid(payload.UserIdentifier);

            // Create User
            _storageService.CreateUser(payload.UserIdentifier);
        }

        public void UpdateUser(UserControllerRootPut payload)
        {
            // Validate Payload
            DataValidator.ValidateField(nameof(payload.UserIdentifier), payload.UserIdentifier);
            DataValidator.ValidateGuid(payload.UserIdentifier);
            DataValidator.ValidateField(nameof(payload.UserIdentifier), payload.UserName);


            // Update User
            var updateResult = _storageService.UpdateUser(payload.UserIdentifier, payload.UserName);

            // Check Update Result
            if (updateResult.ModifiedCount == 0)
                throw new NoNullAllowedException("UserService (UpdateUser) - No updates were performed.");
            if (updateResult.ModifiedCount > 1)
                throw new InvalidOperationException("UserService (UpdateUser) - More than one user was updated.");
        }

        public void DeleteUser(UserControllerRootDelete payload)
        {
            // Validate Payload
            DataValidator.ValidateField(nameof(payload.UserIdentifier), payload.UserIdentifier);
            DataValidator.ValidateGuid(payload.UserIdentifier);

            // Delete User
            var deletionResult = _storageService.DeleteUser(payload.UserIdentifier);

            // Check Deletion Result
            if (deletionResult.DeletedCount == 0)
                throw new NoNullAllowedException("UserService (DeleteUser) - No deletions were performed.");
            if (deletionResult.DeletedCount > 1)
                throw new InvalidOperationException("UserService (DeleteUser) - More than one user was deleted.");
        }
    }
}