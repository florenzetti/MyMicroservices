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
using System.Threading.Tasks;
using Xunit;

namespace MyMicroservices.Customers.UnitTest.Controllers
{
    public class CustomersControllerUnitTest : DbContextUnitTest<CustomersContext>
    {
        private readonly CustomersContext _context;
        private readonly Mock<ICreditCardHasher> _creditCardHasherMock;
        private readonly IMapper _mapper;
        private readonly CustomersController _sut;

        public CustomersControllerUnitTest()
        {
            _creditCardHasherMock = new Mock<ICreditCardHasher>();

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
                .BeEquivalentTo(new CreatedAtActionResult(nameof(CustomersController.Get), null, null, customer),
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


        public override void Dispose()
        {
            _context.Dispose();
            base.Dispose();
        }
    }
}
