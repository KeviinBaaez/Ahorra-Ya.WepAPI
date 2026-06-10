using AhorraYa.Abstractions;
using AhorraYa.Application.Dtos.Identity.User;
using AhorraYa.Application.Dtos.Login;
using AhorraYa.Entities.MicrosoftIdentity;
using AhorraYa.Services.Interfaces;
using AhorraYa.WebApi.Configurations;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AhorraYa.WebApi.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IServiceTokenHandler _serviceTokenHandler;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public AuthController(UserManager<User> userManager,
              ILogger<AuthController> logger,
              IServiceTokenHandler serviceTokenHandler,
              IMapper mapper, IEmailService emailService)
        {
            _userManager = userManager;
            _logger = logger;
            _serviceTokenHandler = serviceTokenHandler;
            _mapper = mapper;
            _emailService = emailService;
        }

        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterRequestDto user)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var userExist = await _userManager.FindByEmailAsync(user.Email);
                    if (userExist != null)
                    {
                        return BadRequest($"A user with the email address already exists.\nEmail: {user.Email}");
                    }
                    var newUser = await _userManager.CreateAsync(new User()
                    {
                        Name = user.Name,
                        UserName = user.UserName,
                        Email = user.Email
                    }, user.Password);
                    if (newUser.Succeeded)
                    {
                        var createdUser = await _userManager.FindByEmailAsync(user.Email);

                        await _userManager.AddToRoleAsync(createdUser, "User");

                        // GENERACIÓN DEL CÓDIGO DE 6 DÍGITOS NATIVO DE IDENTITY
                        string codigoDeSeisDigitos = await _userManager.GenerateTwoFactorTokenAsync(createdUser, "Email");

                        // ENVÍO DE MAIL REAL
                        await _emailService.SendVerificationEmailAsync(createdUser.Email, codigoDeSeisDigitos);
                        _logger.LogInformation($"Código generado para {createdUser.UserName}: {codigoDeSeisDigitos}");


                        var newUserResponse = _mapper.Map<UserRegisterResponseDto>(user);
                        return Ok(newUserResponse);
                    }
                    else
                    {
                        return BadRequest(newUser.Errors.Select(e => e.Description).ToList());
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error durante el registro de usuario");
                    throw;
                }
            }
            return BadRequest("The data is invalid");
        }

        [HttpPost]
        [Route("VerifyCode")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return NotFound("User nor found");
            }

            // Valida el token de 6 dígitos contra la infraestructura interna de Identity
            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", request.Code);

            if (!isValid)
            {
                return BadRequest("El código es incorrecto o ya expiró.");
            }

            // Si es válido, actualizamos la columna EmailConfirmed a true (1)
            user.EmailConfirmed = true;
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return BadRequest("Error al activar la cuenta.");
            }

            return Ok(new { message = "Cuenta Activada con éxito." });
        }


        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserRequestDto userLogin)
        {
            if (ModelState.IsValid)
            {
                User existUser = null;
                if (string.IsNullOrEmpty(userLogin.Email) || userLogin.Email == "string")
                {
                    existUser = await _userManager.FindByNameAsync(userLogin.UserName);
                }
                else
                {
                    existUser = await _userManager.FindByEmailAsync(userLogin.Email);
                }
                    
                if (existUser != null)
                {
                    if(!existUser.EmailConfirmed)
                    {
                        return BadRequest(new LoginUserResponseDto()
                        {
                            Login = false,
                            Errores = new List<string> { "Debes confirmar tu cuenta mediante el código enviado a tu mail antes de iniciar sesión." }
                        });
                    }

                    var isCorrect = await _userManager.CheckPasswordAsync(existUser, userLogin.Password);
                    if (isCorrect)
                    {
                        try
                        {
                            var roles = await _userManager.GetRolesAsync(existUser);
                            var parameters = new TokenParameters()
                            {
                                Id = existUser.Id.ToString(),
                                PasswordHash = existUser.PasswordHash,
                                UserName = existUser.UserName,
                                Email = existUser.Email,
                                Roles = roles
                            };
                            var jwt = _serviceTokenHandler.GenerateJwtTokens(parameters);
                            return Ok(new LoginUserResponseDto()
                            {
                                Login = true,
                                Token = jwt,
                                UserName = existUser.UserName,
                                Mail = existUser.Email,
                                Role = roles.FirstOrDefault()
                            });
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }
            }
            return BadRequest(new LoginUserResponseDto()
            {
                Login = false,
                Errores = new List<string>()
                    {
                       "Incorrect username or password"
                    }
            });
        }
    }
}
