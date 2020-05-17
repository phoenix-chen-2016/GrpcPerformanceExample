using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TestClient
{
	class TurnAroundSocketsHttpHandler : HttpMessageHandler
	{
		private HttpMessageInvoker[]? m_Handlers;
		private int m_HandlerIndex = 0;
		private object m_Locker = new object();
		private object m_InitLocker = new object();

		public int Connections { get; set; } = 10;

		private IEnumerable<HttpMessageInvoker> GenerateHttpMessageHandler()
		{
			return Enumerable.Range(0, Connections)
				.Select(_ => new HttpMessageInvoker(new SocketsHttpHandler()));
		}

		private HttpMessageInvoker GetHttpMessageInvoker()
		{
			var handlers = GetAllHandlers();

			lock (m_Locker)
			{
				var handler = handlers[m_HandlerIndex++];

				m_HandlerIndex %= handlers.Length;

				return handler;
			}
		}

		private HttpMessageInvoker[] GetAllHandlers()
		{
			if (m_Handlers == null)
				lock (m_InitLocker)
					if (m_Handlers == null)
						m_Handlers = GenerateHttpMessageHandler().ToArray();

			return m_Handlers;
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var handler = GetHttpMessageInvoker();

			return handler.SendAsync(request, cancellationToken);
		}
	}
}
