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
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
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
            var data = await _customerService.GetById(id);
            if (data is null)
                return NotFound();

            return Ok(data);
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
