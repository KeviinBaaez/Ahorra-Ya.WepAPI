using AhorraYa.Application.Dtos.Product;
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
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IApplication<Product> _product;
        private readonly IApplication<Brand> _brand;
        private readonly IApplication<MeasurementUnit> _measurement;
        private readonly IApplication<Category> _category;
        private readonly IMapper _mapper;

        public ProductsController(ILogger<ProductsController> logger,
            IApplication<Product> product, 
            IApplication<Brand> brand,
            IApplication<MeasurementUnit> measurement,
            IApplication<Category> category,
            IMapper mapper)
        {
            _logger = logger;
            _product = product;
            _brand = brand;
            _measurement = measurement;
            _category = category;
            _mapper = mapper;
        }

        [HttpGet("All")]
        [Authorize(Roles = "Admin, ViewerPlus, User")]
        public async Task<IActionResult> GetAll(string? searchText, string? orderBy="A-Z")
        {
            try
            {
                Func<IQueryable<Product>, IOrderedQueryable<Product>>? productOrder = null;
                if(orderBy == "A-Z")
                {
                    productOrder = p => p.OrderBy(p => p.Name);
                }
                else
                {
                    productOrder = p => p.OrderByDescending(p => p.Name);
                }

                Expression<Func<Product, bool>>? filter = null;
                if (searchText != null)
                {
                    filter = p => p.Name.Contains(searchText);
                }

                var products = _mapper.Map<IList<ProductResponseDto>>(_product.GetAll(filter, productOrder));
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
                Product product = _product.GetById(id.Value);
                return Ok(_mapper.Map<ProductResponseDto>(product));
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
        public async Task<IActionResult> Create(ProductRequestDto productRequestDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if(productRequestDto.Id != 0)
                    {
                        throw new ExceptionIdNotZero(typeof(Product), productRequestDto.Id.ToString()); 
                    }


                    Brand brand = _brand.GetById(productRequestDto.BrandId);
                    Category category = _category.GetById(productRequestDto.CategoryId);

                    var product = _mapper.Map<Product>(productRequestDto);
                    #region Exist
                    //Valido si el producto ya existe.
                    //De momento me funciona, pero siento que no es la mejor opción de establecer esto acá
                    _product.Exist(p => p.Name == product.Name &&
                                 p.BrandId == product.BrandId);
                    #endregion
                    _product.Save(product);
                    return Ok(product.Id);
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
                catch(ExceptionIdNotFound ex)
                {
                    return StatusCode(500, ex.Message);
                }
                catch (ExceptionAlreadyExist ex) //Ya existe una product con el mismo nombre.
                {
                    return StatusCode(500, ex.Message.Remove(ex.Message.Length - 15) + " and brand already exist");
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
        public async Task<IActionResult> Update(int? id, ProductRequestDto productRequestDto)
        {
            if (ModelState.IsValid && id.HasValue)
            {
                try
                {
                    Brand brand = _brand.GetById(productRequestDto.BrandId);
                    Category category = _category.GetById(productRequestDto.CategoryId);

                    Product productBack = _product.GetById(id.Value);

                    productBack = _mapper.Map<Product>(productRequestDto);
                    productBack.Brand = brand;
                    productBack.Category = category;

                    #region Exist
                    //Valido si el producto ya existe.
                    //De momento me funciona, pero siento que no es la mejor opción de establecer esto acá
                    _product.Exist(p => p.Name == productBack.Name &&
                                 p.BrandId == productBack.BrandId &&
                                 p.CategoryId == productBack.CategoryId &&
                                 p.Image == productBack.Image &&
                                 p.Id != productBack.Id);
                    #endregion
                    _product.Save(productBack);

                    var response = _mapper.Map<ProductResponseDto>(productBack);
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
                    Product productBack = _product.GetById(id.Value);
                    _product.RemoveById(productBack.Id);
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

        //public async Task ExistProductAsync(Product product)
        //{
        //    var products = _product.GetAll(null, null);
        //    bool exist = products.Any(p =>
        //    p.Name == product.Name &&
        //    p.BrandId == product.BrandId);
            
        //    if (exist)
        //    {
        //        throw new ExceptionAlreadyExist(typeof(Product));
        //    }

        //}

    }
}
