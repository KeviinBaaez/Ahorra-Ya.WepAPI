using AhorraYa.Application.Dtos.Brand;
using AhorraYa.Application.Dtos.Country;
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
    public class CountriesController : ControllerBase
    {

        private readonly ILogger<CountriesController> _logger;
        private readonly IApplication<Country> _country;
        private readonly IMapper _mapper;
        public CountriesController(ILogger<CountriesController> logger,
            IApplication<Country> country,
            IMapper mapper)
        {
            _logger = logger;
            _country = country;
            _mapper = mapper;
        }

        [HttpGet("All")]
        [Authorize(Roles = "Admin, ViewerPlus, Viewer")]
        public async Task<IActionResult> GetAll(string? searchText, string orderBy = "A-Z")
        {
            try
            {
                Func<IQueryable<Country>, IOrderedQueryable<Country>>? countryOrder = null;
                if (orderBy == "A-Z")
                {
                    countryOrder = c => c.OrderBy(c => c.CountryName);
                }
                else
                {
                    countryOrder = c => c.OrderByDescending(c => c.CountryName);
                }

                Expression<Func<Country, bool>>? filter = null;
                if (searchText != null)
                {
                    filter = c => c.CountryName.Contains(searchText);
                }

                var countries = _mapper.Map<IList<CountryResponseDto>>(_country.GetAll(filter, countryOrder));
                if (countries.Count > 0)
                {
                    return Ok(countries);
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
                Country country = _country.GetById(id.Value);
                return Ok(_mapper.Map<CountryResponseDto>(country));
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
        public async Task<IActionResult> Create(CountryRequestDto countryRequestDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (countryRequestDto.Id != 0) //Si estas creando el id debe ser cero.
                    {
                        throw new ExceptionIdNotZero(typeof(Country), countryRequestDto.Id.ToString());
                    }
                    var country = _mapper.Map<Country>(countryRequestDto);

                    #region Exist
                    _country.Exist(c => c.CountryName == country.CountryName);
                    #endregion

                    _country.Save(country);
                    return Ok(country.Id);
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
        public async Task<IActionResult> Update(int? id, CountryRequestDto countryRequestDto)
        {
            if (ModelState.IsValid && id.HasValue)
            {
                try
                {
                    Country countryBack = _country.GetById(id.Value);

                    countryBack = _mapper.Map<Country>(countryRequestDto);

                    #region Exist
                    _country.Exist(c => c.CountryName == countryBack.CountryName &&
                                c.Id != countryBack.Id);
                    #endregion

                    _country.Save(countryBack);

                    var response = _mapper.Map<CountryRequestDto>(countryRequestDto);
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
                    Country countryBack = _country.GetById(id.Value);

                    _country.RemoveById(countryBack.Id);
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
