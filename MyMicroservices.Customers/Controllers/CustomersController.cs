using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMicroservices.Customers.Dto;
using MyMicroservices.Customers.Infrastructure;
using MyMicroservices.Customers.Models;
using MyMicroservices.Customers.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMicroservices.Customers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomersContext _context;
        private readonly ICreditCardHasher _creditCardHasher;
        private readonly IMapper _mapper;

        public CustomersController(CustomersContext context, ICreditCardHasher creditCardHasher, IMapper mapper)
        {
            _context = context;
            _creditCardHasher = creditCardHasher;
            _mapper = mapper;
        }

        //List all Customers
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var customers = _mapper
                .Map<IEnumerable<CustomerDto>>(await _context.Customers.ToListAsync());

            return Ok(customers);
        }

        //Get a specific Customer
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CustomerDto>(customer));
        }

        //Add a new Customer
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CustomerDto customerDto)
        {
            var customer = _mapper.Map<Customer>(customerDto);
            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = customer.Id }, customerDto);
        }

        //Edit an existing Customer
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, CustomerDto customerDto)
        {
            var customerToUpdate = await _context.Customers.FindAsync(id);
            if (customerToUpdate == null)
            {
                return NotFound();
            }

            customerToUpdate.Name = customerDto.Name;
            customerToUpdate.Address = customerDto.Address;

            await _context.SaveChangesAsync();

            return Ok(customerDto);
        }

        //Delete an existing Customer
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //Add a new Credit Card to an existing Customer
        [HttpPost("{id}/CreditCards")]
        public async Task<IActionResult> AddCustomerCreditCard([FromRoute] Guid id, [FromBody] CreditCardDto creditCardDto)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var creditCard = _creditCardHasher.Create(creditCardDto, _creditCardHasher.CreateSalt());
            _context.CreditCards.Add(creditCard);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(AddCustomerCreditCard), new { id = creditCard.Id }, null);
        }

        //Edit an existing Credit Card
        [HttpPut("{id}/CreditCards/{creditCardId}")]
        public async Task<IActionResult> EditCustomerCreditCard([FromRoute] Guid id, [FromRoute] Guid creditCardId, [FromBody] CreditCardDto creditCardDto)
        {
            var creditCard = await _context.CreditCards.FindAsync(creditCardId);
            if (creditCard == null || creditCard.CustomerId != id)
            {
                return NotFound();
            }

            var newSalt = _creditCardHasher.CreateSalt();
            var creditCardUpdated = _creditCardHasher.Create(creditCardDto, newSalt);
            creditCard.CardNumberHash = creditCardUpdated.CardNumberHash;
            creditCard.ExpiryDateHash = creditCardUpdated.ExpiryDateHash;
            creditCard.CVVHash = creditCardUpdated.CVVHash;
            creditCard.Customer.Salt = Convert.ToBase64String(newSalt);

            await _context.SaveChangesAsync();

            return Ok();
        }

        //Delete an existing Credit Card
        [HttpDelete("{id}/CreditCards/{creditCardId}")]
        public async Task<IActionResult> DeleteCustomerCreditCard([FromRoute] Guid id, [FromRoute] Guid creditCardId, [FromBody] CreditCardDto creditCardDto)
        {
            var creditCard = await _context.CreditCards.FindAsync(creditCardId);
            if (creditCard == null || creditCard.CustomerId != id)
            {
                return NotFound();
            }

            _context.CreditCards.Remove(creditCard);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //Validate a Credit Card: This method will accept credit card information and validate that
        //the customer indeed “owns” that credit card.
        //not a REST endpoint
        [HttpPost("{id}/CreditCards/Validate")]
        public async Task<IActionResult> CheckCustomerCreditCard([FromRoute] Guid id, [FromBody] CreditCardDto creditCardDto)
        {
            var customer = await _context.Customers
                .Include(o => o.CreditCards)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            var creditCardToValidate = _creditCardHasher.Create(creditCardDto, Convert.FromBase64String(customer.Salt));

            if (customer.CreditCards.Any(o =>
             o.CardNumberHash == creditCardToValidate.CardNumberHash &&
             o.ExpiryDateHash == creditCardToValidate.ExpiryDateHash &&
             o.CVVHash == creditCardToValidate.CVVHash))
            {
                return Ok();
            }

            return NotFound();
        }
    }
}
