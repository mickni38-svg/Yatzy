using BatchPay.Contracts.Dto;
using System.Net.Http.Json;

namespace BatchPay.Frontend.Services;

public sealed class BatchPayApiClient( HttpClient http )
{
    public Task<IReadOnlyList<NotificationDto>> GetNotificationsForUserAsync( int userId, CancellationToken ct )
    => http.GetFromJsonAsync<IReadOnlyList<NotificationDto>>( $"api/notifications/for-user/{userId}", ct );

    public async Task<List<UserDto>> GetAllUsersAsync( CancellationToken ct )
        => await http.GetFromJsonAsync<List<UserDto>>( "/api/users", ct ) ?? new();

    public async Task<List<DirectoryEntryDto>> GetDirectoryAsync( CancellationToken ct )
    => await http.GetFromJsonAsync<List<DirectoryEntryDto>>( "/api/directory", ct ) ?? new();

    public async Task<List<UserDto>> GetFriendsAsync( int requesterId, CancellationToken ct )
        => await http.GetFromJsonAsync<List<UserDto>>( $"/api/friends/{requesterId}", ct ) ?? new();

    public async Task<bool> AddFriendAsync( int requesterId, int receiverId, CancellationToken ct )
    {
        var res = await http.PostAsJsonAsync(
            "api/friends",
            new AddFriendRequestDto( requesterId, receiverId ),
            ct );

        return res.IsSuccessStatusCode;
    }

    public async Task<IReadOnlyList<MemberLatestOrderDto>> GetLatestOrdersForGroupPaymentAsync( int groupPaymentId, CancellationToken ct )
    {
        return await http.GetFromJsonAsync<List<MemberLatestOrderDto>>(
            $"api/orders/group/{groupPaymentId}/latest", ct
        ) ?? new List<MemberLatestOrderDto>();
    }
    public async Task<GroupPaymentDto> CreateGroupPaymentAsync( CreateGroupPaymentRequestDto dto, CancellationToken ct )
    {
        var res = await http.PostAsJsonAsync( "/api/group-payments", dto, ct );
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<GroupPaymentDto>( cancellationToken: ct ))!;
    }

    public async Task<IReadOnlyList<GroupPaymentDto>> GetGroupPaymentsForUserAsync( int userId, CancellationToken ct )
    {
        var res = await http.GetAsync( $"/api/group-payments/for-user/{userId}", ct );
        var raw = await res.Content.ReadAsStringAsync( ct );

        if (!res.IsSuccessStatusCode)
            throw new Exception( $"HTTP {(int)res.StatusCode} {res.ReasonPhrase}\n{raw}" );

        return (await res.Content.ReadFromJsonAsync<List<GroupPaymentDto>>( cancellationToken: ct )) ?? new();
    }

    public async Task DeactivateGroupPaymentAsync( int groupPaymentId, CancellationToken ct )
    {
        var res = await http.DeleteAsync( $"/api/group-payments/{groupPaymentId}", ct );
        var raw = await res.Content.ReadAsStringAsync( ct );

        if (!res.IsSuccessStatusCode)
            throw new Exception( $"HTTP {(int)res.StatusCode} {res.ReasonPhrase}\n{raw}" );
    }
}
