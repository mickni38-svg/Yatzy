using BatchPay.Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace BatchPay.Api.Controllers;

[ApiController]
[Route( "api/users" )]
public sealed class UsersController( IUserService users ) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll( CancellationToken ct )
        => Ok( await users.GetAllAsync( ct ) );
}
