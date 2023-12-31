using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _platformRepository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(
            IPlatformRepo platformRepo, 
            IMapper mapper,
            ICommandDataClient commandDataClient)
        {
            _platformRepository = platformRepo;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("--> Getting platforms");

            var platforms = _platformRepository.GetPlatforms();

            var platformDtos = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);

            return Ok(platformDtos);
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            Console.WriteLine($"--> Getting platform by id '{id}'");

            var platform = _platformRepository.GetPlatformById(id);

            if (platform == null) { return NotFound(); }

            var platformDto = _mapper.Map<PlatformReadDto>(platform);

            return Ok(platformDto);
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto createDto)
        {
            Console.WriteLine($"--> Creating platform [{createDto.ToString()}]");

            var platformModel = _mapper.Map<Platform>(createDto);

            _platformRepository.CreatePlatform(platformModel);
            _platformRepository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
        }
    }
}
