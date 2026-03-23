using LetsEncrypt.Dashboard.Web.Shared;
using LetsEncrypt.Managers.Interfaces;
using LetsEncrypt.Model;
using MudBlazor;

namespace LetsEncrypt.Dashboard.Web.Pages;

public partial class Index
{
    private List<CertificateEntry> Elements = new();
    private bool _isLoading = true;

    private int _healthyCount => Elements.Count(e => e.LastError == null && e.ExpiresOn.HasValue && (e.ExpiresOn.Value - DateTime.Now).TotalDays > 30);
    private int _expiringSoonCount => Elements.Count(e => e.ExpiresOn.HasValue && (e.ExpiresOn.Value - DateTime.Now).TotalDays is > 0 and <= 30);
    private int _errorCount => Elements.Count(e => e.LastError != null);

    protected override async Task OnInitializedAsync()
    {
        await LoadCertificates();
    }

    private async Task LoadCertificates()
    {
        _isLoading = true;

        try
        {
            var manager = ScopedServices.GetRequiredService<ICertificateEntryManager>();
            var certificates = await manager.GetCertificateEntries();
            Elements = certificates.ToList();
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task OpenAddDialog()
    {
        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            CloseOnEscapeKey = true
        };

        var dialog = await DialogService.ShowAsync<AddCertificateDialog>("Add New Certificate", options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: CertificateEntry entry })
        {
            var manager = ScopedServices.GetRequiredService<ICertificateEntryManager>();
            await manager.AddCertificateEntry(entry);
            await LoadCertificates();
            Snackbar.Add($"Certificate for '{entry.DomainName}' added successfully!", Severity.Success);
            StateHasChanged();
        }
    }

    private async Task ConfirmDelete(CertificateEntry item)
    {
        var parameters = new DialogParameters<ConfirmDeleteDialog>
        {
            { x => x.ContentText, $"Are you sure you want to remove the certificate for '{item.DomainName}'? This action cannot be undone." },
            { x => x.ButtonText, "Delete" },
            { x => x.Color, Color.Error }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small };
        var dialog = await DialogService.ShowAsync<ConfirmDeleteDialog>("Confirm Deletion", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false })
        {
            var manager = ScopedServices.GetRequiredService<ICertificateEntryManager>();
            await manager.RemoveCertificateEntry(item);
            await LoadCertificates();
            Snackbar.Add($"Certificate for '{item.DomainName}' has been removed.", Severity.Info);
            StateHasChanged();
        }
    }

    private (string Label, Color Color) GetStatus(CertificateEntry item)
    {
        if (item.LastError != null)
            return ("Error", Color.Error);

        if (!item.ExpiresOn.HasValue)
            return ("Pending", Color.Default);

        var daysLeft = (item.ExpiresOn.Value - DateTime.Now).TotalDays;

        if (daysLeft <= 0)
            return ("Expired", Color.Error);
        if (daysLeft <= 30)
            return ("Expiring", Color.Warning);

        return ("Valid", Color.Success);
    }

    private (string Text, Color Color) GetExpiryInfo(CertificateEntry item)
    {
        if (!item.ExpiresOn.HasValue)
            return ("Not yet issued", Color.Default);

        var days = (int)Math.Round((item.ExpiresOn.Value - DateTime.Now).TotalDays);

        if (days <= 0)
            return ($"{item.ExpiresOn:MMM d, yyyy} (expired)", Color.Error);
        if (days <= 30)
            return ($"{item.ExpiresOn:MMM d, yyyy} (in {days} {(days == 1 ? "day" : "days")})", Color.Warning);

        return ($"{item.ExpiresOn:MMM d, yyyy} (in {days} {(days == 1 ? "day" : "days")})", Color.Success);
    }

    private string FormatRenewedOn(CertificateEntry item)
    {
        if (!item.RenewedOn.HasValue)
            return "Never";

        var days = (int)Math.Round((DateTime.Now - item.RenewedOn.Value).TotalDays);
        return $"{item.RenewedOn:MMM d, yyyy} ({days} {(days == 1 ? "day" : "days")} ago)";
    }
}