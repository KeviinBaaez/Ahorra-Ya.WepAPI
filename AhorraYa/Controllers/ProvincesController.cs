using AhorraYa.Application.Dtos.Province;
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
    public class ProvincesController : ControllerBase
    {

        private readonly ILogger<ProvincesController> _logger;
        private readonly IApplication<Province> _province;
        private readonly IMapper _mapper;
        public ProvincesController(ILogger<ProvincesController> logger,
            IApplication<Province> province,
            IMapper mapper)
        {
            _logger = logger;
            _province = province;
            _mapper = mapper;
        }

        [HttpGet("All")]
        [Authorize(Roles = "Admin, ViewerPlus, Viewer")]
        public async Task<IActionResult> GetAll(string? searchText, string orderBy = "A-Z")
        {
            try
            {
                Func<IQueryable<Province>, IOrderedQueryable<Province>>? provinceOrder = null;
                if (orderBy == "A-Z")
                {
                    provinceOrder = p => p.OrderBy(p => p.ProvinceName);
                }
                else
                {
                    provinceOrder = p => p.OrderByDescending(p => p.ProvinceName);
                }

                Expression<Func<Province, bool>>? filter = null;
                if (searchText != null)
                {
                    filter = p => p.ProvinceName.Contains(searchText);
                }

                var provinces = _mapper.Map<IList<ProvinceResponseDto>>(_province.GetAll(filter, provinceOrder));
                if (provinces.Count > 0)
                {
                    return Ok(provinces);
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
                Province province = _province.GetById(id.Value);
                return Ok(_mapper.Map<ProvinceResponseDto>(province));
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
        public async Task<IActionResult> Create(ProvinceRequestDto provinceRequestDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (provinceRequestDto.Id != 0) //Si estas creando el id debe ser cero.
                    {
                        throw new ExceptionIdNotZero(typeof(Province), provinceRequestDto.Id.ToString());
                    }
                    var province = _mapper.Map<Province>(provinceRequestDto);

                    #region Exist
                    _province.Exist(p => p.ProvinceName == province.ProvinceName &&
                                p.CountryId == province.CountryId);
                    #endregion

                    _province.Save(province);
                    return Ok(province.Id);
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
        public async Task<IActionResult> Update(int? id, ProvinceRequestDto provinceRequestDto)
        {
            if (ModelState.IsValid && id.HasValue)
            {
                try
                {
                    Province provinceBack = _province.GetById(id.Value);

                    provinceBack = _mapper.Map<Province>(provinceRequestDto);

                    #region Exist
                    _province.Exist(p => p.ProvinceName == provinceBack.ProvinceName &&
                                p.CountryId == provinceBack.CountryId && 
                                p.Id != provinceBack.Id);
                    #endregion

                    _province.Save(provinceBack);

                    var response = _mapper.Map<ProvinceRequestDto>(provinceRequestDto);
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
                    Province provinceBack = _province.GetById(id.Value);

                    _province.RemoveById(provinceBack.Id);
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
