using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcPerformanceExample;

namespace TestClient
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var channel = GrpcChannel.ForAddress("https://localhost:5001/");
			var client = new Greeter.GreeterClient(channel);

			var sw = new Stopwatch();

			sw.Start();
			var tasks = Enumerable.Range(0, 10000)
				.Select(_ =>
				{
					return client.SayHelloAsync(new HelloRequest()
					{
						Name = "Tester"
					}).ResponseAsync;
				}).ToArray();

			await Task.WhenAll(tasks).ConfigureAwait(false);
			sw.Stop();

			Console.WriteLine(sw.ElapsedMilliseconds);
			Console.WriteLine($"Requests per seconds: {10000D * 1000 / sw.ElapsedMilliseconds:N0}");
			Console.WriteLine("Completed");
		}
	}
}
