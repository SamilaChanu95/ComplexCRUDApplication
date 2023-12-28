using AutoMapper;
using ComplexCRUDApplication.Dtos;
using ComplexCRUDApplication.Models;
using ComplexCRUDApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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

        [HttpGet]
        [Route("get-customer/{code}")]
        public async Task<IActionResult> GetCustomerList([FromRoute] string code)
        {
            try
            {
                bool existing = await _customerService.IsExistsCustomer(code);
                TblCustomer tblCustomer = await _customerService.GetCustomerByCode(code);
                return existing ? Ok(_mapper.Map<CustomerDto>(tblCustomer)) : Ok(new ApiResponse { ResponseCode = 404, Result = "This data not found.", ErrorMessage = "Not Found." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("create-customer")]
        public async Task<IActionResult> CreateCustomer(CustomerDto customerDto)
        {
            try
            {
                TblCustomer tblCustomer = _mapper.Map<TblCustomer>(customerDto);
                bool existing = await _customerService.IsExistsCustomer(tblCustomer.Code);
                if (!existing)
                    await _customerService.AddCustomer(tblCustomer);
                else
                    return Ok(new ApiResponse { ResponseCode = 400, Result = "This code already exists.", ErrorMessage = "Bad Request." });
                bool created = await _customerService.IsExistsCustomer(tblCustomer.Code);
                return created ? Ok(new ApiResponse { ResponseCode = 200, Result = "Successfully Added.", ErrorMessage = "None." }) : Ok(new ApiResponse { ResponseCode = 400, Result = "Error in Adding.", ErrorMessage = "Bad Request." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("delete-customer/{code}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute] string code)
        {
            try
            {
                TblCustomer tblCustomer;
                bool existing = await _customerService.IsExistsCustomer(code);
                if (existing)
                   tblCustomer = await _customerService.GetCustomerByCode(code);
                else
                    return Ok(new ApiResponse { ResponseCode = 404, Result = "This data not found.", ErrorMessage = "Not Found." });
                int changes = await _customerService.DeleteCustomer(tblCustomer);
                return changes > 0 ? Ok(new ApiResponse { ResponseCode = 200, Result = "Successfully Deleted.", ErrorMessage = "None." }) : Ok(new ApiResponse { ResponseCode = 400, Result = "Error in Deleting.", ErrorMessage = "Bad Request." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("update-customer")]
        public async Task<IActionResult> UpdateCustomer(CustomerDto customerDto)
        {
            try
            {
                TblCustomer tblCustomer = _mapper.Map<TblCustomer>(customerDto);
                bool existing = await _customerService.IsExistsCustomer(tblCustomer.Code);
                int updates;
                if (existing)
                    updates = await _customerService.UpdateCustomer(tblCustomer);
                else
                    return Ok(new ApiResponse { ResponseCode = 404, Result = "This data not found.", ErrorMessage = "Not Found." });
                return updates > 0 ? Ok(new ApiResponse { ResponseCode = 200, Result = "Successfully Updated.", ErrorMessage = "None." }) : Ok(new ApiResponse { ResponseCode = 400, Result = "Error in Updating.", ErrorMessage = "Bad Request." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }
    }
}
