using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace OTP_WEBSERVER.Models
{
    public abstract class Base
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public DateTime UpdatedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
