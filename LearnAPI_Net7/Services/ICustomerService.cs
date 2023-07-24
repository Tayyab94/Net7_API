using LearnAPI_Net7.Models;
using LearnAPI_Net7.Models.ViewModels;

namespace LearnAPI_Net7.Services
{
    public interface ICustomerService
    {
        List<CustomerVM> GetAll();
    }
}
