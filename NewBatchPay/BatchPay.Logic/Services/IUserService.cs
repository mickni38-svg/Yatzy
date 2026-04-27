using BatchPay.Contracts.Dto;

namespace BatchPay.Logic.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetAllAsync( CancellationToken ct );
}
