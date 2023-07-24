using AutoMapper;
using LearnAPI_Net7.ContaxtFiles;
using LearnAPI_Net7.Helpers;
using LearnAPI_Net7.Models;
using LearnAPI_Net7.Models.ViewModels;
using LearnAPI_Net7.Services;
using Microsoft.EntityFrameworkCore;

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

        public async Task<APIResponse> CreateCustomer(CreateCustomreVM model)
        {
            APIResponse response = new APIResponse();
            try
            {
                var customer = this._mapper.Map<Customer>(model);
                await this._context.Customers.AddAsync(customer);
                await this._context.SaveChangesAsync();
                response.Result = "Customer added";
                response.ResponseCode = 200;

            }
            catch (Exception e)
            {
                response.ResponseCode=500;
                response.ErrorMessage = e.Message;
            }

            return response;
        }

        public async Task<List<CustomerVM>> GetAll()
        {
            var response=new List<CustomerVM>();
            var data = await _context.Customers.ToListAsync();


            //if(data!=null) return this._mapper.Map<List<CustomerVM>>(data);
            if(data is not null)
            {
                response = this._mapper.Map<List<Customer>,List<CustomerVM>>(data);

            }

            return response;
        }


        public async Task<CustomerVM> GetById(int id)
        {
            var response = new CustomerVM();
            var data =await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
            if (data is not null)
                return this._mapper.Map<Customer, CustomerVM>(data);
            return response;
        }

        public async Task<APIResponse> Remove(int id)
        {
            APIResponse response = new APIResponse();
            try
            {
                 var customer = await this._context.Customers.FindAsync(int.Parse(id.ToString()));
                if(customer is not null)
                {
                    this._context.Remove(customer);
                    await this._context.SaveChangesAsync();
                    response.ResponseCode = 200;
                    response.Result = "Customer deleted";
                }

            }
            catch (Exception e)
            {
                response.ResponseCode = 500;
                response.ErrorMessage = e.Message;
            }

            return response;
        }

        public async Task<APIResponse> UpdateCustomer(CustomerVM model, int id)
        {
            APIResponse response = new APIResponse();
            try
            {
                var customer = await
                     this._context.Customers.FirstOrDefaultAsync(s => s.Id == id);
                if(customer is not null)
                {
                    customer.Name= model.Name; customer.Email= model.Email;
                     this._context.Update(customer);
                    await this._context.SaveChangesAsync();
                    response.Result = "Updated customer";
                    response.ResponseCode = 200;
                }
            }
            catch (Exception e)
            {
                response.ResponseCode = 500;
                response.ErrorMessage = e.Message;
            }

            return response;
        }
    }
}
