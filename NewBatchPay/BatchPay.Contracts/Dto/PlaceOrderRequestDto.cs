namespace BatchPay.Contracts.Dto;

public record PlaceOrderRequestDto(
    int GroupPaymentId,
    int UserId,
    int MerchantId,
    string? MerchantName,
    string? MerchantLink,
    List<PlaceOrderLineDto> Lines
);

public record PlaceOrderLineDto(
    string ItemName,
    int Quantity,
    decimal UnitPrice
);