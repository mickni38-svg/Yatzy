using BatchPay.Contracts.Dto;

namespace BatchPay.Logic.Services;

public interface IOrderService
{
    Task<int> PlaceAsync( PlaceOrderRequestDto dto, CancellationToken ct );

    // ✅ NEW: til Overview – seneste ordre pr medlem for en groupPayment
    Task<IReadOnlyList<MemberLatestOrderDto>> GetLatestForGroupPaymentAsync( int groupPaymentId, CancellationToken ct );
}