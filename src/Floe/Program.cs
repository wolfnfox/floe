using Floe.Configuration;
using Floe.Models.Oci;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

// Configuration
builder.Services.Configure<FloeOptions>(
    builder.Configuration.GetSection(FloeOptions.SectionName));

builder.AddServiceDefaults();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

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
app.MapGet("/v2/", (HttpContext ctx) =>
{
    ctx.Response.Headers["Docker-Distribution-API-Version"] = "registry/2.0";
    return Results.Ok(new OK());
})
.Produces(200)
.WithName("ApiVersionCheck");

// TODO: Add other endpoint groups

app.Run();

// TODO: Potentially move to a separate file
public record OK();

[JsonSerializable(typeof(OK))]
[JsonSerializable(typeof(OciDescriptor))]
[JsonSerializable(typeof(OciError))]
[JsonSerializable(typeof(OciErrorResponse))]
[JsonSerializable(typeof(OciImageIndex))]
[JsonSerializable(typeof(OciManifest))]
[JsonSerializable(typeof(OciPlatform))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
