using FinanceFolio.Data;
using FinanceFolio.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinanceFolio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoController:ControllerBase
    {
        private readonly AppDbcontext _dbcontext;
        public DemoController(AppDbcontext dbcontext)
        {

            _dbcontext = dbcontext;

        }
        [HttpGet("GetAll")]
        public IActionResult GetAllUsers()
        {
            return Ok(_dbcontext.Demos.FirstOrDefault()?.demo);
        }
        [HttpPost("Create")]
        public IActionResult Create()
        {
            var demo = new Demo
            {
                Id=1,
                demo ="test"
            };
            _dbcontext.Add(demo);
            _dbcontext.SaveChanges();
            return Ok("");
        }
    }
}
