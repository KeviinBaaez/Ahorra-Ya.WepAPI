using AhorraYa.Application.Dtos.Brand;
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
    public class BrandsController : ControllerBase
    {

        private readonly ILogger<BrandsController> _logger;
        private readonly IApplication<Brand> _brand;
        private readonly IMapper _mapper;
        public BrandsController(ILogger<BrandsController> logger,
            IApplication<Brand> brand,
            IMapper mapper)
        {
            _logger = logger;
            _brand = brand;
            _mapper = mapper;
        }

        [HttpGet("All")]
        [Authorize(Roles = "Admin, ViewerPlus, Viewer")]
        public async Task<IActionResult> GetAll(string? searchText, string orderBy="A-Z")
        {
            try
            {
                Func<IQueryable<Brand>, IOrderedQueryable<Brand>>? brandOrder = null;
                if(orderBy == "A-Z")
                {
                    brandOrder = b => b.OrderBy(b => b.BrandName);
                }
                else
                {
                    brandOrder = b => b.OrderByDescending(b => b.BrandName);
                }

                Expression<Func<Brand, bool>>? filter = null;
                if(searchText != null)
                {
                    filter = b => b.BrandName.Contains(searchText);
                }

                var brands = _mapper.Map<IList<BrandResponseDto>>(_brand.GetAll(filter, brandOrder));
                if (brands.Count > 0)
                {
                    return Ok(brands);
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
                Brand brand = _brand.GetById(id.Value);
                return Ok(_mapper.Map<BrandResponseDto>(brand));
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
        public async Task<IActionResult> Create(BrandRequestDto brandRequestDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (brandRequestDto.Id != 0) //Si estas creando el id debe ser cero.
                    {
                        throw new ExceptionIdNotZero(typeof(Brand), brandRequestDto.Id.ToString());
                    }
                    var brand = _mapper.Map<Brand>(brandRequestDto);

                    _brand.Save(brand);
                    return Ok(brand.Id);
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
        public async Task<IActionResult> Update(int? id, BrandRequestDto brandRequestDto)
        {
            if (ModelState.IsValid && id.HasValue)
            {
                try
                {
                    Brand brandBack = _brand.GetById(id.Value);

                    brandBack = _mapper.Map<Brand>(brandRequestDto);
                    _brand.Save(brandBack);

                    var response = _mapper.Map<BrandResponseDto>(brandBack);
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
                    Brand brandBack = _brand.GetById(id.Value);

                    _brand.RemoveById(brandBack.Id);
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
