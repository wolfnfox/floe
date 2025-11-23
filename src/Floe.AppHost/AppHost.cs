var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Floe>("floe");

builder.Build().Run();
