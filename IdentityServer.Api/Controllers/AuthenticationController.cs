using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer.Api.Security;
using IdentityServer.Core.Entities;
using IdentityServer.Infrastructure.Interfaces;
using IdentityServer.Infrastructure.RequestModels;
using IdentityServer.Infrastructure.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly ITokenHelper _tokenHelper;

        public AuthenticationController(IAccountRepository accountRepository, IMapper mapper,ITokenHelper tokenHelper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _tokenHelper = tokenHelper;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody]SignUpRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var account = _mapper.Map<Account>(request);
                account = await _accountRepository.SignUpAsync(account);
                // TODO Mail send integration

                return Ok();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Exception", e.Message);
            }

            return BadRequest(ModelState);
        }

        [HttpGet("Activation/{activationToken}")]
        public IActionResult CheckActivationToken(string activationToken)
        {
            try
            {
                Guid token = Guid.Parse(activationToken);

                var isValid = _accountRepository.CheckActivationToken(token);
                if (isValid)
                {
                    return Ok();
                }
                else
                {
                    ModelState.AddModelError("Error", "Token is not valid");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Exception", e.Message);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("Activation/{activationToken}")]
        public IActionResult ActivateAccount(string activationToken)
        {
            try
            {
                Guid token = Guid.Parse(activationToken);
                _accountRepository.ActivateAccount(token);
                return Ok();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Exception", e.Message);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody]SignInRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var account = _mapper.Map<Account>(request);
                account = await _accountRepository.SignInAsync(account);

                if (account == null)
                {
                    return Unauthorized();
                }

                return Ok(_tokenHelper.GenerateToken(account));
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Exception", e.Message);
            }

            return BadRequest(ModelState);
        }
        
        [HttpGet("Account"),Authorize]
        public IActionResult CurrentAccount()
        {
            try
            {
                var claims = User.Claims;
                var userId = claims.FirstOrDefault(x => x.Type == "Id")?.Value;
                if (userId == null) return Unauthorized();
                Guid id = Guid.Parse(userId);
                var account = _accountRepository.GetAccountById(id);
                if (account == null) return Unauthorized();
                return Ok(_mapper.Map<CurrentAccount>(account));
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Exception",e.Message);
            }

            return BadRequest(ModelState);
        }
        
    }
}