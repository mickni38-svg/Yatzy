using BatchPay.Contracts.Dto;
using System;

namespace BatchPay.Data.Entities;

public sealed class FriendRequestEntity
{    public int Id { get; set; }
    public byte Status { get; set; }
    public DateTime CreatedAt { get; set; }

    // MODIFIED: Foreign keys are now generic
    public int RequesterId { get; set; }
    public int ReceiverId { get; set; }

    public DirectoryEntryEntity Requester { get; set; } = null!;
    public DirectoryEntryEntity Receiver { get; set; } = null!;
}