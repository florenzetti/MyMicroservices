using MyMicroservices.Customers.Dto;
using MyMicroservices.Customers.Models;

namespace MyMicroservices.Customers.Services
{
    public interface ICreditCardHasher
    {
        CreditCard Create(CreditCardDto input, byte[] salt);
        byte[] CreateSalt();
    }
}
