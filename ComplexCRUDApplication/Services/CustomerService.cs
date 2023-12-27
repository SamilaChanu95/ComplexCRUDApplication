using ComplexCRUDApplication.Dtos;
using ComplexCRUDApplication.Models;
using ComplexCRUDApplication.Repos;
using Microsoft.EntityFrameworkCore;

namespace ComplexCRUDApplication.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly DataContext _dataContext;
        public CustomerService(DataContext dataContext) 
        {
            _dataContext = dataContext;
        }

        public async Task<int> AddCustomer(TblCustomer tblCustomer)
        {
            await _dataContext.TblCustomers.AddAsync(tblCustomer);
            return await _dataContext.SaveChangesAsync();            
        }

        public async Task<int> DeleteCustomer(TblCustomer tblCustomer)
        {
            _dataContext.TblCustomers.Remove(tblCustomer);
            return await _dataContext.SaveChangesAsync();
        }

        public async Task<List<TblCustomer>> GetAll()
        {
            List<TblCustomer> CustomersList = await _dataContext.TblCustomers.ToListAsync();
            return CustomersList;
        }

        public async Task<TblCustomer> GetCustomerByCode(string code)
        {
            TblCustomer Customer = await _dataContext.TblCustomers.Where(c => c.Code == code).FirstOrDefaultAsync() ?? new TblCustomer { Code = "0" };
            return Customer;
        }

        public async Task<bool> IsExistsCustomer(string code)
        {
            TblCustomer customer = await _dataContext.TblCustomers.Where(p => p.Code == code).FirstOrDefaultAsync() ?? new TblCustomer { Code = "0" };
            return (customer.Code == code && customer != null) ? true : false;
        }

        public async Task<int> UpdateCustomer(TblCustomer tblCustomer)
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
    }
}
