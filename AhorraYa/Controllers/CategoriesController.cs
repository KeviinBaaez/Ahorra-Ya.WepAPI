using AhorraYa.Application.Dtos.Category;
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
    public class CategoriesController : ControllerBase
    {

        private readonly ILogger<CategoriesController> _logger;
        private readonly IApplication<Category> _category;
        private readonly IMapper _mapper;

        public CategoriesController(ILogger<CategoriesController> logger,
            IApplication<Category> category,
            IMapper mapper)
        {
            _logger = logger;
            _category = category;
            _mapper = mapper;
        }

        [HttpGet("All")]
        [Authorize(Roles = "Admin, ViewerPlus, Viewer")]
        public async Task<IActionResult> GetAll(string? searchText, string? orderBy="A-Z")
        {
            try
            {
                Func<IQueryable<Category>, IOrderedQueryable<Category>>? categoryOrder = null;
                if(orderBy == "A-Z")
                {
                    categoryOrder = c => c.OrderBy(c => c.CategoryName);
                }
                else
                {
                    categoryOrder = c => c.OrderByDescending(c => c.CategoryName);
                }
                Expression<Func<Category, bool>>? filter = null;
                if(searchText != null)
                {
                    filter = c => c.CategoryName.Contains(searchText);
                }
                var categories = _mapper.Map<IList<CategoryResponseDto>>(_category.GetAll(filter, categoryOrder));
                if (categories.Count > 0)
                {
                      return Ok(categories);
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

        [HttpGet("GetById")]
        [Authorize(Roles = "Admin, ViewerPlus, Viewer")]
        public async Task<IActionResult> GetById(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }
            try
            {
                Category category = _category.GetById(id.Value);
                return Ok(_mapper.Map<CategoryResponseDto>(category));
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
        public async Task<IActionResult> Create(CategoryRequestDto categoryRequestDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (categoryRequestDto.Id != 0)
                    {
                        throw new ExceptionIdNotZero(typeof(Category), categoryRequestDto.Id.ToString());
                    }
                    var category = _mapper.Map<Category>(categoryRequestDto);
                    _category.Save(category);
                    return Ok(category.Id);
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
        public async Task<IActionResult> Update(int? id, CategoryRequestDto categoryRequestDto)
        {
            if (ModelState.IsValid && id.HasValue)
            {
                try
                {
                    Category categoryBack = _category.GetById(id.Value);

                    categoryBack = _mapper.Map<Category>(categoryRequestDto);
                    _category.Save(categoryBack);

                    var response = _mapper.Map<CategoryResponseDto>(categoryBack);
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
                    Category categoryBack = _category.GetById(id.Value);

                    _category.RemoveById(categoryBack.Id);
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
