using ComplexCRUDApplication.Models;
using ComplexCRUDApplication.Repos;

namespace ComplexCRUDApplication.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly DataContext _dataContext;
        public CustomerService(DataContext dataContext) 
        {
            _dataContext = dataContext;
        } 
        public List<TblCustomer> GetAll()
        {
            List<TblCustomer> CustomersList;
            CustomersList = _dataContext.TblCustomers.ToList();
            return CustomersList;
        }
    }
}
