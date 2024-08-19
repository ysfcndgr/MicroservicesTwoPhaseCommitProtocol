﻿var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/ready", () => {
    Console.WriteLine("Payment Service is Ready");
    return true;

});

app.MapGet("/commit", () => {
    Console.WriteLine("Payment Service is commited");
    return true;

});

app.MapGet("/rollback", () => {
    Console.WriteLine("Payment Service is rollbacked");
});

app.Run();

