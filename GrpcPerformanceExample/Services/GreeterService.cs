using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcPerformanceExample
{
	public class GreeterService : Greeter.GreeterBase
	{
		private readonly ILogger<GreeterService> _logger;
		public GreeterService(ILogger<GreeterService> logger)
		{
			_logger = logger;
		}

		public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
		{
			await Task.Delay(200).ConfigureAwait(false);
			return new HelloReply
			{
				Message = "Hello " + request.Name
			};
		}
	}
}
