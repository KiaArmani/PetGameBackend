using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PetGameBackend.Attributes;
using PetGameBackend.Models.Data;
using PetGameBackend.Statics;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using static System.String;

namespace PetGameBackend.Services
{
    public class StorageService
    {
        private readonly ILogger<StorageService> _logger;

        private MongoClient _mongoClient;

        private string _mongoDatabaseName;
        private string _mongoDataCollectionName;
        private IMongoCollection<User> _userCollection;

        public StorageService(ILogger<StorageService> logger, IServiceProvider services)
        {
            _logger = logger;
        }

        private void Log(string message)
        {
            _logger.LogDebug(message);
        }

        #region Database

        public void InitializeMongoDatabase()
        {
            // Validate Configuration
            var mongoConnectionString = Environment.GetEnvironmentVariable("MT_MONGODB_CONNECTION");
            _mongoDatabaseName = Environment.GetEnvironmentVariable("MT_MONGODB_DATABASE");
            _mongoDataCollectionName = Environment.GetEnvironmentVariable("MT_MONGODB_COLLECTION");
            InitializeMongoDatabaseValidation(mongoConnectionString);

            Log("Initializing database connection..");
            _mongoClient = new MongoClient(mongoConnectionString);
            Log("Database connection established!");

#if DEBUG
            CreateStructure();
#endif

            Log("Loading collection..");
            var database = _mongoClient.GetDatabase(_mongoDatabaseName);
            _userCollection = database.GetCollection<User>(_mongoDataCollectionName);
            Log("Collection loaded!");
        }

        private void InitializeMongoDatabaseValidation(string mongoConnectionString)
        {
            if (IsNullOrEmpty(mongoConnectionString))
                throw new InvalidOperationException(
                    "StorageService (InitializeMongoDatabase) - MT_MONGODB_CONNECTION is empty. Unable to connect to database. Please make sure the environment variable MT_MONGODB_CONNECTION is set with a valid MongoDB connection string.");

            if (IsNullOrEmpty(_mongoDatabaseName))
                throw new InvalidOperationException(
                    "StorageService (InitializeMongoDatabase) - MT_MONGO_DATABASE is empty. Unable to connect to database. Please make sure the environment variable MT_MONGO_DATABASE is set with the name of the MongoDB database you try to connect to.");

            if (IsNullOrEmpty(_mongoDataCollectionName))
                throw new InvalidOperationException(
                    "StorageService (InitializeMongoDatabase) - MT_MONGO_COLLECTION is empty. Unable to load collection. Please make sure the environment variable MT_MONGO_COLLECTION is set with the name of the collection you store the data in.");
        }

        private void CreateStructure()
        {
            var databaseExists = _mongoClient.ListDatabaseNames().ToList().Contains(_mongoDatabaseName);
            if (databaseExists)
                _mongoClient.DropDatabase(_mongoDatabaseName);
            _mongoClient.GetDatabase(_mongoDatabaseName);
        }

        #endregion

        #region User

        /// <summary>
        ///     Returns the <see cref="User" /> with the given identifier
        /// </summary>
        /// <param name="userIdentifier">GUID of the <see cref="User" /> as string</param>
        /// <returns></returns>
        public virtual User GetUser(string userIdentifier)
        {
            var queryResult = _userCollection.Find(x => x.UserIdentifier.Equals(userIdentifier));
            if (queryResult.CountDocuments() > 1)
                throw new InvalidDataException(
                    "StorageService (GetUser) - Ambiguous user identifier found. Please verify data in database.");

            return queryResult.FirstOrDefault();
        }

        /// <summary>
        ///     Creates a new <see cref="User" /> with the given identifier
        /// </summary>
        /// <param name="userIdentifier">GUID of the new <see cref="User" /> as string</param>
        public virtual void CreateUser(string userIdentifier)
        {
            // Check if User already exists
            var user = GetUser(userIdentifier);
            if (user != null)
                throw new InvalidOperationException(
                    $"StorageService (CreateUser) - Unable to create user. A user with the provided user identifier already exists. ({userIdentifier})");

            // Create new User with given Guid
            _userCollection.InsertOne(new User {Pets = new List<Pet>(), UserIdentifier = userIdentifier});
        }

        /// <summary>
        ///     Updates the username of a <see cref="User" />.
        /// </summary>
        /// <param name="userIdentifier">GUID of the user as string</param>
        /// <param name="userName">New username for the <see cref="User" /></param>
        /// <returns>
        ///     <see cref="UpdateResult" />
        /// </returns>
        public virtual UpdateResult UpdateUser(string userIdentifier, string userName)
        {
            // Check if User already exists
            var user = GetUser(userIdentifier);
            if (user == null)
                throw new KeyNotFoundException(
                    $"UserService (UpdateUser) - Unable to update user. No user with the provided user identifier exists. ({userIdentifier})");

            // Update user with payload data
            var filter = Builders<User>.Filter.Eq(x => x.UserIdentifier, userIdentifier);
            var update = Builders<User>.Update.Set(x => x.UserName, userName);
            return _userCollection.UpdateOne(filter, update);
        }

        /// <summary>
        ///     Deletes the <see cref="User" /> with the provided identifier
        /// </summary>
        /// <param name="userIdentifier">GUID of the <see cref="User" /> as string</param>
        /// <returns>
        ///     <see cref="DeleteResult" />
        /// </returns>
        public virtual DeleteResult DeleteUser(string userIdentifier)
        {
            // Check if User exists
            var user = GetUser(userIdentifier);
            if (user == null)
                throw new KeyNotFoundException(
                    $"UserService (DeleteUser) - Unable to delete user. A user with the provided user identifier doesn't exist. ({userIdentifier})");

            // Delete User
            var filter = Builders<User>.Filter.Eq(x => x.UserIdentifier, userIdentifier);
            return _userCollection.DeleteOne(filter);
        }

        #endregion

        #region Pet

        /// <summary>
        ///     Returns the <see cref="Pet" /> that has the given identifier
        /// </summary>
        /// <param name="petIdentifier">GUID of the pet as string</param>
        /// <returns></returns>
        public virtual Pet GetPet(string petIdentifier)
        {
            var queryResult = _userCollection.AsQueryable().SelectMany(x => x.Pets)
                .Where(x => x.PetIdentifier.Equals(petIdentifier));
            if (queryResult.Count() > 1)
                throw new InvalidDataException(
                    "StorageService (GetPet) - Ambiguous pet identifier found. Please verify data in database.");

            return queryResult.FirstOrDefault();
        }

        /// <summary>
        ///     Creates a new pet and assigns it to the provided user
        /// </summary>
        /// <param name="userIdentifier">GUID of the user as string</param>
        /// <param name="petType">Type of animal the pet is (see <see cref="AnimalTypeEnum.AnimalType" />)</param>
        /// <returns></returns>
        public virtual string CreatePet(string userIdentifier, AnimalTypeEnum.AnimalType petType)
        {
            // Check if User already exists
            var user = GetUser(userIdentifier);
            if (user == null)
                throw new InvalidOperationException(
                    $"StorageService (CreatePet) - Unable to create pet. A user with the provided identifier doesn't exist. ({userIdentifier})");

            // Get current Date
            var currentTime = DateTime.Now.ToUniversalTime();

            // Create new Guid for Pet
            var petGuid = Guid.NewGuid().ToString();

            // Create new User with given Guid
            // Update user with payload data
            var filter = Builders<User>.Filter.Eq(x => x.UserIdentifier, userIdentifier);
            var update = Builders<User>.Update.Push(x => x.Pets, new Pet
            {
                LastHappiness = PetDefaults.DefaultHappiness,
                LastHappinessUpdate = currentTime,
                LastHunger = PetDefaults.DefaultHunger,
                LastHungerUpdate = currentTime,
                PetIdentifier = petGuid,
                AnimalType = petType
            });

            _userCollection.FindOneAndUpdate(filter, update, new FindOneAndUpdateOptions<User> {IsUpsert = true});
            return petGuid;
        }

        /// <summary>
        ///     Updates the data of a pet
        /// </summary>
        /// <param name="payloadPet">Payload of type <see cref="Pet" /></param>
        /// <returns></returns>
        public virtual UpdateResult UpdatePet(Pet payloadPet)
        {
            // Check if Pet already exists
            var pet = GetPet(payloadPet.PetIdentifier);
            if (pet == null)
                throw new KeyNotFoundException(
                    $"UserService (UpdatePet) - Unable to update pet. No pet with the provided pet identifier exists. ({payloadPet.PetIdentifier})");

            // Get Properties of payload that have a value
            // Do ignore calculated values here though as those will be ignored anyway
            var props = typeof(Pet).GetProperties()
                .Where(x => x.GetCustomAttributes().All(y => y.GetType() != typeof(PropParserIgnoreAttribute)))
                .Select(x => new {Property = x.Name, Value = x.GetValue(payloadPet)})
                .Where(x => !IsDefaultValue(x.Value))
                .ToList();

            // Loop through properties, update values in Pet
            foreach (var kvp in props) typeof(Pet).GetProperty(kvp.Property)?.SetValue(pet, kvp.Value);

            // Create filter and update pet with updated values
            var filter = Builders<User>.Filter.ElemMatch(x => x.Pets, x => x.PetIdentifier == payloadPet.PetIdentifier);
            var update = Builders<User>.Update.Set(x => x.Pets[-1], pet);
            return _userCollection.UpdateOne(filter, update);
        }

        /// <summary>
        ///     Deletes a pet
        /// </summary>
        /// <param name="petIdentifier">GUID of the pet as string</param>
        /// <returns></returns>
        public virtual UpdateResult DeletePet(string petIdentifier)
        {
            // Check if Pet already exists
            var pet = GetPet(petIdentifier);
            if (pet == null)
                throw new KeyNotFoundException(
                    $"UserService (UpdatePet) - Unable to delete pet. No pet with the provided pet identifier exists. ({petIdentifier})");

            // Update pet with payload data
            var filter = Builders<User>.Filter.ElemMatch(x => x.Pets, x => x.PetIdentifier == petIdentifier);
            var update = Builders<User>.Update.Unset(x => x.Pets[-1]);
            return _userCollection.UpdateOne(filter, update);
        }

        /// <summary>
        ///     Returns whether or not a value is the default value of its type
        /// </summary>
        /// <param name="newValue">Value to check</param>
        /// <returns></returns>
        private static bool IsDefaultValue(object newValue)
        {
            var valueType = newValue.GetType();
            dynamic defaultObject = null;
            if (valueType.IsValueType)
                defaultObject = Activator.CreateInstance(valueType);

            return object.Equals(newValue, defaultObject);
        }

        #endregion
    }
}