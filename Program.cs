using System;
using System.Collections.Generic;

namespace CarService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CarService carService = new CarService();
            carService.Work();
        }
    }

    class CarService
    {
        private int _money;
        private int _priceRepair;

        private int _costRepair = 100;

        private int _fixPenalty = 1000;
        private int _fixPenaltyForPart = 100;

        private Queue<Car> _cars = new Queue<Car>();
        private Storage _storage = new Storage();

        public CarService()
        {
            int minLimitMoney = 10000;
            int maxLimitMoney = 100000;

            _money = UserUtils.GenerateRandomNumber(minLimitMoney, maxLimitMoney);
            _cars = GenerateCars();
        }

        public void Work()
        {
            int remainderCars = _cars.Count;

            for (int i =0; i< remainderCars; i++)
            {
                Console.WriteLine($"В очереди {_cars.Count} машин");

                if (IsService())
                    TryRepairCar(_cars.Dequeue());
                else
                    DeleteMoney(_fixPenalty);
            }

            Console.WriteLine("Всё сделано.");
        }

        private void TryRepairCar(Car car)
        {
            if (_storage.PartsCount != 0)
                RepairCar(car);
            else
                Console.WriteLine("Склад деталей пуст.");
        }

        private void RepairCar(Car car)
        {
            Console.WriteLine("Починка машины.");

            _priceRepair = _costRepair;

            bool isService = true;

            do
            {
                if (_storage.PartsCount != 0)
                {
                    Console.WriteLine($"Осталось {_storage.PartsCount} деталей.");

                    if (IsService())
                    {
                        if (TryRepairPart(ref car))
                            isService = true;
                        else
                            isService = false;
                    }
                    else
                    {
                        isService = false;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            while (car.CountBrokenParts != 0);

            if (IsRepairSuccess(car) && isService)
                AddMoney(_priceRepair);
            else
                CountPenalty(car.CountBrokenParts);

            Console.WriteLine($"Счет автосервиса: {_money}.");
        }

        private bool TryRepairPart(ref Car car)
        {
            Part unbrokenPart = null;

            if (car.TryGetBrokenPart(out Part part) && _storage.TryGetPart(part, out unbrokenPart))
            {
                car.ReplaceBrokenPart(unbrokenPart);

                car.ShowBrokenPartsCountMessage();

                CountPrice(unbrokenPart.Cost);

                return true;
            }
            else
            {
                Console.WriteLine("Что-то пошло не так.");

                return false;
            }
        }

        private bool IsRepairSuccess(Car car)
        {
            if (car.CountBrokenParts != 0)
                return false;
            else
                return true;
        }

        private bool IsService()
        {
            string CommandDontService = "1";

            Console.WriteLine($"Введите {CommandDontService}, чтобы не обслуживать.");
            string userInput = Console.ReadLine();

            if (userInput == CommandDontService)
                return false;
            else
                return true;
        }

        private void CountPrice(int money)
        {
            _priceRepair += money;
        }

        private void CountPenalty(int countBrokenParts)
        {
            int penalty = countBrokenParts * _fixPenaltyForPart;

            DeleteMoney(penalty);
        }

        private void AddMoney(int money)
        {
            _money += money;
        }

        private void DeleteMoney(int money)
        {
            _money -= money;
        }


        private Queue<Car> GenerateCars()
        {
            int minLimitAmountCar = 5;
            int maxLimitAmountCar = 10;

            int amountCar = UserUtils.GenerateRandomNumber(minLimitAmountCar, maxLimitAmountCar);

            Queue<Car> cars = new Queue<Car>();

            CreatorCar creatorCar = new CreatorCar();

            for (int i = 0; i < amountCar; i++)
                cars.Enqueue(creatorCar.GenerateCar());

            return cars;
        }
    }

    class CreatorCar
    {
        public Car GenerateCar()
        {
            int minLimitAmountPart = 20;
            int maxLimitAmountPart = 50;

            List<Part> parts = new List<Part>();

            CreatorPart creatorPart = new CreatorPart();

            parts.Add(creatorPart.GenerateBrokenPart());

            int amountPart = UserUtils.GenerateRandomNumber(minLimitAmountPart, maxLimitAmountPart) - 1;

            for (int i = 0; i < amountPart; i++)
                parts.Add(creatorPart.GeneratePart());

            return new Car(parts);
        }
    }

    class Car
    {
        private int _amountPart;
        private List<Part> _parts;
        private List<Part> _brokenParts = new List<Part>();

        public Car(List<Part> parts)
        {
            _parts = parts;
            _amountPart = parts.Count;

            CreateListBrokenParts();
        }

        public int CountBrokenParts => _brokenParts.Count;

        public void ReplaceBrokenPart(Part part)
        {
            for (int i = 0; i < _parts.Count; i++)
            {
                if (_parts[i].Name == part.Name && _parts[i].IsBroken)
                {
                    _parts.Remove(_parts[i]);
                    _parts.Add(part);

                    Console.WriteLine("Деталь заменена.");
                    break;
                }
            }

            CreateListBrokenParts();
        }

        public void ShowBrokenPartsCountMessage()
        {
            Console.WriteLine($"{_brokenParts.Count} деталей сломано");
        }

        public bool TryGetBrokenPart(out Part part)
        {
            if (_brokenParts.Count != 0)
            {
                part = _brokenParts[0];

                return true;
            }
            else
            {
                part = null;

                return false;
            }
        }

        private void CreateListBrokenParts()
        {
            _brokenParts.Clear();

            foreach (Part part in _parts)
            {
                if (part.IsBroken)
                {
                    Part tempPart = part;
                    _brokenParts.Add(tempPart);
                }
            }
        }
    }

    class CreatorPart
    {
        public Part GeneratePart()
        {
            int minLimitRandom = 0;
            int maxLimitRandom = 2;

            int randomNumber = UserUtils.GenerateRandomNumber(minLimitRandom, maxLimitRandom);

            bool isBroken;

            if (randomNumber == 0)
                isBroken = true;
            else
                isBroken = false;

            return new Part(GetNamePart(), isBroken);
        }

        public Part GenerateUnbrokenPart()
        {
            bool isBroken = false;

            return new Part(GetNamePart(), isBroken);
        }

        public Part GenerateBrokenPart()
        {
            bool isBroken = true;

            return new Part(GetNamePart(), isBroken);
        }

        private string GetNamePart()
        {
            List<string> Names = new List<string>() { "колесо", "аккумулятор", "двигатель", "свеча зажигания", "окно", "прокладка", "колодка тормозная" };

            return Names[UserUtils.GenerateRandomNumber(0, Names.Count)];
        }
    }

    class Part
    {
        public Part(string name, bool isBroken)
        {
            Name = name;
            Cost = 200;
            IsBroken = isBroken;
        }

        public string Name { get; private set; }
        public bool IsBroken { get; private set; }
        public int Cost { get; private set; }
    }

    class Storage
    {
        private List<Part> _parts = new List<Part>();

        public Storage()
        {
            GenerateParts();
        }

        public int PartsCount => _parts.Count;

        public bool TryGetPart(Part part, out Part unbrokenPart)
        {

            for (int i = 0; i < _parts.Count; i++)
            {
                if (_parts[i].Name == part.Name)
                {
                    unbrokenPart = _parts[i];
                    _parts.RemoveAt(i);

                    return true;
                }
            }

            unbrokenPart = null;

            Console.WriteLine("Нужной детали нет.");

            return false;
        }

        private void GenerateParts()
        {
            int minLimitAmountPart = 100;
            int maxLimitAmountPart = 500;

            int amountPart = UserUtils.GenerateRandomNumber(minLimitAmountPart, maxLimitAmountPart);

            CreatorPart creatorPart = new CreatorPart();

            for (int i = 0; i < amountPart; i++)
                _parts.Add(creatorPart.GenerateUnbrokenPart());
        }
    }

    class UserUtils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int min, int max)
        {
            return s_random.Next(min, max);
        }
    }
}
