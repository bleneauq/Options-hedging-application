using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcBacktestServer;
using System.Text.Json;
using System.Xml;
using Hedger;
using GrpcBacktestServer.utils;
using PricingLibrary.DataClasses;
using PricingLibrary.MarketDataFeed;
using System.Collections.Generic;

namespace GrpcBacktestServer.Services
{
    public class BacktestRunnerService : BacktestRunner.BacktestRunnerBase
    {
        private readonly ILogger<BacktestRunnerService> _logger;
        public BacktestRunnerService(ILogger<BacktestRunnerService> logger)
        {
            _logger = logger;
        }

        public override Task<BacktestOutput> RunBacktest(BacktestRequest request, ServerCallContext context)
        {
            BasketTestParameters basketSample = inputProtoConverter.convertProtoToBasket(request);

            List<DataFeed> dataFeedList = inputProtoConverter.convertProtoToDataFeed(request);

            List<OutputData> outputData = Strategy.getOuputData(dataFeedList, basketSample);

            BacktestOutput finalOutput = outputProtoConverter.convertOutputDataToProto(outputData);

            return Task.FromResult(finalOutput);


        }
    }
}