using System;
using LetsEncrypt.Model.Base;

namespace LetsEncrypt.Model;

public class CertificateEntry : ModelBase
{
    public string Email { get; set; }
    public string CountryName { get; set; }
    public string State { get; set; }
    public string Locality { get; set; }
    public string Organization { get; set; }
    public string OrganizationUnit { get; set; }
    public string DomainName { get; set; }
    public bool IsStaging { get; set; }
    public string AccountPem { get; set; }
    public DateTime? RenewedOn { get; set; }
    public DateTime? ExpiresOn { get; set; }
    public string LastError { get; set; }
}