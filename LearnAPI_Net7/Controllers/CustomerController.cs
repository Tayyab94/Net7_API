using ClosedXML.Excel;
using LearnAPI_Net7.Models;
using LearnAPI_Net7.Models.ViewModels;
using LearnAPI_Net7.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Data;

namespace LearnAPI_Net7.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    //[EnableRateLimiting("fixedWindow")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger,
            IWebHostEnvironment webHostEnvironment)
        {
            _customerService = customerService;
            _logger = logger;   
            this._webHostEnvironment= webHostEnvironment;

        }

        //[DisableRateLimiting]
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

        [AllowAnonymous]
        [HttpGet("ExportCustomerExcelSheet")]
        public async Task<IActionResult> ExportCustomerExcelSheet(bool SaveFolder)
        {
            try
            {
                string FilePath = GetFilePath();
                string excelPath = FilePath + "\\CustomerInfo.xlsx";

                DataTable dt = new DataTable();
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Email", typeof(string));

                var data = await this._customerService.GetAll();
                if(data is not null && data.Count> 0)
                {
                    data.ForEach(item =>
                    {
                        dt.Rows.Add(item.CustomerId, item.Name, item.Email);
                    });
                }
                using(XLWorkbook wb= new XLWorkbook())
                {
                    wb.Worksheets.Add(dt, "Customer Info");
                    using(MemoryStream  ms=new MemoryStream())
                    {
                        wb.SaveAs(ms);

                        // To sve Excel sheet into the Project's Folder
                        if(SaveFolder)
                        {
                            if(System.IO.File.Exists(excelPath))
                            {
                                System.IO.File.Delete(excelPath);
                            }
                            wb.SaveAs(excelPath);
                        }
                        return File(ms.ToArray(), "application/vnd.openxlmformats-officedocument.spreadsheetml.sheet", "Customer.xlsx");
                    }
                }

            }
            catch (Exception e)
            {
                return NotFound();
            }
        }

        [NonAction]
        private string GetFilePath()
        {
            return this._webHostEnvironment.WebRootPath + "\\Upload\\Excelsheet\\";
        }
    }
}
