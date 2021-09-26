using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMicroservices.Customers.Dto;
using MyMicroservices.Customers.Infrastructure;
using MyMicroservices.Customers.Models;
using MyMicroservices.Customers.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMicroservices.Customers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomersContext _context;
        private readonly IHashService _hashService;
        private readonly IMapper _mapper;

        public CustomersController(CustomersContext context, IHashService creditCardHasher, IMapper mapper)
        {
            _context = context;
            _hashService = creditCardHasher;
            _mapper = mapper;
        }

        //List all Customers
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var customers = _mapper
                .Map<IEnumerable<CustomerDto>>(await _context.Customers.Include(o => o.CreditCards).ToListAsync());

            return Ok(customers);
        }

        //Get a specific Customer
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var customer = await _context.Customers
                .Include(o => o.CreditCards)
                .FirstOrDefaultAsync(o => o.Id == id);
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

            return CreatedAtAction(nameof(Get), new { id = customer.Id }, null);
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
        public async Task<IActionResult> PostCustomerCreditCard([FromRoute] Guid id, [FromBody] CreditCardDto creditCardDto)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var creditCard = _hashService.Create(creditCardDto, _hashService.CreateSalt());
            creditCard.CustomerId = customer.Id;
            _context.CreditCards.Add(creditCard);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ValidateCustomerCreditCard), new { id = customer.Id, creditCardId = creditCard.Id }, null);
        }

        //Edit an existing Credit Card
        [HttpPut("{id}/CreditCards/{creditCardId}")]
        public async Task<IActionResult> PutCustomerCreditCard([FromRoute] Guid id, [FromRoute] Guid creditCardId, [FromBody] CreditCardDto creditCardDto)
        {
            var creditCard = await _context.CreditCards
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == creditCardId);

            if (creditCard == null || creditCard.CustomerId != id)
            {
                return NotFound();
            }

            //TODO: improve the data transfer
            var creditCardUpdated = _hashService.Create(creditCardDto, _hashService.CreateSalt());
            creditCard.Type = creditCardUpdated.Type;
            creditCard.CardNumberHash = creditCardUpdated.CardNumberHash;
            creditCard.ExpiryDate = creditCardUpdated.ExpiryDate;
            creditCard.CVVHash = creditCardUpdated.CVVHash;
            creditCard.Salt = creditCardUpdated.Salt;

            await _context.SaveChangesAsync();

            return Ok();
        }

        //Delete an existing Credit Card
        [HttpDelete("{id}/CreditCards/{creditCardId}")]
        public async Task<IActionResult> DeleteCustomerCreditCard([FromRoute] Guid id, [FromRoute] Guid creditCardId)
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



        /// <summary>
        /// <para>Validate a Credit Card: This method will accept credit card information and validate that
        /// the customer indeed “owns” that credit card.</para>
        /// </summary>
        /// <param name="id">Customer Id</param>
        /// <param name="creditCardDto">Credit card informations</param>
        [HttpPost("{id}/CreditCards/{creditCardId}")]
        public async Task<IActionResult> ValidateCustomerCreditCard([FromRoute] Guid id, [FromRoute] Guid creditCardId, [FromBody] CreditCardDto creditCardDto)
        {
            var creditCard = await _context.CreditCards.FindAsync(creditCardId);

            if (creditCard == null || creditCard.CustomerId != id)
            {
                return NotFound();
            }

            var creditCardToValidate = _hashService.Create(creditCardDto, Convert.FromBase64String(creditCard.Salt));

            if (creditCard.CardNumberHash == creditCardToValidate.CardNumberHash &&
             creditCard.ExpiryDate == creditCardToValidate.ExpiryDate &&
             creditCard.CVVHash == creditCardToValidate.CVVHash)
            {
                return Ok();
            }

            return NotFound();
        }
    }
}
