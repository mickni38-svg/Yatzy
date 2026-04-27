public sealed record CreateGroupPaymentRequestDto(
    int CreatedByUserId,
    int MerchantId,
    string Title,
    string? Message,
    string? IconKey,
    bool IsActive,
    DateTime CreatedAtUtc,
    List<int> MemberIds
);