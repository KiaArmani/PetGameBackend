using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PetGameBackend.Models.Data
{
    public class User
    {
        /// <summary>
        ///     Required field for MongoDB
        ///     <para>Serves as unique identifier</para>
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MongoId { get; set; }

        /// <summary>
        ///     Unique Identifier of the User as <see cref="Guid" />
        /// </summary>
        public string UserIdentifier { get; set; }

        /// <summary>
        ///     Name of the User
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     List of <see cref="Pet" /> the User owns
        /// </summary>
        public List<Pet> Pets { get; set; } = new List<Pet>();
    }
}