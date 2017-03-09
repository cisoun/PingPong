/*
 
	PingPong - Client-server latency tester

	Author :

		Cyriaque Skrapits

	Description :

		This program was made in order to test the latency between two computers 
		connected through a network.

		When started, it lets the user launch a client or server instance.
		Some parameters are then asked to the user in order to configure and run
		the instance.
		
*/

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace PingPong
{
	class PingPong
	{
		static Server server; // Leave this here to let it stops when the application ends.

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		public static void Main(string[] args)
		{
			Console.CancelKeyPress += MainClass_ConsoleCancelEventHandler;

			// Intro.

			Console.WriteLine("---------------------------------------");
			Console.WriteLine("PingPong (client-server latency tester)");
			Console.WriteLine("---------------------------------------");
			Console.WriteLine();
			Console.WriteLine("What do you want to launch ?");
			Console.WriteLine();
			Console.WriteLine("1) Client");
			Console.WriteLine("2) Server");
			Console.WriteLine();

			// Ask what to run.

			int choice = 0;
			bool ok = false;

			while (!ok) 
			{
				Console.Write("Choice : ");

				ok = int.TryParse(Console.ReadLine(), out choice);
				ok &= choice == 1 || choice == 2; // Is choice 1 or 2 ?

				if (!ok)
					Console.WriteLine("Wrong value...");
			}

			// Run desired instance.

			Console.WriteLine("You choose {0}", choice);
			Console.WriteLine();

			switch (choice)
			{
				case 1: LaunchClient(); break;
				case 2: LaunchServer(); break;
			}
		}

		/// <summary>
		/// Launchs a client instance.
		/// User has to enter the remote's address and port in order to get a connection with it.
		/// </summary>
		static void LaunchClient()
		{
			Console.WriteLine("Launched client.");

			Client client = new Client();

			string[] infos = { };
			bool connected = false;

			// Ask for remote address.

			while (!connected)
			{
				Console.Write("On which host do I have to connect ? : ");
				string remote = Console.ReadLine();
				infos = remote.Split(':');

				if (infos.Length != 2) // Has something like xxx:xxx ?
					continue;

				// Get port.
				int port;
				if (!int.TryParse(infos[1], out port))
					continue;

				// Try connecting...
				connected = client.Connect(infos[0], port);

				if (!connected)
					Console.WriteLine("Sorry, I couldn't connect to this host...");
			}

			// Ready.

			client.SendPing();
		}
	
 		/// <summary>
 		/// Launchs the server.
		/// User must enter a port in order to start the server.
		/// IP is automatically found by looking up the address associated with the hostname in the DNS.
 		/// </summary>	
		static void LaunchServer()
		{
			Console.WriteLine("Launched server.");

			int port = 0;
			bool ok = false;

			// Ask for port.

			while (!ok)
			{
				Console.Write("Port to use : ");

				ok = int.TryParse(Console.ReadLine(), out port);
				ok &= port > 1023 && port < 65535;

				if (!ok)
					Console.WriteLine("Wrong value...");
			}

			// Get local IP address.

			IPAddress local = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
		
			// Run server.

			server = new Server(local, port);
			server.Start();

			Console.WriteLine("Listening for client on {0}:{1}...", local.ToString(), port);
		}

		/// <summary>
		/// Handle the class console cancel event handler.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		static void MainClass_ConsoleCancelEventHandler(object sender, ConsoleCancelEventArgs e)
		{
			if (server != null)
				server.Stop();

			Console.Clear();
			Console.WriteLine("Bye !");

			Environment.Exit(0);
		}
	}
}
