using BatchPay.Contracts.Dto;
using BatchPay.Data;
using BatchPay.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BatchPay.Logic.Services;

public sealed class UserService( BatchPayContext db ) : IUserService
{
    public async Task<IReadOnlyList<UserDto>> GetAllAsync( CancellationToken ct )
        => await db.Set<UserEntity>()
            .OrderBy( x => x.DisplayName )
            .Select( x => new UserDto( x.Id, x.DisplayName, x.Handle ) )
            .ToListAsync( ct );
}
