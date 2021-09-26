using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MyMicroservices.Customers.Dto;
using MyMicroservices.Customers.Models;
using System;
using System.Security.Cryptography;

namespace MyMicroservices.Customers.Services
{
    //based on https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-5.0
    public class HashService : IHashService
    {
        public CreditCard Create(CreditCardDto input, byte[] salt)
        {
            return new CreditCard()
            {
                Type = input.Type,
                CardNumberHash = HashValue(input.CardNumber, salt),
                ExpiryDate = input.ExpiryDate,
                CVVHash = HashValue(input.CVV, salt),
                Salt = Convert.ToBase64String(salt)
            };
        }

        public byte[] CreateSalt()
        {
            byte[] salt = new byte[16];
            using var rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetNonZeroBytes(salt);

            return salt;
        }

        private string HashValue(string value, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: value,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32));
        }
    }
}
