using AutoMapper;
using LearnAPI_Net7.ContaxtFiles;
using LearnAPI_Net7.Models;
using LearnAPI_Net7.Models.ViewModels;
using LearnAPI_Net7.Services;

namespace LearnAPI_Net7.Container
{
    public class CustomerService : ICustomerService
    {
        private readonly LearnDataContaxt _context;
        private readonly IMapper _mapper;


        public CustomerService(LearnDataContaxt context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public List<CustomerVM> GetAll()
        {
            var response=new List<CustomerVM>();
            var data = _context.Customers.ToList();


            //if(data!=null) return this._mapper.Map<List<CustomerVM>>(data);
            if(data is not null)
            {
                response = this._mapper.Map<List<Customer>,List<CustomerVM>>(data);

            }

            return response;
        }
    }
}
