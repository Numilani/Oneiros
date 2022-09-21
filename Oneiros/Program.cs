using Microsoft.AspNetCore.ResponseCompression;
using Oneiros;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
// builder.Services.AddResponseCompression(opts =>
// {
//     opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
//         new[] { "application/octet-stream" });
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapHub<TestHub>("test");

app.Run();