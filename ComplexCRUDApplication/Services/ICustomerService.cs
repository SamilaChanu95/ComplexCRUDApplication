using ComplexCRUDApplication.Models;

namespace ComplexCRUDApplication.Services
{
    public interface ICustomerService
    {
        public Task<List<TblCustomer>> GetAll();
    }
}
