using AhorraYa.Application.Dtos.City;
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
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {

        private readonly ILogger<CitiesController> _logger;
        private readonly IApplication<City> _city;
        private readonly IMapper _mapper;
        public CitiesController(ILogger<CitiesController> logger,
            IApplication<City> city,
            IMapper mapper)
        {
            _logger = logger;
            _city = city;
            _mapper = mapper;
        }

        [HttpGet("All")]
        [Authorize(Roles = "Admin, ViewerPlus, Viewer")]
        public async Task<IActionResult> GetAll(string? searchText, string orderBy = "A-Z")
        {
            try
            {
                Func<IQueryable<City>, IOrderedQueryable<City>>? cityOrder = null;
                if (orderBy == "A-Z")
                {
                    cityOrder = c => c.OrderBy(c => c.CityName);
                }
                else
                {
                    cityOrder = c => c.OrderByDescending(c => c.CityName);
                }

                Expression<Func<City, bool>>? filter = null;
                if (searchText != null)
                {
                    filter = c => c.CityName.Contains(searchText);
                }

                var cities = _mapper.Map<IList<CityResponseDto>>(_city.GetAll(filter, cityOrder));
                if (cities.Count > 0)
                {
                    return Ok(cities);
                }
                else
                {
                    return NotFound("No records were found.");
                }
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
                City city = _city.GetById(id.Value);
                return Ok(_mapper.Map<CityResponseDto>(city));
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
        public async Task<IActionResult> Create(CityRequestDto cityRequestDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (cityRequestDto.Id != 0) //Si estas creando el id debe ser cero.
                    {
                        throw new ExceptionIdNotZero(typeof(City), cityRequestDto.Id.ToString());
                    }
                    var city = _mapper.Map<City>(cityRequestDto);
                    #region Exist
                    _city.Exist(c => c.CityName == city.CityName && c.ProvinceId == city.ProvinceId);
                    #endregion
                    _city.Save(city);
                    return Ok(city.Id);
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
                catch (ExceptionAlreadyExist ex) //Ya existe una marca con el mismo nombre.
                {
                    return StatusCode(500, ex.Message);
                }
                catch (Exception)
                {
                    return StatusCode(500, "An unexpected error occurred");
                }
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpPut("Update")]
        [Authorize(Roles = "Admin, ViewerPlus")]
        public async Task<IActionResult> Update(int? id, CityRequestDto cityRequestDto)
        {
            if (ModelState.IsValid && id.HasValue)
            {
                try
                {
                    City cityBack = _city.GetById(id.Value);

                    cityBack = _mapper.Map<City>(cityRequestDto);

                    #region Exist
                    _city.Exist(c => c.CityName == cityBack.CityName && 
                                c.ProvinceId == cityBack.ProvinceId &&
                                c.Id != cityBack.Id);
                    #endregion

                    _city.Save(cityBack);

                    var response = _mapper.Map<CityRequestDto>(cityRequestDto);
                    return Ok(response);
                }
                catch (AutoMapperMappingException)
                {
                    throw new ExceptionRequestMappingError(); //No pudo mapear del Request al objeto local.
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
                catch (ExceptionAlreadyExist ex) //Ya existe una marca con el mismo nombre.
                {
                    return StatusCode(500, ex.Message);
                }
                catch (Exception)
                {
                    return StatusCode(500, "An unexpected error occurred");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete("Remove")]
        [Authorize(Roles = "Admin, ViewerPlus")]
        public async Task<IActionResult> Remove(int? id)
        {
            if (ModelState.IsValid && id.HasValue)
            {
                try
                {
                    City cityBack = _city.GetById(id.Value);

                    _city.RemoveById(cityBack.Id);
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
            else
            {
                return BadRequest();
            }
        }

    }
}