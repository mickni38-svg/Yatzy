namespace BatchPay.Contracts.Dto;  

public sealed record GroupPaymentDto(
    int Id,
    string Title,
    string? Message,
    int CreatedByUserId,
    DateTime CreatedAtUtc,
    string IconKey,
    int MerchantId,
    string MerchantDisplayName,
    IReadOnlyList<DirectoryEntryDto> Members
);