using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
		public MainWindow()
		{
			InitializeComponent();
		}

		private void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var ipAddress = IPAddress.Parse(IpAdressInput.Text);
				client = new TcpClient();
				client.Connect(ipAddress, 5000);


				SendButton.IsEnabled = true;
				
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
			var messageText = MessageInput.Text;
			MessageInput.Text = string.Empty;

			var stream = client.GetStream();

			var messageTextBytes = Encoding.ASCII.GetBytes(messageText);//schreiben auf dem Network-Stream nur mit Binärzahlen möglch
			stream.Write(messageTextBytes, 0, messageTextBytes.Length);
		}
	}
}
