using BatchPay.Contracts.Dto;
using BatchPay.Frontend.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BatchPay.Frontend.ViewModels;

public partial class OverviewViewModel : ObservableObject
{
    private readonly BatchPayApiClient _api;

    private readonly IUserContext _userContext;

    private int CurrentUserId => _userContext.CurrentUserId ?? 0;

    public ObservableCollection<GroupPaymentTabVm> Tabs { get; } = new();
    public ObservableCollection<GroupPaymentMemberVm> MemberAccordions { get; } = new();

    private GroupPaymentDto? _selectedGroupPayment;

    public GroupPaymentDto? SelectedGroupPayment
    {
        get => _selectedGroupPayment;
        private set
        {
            if (ReferenceEquals( _selectedGroupPayment, value ))
                return;

            _selectedGroupPayment = value;
            OnPropertyChanged();

            RebuildMembers();
            _ = LoadOrdersForSelectedAsync(); // ✅ hent orders efter rebuild
        }
    }

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string? statusMessage;

    public OverviewViewModel( BatchPayApiClient api, IUserContext userContext )
    {
        _api = api;
        _userContext = userContext;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        StatusMessage = null;

        try
        {
            var gps = await _api.GetGroupPaymentsForUserAsync( CurrentUserId, CancellationToken.None );

            MainThread.BeginInvokeOnMainThread( () =>
            {
                Tabs.Clear();

                foreach (var gp in gps)
                    Tabs.Add( new GroupPaymentTabVm( gp ) );

                for (int i = 0; i < Tabs.Count; i++)
                    Tabs[ i ].ShowDivider = i != Tabs.Count - 1;

                var first = Tabs.FirstOrDefault();
                if (first is not null)
                    SelectGroupPayment( first );
                else
                    ClearMembers();
            } );
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void SelectGroupPayment( GroupPaymentTabVm tab )
    {
        foreach (var t in Tabs)
            t.IsSelected = false;

        tab.IsSelected = true;
        SelectedGroupPayment = tab.Model;
    }

    [RelayCommand]
    private async Task DeleteGroupPaymentAsync( GroupPaymentTabVm tab )
    {
        if (tab?.Model is null)
            return;

        var ok = await Shell.Current.DisplayAlert(
            "Slet",
            $"Vil du slette \"{tab.Model.Title}\"?\n(Bliver kun deaktiveret)",
            "Slet",
            "Annuller" );

        if (!ok)
            return;

        try
        {
            IsBusy = true;
            StatusMessage = null;

            await _api.DeactivateGroupPaymentAsync( tab.Model.Id, CancellationToken.None );

            await LoadAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task PayOrderAsync()
    {
        if (SelectedGroupPayment is null)
            return;

        await Shell.Current.DisplayAlert( "Betal ordre", $"Valgt: {SelectedGroupPayment.Title}", "OK" );
    }

    private void RebuildMembers()
    {
        MemberAccordions.Clear();

        var members = SelectedGroupPayment?.Members;
        if (members is null || members.Count == 0)
            return;

        foreach (var m in members)
        {
            if (m.Type == DirectoryEntryType.Merchant)
                continue;

            // Hvis GroupPaymentMemberVm i dag forventer UserDto,
            // skal den opdateres til at tage DirectoryEntryDto i stedet.
            MemberAccordions.Add( new GroupPaymentMemberVm( m ) );
        }
    }

    private async Task LoadOrdersForSelectedAsync()
    {
        var gp = SelectedGroupPayment;
        if (gp is null)
            return;

        try
        {
            // Henter seneste ordre pr medlem
            var latest = await _api.GetLatestOrdersForGroupPaymentAsync( gp.Id, CancellationToken.None );

            // map key = memberId/userId
            var map = latest.ToDictionary( x => x.MemberId, x => x.LatestOrder );

            // apply til hver member-vm
            foreach (var m in MemberAccordions)
            {
                if (m.IsMerchant)
                    continue;

                map.TryGetValue( m.UserId, out var order );
                    m.ApplyLatestOrder( order );
            }
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private void ClearMembers()
    {
        SelectedGroupPayment = null;
        MemberAccordions.Clear();
    }
}
