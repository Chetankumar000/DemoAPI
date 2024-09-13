using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DemoAPI.Dto;
using DemoAPI.Interfaces;
using DemoAPI.Models;


namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository _ownerRespository;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public OwnerController(IOwnerRepository ownerRespository, ICountryRepository countryRepository,IMapper mapper)
        {
            _ownerRespository = ownerRespository;
            _countryRepository = countryRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public IActionResult GetOwners()
        {
            var owners = _mapper.Map<List<OwnerDto>>(_ownerRespository.GetOwners());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(owners);
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(400)]

        public IActionResult GetOwner(int ownerId)
        {
            if (!_ownerRespository.OwnerExists(ownerId))
            {
                return NotFound();
            }
            var Owner = _mapper.Map<OwnerDto>(_ownerRespository.GetOwner(ownerId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(Owner);

        }

        [HttpGet("{ownerId}/pokemons")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(400)]

        public IActionResult GetPokemonsByOwner(int ownerId)
        {
            if (!_ownerRespository.OwnerExists(ownerId))
            {
                return NotFound();
            }
            var Pokemons = _mapper.Map<List<PokemonDto>>(_ownerRespository.GetPokemonByOwner(ownerId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(Pokemons);
        }

        [HttpGet("{pokeId}/owners")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(400)]

        public IActionResult GetOwnerByPokemon(int pokeId)
        {
            if (!_ownerRespository.OwnerExists(pokeId))
            {
                return NotFound();
            }
            var Owners = _mapper.Map<List<OwnerDto>>(_ownerRespository.GetOwnerOfAPokemon(pokeId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(Owners);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]

        public IActionResult CreateOwner([FromQuery] int countryId,   [FromBody] OwnerDto ownerCreate)
        {
            if (ownerCreate == null)
            {
                return BadRequest(ModelState);
            }

            var owner = _ownerRespository.GetOwners().Where(c => c.LastName.Trim().ToUpper() == ownerCreate.LastName.Trim().ToUpper()).FirstOrDefault();

            if (owner != null)
            {
                ModelState.AddModelError("", "Owner already exists");
                return StatusCode(422, ModelState);
            }

            var ownerMap = _mapper.Map<Owner>(ownerCreate);
            ownerMap.Country = _countryRepository.GetCountry(countryId);
            if (!_ownerRespository.CreateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }

        [HttpPut("{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]

        public IActionResult UpdateOwner(int ownerId, [FromBody] OwnerDto OwnerUpdate)
        {
            if (OwnerUpdate == null)
            {
                return BadRequest(ModelState);
            }

            if (ownerId != OwnerUpdate.Id)
            {
                return BadRequest(ModelState);
            }

            if (!_ownerRespository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var ownerMap = _mapper.Map<Owner>(OwnerUpdate);
            if (!_ownerRespository.UpdateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }
            return NoContent();



        }
        [HttpDelete("{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]

        public IActionResult DeleteOwner(int ownerId)
        {
          

            if (!_ownerRespository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var owner = _ownerRespository.GetOwner(ownerId);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

     
            if (!_ownerRespository.DeleteOwner(owner))
            {
                ModelState.AddModelError("", "Something went wrong while deleting");
                return StatusCode(500, ModelState);
            }
            return NoContent();



        }
    }
}
