using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.DTO;
using PaymentService.Application.Interfaces;

namespace PaymentService.Web.Controllers;

[ApiController]
[Route("api/v1/balance/")]
public class BalanceController : ControllerBase
{
    private readonly IBalanceService _balanceService;

    public BalanceController(IBalanceService balanceService)
    {
        _balanceService = balanceService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBalanceAsync(Guid id)
    {
        var balance = await _balanceService.GetBalanceByUserId(id);
        return Ok(balance);
    }

    [HttpPost("transaction/withdraw")]
    public async Task<IActionResult> WithdrawAsync(WithdrawDto dto)
    {
        var success = await _balanceService.WithdrawAsync(dto);
        
        if (success)
            return Ok(new { Success = true });
        else
            return BadRequest(new { Success = false, Error = "Insufficient funds" });
    }

    [HttpPost("transaction/deposit")]
    public async Task<IActionResult> DepositAsync(DepositDto dto)
    {
        var deposit = await _balanceService.DepositAsync(dto);
        return Ok(deposit);
    }
}