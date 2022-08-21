using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Buyer buyer = new Buyer();
            Market market = new Market();
            bool isContinue = true;

            while (isContinue)
            {
                Console.Clear();
                Console.WriteLine("1) Показать товары в магазине \n2) купить товар \n3) показать товары в корзине покупателя \n4) выход");
                ConsoleKeyInfo key = Console.ReadKey(true);
                Console.Clear();

                switch (key.KeyChar)
                {
                    case '1':
                        market.ShowInfo();
                        break;
                    case '2':                        
                        buyer.BuyProduct(market.SellProducts(buyer.CheckMoney()));
                        break;
                    case '3':
                        buyer.ShowInfo();
                        break;
                    case '4':
                        isContinue = false;
                        break;
                    default:
                        Console.WriteLine("Неверно выбрана команда. Для продолжения нажмите любую клавишу...");
                        break;
                }

                Console.ReadKey(true);
            }
        }
    }

    class Trader
    {
        private List <Product> _products = new List <Product>();
        protected List<Stack> _stacks = new List<Stack>();             
        protected int _money;

        public Trader(int money)
        {
            _products.Add(new Product("Яблоки", 10));
            _products.Add(new Product("Капуста", 35));
            _products.Add(new Product("Помидоры", 20));
            _products.Add(new Product("Огурцы", 15));
            _products.Add(new Product("Арбузы", 55));
            _money = money;

            foreach (Product product in _products)
            {
                _stacks.Add(new Stack(product, 0));
            }
        }
    }

    class Buyer : Trader
    {
        public Buyer() : base(500)  { }

        public void BuyProduct(Stack stack)
        {
            if (stack != null)
            {
                Product product = stack.GetProduct();
                int quantity = stack.Quantity;
                AddQuantity(product, quantity);
                _money -= product.Prise * quantity;
            }
        }

        public void ShowInfo()
        {
            Console.WriteLine($"На счету покупателя {_money} рублей \nВ сумке сейчас:");

            foreach (Stack stack in _stacks)
            {
                stack.ShowInfoBuyer();
            }
        }

        public int CheckMoney()
        {
            return _money;
        }
               
        private void AddQuantity(Product product, int quantity)
        {
            int index = 0;

            for (int i = 0; i < _stacks.Count; i++)
            {
                if (_stacks[i].GetProduct() == product)
                {
                    index = i;
                }
            }

            _stacks[index].AddQuantity(quantity);
        }       
    }

    class Market : Trader
    {
        public Market() : base (0)
        {
            Random random = new Random();

            foreach (Stack stack in _stacks)
            {
                stack.AddQuantity(random.Next(5,31));
            }
        }

        public Stack SellProducts(int BayerMoney)
        {
            int productIndex = ChooseProduct();
            int quantity = GetProductQuantity();

            if (CheckSolvency(BayerMoney, productIndex, quantity))
            {
                return GiveProductsToSell(productIndex, quantity);
            }
            else
            {
                return null;
            }
        }

        public void ShowInfo()
        {
            Console.WriteLine($"На счету магазина {_money} рублей \nНа прилавке сейчас находятся:");

            foreach (Stack stack in _stacks)
            {
                stack.ShowInfoMarket();
            }
        }

        private bool CheckSolvency(int BayerMoney, int productIndex, int quantity)
        {
            if (BayerMoney >= (_stacks[productIndex].GetPrice() * quantity))
            {
                return true;
            }
            else
            {
                Console.WriteLine("У вас недостаточно денег");
                return false;
            }
        }

        private Stack GiveProductsToSell(int productIndex, int quantity) 
        {
            if (CheckProductAvailability(productIndex, quantity))
            {
                Stack stack = new Stack (_stacks[productIndex].GetProduct(), quantity);
                _money += _stacks[productIndex].GetPrice() * quantity;
                return stack;
            }
            else
            {
                return null;
            }
        }

        private bool CheckProductAvailability(int productIndex, int quantity)
        {
            if (_stacks[productIndex].IsEnough(quantity) && quantity > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int GetProductQuantity()
        {
            Console.WriteLine("Введите количество товара который хотиет купить"); 
            return GetNumber();
        }

        private int GetNumber()
        {
            int number = 0;
            bool success = false;

            while (success == false)
            {
                string userInput = Console.ReadLine();

                success = int.TryParse(userInput, out number);
                if (success == false)
                {
                    Console.WriteLine($"Введенное значение: '{userInput}' не явлеяется числом, поробуйте еще раз.");
                }

                if (number <= 0)
                {
                    success = false;
                    Console.WriteLine("Количество товара не может быть меньше 0 попробуйте еще раз");
                }
            }

            return number;
        }


        private int ChooseProduct()
        {
            bool isFound = false;
            int productIndex = 0;

            Console.WriteLine("Введите название товара который хотите купить:");
            string productName = Console.ReadLine();

            while (isFound == false)
            {
                for (int i = 0; i < _stacks.Count; i++)
                {
                    if (_stacks[i].GetProductName() == productName)
                    {
                        isFound = true;
                        productIndex = i;
                    }
                }

                if (isFound == false)
                {
                    Console.WriteLine("Такого товара в наличии нет, попробуйте еще раз");
                    productName = Console.ReadLine();
                }
            }

            return productIndex;
        }       
    }

    class Stack
    {
        private Product _product;

        public int Quantity { get; private set; }

        public Stack(Product product, int quantity)
        {
            _product = product;
            Quantity = quantity;
        }

        public Product GetProduct()
        {
            return _product;
        }

        public string GetProductName()
        {
            return _product.Name;
        }

        public int GetPrice()
        {
            return _product.Prise;
        }

        public bool IsEnough(int quantity)
        {
            if (Quantity >= quantity)
            {
                Quantity -= quantity;
                return true;
            }
            else
            {
                Console.WriteLine($"Продукта {_product.Name} нет в достаточном количестве");
                return false;
            }
        }

        public void AddQuantity(int quantity)
        {
            Quantity += quantity;
        }

        public void ShowInfoMarket()
        {
            if (Quantity > 0)
            {
            Console.WriteLine($"{_product.Name}. Цена за штуку: {_product.Prise} рублей. В наличии шт.: {Quantity}.");
            }
            else
            {
                Console.WriteLine($"{_product.Name} - закончились");
            }
        }

        public void ShowInfoBuyer()
        {
            if (Quantity > 0)
            {
                Console.WriteLine($"{_product.Name}. {Quantity} шт.");
            }
        }
    }

    class Product
    {
        public string Name { get; }
        public int Prise { get; }

        public Product(string name, int prise)
        {
            Name = name;
            Prise = prise;
        }  
    }
}
