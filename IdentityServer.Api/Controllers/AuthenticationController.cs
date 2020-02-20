using System;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer.Core.Entities;
using IdentityServer.Infrastructure.Interfaces;
using IdentityServer.Infrastructure.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public AuthenticationController(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpRequest request)
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
        public async Task<IActionResult> SignIn(SignInRequest request)
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
                
                // TODO Integrate Jwt token authentication
                return Ok();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Exception", e.Message);
            }

            return BadRequest(ModelState);
        }
    }
}