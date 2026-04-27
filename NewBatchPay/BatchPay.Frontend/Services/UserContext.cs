using Microsoft.Maui.Storage;

namespace BatchPay.Frontend.Services;

public class UserContext : IUserContext
{
    private const string Key = "CurrentUserId";

    public int? CurrentUserId
    {
        get
        {
            var value = Preferences.Get( Key, 0 );
            return value <= 0 ? null : value;
        }
        set
        {
            if (value is null || value <= 0)
                Preferences.Remove( Key );
            else
                Preferences.Set( Key, value.Value );
        }
    }
}