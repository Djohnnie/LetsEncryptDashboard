using LetsEncrypt.Worker;
using LetsEncrypt.Managers.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddManagers();

var host = builder.Build();
host.Run();