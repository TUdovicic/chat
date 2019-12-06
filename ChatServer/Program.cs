using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace ChatServer
{
	class Program  
	{
		static List<TcpClient> Clients = new List<TcpClient>();
		static bool ParallelExecution = false;
		static Timer SendClientUpdateTimer;
		static List<string> Usernames = new List<string>();


		static void Main(string[] args)
		{
			Console.WriteLine("Soll der Server parallel verarbeiten? (y/n)");
			var input = Console.ReadKey();
			
			//ParallelExecution = (input.Key == ConsoleKey.Y);
			if (input.Key == ConsoleKey.Y)
			{
				ParallelExecution = true;
				Console.WriteLine("Parrallel Verarbeitung aktiviert");
			}
			else
			{
				Console.WriteLine("Parrallel Verarbeitung deaktiviert");
			}


			Console.OutputEncoding = Encoding.UTF8;
			// var tcpListener = new TcpListener(5000); <- obsolete
			var tcpListener = new TcpListener(IPAddress.Any, 5000);
			tcpListener.Start();

			SendClientUpdateTimer = new Timer(SendClientUpddate, null, 0, 5000);		//1. Parameter, Methode, 2.state	

			while(true)
			{
				var client = tcpListener.AcceptTcpClient();
				Clients.Add(client);
				NetworkStream stream = client.GetStream();
				var thread = new Thread(() => HandleClient(client));
				thread.Start();

				Console.WriteLine("Verbunden: {0}", client.Client.RemoteEndPoint);
				Console.WriteLine("Es sind {0} Teilnehmer online. ", Clients.Count);
			 }
			
		}

		private static void SendClientUpddate(object state)
		{
			string message = string.Format("userCount|{0}", Clients.Count);
			var byteMessage = Encoding.UTF8.GetBytes(message);
			Broadcast(byteMessage);
		}

		static void HandleClient(TcpClient client)
		{
			while (true)
			{
				NetworkStream stream = client.GetStream();

				var buffer = new byte[1024];
				var byteCount = stream.Read(buffer, 0, buffer.Length);

				if (byteCount > 0)
				{
					Broadcast(buffer);

					string data = Encoding.UTF8.GetString(buffer, 0, byteCount);
					Console.WriteLine(data);

				}
			}
			
		}

		private static void Broadcast(byte[] buffer)
		{
			//Broadcast Anfang
			if (ParallelExecution)
			{

				Parallel.ForEach(Clients, (otherClient) =>
				{
					var otherStream = otherClient.GetStream();
					otherStream.Write(buffer, 0, buffer.Length);
				});
			}
			else
			{
				foreach (var otherClient in Clients)
				{
					var otherStream = otherClient.GetStream();
					otherStream.Write(buffer, 0, buffer.Length);

				}
			}
			//Broadcast Ende
		}
	}
}
