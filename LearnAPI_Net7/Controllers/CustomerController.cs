using LearnAPI_Net7.Models;
using LearnAPI_Net7.Models.ViewModels;
using LearnAPI_Net7.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnAPI_Net7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;


        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;   

        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data=await _customerService.GetAll();
            if (data is null) return NotFound();
            return Ok(data);
        }


        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {

            try
            {
                _logger
                .LogInformation("---- Get Customer by Id ------");
                var data = await _customerService.GetById(id);
                if (data==null) 
                    return NotFound();

                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }
            return BadRequest();
            
        }

        [HttpPost(Name ="Create")]
        public async 
            Task<IActionResult>Create(CreateCustomreVM model)
        {
            if(ModelState.IsValid is not true)
                return BadRequest(ModelState);

            var data = await this._customerService.CreateCustomer(model);
            return Ok(data);
        }

        [HttpPut(Name = "Update")]
        public async Task<IActionResult> Update (CustomerVM model, int id )
        {
             var data= await this._customerService.UpdateCustomer(model, id);
            return Ok(data);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult>Delete(int id)
        {
            var data = await this._customerService.Remove(id);
            return Ok(data);
        }
    }
}
