using System;
using System.Collections.Generic;

namespace BatchPay.Contracts.Dto;

public record OrderLineDto(
    string ItemName,
    int Quantity,
    decimal UnitPrice
);

public record OrderDto(
    int Id,
    int GroupPaymentMemberId,
    int? MerchantId,
    string? MerchantName,
    string? MerchantLink,
    DateTime CreatedAtUtc,
    IReadOnlyList<OrderLineDto> Lines
);

// Return-type til Overview: seneste ordre pr. medlem i en groupPayment
public record MemberLatestOrderDto(
    int MemberId,
    OrderDto? LatestOrder
);