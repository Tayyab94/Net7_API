using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace LearnAPI_Net7.Controllers
{
    [EnableCors("CorsForController")]

    [DisableCors]// if you want to disable CORS from the Controller leve the add in to the ActionMethod only

    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {

        [HttpGet("GetAllStudent")]
        public async Task<IActionResult> GetAllStudent()
        {
            return Ok();
        }



        [EnableCors("CorsForController")]
        [HttpGet("GetStudentById")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            return Ok();
        }
    }
}
