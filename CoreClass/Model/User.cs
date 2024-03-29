﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CoreClass.Model
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public string Account { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Organization { get; set; } = null!;
        public string UserWeight { get; set; } = "5"!;

        public void CreatePasswordHash(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace.", nameof(password));
            }
            if (password.Length < 8)
            {
                throw new ArgumentException("Expected at least 8 characters long.", nameof(password));
            }

            using var hmac = new HMACSHA512();
            PasswordSalt = hmac.Key;
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public bool VerifyPasswordHash(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace.", nameof(password));
            }
            if (PasswordHash.Length != 64)
            {
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).");
            }
            if (PasswordSalt.Length != 128)
            {
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).");
            }

            using (var hmac = new HMACSHA512(PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != PasswordHash[i])
                        return false;
                }
            }
            return true;
        }

        public void JudgeUserWeight(string Organization)
        {
            if (Organization == null)
            {
                throw new ArgumentException("Invalid Organization.");
            }
            if (Organization == "EAC")
            {
                UserWeight = "2";
            }
        }
        [JsonIgnore]
        public static User AutoJudgeUser
        {
            get
            {
                return new User() {Username = "AutoJudgeUser" , Account = "10086"};
            }
        }
    }
}
