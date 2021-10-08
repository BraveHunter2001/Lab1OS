using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1OS
{
	public interface IYNmessageBox
	{
		bool Show(string header);
	}

	public class YNMessageBox : IYNmessageBox
	{
		public bool Show(string header)
		{
			bool success = false;
			do
			{
				Console.WriteLine(header);
				Console.WriteLine("Y/N? ");
				string res = Console.ReadLine().ToLower();
				if (res == "y" || res == "yes")
					return true;
				if (res == "n" || res == "no")
					return false;
			} while (!success);
			return false;
		}
	}
	class Menu
	{
		
	}
}
