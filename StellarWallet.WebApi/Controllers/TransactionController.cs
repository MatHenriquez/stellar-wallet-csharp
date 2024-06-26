﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Interfaces;

namespace StellarWallet.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController(ITransactionService transactionService) : ControllerBase
    {
        private readonly ITransactionService _transactionService = transactionService;

        [HttpPost("AccountCreation")]
        [Authorize]
        public async Task<IActionResult> CreateAccount()
        {
            try
            {
                string jwt = await HttpContext.GetTokenAsync("access_token") ?? throw new Exception("Unauthorized");

                return Ok(_transactionService.CreateAccount(jwt));
            }
            catch (Exception e)
            {
                if (e.Message == "User not found")
                    return NotFound(e.Message);
                else if (e.Message == "Unauthorized")
                    return Unauthorized();

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost("Payment")]
        [Authorize]
        public async Task<IActionResult> SendPayment([FromBody] SendPaymentDto sendPaymentDto)
        {
            try
            {
                string jwt = await HttpContext.GetTokenAsync("access_token") ?? throw new Exception("Unauthorized");
                await _transactionService.SendPayment(sendPaymentDto, jwt);
                return Ok();
            }
            catch (Exception e)
            {
                if (e.Message == "User not found")
                    return NotFound(e.Message);
                else if (e.Message == "Unauthorized")
                    return Unauthorized();

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("Payment")]
        [Authorize]
        public async Task<IActionResult> GetPayments([FromQuery] int pageNumber, int pageSize)
        {
            try
            {
                string jwt = await HttpContext.GetTokenAsync("access_token") ?? throw new Exception("Unauthorized");

                return Ok(await _transactionService.GetTransaction(jwt, pageNumber, pageSize));
            }
            catch (Exception e)
            {
                if (e.Message == "User not found")
                    return NotFound(e.Message);
                else if (e.Message == "Unauthorized")
                    return Unauthorized();

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost("TestFund")]
        [Authorize]
        public async Task<IActionResult> GetTestFunds([FromBody] GetTestFundsDto getTestFundsDto)
        {
            string? jwt = await HttpContext.GetTokenAsync("access_token");
            if (jwt is null)
                return Unauthorized();

            bool wasFunded = await _transactionService.GetTestFunds(getTestFundsDto.PublicKey);

            if (wasFunded)
                return Ok();
            else
                return StatusCode(StatusCodes.Status500InternalServerError, "Funds not received");
        }

        [HttpGet("Balance")]
        [Authorize]
        public async Task<IActionResult> GetBalances([FromQuery] GetBalancesDto getBalancesDto)
        {
            string? jwt = await HttpContext.GetTokenAsync("access_token");
            if (jwt is null)
                return Unauthorized();

            try
            {
                return Ok(await _transactionService.GetBalances(getBalancesDto));
            }
            catch (Exception e)
            {
                if (e.Message == "User not found")
                    return NotFound(e.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}

