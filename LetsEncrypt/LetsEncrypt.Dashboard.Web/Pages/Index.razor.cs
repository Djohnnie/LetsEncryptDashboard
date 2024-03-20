using LetsEncrypt.Managers.Interfaces;
using LetsEncrypt.Model;

namespace LetsEncrypt.Dashboard.Web.Pages;

public partial class Index
{
    private IEnumerable<CertificateEntry> Elements = new List<CertificateEntry>();
    private bool _readOnly;
    private bool _isCellEditMode;
    private List<string> _events = new();
    private bool _editTriggerRowClick;

    protected override async Task OnInitializedAsync()
    {
        var certificateEntryManager = ScopedServices.GetRequiredService<ICertificateEntryManager>();
        var certificates = await certificateEntryManager.GetCertificateEntries();

        Elements = certificates;
    }

    // events
    void StartedEditingItem(CertificateEntry item)
    {
        _events.Insert(0, $"Event = StartedEditingItem, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
    }

    void CanceledEditingItem(CertificateEntry item)
    {
        _events.Insert(0, $"Event = CanceledEditingItem, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
    }

    void CommittedItemChanges(CertificateEntry item)
    {
        _events.Insert(0, $"Event = CommittedItemChanges, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
    }
}