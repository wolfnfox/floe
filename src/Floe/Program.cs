using Floe.Configuration;
using Microsoft.AspNetCore.Http.HttpResults;
using System.IO;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

// Configuration
builder.Services.Configure<FloeOptions>(
    builder.Configuration.GetSection(FloeOptions.SectionName));

builder.AddServiceDefaults();

// N.B.: Example record and JsonSerializerContext for source generation
//builder.Services.ConfigureHttpJsonOptions(options =>
//{
//    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
//});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// TODO: Register services

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Base endpoint
app.MapGet("/v2/", () => Results.Ok(new { }))
    .Produces(200)
    .WithName("ApiVersionCheck");

// TODO: Add other endpoint groups

app.Run();


// N.B.: Example record and JsonSerializerContext for source generation

//public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

//[JsonSerializable(typeof(Todo[]))]
//internal partial class AppJsonSerializerContext : JsonSerializerContext
//{

//}
