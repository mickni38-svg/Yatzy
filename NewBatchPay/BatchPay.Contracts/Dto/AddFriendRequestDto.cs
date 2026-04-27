namespace BatchPay.Contracts.Dto;

// MODIFIED: Renamed properties to match the new generic model.
public sealed record AddFriendRequestDto(int RequesterId, int ReceiverId);