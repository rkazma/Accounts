using Accounts.Common;
using Accounts.Domain.Models;
using Accounts.DTOModels;
using Accounts.Service.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountService _accountService;
        public IMapper _mapper;
        private readonly ILogger<AccountsController> _logger;
        public AccountsController(IAccountService accountService, IMapper mapper, ILogger<AccountsController> logger)
        {
            _accountService = accountService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("{customerId}")]
        public async Task<IActionResult> CreateAccount([FromRoute] long customerId, [FromBody] AccountCreationDTO accountCreationDTO)
        {
            _logger.LogInformation("Accessing CreateAccount service");
            var result = await _accountService.CreateAccount(customerId, accountCreationDTO.InitialCredit);

            switch (result.result)
            {
                case DBErrorCode.ACCOUNT_ALREADY_EXISTS:
                    Response.Headers.Add("accountId", result.accountId.ToString());
                    return Ok(new AppExceptionResponse<ErrorResult>(HttpStatusCode.OK, APIErrorCode.ACCOUNT_ALREADY_EXISTS.Message, APIErrorCode.ACCOUNT_ALREADY_EXISTS));

                default: return Ok(result.accountId);
            }
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomerInfo([FromRoute] long customerId)
        {
            var result = await _accountService.GetCustomerInfo(customerId);

            CustomerInfoDTO resultDto = _mapper.Map<CustomerInfoObj, CustomerInfoDTO>(result);

            return Ok(resultDto);

        }
    }
}
