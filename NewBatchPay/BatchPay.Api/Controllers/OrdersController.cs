using BatchPay.Contracts.Dto;
using BatchPay.Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace BatchPay.Api.Controllers;

[ApiController]
[Route( "api/orders" )]
public sealed class OrdersController( IOrderService orders ) : ControllerBase
{
    [HttpPost( "place" )]
    public async Task<IActionResult> Place( [FromBody] PlaceOrderRequestDto req, CancellationToken ct )
    {
        try
        {
            var orderId = await orders.PlaceAsync( req, ct );
            return Ok( new { orderId } );
        }
        catch (ArgumentException ex)
        {
            return BadRequest( ex.Message );
        }
        catch (InvalidOperationException ex)
        {
            return NotFound( ex.Message );
        }
    }

    // ✅ NEW: Overview -> hent seneste ordre pr medlem
    // GET api/orders/group/123/latest
    [HttpGet( "group/{groupPaymentId:int}/latest" )]
    public async Task<IActionResult> GetLatestForGroupPayment( int groupPaymentId, CancellationToken ct )
    {
        try
        {
            var list = await orders.GetLatestForGroupPaymentAsync( groupPaymentId, ct );
            return Ok( list );
        }
        catch (ArgumentException ex)
        {
            return BadRequest( ex.Message );
        }
    }
}