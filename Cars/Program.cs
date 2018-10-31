using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            var cars = ProcessFile("fuel.csv");
            var manufacturers = ProcessManufacturers("manufacturers.csv");
            //foreach (var car in cars)
            //{
            //    Console.WriteLine(car.Name);
            //}

            // ordenando
            //var query = cars.Where(c => c.Manufacturer == "BMW" && c.Year == 2016).OrderByDescending(c => c.Combined).ThenBy(c => c.Name);
            //query syntax
            var query = from car in cars
                        where car.Manufacturer == "BMW" & car.Year == 2016
                        orderby car.Combined descending, car.Name
                        select car;

            foreach (var car in query.Take(10))
            {
                Console.WriteLine($"{car.Manufacturer} {car.Name} : {car.Combined}");
            }

            var a = new { nome = "asd" };
            

        }

        private static object ProcessManufacturers(string v)
        {
            var query =
                File.ReadAllLines(v)
                .Where(l => l.Length > 1)
                .Select(l =>
                {
                    var columns = l.Split(',');
                    return new Manufacturer
                    {
                        Name = columns[0],
                        Headquarters = columns[1],
                        Year = int.Parse(columns[2])
                    };
                });
            return query.ToList();
        }

        private static List<Car> ProcessFile(string path)
        {
            //return File.ReadAllLines(path)
            //    .Where(l => l.Length > 1)
            //    .Skip(1)
            //    .Select(Car.ParseFromCsv)
            //    .ToList();

            // fazendo com query syntax

            //return
            //    (from line in File.ReadAllLines(path).Skip(1)
            //     where line.Length > 1
            //     select Car.ParseFromCsv(line)).ToList();

            //fazendo com extension method

            return File.ReadAllLines(path)
                .Where(l => l.Length > 1)
                .Skip(1)
                .ToCar()
                .ToList();
        }
    }

    public static class CarExtension
    {
        public static IEnumerable<Car> ToCar(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');
                yield return new Car
                {
                    Year = int.Parse(columns[0]),
                    Manufacturer = columns[1],
                    Name = columns[2],
                    Displacement = double.Parse(columns[3]),
                    Cylinders = int.Parse(columns[4]),
                    City = int.Parse(columns[5]),
                    Highway = int.Parse(columns[6]),
                    Combined = int.Parse(columns[7])
                };
            }
        }
    }
}
