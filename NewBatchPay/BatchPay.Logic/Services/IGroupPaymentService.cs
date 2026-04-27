using BatchPay.Contracts.Dto;

namespace BatchPay.Logic.Services;

public interface IGroupPaymentService
{
    Task<GroupPaymentDto> CreateAsync( CreateGroupPaymentRequestDto dto, CancellationToken ct );
    Task<IReadOnlyList<GroupPaymentDto>> GetForUserAsync( int userId, CancellationToken ct );

}
