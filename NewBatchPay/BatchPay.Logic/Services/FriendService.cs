using BatchPay.Contracts.Dto;
using BatchPay.Data;
using BatchPay.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BatchPay.Logic.Services;

public sealed class FriendService( BatchPayContext db ) : IFriendService
{
    private const byte Accepted = 1;

    // ✅ eksisterende: bruges stadig af steder der kun skal have "brugere" uden type
    public async Task<IReadOnlyList<UserDto>> GetFriendsAsync( int requesterId, CancellationToken ct )
    {
        return await db.FriendRequests
            .Where( fr => fr.RequesterId == requesterId && fr.Status == Accepted )
            .Select( fr => fr.Receiver )
            .OrderBy( e => e.DisplayName )
            .Select( e => new UserDto( e.Id, e.DisplayName, e.Handle ) )
            .ToListAsync( ct );
    }

    // ✅ NY: bruges når du skal kunne vælge merchant (type + evt. merchant-data)
    public async Task<IReadOnlyList<DirectoryEntryDto>> GetFriendsDirectoryAsync( int requesterId, CancellationToken ct )
    {
        return await db.FriendRequests
            .Where( fr => fr.RequesterId == requesterId && fr.Status == Accepted )
            .Select( fr => fr.Receiver )
            .OrderBy( e => e.DisplayName )
            .Select( e => new DirectoryEntryDto(
                e is UserEntity ? DirectoryEntryType.User : DirectoryEntryType.Merchant,
                e.Id,
                e.DisplayName,
                e.Handle,
                e is MerchantEntity ? ((MerchantEntity)e).Description : null,
                e is MerchantEntity ? ((MerchantEntity)e).LogoUrl : null
            ) )
            .ToListAsync( ct );
    }

    public async Task<bool> AddFriendAsync( AddFriendRequestDto dto, CancellationToken ct )
    {
        if (dto.RequesterId == dto.ReceiverId)
            return false;

        var requesterExists = await db.DirectoryEntries.AnyAsync( e => e.Id == dto.RequesterId, ct );
        var receiverExists = await db.DirectoryEntries.AnyAsync( e => e.Id == dto.ReceiverId, ct );

        if (!requesterExists || !receiverExists)
            return false;

        var exists = await db.FriendRequests.AnyAsync( fr =>
            fr.RequesterId == dto.RequesterId &&
            fr.ReceiverId == dto.ReceiverId, ct );

        if (exists)
            return true;

        db.FriendRequests.Add( new FriendRequestEntity
        {
            RequesterId = dto.RequesterId,
            ReceiverId = dto.ReceiverId,
            Status = Accepted,
            CreatedAt = DateTime.UtcNow
        } );

        await db.SaveChangesAsync( ct );
        return true;
    }
}