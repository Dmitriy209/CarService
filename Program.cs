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

        private bool _isService = true;

        private List<Car> _cars = new List<Car>();
        private List<Part> _parts = new List<Part>();

        public CarService()
        {
            int minLimitMoney = 10000;
            int maxLimitMoney = 100000;

            _money = UserUtils.GenerateRandomNumber(minLimitMoney, maxLimitMoney);
            _cars = GenerateCars();
            _parts = GenerateParts();
        }

        public void Work()
        {
            int remainderCars = _cars.Count;

            for (int i = 0; i < _cars.Count; i++)
            {
                remainderCars -= 1;

                Console.WriteLine($"Осталось {remainderCars} машин");

                if (IsService())
                    TryRepairCar(_cars[i]);
                else
                    DeleteMoney(_fixPenalty);
            }

            Console.WriteLine("Всё сделано.");
        }

        private void TryRepairCar(Car car)
        {
            if (_parts.Count != 0)
                RepairCar(car);
            else
                Console.WriteLine("Склад деталей пуст.");
        }

        private void RepairCar(Car car)
        {
            Console.WriteLine("Починка машины.");

            _priceRepair = _costRepair;

            do
            {
                if (_parts.Count != 0)
                {
                    Console.WriteLine($"Осталось {_parts.Count} деталей.");

                    if (IsService())
                    {
                        RepairPart(ref car);

                        _isService = true;
                    }
                    else
                    {
                        _isService = false;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            while (car.CountBrokenParts != 0);

            if (IsRepairSuccess(car) && _isService)
                AddMoney(_priceRepair);
            else
                CountPenalty(car.CountBrokenParts);

            Console.WriteLine($"Счет автосервиса: {_money}.");
        }

        private void RepairPart(ref Car car)
        {
            Part part = _parts[0];

            _parts.Remove(part);
            car.ReplacementBrokenPart(part);

            car.ShowBrokenPartsCountMessage();

            CountPrice(part.Cost);
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

        private List<Part> GenerateParts()
        {
            List<Part> parts = new List<Part>();

            int minLimitAmountPart = 100;
            int maxLimitAmountPart = 500;

            int amountPart = UserUtils.GenerateRandomNumber(minLimitAmountPart, maxLimitAmountPart);

            CreatorPart creatorPart = new CreatorPart();

            for (int i = 0; i < amountPart; i++)
                parts.Add(creatorPart.GenerateUnbrokenPart());

            return parts;
        }

        private List<Car> GenerateCars()
        {
            int minLimitAmountCar = 5;
            int maxLimitAmountCar = 10;

            int amountCar = UserUtils.GenerateRandomNumber(minLimitAmountCar, maxLimitAmountCar);

            List<Car> cars = new List<Car>();

            CreatorCar creatorCar = new CreatorCar();

            for (int i = 0; i < amountCar; i++)
                cars.Add(creatorCar.GenerateCar());

            return cars;
        }
    }

    class CreatorCar
    {
        public Car GenerateCar()
        {
            int minLimitAmountPart = 20;
            int maxLimitAmountPart = 50;

            return new Car(UserUtils.GenerateRandomNumber(minLimitAmountPart, maxLimitAmountPart));
        }
    }

    class Car
    {
        private int _amountPart;
        private List<Part> _parts;
        private List<Part> _brokenParts = new List<Part>();

        public Car(int amountPart)
        {
            _amountPart = amountPart;
            _parts = GenerateParts(_amountPart);

            CreateListBrokenParts();
        }

        public int CountBrokenParts => _brokenParts.Count;

        public void ReplacementBrokenPart(Part part)
        {
            for (int i = 0; i< _parts.Count; i++)
            {
                if (_parts[i].IsBroken)
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

        private List<Part> GenerateParts(int amountPart)
        {
            List<Part> parts = new List<Part>();

            CreatorPart creatorCar = new CreatorPart();

            parts.Add(creatorCar.GenerateBrokenPart());

            amountPart -= 1;

            for (int i = 0; i < amountPart; i++)
                parts.Add(creatorCar.GeneratePart());

            return parts;
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

            return new Part(isBroken);
        }

        public Part GenerateUnbrokenPart()
        {
            bool isBroken = false;

            return new Part(isBroken);
        }

        public Part GenerateBrokenPart()
        {
            bool isBroken = true;

            return new Part(isBroken);
        }
    }

    class Part
    {
        public bool IsBroken { get; private set; }
        public int Cost { get; private set; }

        public Part(bool isBroken)
        {
            Cost = 200;
            IsBroken = isBroken;
        }

        public void ShowStats()
        {
            if (IsBroken)
                Console.WriteLine("Деталь сломана.");
            else
                Console.WriteLine("Деталь цела.");
        }
    }

    class UserUtils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int min, int max)
        {
            return s_random.Next(min, max);
        }

        public static int ReadInt()
        {
            int number;

            while (int.TryParse(Console.ReadLine(), out number) == false)
                Console.WriteLine("Это не число.");

            return number;
        }
    }
}
