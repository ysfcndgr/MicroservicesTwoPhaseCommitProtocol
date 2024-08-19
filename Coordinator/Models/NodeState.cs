using System;
using Coordinator.Enums;

namespace Coordinator.Models
{
	public record NodeState(Guid TransactionId)
	{
		public Guid Id { get; set; }
		/// <summary>
		/// 1. aşamanın durumunu ifade ediyor.
		/// </summary>
		public ReadyType IsReady { get; set; }
		/// <summary>
		/// 2. aşamanın neticesinde işlemin başarılı tamamlanıp tamamlanmadıgını kontrol ediyor.
		/// </summary>
		public TransactionState TransactionState { get; set; }
		public Node Node { get; set; }
	}
}