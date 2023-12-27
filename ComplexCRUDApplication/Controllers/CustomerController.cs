using AutoMapper;
using ComplexCRUDApplication.Dtos;
using ComplexCRUDApplication.Models;
using ComplexCRUDApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace ComplexCRUDApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        public CustomerController(ICustomerService customerService, IMapper mapper) 
        {
            this._customerService = customerService;
            this._mapper = mapper;
        }

        [HttpGet]
        [Route("customer-list")]
        public async Task<IActionResult> GetCustomerList() 
        {
            try 
            {
                var customer = await _customerService.GetAll();
                // List<TblCustomer> tblCustomers = _customerService.GetAll();
                List<CustomerDto> CustomersList;
                if (customer.Count > 0)
                {
                    CustomersList = _mapper.Map<List<CustomerDto>>(customer);
                    return Ok(CustomersList);
                }
                else {
                    return NoContent();
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }
    }
}
