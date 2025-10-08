using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Interfaces;

namespace PaymentService.Web.Controllers;

[ApiController]
[Route("api/v1/balance")]
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
}