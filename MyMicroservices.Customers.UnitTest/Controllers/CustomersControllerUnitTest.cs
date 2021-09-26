using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyMicroservices.Customers.Controllers;
using MyMicroservices.Customers.Dto;
using MyMicroservices.Customers.Infrastructure;
using MyMicroservices.Customers.Models;
using MyMicroservices.Customers.Services;
using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MyMicroservices.Customers.UnitTest.Controllers
{
    public class CustomersControllerUnitTest : DbContextUnitTest<CustomersContext>
    {
        private readonly CustomersContext _context;
        private readonly Mock<IHashService> _creditCardHasherMock;
        private readonly IMapper _mapper;
        private readonly CustomersController _sut;

        public CustomersControllerUnitTest()
        {
            _creditCardHasherMock = new Mock<IHashService>();

            _context = new CustomersContext(ContextOptions);
            _context.Database.EnsureCreated();

            _mapper = new MapperConfiguration(o =>
            {
                o.CreateMap<CustomerDto, Customer>();
                o.CreateMap<Customer, CustomerDto>();

            }).CreateMapper();


            _sut = new CustomersController(_context, _creditCardHasherMock.Object, _mapper);
        }

        [Theory, AutoData]
        public async Task Get_ShouldReturnNotFound_IfCustomerDoesNotExist(Guid id)
        {
            //arrange

            //act 
            var result = await _sut.Get(id);

            //assert
            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Theory, OmitRecursionAutoData]
        public async Task Get_ShouldReturnOkResult_IfCustomerExist(Customer customer)
        {
            //arrange
            using (var context = new CustomersContext(ContextOptions))
            {
                context.Customers.Add(customer);
                await context.SaveChangesAsync();
            }
            //act 
            var result = await _sut.Get(customer.Id);

            //assert
            var expectedResult = _mapper.Map<CustomerDto>(customer);
            result.Should().BeEquivalentTo(new OkObjectResult(expectedResult));
        }

        [Theory, AutoData]
        public async Task Post_ShouldReturnCreatedAtResult(CustomerDto customer)
        {
            //arrange

            //act 
            var result = await _sut.Post(customer);

            //assert
            result.Should()
                .BeEquivalentTo(new CreatedAtActionResult(nameof(CustomersController.Get), null, null, null),
                o => o.Excluding(o => o.RouteValues));
        }

        [Theory, AutoData]
        public async Task Put_ShouldReturnNotFound_IfCustomerDoesNotExist(Guid customerId, CustomerDto customerUpdated)
        {
            //arrange

            //act 
            var result = await _sut.Put(customerId, customerUpdated);

            //assert
            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Theory, OmitRecursionAutoData]
        public async Task Put_ShouldReturnOk_IfCustomerExist(Customer customer, CustomerDto customerUpdated)
        {
            //arrange
            using (var context = new CustomersContext(ContextOptions))
            {
                context.Customers.Add(customer);
                await context.SaveChangesAsync();
            }
            //act 
            var result = await _sut.Put(customer.Id, customerUpdated);

            //assert
            result.Should().BeEquivalentTo(new OkObjectResult(customerUpdated));
        }

        [Theory, OmitRecursionAutoData]
        public async Task Delete_ShouldReturnNoContent_IfCustomerExist(Customer customer)
        {
            //arrange
            using (var context = new CustomersContext(ContextOptions))
            {
                context.Customers.Add(customer);
                await context.SaveChangesAsync();
            }
            //act 
            var result = await _sut.Delete(customer.Id);

            //assert
            result.Should().BeEquivalentTo(new NoContentResult());
        }

        [Theory, AutoData]
        public async Task Delete_ShouldReturnNotFound_IfCustomerDoesNotExist(Guid id)
        {
            //arrange

            //act 
            var result = await _sut.Delete(id);

            //assert
            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Theory, AutoData]
        public async Task AddCustomerCreditCard_ShouldReturnNotFound_IfCustomerDoesNotExist(Guid id, CreditCardDto creditCardDto)
        {
            //arrange

            //act
            var result = await _sut.PostCustomerCreditCard(id, creditCardDto);

            //assert
            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Theory, OmitRecursionAutoData]
        public async Task AddCustomerCreditCard_ShouldReturnCreatedAtResult_IfCustomerExist(Customer customer, CreditCardDto creditCardDto, CreditCard hashedCreditCard)
        {
            //arrange
            using (var context = new CustomersContext(ContextOptions))
            {
                context.Customers.Add(customer);
                await context.SaveChangesAsync();
            }
            _creditCardHasherMock
                .Setup(o => o.Create(creditCardDto, It.IsAny<byte[]>()))
                .Returns(hashedCreditCard);

            //act 
            var result = await _sut.PostCustomerCreditCard(customer.Id, creditCardDto);

            //assert
            result.Should().BeEquivalentTo(new CreatedAtActionResult(nameof(CustomersController.ValidateCustomerCreditCard), null, null, null),
                o => o.Excluding(o => o.RouteValues));
        }

        [Theory, OmitRecursionAutoData]
        public async Task PutCustomerCreditCard_ShouldReturnOkResult_IfCreditCardExist(Customer customer, CreditCardDto creditCardDto, CreditCard hashedCreditCard)
        {
            //arrange
            using (var context = new CustomersContext(ContextOptions))
            {
                hashedCreditCard.Customer = customer;
                context.CreditCards.Add(hashedCreditCard);
                await context.SaveChangesAsync();
            }
            _creditCardHasherMock
                .Setup(o => o.Create(creditCardDto, It.IsAny<byte[]>()))
                .Returns(hashedCreditCard);

            //act 
            var result = await _sut.PutCustomerCreditCard(customer.Id, hashedCreditCard.Id, creditCardDto);

            //assert
            result.Should().BeEquivalentTo(new OkResult());
        }

        [Theory, OmitRecursionAutoData]
        public async Task PutCustomerCreditCard_ShouldReturnNotFound_IfCreditCardDoesNotExist(Customer customer, CreditCardDto creditCardDto, CreditCard hashedCreditCard)
        {
            //arrange
            using (var context = new CustomersContext(ContextOptions))
            {
                context.Customers.Add(customer);
                await context.SaveChangesAsync();
            }

            //act 
            var result = await _sut.PutCustomerCreditCard(customer.Id, hashedCreditCard.Id, creditCardDto);

            //assert
            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Theory, OmitRecursionAutoData]
        public async Task DeleteCustomerCreditCard_ShouldReturnNoContent_IfCreditCardExist(Customer customer, CreditCard hashedCreditCard)
        {
            //arrange
            using (var context = new CustomersContext(ContextOptions))
            {
                hashedCreditCard.Customer = customer;
                context.CreditCards.Add(hashedCreditCard);
                await context.SaveChangesAsync();
            }

            //act 
            var result = await _sut.DeleteCustomerCreditCard(customer.Id, hashedCreditCard.Id);

            //assert
            result.Should().BeEquivalentTo(new NoContentResult());
        }

        [Theory, AutoData]
        public async Task DeleteCustomerCreditCard_ShouldReturnNotFound_IfCreditCardDoesNotExist(Guid customerId, Guid creditCardId)
        {
            //arrange

            //act 
            var result = await _sut.DeleteCustomerCreditCard(customerId, creditCardId);

            //assert
            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Theory, AutoData]
        public async Task ValidateCustomerCreditCard_ShouldReturnNotFound_IfCreditCardDoesNotExist(
            Guid customerId,
            Guid creditCardId,
            CreditCardDto creditCard)
        {
            //arrange

            //act 
            var result = await _sut.ValidateCustomerCreditCard(customerId, creditCardId, creditCard);

            //assert
            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Theory, OmitRecursionAutoData]
        public async Task ValidateCustomerCreditCard_ShouldReturnOk_IfCreditCardIsValid(
            Customer customer,
            CreditCardDto creditCard,
            CreditCard hashedCreditCard)
        {
            //arrange
            hashedCreditCard.Salt = Convert.ToBase64String(Encoding.UTF8.GetBytes(hashedCreditCard.Salt));
            using (var context = new CustomersContext(ContextOptions))
            {
                hashedCreditCard.Customer = customer;
                context.CreditCards.Add(hashedCreditCard);
                await context.SaveChangesAsync();
            }
            _creditCardHasherMock
                .Setup(o => o.Create(creditCard, It.IsAny<byte[]>()))
                .Returns(hashedCreditCard);

            //act 
            var result = await _sut.ValidateCustomerCreditCard(customer.Id, hashedCreditCard.Id, creditCard);

            //assert
            result.Should().BeEquivalentTo(new OkResult());
        }


        public override void Dispose()
        {
            _context.Dispose();
            base.Dispose();
        }
    }
}
