namespace BatchPay.Frontend.Services;

public interface IUserContext
{
    int? CurrentUserId { get; set; }
}