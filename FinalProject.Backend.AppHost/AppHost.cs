var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.FinalProject_Backend>("finalproject-backend");

builder.Build().Run();
