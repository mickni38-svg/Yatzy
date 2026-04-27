using System;

namespace BatchPay.Data.Entities;

public partial class GroupPaymentMemberEntityMemberType
{
    /// <summary>
    /// Directory entry type for the group payment member.
    /// 0 = User, 1 = Merchant.
    /// Mapped by EF Core by convention to column "MemberType".
    /// </summary>
   // public byte MemberType { get; set; }
}