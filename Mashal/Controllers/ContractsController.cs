using System.Security.Claims;
using AutoMapper;
using Core.DTOs.App;
using Core.Models;
using Core.Models.Identity;
using Core.Services;
using Core.Specification.Params;
using Core.Specification.Serv.Spec;
using lawyer.Api.Helpers;
using Mashal.Helpers.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lawyer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
        private readonly IGenericRepository<ContractTemplate> _repo;
        private readonly IGenericRepository<UserContract> _userRepo;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly UserManager<AppUser> _userManager;

        public ContractsController(IGenericRepository<ContractTemplate> repo,
            IMapper mapper,
            IGenericRepository<UserContract> userRepo,
            IWebHostEnvironment iWebHostEnvironment,
            UserManager<AppUser> userManager)
        {
            _repo = repo;
            _mapper = mapper;
            _userRepo = userRepo;
            _IWebHostEnvironment = iWebHostEnvironment;
            _userManager = userManager;
        }


        [HttpGet]
        public async Task<ActionResult<Pagination<ContractTemplateDto>>> GetAll([FromQuery] ContractTemplateParams contractParams)
        {
            var countSpec = new ContractTemplatesWithPagingSpec(contractParams);
            var Templates = await _repo.GetAllWithSpecAsync(countSpec);
            var spec = new ContractTemplatesWithCountSpec(contractParams);
            var totalItems = await _repo.GetCountWithSpecAsync(spec);
            var productDtos = _mapper.Map<IReadOnlyList<ContractTemplateDto>>(Templates.ToList());
            return Ok(productDtos);
        }


        [HttpGet("product/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var spec = new ContractTemplateByIdSpec(id);
            var Template = await _repo.GetByIdWithSpecAsync(spec);
            if (Template is null)
                return NotFound(new ApiResponse(404));
            var TemplateDto = _mapper.Map<ContractTemplateDto>(Template);
            return Ok(TemplateDto);
        }


        [Authorize]
        [HttpPost("fill-template")]
        public async Task<ActionResult<UserContractResponseDto>> UploadUserContract(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new ApiResponse(400));


            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var savePath = Path.Combine(_IWebHostEnvironment.WebRootPath, "user-contracts", fileName);

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("User not found.");
            }


            var userContract = new UserContract
            {
                UserId = userId,
                FilePath = $"/user-contracts/{fileName}",
                CreatedAt = DateTime.UtcNow
            };

            await _userRepo.AddAsync(userContract);
            await _userRepo.SaveAsync();


            return Ok(userContract);
        }


        // عاوز اعمل كنترولر يرجع كل العقود الخاصه بستخدم معين 
        [Authorize]
        [HttpGet("user-contracts")]
        public async Task<IActionResult> GetUserContracts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401));

            var contracts = await _userRepo.FindAsync(c => c.UserId == userId);

            if (contracts == null || !contracts.Any())
                return NotFound(new ApiResponse (404,"No contracts found for this user"));


            return Ok(contracts);
        }


    }
}
