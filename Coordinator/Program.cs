using Coordinator.Models.Contexts;
using Coordinator.Services.Abstractions;
using Coordinator.Services.Concrete;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TwoPhaseCommitContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));


builder.Services.AddHttpClient("Order.API", client => client.BaseAddress = new("https://localhost:7238/"));
builder.Services.AddHttpClient("Stock.API", client => client.BaseAddress = new("https://localhost:7125/"));
builder.Services.AddHttpClient("Payment.API", client => client.BaseAddress = new("https://localhost:7184/"));

builder.Services.AddTransient<ITransactionService, TransactionService>();

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

app.MapGet("/create-order-transaction", async (ITransactionService transactionService) =>
{
    //Phase 1 Prepare
    var transactionId = await transactionService.CreateTransactionAsync();
    await transactionService.PreapareServicesAsync(transactionId);
    bool transactionState = await transactionService.CheckReadyServicesAsync(transactionId);
    if (transactionState)
    {
        //Phase 2 Commit
        await transactionService.CommitAsync(transactionId);
        transactionState = await transactionService.CheckTransactionStateServicesAsync(transactionId);
    }
    if (!transactionState)
    {
        await transactionService.RollBackAsync(transactionId);
    }

});


app.Run();

