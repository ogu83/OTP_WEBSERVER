using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using OTP_WEBSERVER.Helpers;
using System;

namespace OTP_WEBSERVER.Models
{
    public abstract class Base
    {        
        [BsonId]
        [JsonConverter(typeof(ObjectIdJsonConverter))]        
        public ObjectId Id { get; set; }

        public DateTime UpdatedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
