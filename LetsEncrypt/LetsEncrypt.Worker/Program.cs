using LetsEncrypt.Worker;
using LetsEncrypt.Managers.DependencyInjection;
using LetsEncrypt.Worker.Processing;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddScoped<CertificateProcessor>();
builder.Services.AddHostedService<Worker>();
builder.Services.AddManagers();

var host = builder.Build();
host.Run();