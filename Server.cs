using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PingPong
{
	public class Server
	{
		bool _running;
		TcpListener _server;

		public Server(IPAddress address, int port)
		{
			_server = new TcpListener(address, port);
		}

		void HandleClient(object obj)
		{
			int id = Thread.CurrentThread.ManagedThreadId;

			Console.WriteLine("Hello client #{0} !", id);

			TcpClient client = (TcpClient)obj;

			StreamWriter _streamWriter = new StreamWriter(client.GetStream(), Encoding.UTF8);
			StreamReader _streamReader = new StreamReader(client.GetStream(), Encoding.UTF8);

			while (client.GetStream().CanRead) // While clients are connected...
			{
				string request = _streamReader.ReadLine();

				if (request == "Ping")
				{
					_streamWriter.WriteLine("Pong");
					_streamWriter.Flush();
				}
			}
		}

		void WaitForClients()
		{
			while (_running)
			{
				TcpClient client = _server.AcceptTcpClient();

				// Client found.
				//Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
				//clientThread.Start(client);
				Task.Run(() =>
				{
					HandleClient(client);
				});
			}
		}

		public void Start()
		{
			_running = true;
			_server.Start();

			WaitForClients();
		}

		public void Stop()
		{
			_running = false;
			_server.Stop();
		}
	}
}
