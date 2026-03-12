using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // /api/walks
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalksRepository walksRepository;

        public WalksController(IMapper mapper, IWalksRepository walksRepository)
        {
            this.mapper = mapper;
            this.walksRepository = walksRepository;
        }
        //Create walks
        // Post: /api/walks
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddWalksRequestDto addWalksRequestDto)
        {
            //Map DTO to Domain model
            var walkDomainModel = mapper.Map<Walk>(addWalksRequestDto);

            await walksRepository.CreateAsync(walkDomainModel);

            //Map Domain model to DTO

            return Ok(mapper.Map<WalkDto>(walkDomainModel));

        }

        //Get Walks
        // Get: /api/walks
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var walksDomainModel = await walksRepository.GetAllAsync();
            //Map Domain model to DTO
            return Ok(mapper.Map<List<WalkDto>>(walksDomainModel));
        }
    }
}
