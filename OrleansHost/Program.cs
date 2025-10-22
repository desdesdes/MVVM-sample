using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.UseOrleans(siloBuilder => siloBuilder.UseLocalhostClustering());

using var host = builder.Build();
host.Start();

Console.WriteLine("Orleans host started. Press Enter to terminate...");
Console.ReadLine();
