using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.http;

namespace PlatformService.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repo;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(IPlatformRepo platformRepo, IMapper mapper, ICommandDataClient commandDataClient)
        {
            _repo = platformRepo;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            var platforms = _repo.GetAllPlatforms();
            var platformDtos = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
            return Ok(platformDtos);
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var platform = _repo.GetPlatformById(id);
            if (platform == null) return NotFound();

            var platformDto = _mapper.Map<PlatformReadDto>(platform);
            return Ok(platformDto);
        }


        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            if (platformCreateDto is null)
            {
                throw new ArgumentNullException(nameof(platformCreateDto));
            }

            var platform = _mapper.Map<Platform>(platformCreateDto);

            _repo.CreatePlatform(platform);
            _repo.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platform);

            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not send synchronously: {e.Message}");
            }
            
            return CreatedAtRoute(nameof(GetPlatformById), new {Id = platformReadDto.Id}, platformReadDto);

        }




    }
}
