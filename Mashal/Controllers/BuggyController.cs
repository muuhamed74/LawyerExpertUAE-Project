using Mashal.Helpers.Errors;
using Microsoft.AspNetCore.Mvc;
using Repo.Data;

namespace Mashal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuggyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BuggyController(ApplicationDbContext context)
        {
            _context = context;
        }



        [HttpGet("NotFound")]
        public ActionResult GetNotFountRequest()
        {
            var Product = _context.ContractTemplates.Find(100);
            if (Product is null)
                return NotFound(new ApiResponse(404));
            return Ok(Product);
        }



        [HttpGet("ServerError")]
        public ActionResult GetServerError()
        {
            var Template = _context.ContractTemplates.Find(100);
            var ProductToReturn = Template.ToString();
            return Ok(ProductToReturn);
        }

        [HttpGet("BadRequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }


        [HttpGet("BadRequest/{id}")]
        public ActionResult GetBadRequest(int id)
        {
            return BadRequest(new ApiResponse(400));
        }

    }
}
