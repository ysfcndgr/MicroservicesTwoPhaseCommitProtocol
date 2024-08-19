using System;
namespace Coordinator.Services.Abstractions
{
	public interface ITransactionService
	{
		Task<Guid> CreateTransactionAsync();
		Task PreapareServicesAsync(Guid transactionId);
		Task<bool> CheckReadyServicesAsync(Guid transactionId);
		Task CommitAsync(Guid transactionId);
		Task<bool> CheckTransactionStateServicesAsync(Guid transactionId);
		Task RollBackAsync(Guid transactionId);
	}
}

