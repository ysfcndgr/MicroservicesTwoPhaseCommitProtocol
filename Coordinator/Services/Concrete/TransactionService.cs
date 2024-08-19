using System;
using Coordinator.Models;
using Coordinator.Models.Contexts;
using Coordinator.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Coordinator.Services.Concrete
{
    public class TransactionService : ITransactionService
    {
        TwoPhaseCommitContext _context;
        IHttpClientFactory _clientFactor;

        HttpClient _orderHttpClient;
        HttpClient _stockHttpClient;
        HttpClient _paymentHttpClient;

        public TransactionService(TwoPhaseCommitContext context, IHttpClientFactory clientFactor)
        {
            _context = context;
            _clientFactor = clientFactor;
            _orderHttpClient = _clientFactor.CreateClient("Order.API");
            _stockHttpClient = _clientFactor.CreateClient("Stock.API"); ;
            _paymentHttpClient = _clientFactor.CreateClient("Payment.API"); ;

        }


        public async Task<Guid> CreateTransactionAsync()
        {
            Guid transactionId = Guid.NewGuid();
            var nodes = await _context.Nodes.ToListAsync();
            nodes.ForEach( node => node.NodeStates = new List<NodeState>()
            {
                new(transactionId)
                {
                    IsReady = Enums.ReadyType.Pending,
                    TransactionState = Enums.TransactionState.Pending
                }
            });

            await _context.SaveChangesAsync();

            return transactionId;
        }

        public async Task PreapareServicesAsync(Guid transactionId)
        {
           var transactionNodes = await _context.NodeStates
                .Include(ns => ns.Node)
                .Where(ns => ns.TransactionId == transactionId)
                .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    var response = await (transactionNode.Node.Name switch
                    {
                        "Order.API" => _orderHttpClient.GetAsync("ready"),
                        "Stock.API" => _stockHttpClient.GetAsync("ready"),
                        "Payment.API" => _paymentHttpClient.GetAsync("ready")
                    });

                    var result = bool.Parse(await response.Content.ReadAsStringAsync());
                    transactionNode.IsReady = result ? Enums.ReadyType.Ready : Enums.ReadyType.Unready;

                }

                catch(Exception)
                {
                    transactionNode.IsReady = Enums.ReadyType.Unready;
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckReadyServicesAsync(Guid transactionId)
        {
            return (await _context.NodeStates
                .Where(x => x.TransactionId == transactionId)
                .ToListAsync()).TrueForAll(ns => ns.IsReady == Enums.ReadyType.Ready);
        }

        public async Task CommitAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates.Include(x => x.Node)
                .Where(x => x.TransactionId == transactionId)
                .ToListAsync();

            foreach (var transacitonNode in transactionNodes)
            {

                try
                {
                    var response = await (transacitonNode.Node.Name switch
                    {
                        "Order.API" => _orderHttpClient.GetAsync("commit"),
                        "Stock.API" => _stockHttpClient.GetAsync("commit"),
                        "Payment.API" => _paymentHttpClient.GetAsync("commit")
                    });

                    var result = bool.Parse(await response.Content.ReadAsStringAsync());
                    transacitonNode.TransactionState = result ? Enums.TransactionState.Done : Enums.TransactionState.Abort;
                }
                catch
                {
                    transacitonNode.TransactionState = Enums.TransactionState.Abort;

                }
            }
            await _context.SaveChangesAsync();

        }

        public async Task<bool> CheckTransactionStateServicesAsync(Guid transactionId)
        {
          return  (await _context.NodeStates.Where(x => x.TransactionId == transactionId)
                .ToListAsync()).TrueForAll(x=>x.TransactionState == Enums.TransactionState.Done);
        }

        public async Task RollBackAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates
                .Where(x => x.TransactionId == transactionId)
                .Include(x => x.Node)
                .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    if(transactionNode.TransactionState == Enums.TransactionState.Done)
                    {
                       _= await (transactionNode.Node.Name switch
                        {
                            "Order.API" => _orderHttpClient.GetAsync("rollback"),
                            "Stock.API" => _stockHttpClient.GetAsync("rollback"),
                            "Payment.API" => _paymentHttpClient.GetAsync("rollback")
                        });

                        transactionNode.TransactionState = Enums.TransactionState.Abort;

                    }
                }
                catch
                {
                    transactionNode.TransactionState = Enums.TransactionState.Abort;

                }
            }
            await _context.SaveChangesAsync();
        }
    }
}

