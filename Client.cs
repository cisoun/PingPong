using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace PingPong
{
	public class Client
	{
		bool			_connected;
		Stopwatch		_stopwatch;
		StreamReader	_streamReader;
		StreamWriter	_streamWriter;
		TcpClient		_client;

		public Client() {}

		public bool Connect(string host, int port)
		{
			try
			{
				_client = new TcpClient(host, port);
			}
			catch (Exception e)
			{
				Console.WriteLine("Oops... Got '{0}'", e.Message);
				return false;
			}

			_connected = _client.Connected;
			return _connected;
		}

		public void SendPing()
		{
			_stopwatch = new Stopwatch();

			_streamReader = new StreamReader(_client.GetStream(), Encoding.UTF8);
			_streamWriter = new StreamWriter(_client.GetStream(), Encoding.UTF8);

			while (_connected)
			{
				Console.WriteLine();
				Console.WriteLine("Press ENTER to launch a test.");
				Console.ReadLine();
				Console.WriteLine("Ping !");

				_stopwatch.Restart();

				_streamWriter.WriteLine("Ping");
				_streamWriter.Flush();

				string response = _streamReader.ReadLine();

				_stopwatch.Stop();

				if (response == "Pong")
					Console.WriteLine("Pong ! (took {0} ms)", _stopwatch.ElapsedMilliseconds);
			}
		}
	}
}


