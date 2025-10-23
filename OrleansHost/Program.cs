using Microsoft.Extensions.Hosting;
using Orleans.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.UseOrleans(siloBuilder =>
{
  siloBuilder.Configure<GrainCollectionOptions>(options => options.CollectionQuantum = TimeSpan.FromSeconds(5));

  siloBuilder.UseLocalhostClustering();
  siloBuilder.AddMemoryGrainStorageAsDefault();
});

using var host = builder.Build();
host.Start();

Console.WriteLine("Orleans host started. Press Enter to terminate...");
Console.ReadLine();
