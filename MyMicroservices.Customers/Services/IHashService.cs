using MyMicroservices.Customers.Dto;
using MyMicroservices.Customers.Models;

namespace MyMicroservices.Customers.Services
{
    public interface IHashService
    {
        CreditCard Create(CreditCardDto input, byte[] salt);
        byte[] CreateSalt();
    }
}
