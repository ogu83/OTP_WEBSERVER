﻿using MongoDB.Bson;
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
                HashMode = OtpHashMode.Sha512,
                Size = 6
            };
        }

        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "SecretKey is neccesary"]
        [MaxLength(16)]
        public string SecretKey { get; set; }

        public OtpHashMode HashMode { get; set; }

        [Required]
        public int Size { get; set; }

        public ObjectId User_Id { get; set; }
    }
}