using BatchPay.Contracts.Dto;
using BatchPay.Data;
using BatchPay.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BatchPay.Logic.Services;

public sealed class OrderService( BatchPayContext db ) : IOrderService
{
    public async Task<int> PlaceAsync( PlaceOrderRequestDto dto, CancellationToken ct )
    {
        if (dto.GroupPaymentId <= 0) throw new ArgumentException( "GroupPaymentId is required" );
        if (dto.UserId <= 0) throw new ArgumentException( "UserId is required" );
        if (dto.Lines is null || dto.Lines.Count == 0) throw new ArgumentException( "Lines must not be empty" );

        var member = await db.GroupPaymentMembers
            .FirstOrDefaultAsync(
                m => m.GroupPaymentId == dto.GroupPaymentId
                  && m.MemberId == dto.UserId
                  && m.IsActive,
                ct );

        if (member is null)
            throw new InvalidOperationException(
                $"No GroupPaymentMember found for groupPaymentId={dto.GroupPaymentId}, userId={dto.UserId}." );

        var order = new OrderEntity
        {
            GroupPaymentMemberId = member.Id,
            MerchantId = dto.MerchantId == 0 ? null : dto.MerchantId,
            MerchantName = string.IsNullOrWhiteSpace( dto.MerchantName ) ? null : dto.MerchantName.Trim(),
            MerchantLink = string.IsNullOrWhiteSpace( dto.MerchantLink ) ? null : dto.MerchantLink.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            Lines = dto.Lines.Select( l => new OrderLineEntity
            {
                ItemName = (l.ItemName ?? "").Trim(),
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice
            } ).ToList()
        };

        foreach (var line in order.Lines)
        {
            if (line.ItemName.Length == 0) throw new ArgumentException( "Order line ItemName is required" );
            if (line.Quantity <= 0) throw new ArgumentException( "Order line Quantity must be > 0" );
            if (line.UnitPrice < 0) throw new ArgumentException( "Order line UnitPrice must be >= 0" );
        }

        db.Orders.Add( order );
        await db.SaveChangesAsync( ct );

        return order.Id;
    }

    public async Task<IReadOnlyList<MemberLatestOrderDto>> GetLatestForGroupPaymentAsync( int groupPaymentId, CancellationToken ct )
    {
        if (groupPaymentId <= 0) throw new ArgumentException( "groupPaymentId is required" );


        // Hent merchantId'er fra Orders for groupPayment, og excludér GroupPaymentMembers hvor Id matcher et merchantId.
        var merchantId = await db.Orders
            .Where( o => o.GroupPaymentMember.GroupPaymentId == groupPaymentId && o.MerchantId != null )
            .Select( o => o.MerchantId!.Value ).FirstOrDefaultAsync( ct );


        // 1) Find alle aktive medlemmer i groupPayment
        var members = await db.GroupPaymentMembers
            .Where( m => m.GroupPaymentId == groupPaymentId && m.IsActive && m.MemberId != merchantId )
            .Select( m => new { m.Id, m.MemberId } )
            .ToListAsync( ct );

        //memberid er userid
        var memberIds = members.Where( i => i.MemberId != merchantId ).Select( m => m.Id ).ToList();


        // 2) Find seneste order pr GroupPaymentMemberId
        // EF-friendly: sort -> group -> first
        var latestOrders = await db.Orders
            .Where( o => memberIds.Contains( o.GroupPaymentMemberId ) )
            .Include( o => o.Lines )
            .OrderByDescending( o => o.CreatedAtUtc )
            .ToListAsync( ct );

        // tag første pr medlem (i memory)
        var latestByMember = latestOrders
            .GroupBy( o => o.GroupPaymentMemberId )
            .ToDictionary( g => g.Key, g => g.First() );

        // 3) Projektér til DTO pr “MemberId” (DirectoryEntry id)
        var result = new List<MemberLatestOrderDto>( members.Count );

        foreach (var m in members)
        {
            if (!latestByMember.TryGetValue( m.Id, out var order ))
            {
                result.Add( new MemberLatestOrderDto( m.MemberId, null ) );
                continue;
            }

            var dto = new OrderDto(
                order.Id,
                order.GroupPaymentMemberId,
                order.MerchantId,
                order.MerchantName,
                order.MerchantLink,
                order.CreatedAtUtc,
                order.Lines
                    .OrderBy( l => l.Id )
                    .Select( l => new OrderLineDto( l.ItemName, l.Quantity, l.UnitPrice ) )
                    .ToList()
            );

            result.Add( new MemberLatestOrderDto( m.MemberId, dto ) );
        }

        return result;
    }
}