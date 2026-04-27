using BatchPay.Contracts.Dto;
using BatchPay.Data;
using BatchPay.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BatchPay.Logic.Services;

public sealed class GroupPaymentService( BatchPayContext db ) : IGroupPaymentService
{
    public async Task<GroupPaymentDto> CreateAsync( CreateGroupPaymentRequestDto dto, CancellationToken ct )
    {
        var title = (dto.Title ?? "").Trim();
        if (title.Length == 0) throw new ArgumentException( "Title is required" );

        // ✅ Merchant skal findes (og være aktiv)
        var merchant = await db.DirectoryEntries
            .OfType<MerchantEntity>()
            .FirstOrDefaultAsync( m => m.Id == dto.MerchantId && m.IsActive, ct );

        if (merchant is null)
            throw new ArgumentException( "Merchant does not exist" );


        // ✅ Medlemmer + sørg for creator altid er med
        var memberIds = dto.MemberIds.Distinct().ToList();
        if (!memberIds.Contains( dto.CreatedByUserId ))
            memberIds.Insert( 0, dto.CreatedByUserId );

        // Valider at alle userIds findes
        var count = await db.DirectoryEntries.CountAsync( u => memberIds.Contains( u.Id ), ct );
        if (count != memberIds.Count)
            throw new ArgumentException( "One or more users do not exist" );

        var entity = new GroupPaymentEntity
        {
            Title = title,
            Message = string.IsNullOrWhiteSpace( dto.Message ) ? null : dto.Message.Trim(),
            CreatedByUserId = dto.CreatedByUserId,
            MerchantId = dto.MerchantId, // ✅ NYT
            IconKey = dto.IconKey ?? string.Empty,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            Members = memberIds.Select( id => new GroupPaymentMemberEntity
            {
                MemberId = id,
                IsActive = true
            } ).ToList()
        };

        db.GroupPayments.Add( entity );
        await db.SaveChangesAsync( ct );

        // ✅ Notifikationer (inkl opretter)
        // Linket kan også indeholde: userId, merchantId, source, returnUrl, locale...
        var baseUrl = (merchant.WebsiteUrl ?? "").Trim();
        if (baseUrl.Length == 0)
        {
            // Midlertidig fallback til din test-web
            baseUrl = "https://localhost:5173/order";
        }

        foreach (var toUserId in memberIds)
        {
            var link =
                $"{baseUrl}?groupPaymentId={entity.Id}&userId={toUserId}&merchantId={merchant.Id}&source=sbys";

            db.Notifications.Add( new NotificationEntity
            {
                ToUserId = toUserId,
                Type = "GroupPaymentCreated",
                Title = "Ny gruppebetaling",
                Body = $"\"{entity.Title}\" er oprettet. Klik for at bestille hos {merchant.DisplayName}.",
                LinkUrl = link,
                GroupPaymentId = entity.Id,
                IsRead = false,
                CreatedAtUtc = DateTime.UtcNow
            } );
        }

        await db.SaveChangesAsync( ct );

        // DTO retur (samme som før)
        var members = await db.GroupPaymentMembers
            .Where( m => m.GroupPaymentId == entity.Id && m.IsActive )
            .Join( db.DirectoryEntries, m => m.MemberId, u => u.Id, ( _, u ) => u )
            .OrderBy( u => u.DisplayName )
            .Select( u => new DirectoryEntryDto( u is MerchantEntity ? DirectoryEntryType.Merchant : DirectoryEntryType.User, u.Id, u.DisplayName, u.Handle ) )
            .ToListAsync( ct );

        return new GroupPaymentDto(
            entity.Id,
            entity.Title,
            entity.Message,
            entity.CreatedByUserId,
            entity.CreatedAtUtc,
            entity.IconKey ?? string.Empty,
            merchant.Id,
            merchant.DisplayName, // ✅ NYT - midlertidigt, da vi ikke har et felt til merchant display name. Vi kan evt. tilføje det senere, eller hente det via en join.
            members );
    }

    public async Task<IReadOnlyList<GroupPaymentDto>> GetForUserAsync( int userId, CancellationToken ct )
    {
        var groupIds = await db.GroupPaymentMembers
            .Where( m => m.MemberId == userId && m.IsActive )
            .Select( m => m.GroupPaymentId )
            .Distinct()
            .ToListAsync( ct );

        if (groupIds.Count == 0)
            return [];

        var groups = await db.GroupPayments
            .Where( g => groupIds.Contains( g.Id ) && EF.Property<bool>( g, "IsActive" ) )
            .OrderByDescending( g => g.CreatedAtUtc )
            .ToListAsync( ct );

        var members = await db.GroupPaymentMembers
            .Where( m => groupIds.Contains( m.GroupPaymentId ) && m.IsActive )
            .Join(
                db.DirectoryEntries,
                m => m.MemberId,
                u => u.Id,
                ( m, u ) => new { m.GroupPaymentId, User = u } )
            .ToListAsync( ct );

        var x = groups.Select( g =>
            new GroupPaymentDto(
                g.Id,
                g.Title,
                g.Message,
                g.CreatedByUserId,
                g.CreatedAtUtc,
                g.IconKey ?? string.Empty,
                g.MerchantId,
                g.Merchant!.DisplayName, // Vi kan evt. optimere dette senere ved at inkludere merchant info i det store join ovenfor
                members
                    .Where( x => x.GroupPaymentId == g.Id )
                    .OrderBy( x => x.User.DisplayName )
                    .Select( x => new DirectoryEntryDto( x.User is MerchantEntity ? DirectoryEntryType.Merchant : DirectoryEntryType.User, x.User.Id, x.User.DisplayName, x.User.Handle ) )
                    .ToList()
            )
        ).ToList();

        return x;
    }

    public async Task<IReadOnlyList<GroupPaymentMemberDto>> GetMembersAsync( int groupPaymentId, CancellationToken ct )
    {
        var userMembers = db.GroupPaymentMembers
            .Where( gpm => gpm.GroupPaymentId == groupPaymentId )
            .Join(
                db.DirectoryEntries,
                gpm => gpm.MemberId,
                u => u.Id,
                ( gpm, u ) => new
                {
                    MemberId = u.Id,
                    DisplayName = u.DisplayName,
                    Handle = u.Handle,
                    MemberType = DirectoryEntryType.User
                } );

        return await userMembers
            .OrderBy( x => x.DisplayName )
            .Select( x => new GroupPaymentMemberDto( x.MemberId, x.DisplayName, x.Handle, x.MemberType ) )
            .ToListAsync( ct );
    }
}