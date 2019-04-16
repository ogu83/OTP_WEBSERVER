using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace OTP_WEBSERVER.Models
{
    public class User : Base
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }        

        public bool IsMaster { get; set; }
    }
}