using ComplexCRUDApplication.Dtos;
using ComplexCRUDApplication.Models;

namespace ComplexCRUDApplication.Services
{
    public interface ICustomerService
    {
        public Task<List<TblCustomer>> GetAll();
        public Task<TblCustomer> GetCustomerByCode(string code);
        public Task<int> DeleteCustomer(TblCustomer tblCustomer);
        public Task<int> AddCustomer(TblCustomer tblCustomer);
        public Task<bool> IsExistsCustomer(string code);
        public Task<int> UpdateCustomer(TblCustomer tblCustomer);

    }
}
