using LetsEncrypt.Managers.Interfaces;
using LetsEncrypt.Model;

namespace LetsEncrypt.Dashboard.Web.Pages;

public partial class Index
{
    private List<CertificateEntry> Elements = new List<CertificateEntry>();
    private bool _readOnly;
    private bool _isCellEditMode;
    private List<string> _events = new();
    private bool _editTriggerRowClick;

    protected string DomainName { get; set; }
    protected string Email { get; set; }
    protected string Organization { get; set; }
    protected string OrganizationUnit { get; set; }
    protected string Country { get; set; }
    protected string State { get; set; }
    protected string Locality { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var certificateEntryManager = ScopedServices.GetRequiredService<ICertificateEntryManager>();
        var certificates = await certificateEntryManager.GetCertificateEntries();

        Elements = certificates.ToList();
    }

    async Task OnCreateCertificateEntryCommand()
    {
        var entry = new CertificateEntry
        {
            DomainName = DomainName,
            Email = Email,
            Organization = Organization,
            OrganizationUnit = OrganizationUnit,
            CountryName = Country,
            State = State,
            Locality = Locality
        };

        var certificateEntryManager = ScopedServices.GetRequiredService<ICertificateEntryManager>();
        await certificateEntryManager.AddCertificateEntry(entry);

        var certificates = await certificateEntryManager.GetCertificateEntries();
        Elements = certificates.ToList();

        StateHasChanged();
    }

    void SelectedItemChanged(CertificateEntry item)
    {
        DomainName = item.DomainName;
        Email = item.Email;
        Organization = item.Organization;
        OrganizationUnit = item.OrganizationUnit;
        Country = item.CountryName;
        State = item.State;
        Locality = item.Locality;
    }

    private string ConvertRenewedOnToDescription(CertificateEntry item)
    {
        var result = "N.A.";

        if (item.RenewedOn.HasValue)
        {
            var numberOfDays = (int)Math.Round((DateTime.Now - item.RenewedOn).Value.TotalDays);
            result = $"{item.RenewedOn:MMMM d, yyyy} ({numberOfDays} {(numberOfDays == 1 ? "day" : "days")} ago)";
        }

        return result;
    }

    private string ConvertExpiresOnToDescription(CertificateEntry item)
    {
        var result = "N.A.";

        if (item.ExpiresOn.HasValue)
        {
            var numberOfDays = (int)Math.Round((item.ExpiresOn - DateTime.Now).Value.TotalDays);
            result = $"{item.ExpiresOn:MMMM d, yyyy} (in {numberOfDays} {(numberOfDays == 1 ? "day" : "days")})";
        }

        return result;
    }
}