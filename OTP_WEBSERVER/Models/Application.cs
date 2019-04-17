using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using OTP_WEBSERVER.Helpers;
using OtpNet;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OTP_WEBSERVER.Models
{
    public class Application : Base
    {
        private static Random random = new Random();
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static Application GenerateApplication()
        {
            return new Application()
            {
                Name = "New Application",
                SecretKey = RandomString(16),
                SharedKey = RandomString(16),
                HashMode = OtpHashMode.Sha512,
                Size = 6,
                Step = 60 * 2
            };
        }

        [MaxLength(50)]
        [MinLength(3)]
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Secret Key is required")]
        [MaxLength(16)]
        [Display(Name = "Secret Key")]
        public string SecretKey { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Shared Key is required")]
        [MaxLength(16)]
        [Display(Name = "Shared Key")]
        public string SharedKey { get; set; }

        [Display(Name = "Hash")]
        public OtpHashMode HashMode { get; set; }

        [Required]
        [Range(6, 16)]
        [Display(Name = "Otp Length")]
        public int Size { get; set; }

        [JsonIgnore]
        public ObjectId User_Id { get; set; }

        [Required]
        [Range(10, 15 * 60)]
        [Display(Name = "Expire in Seconds")]
        public int Step { get; set; }
    }
}