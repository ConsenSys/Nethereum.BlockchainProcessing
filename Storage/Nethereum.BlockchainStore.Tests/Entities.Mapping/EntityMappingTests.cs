﻿using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using System.Numerics;
using Xunit;

namespace Nethereum.BlockchainStore.Tests.Entities.Mapping
{
    public class EntityMappingTests
    {
        [Event("Transfer")]
        public class TransferEventDto : IEventDTO
        {
            [Parameter("address", "_from", 1, true)]
            public string From { get; set; }

            [Parameter("address", "_to", 2, true)]
            public string To { get; set; }

            [Parameter("uint256", "_value", 3, false)]
            public BigInteger Value { get; set; }
        }

        public class TransactionLogView : ITransactionLogView
        {
            public string Address {get;set; }

            public string Data { get; set; }

            public string EventHash { get; set; }

            public string IndexVal1 { get; set; }

            public string IndexVal2 { get; set; }

            public string IndexVal3 { get; set; }

            public long LogIndex { get; set; }

            public string TransactionHash { get; set; }
        }


        /// <summary>
        /// Demonstrates rehydrating an EventLog<T> from and instance of ITransactionLogView
        /// Not all fields of an event log can be populated as ITransactionLogView doesn't have all of them
        /// However the core fields can be rehydrated
        /// </summary>
        [Fact]
        public void ITransactionLogView_To_FilterLog_To_EventLog()
        {
            var sourceLog = new RPC.Eth.DTOs.FilterLog{ 
                Address = "0x5f236f062f16a9b19819c535127398df9a01d762", 
                TransactionHash = "0x4e80be130e453015a9e82fa2964c1ebe6cb53d058eb7d79e847e699eee0f2e79", 
                LogIndex = new HexBigInteger("0x76"), 
                Data = "0x0000000000000000000000000000000000000000000000261d4b3f127faa5000",
                Topics = new object[]{ "0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef", "0x000000000000000000000000abadc2947fea9e5649827093ae2e4b8645cb6f18", "0x000000000000000000000000e6458f9552833b4f8cb00509390fbb2dc75c3e17" }
            };

            var sourceTransferEventLog = sourceLog.DecodeEvent<TransferEventDto>();

            var transactionFromRepo = new TransactionLogView
            {
                TransactionHash = sourceTransferEventLog.Log.TransactionHash,
                LogIndex = (long)sourceTransferEventLog.Log.LogIndex.Value,
                Address = sourceTransferEventLog.Log.Address,
                Data = sourceTransferEventLog.Log.Data,
                EventHash = (string)sourceTransferEventLog.Log.Topics[0],
                IndexVal1 = (string)sourceTransferEventLog.Log.Topics[1],
                IndexVal2 = (string)sourceTransferEventLog.Log.Topics[2]
            };

            var rehydratedFilterLog = transactionFromRepo.ToFilterLog();
            var rehyrdratedTransferEventLog = rehydratedFilterLog.DecodeEvent<TransferEventDto>();
            
            Assert.Equal(sourceTransferEventLog.Log.TransactionHash, rehyrdratedTransferEventLog.Log.TransactionHash);
            Assert.Equal(sourceTransferEventLog.Log.LogIndex.Value, rehyrdratedTransferEventLog.Log.LogIndex.Value);
            Assert.Equal(sourceTransferEventLog.Log.Address, rehyrdratedTransferEventLog.Log.Address);
            Assert.Equal(sourceTransferEventLog.Log.Data, rehyrdratedTransferEventLog.Log.Data);
            Assert.Equal(sourceTransferEventLog.Event.From, rehyrdratedTransferEventLog.Event.From);
            Assert.Equal(sourceTransferEventLog.Event.To, rehyrdratedTransferEventLog.Event.To);
            Assert.Equal(sourceTransferEventLog.Event.Value, rehyrdratedTransferEventLog.Event.Value);

        }
    }
}
