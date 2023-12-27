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
        public async Task<List<TblCustomer>> GetAll()
        {
            List<TblCustomer> CustomersList;
            CustomersList = await _dataContext.TblCustomers.ToListAsync();
            return CustomersList;
        }
    }
}
