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
                        buyer.BuyProduct(market.SellProduct(buyer.Money));
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
    class Buyer
    {
        private List<Product> _products = new List<Product>();

        public int Money { get; private set; }

        public Buyer()
        {
            _products = new List<Product>(0);
            Money = 500;
        }

        public void BuyProduct(Product product)
        {
            if (product != null)
            {
                if (CheckSimilar(product.Name))
                {
                    AddQuantity(product.Name, product.Quantity);
                }
                else
                {
                    _products.Add(product);
                }

                Money -= product.Prise * product.Quantity;
            }
        }

        public void ShowInfo()
        {
            Console.WriteLine($"На счету покупателя {Money} рублей \nВ сумке сейчас:");

            foreach (Product product in _products)
            {
                product.ShowInfoBuyer();
            }
        }

        private bool CheckSimilar(string productName)
        {  
            foreach (Product product in _products)
            {
                if (product.Name == productName)
                {
                    return true;
                }
            }

            return false;
        }

        private void AddQuantity( string productName, int productQuantity)
        {
            foreach (Product product in _products)
            {
                if (product.Name == productName)
                {
                    product.AddQuantity (productQuantity);
                }
            }
        }       
    }

    class Market
    {
        private List <Product> _products = new List<Product>();
        private int _money;

        public Market()
        {
            _money = 0;
            _products.Add(new Product ("Яблоки", 10, 15));
            _products.Add(new Product("Капуста", 35, 10));
            _products.Add(new Product("Помидоры", 20, 17));
            _products.Add(new Product("Огурцы", 15, 15));
            _products.Add(new Product("Арбузы", 55, 7));
        }

        public Product SellProduct(int BayerMoney)
        {
            int productIndex = GetProductIndex();
            int productQuantity = GetProductQuantity();

            if (CheckSolvency(BayerMoney, productIndex, productQuantity))
            {
                return GiveProductToSell(productIndex, productQuantity);
            }
            else
            {
                return null;
            }
        }

        public void ShowInfo()
        {
            Console.WriteLine($"На счету магазина {_money} рублей \nНа прилавке сейчас находятся:");

            foreach (Product product in _products)
            {
                product.ShowInfoMarket();
            }
        }

        private bool CheckSolvency(int BayerMoney, int productIndex, int productQuantity)
        {
            if (BayerMoney >= (_products[productIndex].Prise * productQuantity))
            {
                return true;
            }
            else
            {
                Console.WriteLine("У вас недостаточно денег");
                return false;
            }
        }

        private Product GiveProductToSell(int productIndex, int productQuantity) 
        {
            if (CheckProductAvailability(productIndex, productQuantity))
            {
                Product product = new Product(_products[productIndex].Name, _products[productIndex].Prise, productQuantity);
                _money += _products[productIndex].Prise * productQuantity;
                DeleteNonRemaining(_products[productIndex].Quantity, productIndex);
                return product;
            }
            else
            {
                return null;
            }
        }

        private bool CheckProductAvailability(int productIndex, int productQuantity)
        {
            if (_products[productIndex].IsEnough(productQuantity) && productQuantity > 0)
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


        private int GetProductIndex()
        {
            bool isFound = false;
            int productIndex = 0;

            Console.WriteLine("Введите название товара который хотите купить:");
            string productName = Console.ReadLine();

            while (isFound == false)
            {
                for (int i = 0; i < _products.Count; i++)
                {
                    if (_products[i].Name == productName)
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

        private void DeleteNonRemaining(int quantity , int productIndex)
        {
            if (quantity == 0)
            {
                _products.RemoveAt(productIndex);
            }
        }
    }

    class Product
    {
        public string Name { get; }
        public int Prise { get; }
        public int Quantity { get; private set; }

        public Product(string name, int prise, int quantity)
        {
            Name = name;
            Prise = prise;
            Quantity = quantity;
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
                Console.WriteLine($"Продукта {Name} нет в достаточном количестве");
                return false;
            }
        }

        public void AddQuantity(int quantity)
        {
            Quantity += quantity;
        }

        public void ShowInfoMarket ()
        {
            Console.WriteLine($"{Name}. Цена за штуку: {Prise} рублей. В наличии шт.: {Quantity}.");
        }

        public void ShowInfoBuyer()
        {
            Console.WriteLine($"{Name}. {Quantity} шт.");
        }
    }
}
