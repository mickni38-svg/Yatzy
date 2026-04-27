using BatchPay.Contracts.Dto;
using BatchPay.Data;
using BatchPay.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BatchPay.Logic.Services;

public interface IDirectoryService
{
    Task<IReadOnlyList<DirectoryEntryDto>> GetDirectoryAsync(CancellationToken ct);
}

public class DirectoryService : IDirectoryService
{
    private readonly BatchPayContext _db;

    public DirectoryService(BatchPayContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<DirectoryEntryDto>> GetDirectoryAsync(CancellationToken ct)
    {
        return await _db.DirectoryEntries
            .Where(u=>u.IsActive)
            .OrderBy(e => e.DisplayName)
            .Select(e => new DirectoryEntryDto(
                e is UserEntity ? DirectoryEntryType.User : DirectoryEntryType.Merchant,
                e.Id,
                e.DisplayName,
                e.Handle,
                e is MerchantEntity ? ((MerchantEntity)e).Description : null,
                e is MerchantEntity ? ((MerchantEntity)e).LogoUrl : null
            ))
            .ToListAsync(ct);
    }
}