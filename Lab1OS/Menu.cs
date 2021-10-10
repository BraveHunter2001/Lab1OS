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


	public interface IMenuItem
    {
		void Select(IMenu invokedFrom = null);
		void PrintItem();
    }

	public interface IMenu: IMenuItem
    {
		IMenu GetLastMenu();
    }

	public class MenuItem : IMenuItem
    {
		Action action;
		String name; 
		
		public MenuItem(string name, Action action)
        {
			this.action = action;
			this.name = name;
        }

        public void PrintItem()
        {
            Console.WriteLine(name);
        }

        public void Select(IMenu invokedFrom)
        {
			action?.Invoke();
			Console.ReadKey();
			invokedFrom.Select(invokedFrom.GetLastMenu());
        }
    }

    public class Menu : IMenu
    {
		protected string headerMenu;

		protected IMenuItem[] menuItems;
		public IMenu PreviousMenu { get; private set; }
		public IMenu GetLastMenu() => PreviousMenu;
		protected Action onMenuLeaveAction;

		readonly string selectAgainMessage = "Incorrect input. Please, try again.",
			selectItemMessage = "Please, select item menu",
			toPrevMenuMessage = "Return to previous menu",
			exitMessage = "Exit";

		public Menu (String headerMenu, IMenuItem[] menuItems)
        {
			this.headerMenu = headerMenu;
			this.menuItems = menuItems;
        }

		private int GetSelectedItemIdexByUser()
        {
			int selIndex = -1;

			do
			{
				if (!int.TryParse(Console.ReadLine(), out selIndex) || selIndex < 0 || selIndex > menuItems.Length)
				{
					selIndex = -1;
                    Console.WriteLine(selectAgainMessage);
					Console.ReadKey();
					PrintMenu();
				}

			} while (selIndex < 0);

			return selIndex;
        }

		protected virtual void PrintMenu()
		{
			Console.Clear();
            Console.WriteLine(headerMenu);

			int counter = 1;

			foreach (var item in menuItems)
            {
                Console.Write(counter++ + ")");
				item.PrintItem();
            }

            Console.WriteLine();
            Console.WriteLine("0)" + (PreviousMenu == null ? exitMessage : toPrevMenuMessage));
            Console.WriteLine();
            Console.WriteLine(selectItemMessage);
		}
      
        
        public void PrintItem()
        {
            Console.WriteLine(headerMenu);
        }

        public virtual void Select(IMenu invokedFrom = null)
        {
			if (invokedFrom != null)
				PreviousMenu = invokedFrom;

			PrintMenu();
			int selection = GetSelectedItemIdexByUser();
			if (selection != 0)
				menuItems[selection - 1]?.Select(this);
			else
			{
				onMenuLeaveAction?.Invoke();
				PreviousMenu?.Select(null);
			}

		}
    }

	public class AdaptiveMenu: Menu
    {
		public AdaptiveMenu(String headerMenu, IMenuItem[] menuItems) : base(headerMenu, menuItems) { }

		public void AddItems(params IMenuItem[] items)
		{
			Array.Resize(ref menuItems, menuItems.Length + items.Length);
			Array.Copy(items, 0, menuItems, menuItems.Length - items.Length, items.Length);
		}

	}
	public class MenuWithDataRequest<T> : Menu
	{
		bool dataSet = false;
		public T Data { get; protected set; }
		protected Func<T> dataRequester;
		public MenuWithDataRequest(string menuHeader, IMenuItem[] menuItems, Func<T> dataRequester) : base(menuHeader, menuItems)
		{
			this.dataRequester = dataRequester;
			onMenuLeaveAction += () => dataSet = false;
		}

		public override void Select(IMenu invokedFrom = null)
		{
			if (!dataSet)
			{
				dataSet = true;
				Data = dataRequester.Invoke();
			}
			base.Select(invokedFrom);
		}
		protected override void PrintMenu()
		{
			base.PrintMenu();
			Console.WriteLine("Inspected item: " + Data.ToString());
		}
	}
}
