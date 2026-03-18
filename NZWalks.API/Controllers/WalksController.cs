using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
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
        [ValidateModel]
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

        // Get Walks by Id
        // Get: /api/walks/{id}
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walkDomainModel = await walksRepository.GetByIdAsync(id);
            if (walkDomainModel == null)
            {
                return NotFound();
            }
            //Map Domain model to DTO
            return Ok(mapper.Map<WalkDto>(walkDomainModel));
        }

        // Update Walks by Id
        // Put: /api/walks/{id}
        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWalkRequestDto updateWalksRequestDto)
        {
            //Map DTO to Domain model
            var walkDomainModel = mapper.Map<Walk>(updateWalksRequestDto);

            walkDomainModel = await walksRepository.UpdateAsync(id, walkDomainModel);

            if (walkDomainModel == null)
            {
                return NotFound();
            }
            //Map Domain model to DTO
            return Ok(mapper.Map<WalkDto>(walkDomainModel));
        }

        // Delete Walks by Id
        // Delete: /api/walks/{id}
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedWalkDomainModel = await walksRepository.DeleteAsync(id);

            if (deletedWalkDomainModel == null)
            {
                return NotFound();
            }

            //Map Domain model to DTO
            return Ok(mapper.Map<WalkDto>(deletedWalkDomainModel));
        }
    }
}
