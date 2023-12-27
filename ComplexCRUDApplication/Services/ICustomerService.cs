using ComplexCRUDApplication.Models;

namespace ComplexCRUDApplication.Services
{
    public interface ICustomerService
    {
        public List<TblCustomer> GetAll();
    }
}
