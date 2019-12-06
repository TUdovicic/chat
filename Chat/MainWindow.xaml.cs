using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chat
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		TcpClient client;
		string Username = string.Empty;
		string UserOutput;
		


		public MainWindow()
		
		{
			InitializeComponent();
		}

		private void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Username = NameInput.Text;
				
				var ipAddress = IPAddress.Parse(IpAdressInput.Text);
				client = new TcpClient();
				client.Connect(ipAddress, 5000);
				
				SendButton.IsEnabled = true;

				SendData(string.Format("userOutput|{0}", Username));

				LabelOnline.Content = "Online";
				LabelOnline.Background = Brushes.LightGreen;

				var thread = new Thread(() => ReceiveData(client));
				thread.Start();
				
			}
			catch(FormatException)
			{
				MessageBox.Show("Ungültige IP Adresse. Bitte erneut versuchen!");
			}
			catch (SocketException)
			{
				MessageBox.Show("Server nicht erreichbar");
			}
			
		}

		private void SendButton_Click(object sender, RoutedEventArgs e)
		{
			var messageText = string.Format("message|{0}|{1}", Username, MessageInput.Text);
			MessageInput.Text = string.Empty;
			SendData(messageText);

			//Tommy Spezial
			//var tommy = ConnectButton.GetType();
			//var ausgabeType = Encoding.ASCII.GetBytes(tommy.ToString());
			//stream.Write(ausgabeType, 0, ausgabeType.Length);

		}

		//Methode zum Senden von Daten an den Stream
		private void SendData(string messageText)
		{
			var stream = client.GetStream();

			var messageTextBytes = Encoding.UTF8.GetBytes(messageText);//schreiben auf dem Network-Stream nur mit Binärzahlen möglch
			stream.Write(messageTextBytes, 0, messageTextBytes.Length);
		}





		void ReceiveData(TcpClient client)
		{
			while (true)
			{
				NetworkStream stream = client.GetStream();

				var buffer = new byte[1024];
				var byteCount = stream.Read(buffer, 0, buffer.Length);

				

				if (byteCount > 0)
				{
					string data = Encoding.UTF8.GetString(buffer, 0, byteCount);
					var messageParts = data.Split('|');

					switch (messageParts[0])
					{
						case "message":
							Dispatcher.Invoke(() =>
							{
								ChatText.Text = messageParts[1] + ": " + messageParts[2] + Environment.NewLine + ChatText.Text;
							});
							break;
						
						case "userCount":
							Dispatcher.Invoke(() =>
							{
								OnlineOutput.Content = messageParts[1];
							});
							break;
						case "userOutput":
							var userList = messageParts[1];
							string userListText = string.Empty;
							foreach (var userName in userList)
							{
								userListText += userName + Environment.NewLine;
							}
							Dispatcher.Invoke(() =>
							{
								UserList.Content = userListText;
							});
							break;
							
					}
					

				}
			}

		}


	}

}
