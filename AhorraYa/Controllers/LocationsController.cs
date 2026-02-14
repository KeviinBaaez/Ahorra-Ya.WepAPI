using AhorraYa.Application.Dtos.Location;
using AhorraYa.Application.Interfaces;
using AhorraYa.Entities;
using AhorraYa.Exceptions;
using AhorraYa.Exceptions.ExceptionsForId;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace AhorraYa.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = "Admin, ViewerPlus")]
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILogger<LocationsController> _logger;
        private readonly IApplication<Location> _location;
        private readonly IMapper _mapper;

        public LocationsController(ILogger<LocationsController> logger,
            IApplication<Location> location,
            IMapper mapper)
        {
            _logger = logger;
            _location = location;
            _mapper = mapper;
        }

        [HttpGet("All")]
        [Authorize(Roles = "Admin, ViewerPlus, Viewer")]
        public async Task<IActionResult> GetAll(string? searchText, string? orderBy = "A-Z")
        {
            try
            {
                Func<IQueryable<Location>, IOrderedQueryable<Location>>? locationOrder = null;
                if(orderBy == "A-Z")
                {
                    locationOrder = l => l.OrderBy(l => l.Address);
                }
                else
                {
                    locationOrder = l => l.OrderByDescending(l => l.Address);
                }

                Expression<Func<Location, bool>>? filter = null;
                if(searchText != null)
                {
                    filter = l => l.Address.Contains(searchText);
                }

                var locations = _mapper.Map<IList<LocationResponseDto>>(_location.GetAll(filter, locationOrder));
                if (locations.Count > 0)
                {
                    return Ok(locations);
                }
                else
                {
                    return NotFound("No records were found.");
                }
            }
            catch (ExceptionByServiceConnection ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (AutoMapperMappingException)
            {
                throw new ExceptionMappingError();
            }
            catch (ExceptionMappingError ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [HttpGet]
        [Route("GetById")]
        [Authorize(Roles = "Admin, ViewerPlus, Viewer")]
        public async Task<IActionResult> GetById(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }
            try
            {
                Location location = _location.GetById(id.Value);
                return Ok(_mapper.Map<LocationResponseDto>(location));
            }
            catch (AutoMapperMappingException)
            {
                throw new ExceptionMappingError();
            }
            catch (ExceptionMappingError ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (ExceptionIdNotFound ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (ExceptionIdNotZero ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred");
            }
        }


        [HttpPost("Create")]
        [Authorize(Roles = "Admin, ViewerPlus")]
        public async Task<IActionResult> Create(LocationRequestDto locationRequestDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (locationRequestDto.Id != 0)
                    {
                        throw new ExceptionIdNotZero(typeof(Location), locationRequestDto.Id.ToString());
                    }

                    var location = _mapper.Map<Location>(locationRequestDto);
                    _location.Save(location);
                    return Ok(location.Id);
                }
                catch (AutoMapperMappingException)
                {
                    throw new ExceptionRequestMappingError(); //No pudo mapear del Request al objeto local.
                }
                catch (ExceptionRequestMappingError ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (ExceptionIdNotZero ex) //El Id es distinto a 0.
                {
                    return BadRequest(ex.Message);
                }
                catch (ExceptionAlreadyExist ex) //Ya existe una location con los mismos datos.
                {
                    return StatusCode(500, ex.Message);
                }
                catch (Exception)
                {
                    return StatusCode(500, "An unexpected error occurred");
                }
            }
            return BadRequest();
        }

        [HttpPut("Update")]
        [Authorize(Roles = "Admin, ViewerPlus")]
        public async Task<IActionResult> Update(int? id, LocationRequestDto locationRequestDto)
        {
            if (ModelState.IsValid && id.HasValue)
            {
                try
                {
                    Location locationBack = _location.GetById(id.Value);

                    locationBack = _mapper.Map<Location>(locationRequestDto);
                    _location.Save(locationBack);

                    var response = _mapper.Map<LocationResponseDto>(locationBack);
                    return Ok(response);
                }
                catch (AutoMapperMappingException)
                {
                    throw new ExceptionRequestMappingError();
                }
                catch (ExceptionRequestMappingError ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (ExceptionIdNotFound ex)
                {
                    return StatusCode(500, ex.Message);
                }
                catch (ExceptionIdNotZero ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (ExceptionAlreadyExist ex)
                {
                    return StatusCode(500, ex.Message);
                }
                catch (Exception)
                {
                    return StatusCode(500, "An unexpected error occurred");
                }
            }
            return BadRequest();
        }

        [HttpDelete("Remove")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remove(int? id)
        {
            if (ModelState.IsValid && id.HasValue)
            {
                try
                {
                    Location locationBack = _location.GetById(id.Value);

                    _location.RemoveById(locationBack.Id);
                    return Ok();
                }
                catch (ExceptionIdNotZero ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (ExceptionIdNotFound ex)
                {
                    return StatusCode(500, ex.Message);
                }
                catch (Exception)
                {
                    return StatusCode(500, "An unexpected error occurred");
                }
            }
            return BadRequest();
        }
    }
}
