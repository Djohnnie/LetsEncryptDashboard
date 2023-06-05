using System;
using LetsEncrypt.Model.Base;
using LetsEncrypt.Model.Enums;

namespace LetsEncrypt.Model;

public class LoggingEntry : ModelBase
{
    public LoggingLevel Level { get; set; }
    public string Message { get; set; }
    public DateTime DateTime { get; set; }
}