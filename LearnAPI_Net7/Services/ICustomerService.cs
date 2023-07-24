using LearnAPI_Net7.Helpers;
using LearnAPI_Net7.Models;
using LearnAPI_Net7.Models.ViewModels;

namespace LearnAPI_Net7.Services
{
    public interface ICustomerService
    {
       Task<List<CustomerVM>> GetAll();

        Task<CustomerVM> GetById(int id);
        Task<APIResponse> Remove(int id);
        Task<APIResponse> CreateCustomer(CreateCustomreVM model);
        Task<APIResponse> UpdateCustomer(CustomerVM model, int id);
    }
}
