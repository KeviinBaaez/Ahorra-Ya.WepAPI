using AhorraYa.Application.Dtos.Shop;
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
    public class ShopsController : ControllerBase
    {
        private readonly ILogger<ShopsController> _logger;
        private readonly IApplication<Shop> _shop;
        private readonly IApplication<Location> _location;
        private readonly IMapper _mapper;

        public ShopsController(ILogger<ShopsController> logger,
            IApplication<Shop> shop,
            IApplication<Location> location,
            IMapper mapper)
        {
            _logger = logger;
            _shop = shop;
            _location = location;
            _mapper = mapper;
        }

        [HttpGet("All")]
        [Authorize(Roles = "Admin, ViewerPlus, Viewer")]
        public async Task<IActionResult> GetAll(string? searchText, string? orderBy = "A-Z")
        {
            try
            {
                Func<IQueryable<Shop>, IOrderedQueryable<Shop>>? shopOrder = null;
                if (orderBy == "A-Z")
                {
                    shopOrder = s => s.OrderBy(s => s.ShopName);
                }
                else
                {
                    shopOrder = s => s.OrderByDescending(s => s.ShopName);
                }

                Expression<Func<Shop, bool>>? filter = null;
                if(searchText is not null)
                {
                    filter = s => s.ShopName.Contains(searchText);
                }

                    var shops = _mapper.Map<IList<ShopResponseDto>>(_shop.GetAll(filter, shopOrder));
                if (shops.Count > 0)
                {
                    return Ok(shops);
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
                Shop shop = _shop.GetById(id.Value);
                var response = _mapper.Map<ShopResponseDto>(shop);
                return Ok(response);
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
        public async Task<IActionResult> Create(ShopRequestDto shopRequestDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if(shopRequestDto.Id != 0)
                    {
                        throw new ExceptionIdNotZero(typeof(Shop), shopRequestDto.Id.ToString());
                    }
                    Location location = _location.GetById(shopRequestDto.LocationId);

                    var shop = _mapper.Map<Shop>(shopRequestDto);
                    _shop.Save(shop);
                    return Ok(shop.Id);
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
                catch(ExceptionIdNotFound ex)
                {
                    return StatusCode(500, ex.Message);
                }
                catch (ExceptionAlreadyExist ex) //Ya existe una category con el mismo nombre.
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
        public async Task<IActionResult> Update(int? id, ShopRequestDto shopRequestDto)
        {
            if (ModelState.IsValid && id.HasValue)
            {
                try
                {
                    Shop shopBack = _shop.GetById(id.Value);
                    Location location = _location.GetById(shopRequestDto.LocationId);

                    shopBack = _mapper.Map<Shop>(shopRequestDto);
                    _shop.Save(shopBack);

                    var response = _mapper.Map<ShopResponseDto>(shopBack);
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
        [Authorize(Roles = "Admin, ViewerPlus")]
        public async Task<IActionResult> Remove(int? id)
        {
            if (ModelState.IsValid && id.HasValue)
            {
                try
                {
                    Shop shopBack = _shop.GetById(id.Value);
                    _shop.RemoveById(shopBack.Id);
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
