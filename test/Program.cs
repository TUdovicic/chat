using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
	class Program
	{
		static void Main(string[] args)
		{
			Mensch Tommy = new Mensch(15);
			Console.WriteLine(Tommy.Alter);
			Console.ReadLine();


		}
	}
	class Mensch
	{
		public int Alter;

		public Mensch(int a)
		{
			Alter = a;
		}

	}

	class Lehrer
	{

	}
}
