using ComplexCRUDApplication.Dtos;
using ComplexCRUDApplication.Models;
using ComplexCRUDApplication.Repos;
using Microsoft.EntityFrameworkCore;

namespace ComplexCRUDApplication.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<CustomerService> _logger;
        public CustomerService(DataContext dataContext, ILogger<CustomerService> logger) 
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public async Task<int> AddCustomer(TblCustomer tblCustomer)
        {
            try 
            {
                await _dataContext.TblCustomers.AddAsync(tblCustomer);
                return await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                _logger.LogError($"DB Error in Creating Customer : {ex.Message}.");
                return 0;
            }
        }

        public async Task<int> DeleteCustomer(TblCustomer tblCustomer)
        {
            try 
            {
                _dataContext.TblCustomers.Remove(tblCustomer);
                return await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                _logger.LogError($"DB Error in Deleting Customer : {ex.Message}.");
                return 0;
            }
        }

        public async Task<List<TblCustomer>> GetAll()
        {
            try 
            {
                List<TblCustomer> CustomersList = await _dataContext.TblCustomers.ToListAsync();
                return CustomersList;
            }
            catch (Exception ex) 
            {
                _logger.LogError($"DB Error in Getting Customers List : {ex.Message}.");
                return new List<TblCustomer>();
            }
        }

        public async Task<TblCustomer> GetCustomerByCode(string code)
        {
            try 
            {
                TblCustomer Customer = await _dataContext.TblCustomers.Where(c => c.Code == code).FirstOrDefaultAsync() ?? new TblCustomer { Code = "0" };
                return Customer;
            }
            catch (Exception ex) 
            {
                _logger.LogError($"DB Error in Getting Customer : {ex.Message}.");
                return new TblCustomer();
            }
        }

        public async Task<bool> IsExistsCustomer(string code)
        {
            TblCustomer customer = await _dataContext.TblCustomers.Where(p => p.Code == code).FirstOrDefaultAsync() ?? new TblCustomer { Code = "0" };
            return (customer.Code == code && customer != null) ? true : false;
        }

        public async Task<int> UpdateCustomer(TblCustomer tblCustomer)
        {
            try 
            {
                TblCustomer UpdatedCustomer = await GetCustomerByCode(tblCustomer.Code);
                UpdatedCustomer.Code = tblCustomer.Code;
                UpdatedCustomer.Name = tblCustomer.Name;
                UpdatedCustomer.Phone = tblCustomer.Phone;
                UpdatedCustomer.Email = tblCustomer.Email;
                UpdatedCustomer.CreditLimit = tblCustomer.CreditLimit;
                UpdatedCustomer.TaxCode = tblCustomer.TaxCode;
                UpdatedCustomer.IsActive = tblCustomer.IsActive;
                _dataContext.TblCustomers.Update(UpdatedCustomer);
                return await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                _logger.LogError($"DB Error in Updating Customer : {ex.Message}.");
                return 0;
            }
        }
    }
}
