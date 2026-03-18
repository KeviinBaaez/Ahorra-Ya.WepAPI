using AhorraYa.Application.Dtos.PriceOfShop;
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
    public class PriceOfShopController : ControllerBase
    {
        private readonly IApplication<PriceOfShop> _priceOfShop;
        private readonly IMapper _mapper;

        private readonly IApplication<Product> _product;
        private readonly IApplication<Shop> _shop;

        public PriceOfShopController(IApplication<PriceOfShop> priceOfShop,
            IMapper mapper,
            IApplication<Product> product,
            IApplication<Shop> shop)
        {
            _priceOfShop = priceOfShop;
            _mapper = mapper;
            _product = product;
            _shop = shop;
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAll(string? searchText, int? brandId, int? shopId, string? orderBy = "A-Z")
        {
            try
            {
                Func<IQueryable<PriceOfShop>, IOrderedQueryable<PriceOfShop>>? order = null;
                if(orderBy == "A-Z")
                {
                    order = p => p.OrderBy(p => p.Price);
                }
                else
                {
                    order = p => p.OrderByDescending(p => p.Price);
                }
                Expression<Func<PriceOfShop, bool>>? filter = null;

                if (searchText != null)
                {
                    filter = p => p.Product!.Name!.Contains(searchText) || p.Product!.Brand!.BrandName.Contains(searchText);
                }
                Expression<Func<PriceOfShop, bool>>? filterByBrands = null;
                if(brandId != null)
                {
                    filterByBrands = p => p.Product!.BrandId == brandId;
                }
                Expression<Func<PriceOfShop, bool>>? filterByShops = null;
                if(shopId != null)
                {
                    filterByShops = p => p.ShopId == shopId;
                }

                var products = _mapper.Map<IList<PriceOfShopResponseDto>>(_priceOfShop.GetAll(filter, order, filterByBrands, filterByShops));
                if (products.Count > 0)
                {
                    return Ok(products);
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
        [Route("GetByProduct")]
        public async Task<IActionResult> GetByProduc(string name)
        {
            try
            {
                var listGeneral = _priceOfShop.GetAll();
                IList<PriceOfShop> listProductName = new List<PriceOfShop>();

                foreach (var item in listGeneral)
                {
                    if (item.Product!.Name!.ToUpper() == name.ToUpper())
                    {
                        listProductName.Add(item);
                    }
                }
                return Ok(_mapper.Map<IList<PriceOfShopResponseDto>>(listProductName));
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
        public async Task<IActionResult> Create(PriceOfShopRequestDto priceOfShopRequestDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (priceOfShopRequestDto.Id != 0)
                    {
                        throw new ExceptionIdNotZero(typeof(PriceOfShop), priceOfShopRequestDto.Id.ToString());
                    }


                    Product product = _product.GetById(priceOfShopRequestDto.ProductId);
                    Shop shop = _shop.GetById(priceOfShopRequestDto.ShopId);

                    var priceOfShop = _mapper.Map<PriceOfShop>(priceOfShopRequestDto);
                    _priceOfShop.Save(priceOfShop);
                    return Ok(priceOfShop.Id);
                }
                catch (AutoMapperMappingException)
                {
                    throw new ExceptionRequestMappingError();
                }
                catch (ExceptionRequestMappingError ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (ExceptionIdNotZero ex) //El Id es distinto a 0.
                {
                    return BadRequest(ex.Message);
                }
                catch (ExceptionIdNotFound ex)
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
        public async Task<IActionResult> Update(int? id, UpdatePriceOfShopRequestDto updPriceOfShopRequestDto)
        {
            if (ModelState.IsValid && id.HasValue)
            {
                try
                {
                    PriceOfShop priceOfShopBack = _priceOfShop.GetById(id.Value);
                    Product product = priceOfShopBack.Product!;
                    Shop shop = priceOfShopBack.Shop!;

                    priceOfShopBack = _mapper.Map<PriceOfShop>(updPriceOfShopRequestDto);
                    priceOfShopBack.Product = product;
                    priceOfShopBack.Shop = shop;
                    priceOfShopBack.RegistrationDate = DateTime.Now;

                    _priceOfShop.Save(priceOfShopBack);

                    var response = _mapper.Map<PriceOfShopResponseDto>(priceOfShopBack);
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
    }
}
