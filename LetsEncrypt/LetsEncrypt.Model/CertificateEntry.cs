using System;
using LetsEncrypt.Model.Base;

namespace LetsEncrypt.Model
{
    public class CertificateEntry : ModelBase
    {
        public string CountryName { get; set; }
        public string State { get; set; }
        public string Locality { get; set; }
        public string Organization { get; set; }
        public string OrganizationUnit { get; set; }
        public string DomainName { get; set; }
        public bool IsStaging { get; set; }
        public DateTime? RenewedOn { get; set; }
        public DateTime? ExpiresOn { get; set; }
    }
}