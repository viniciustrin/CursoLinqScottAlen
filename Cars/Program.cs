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

            //// ordenando
            //var query = cars.Where(c => c.Manufacturer == "BMW" && c.Year == 2016).OrderByDescending(c => c.Combined).ThenBy(c => c.Name);

            ////query syntax
            //var query = from car in cars
            //            where car.Manufacturer == "BMW" & car.Year == 2016
            //            orderby car.Combined descending, car.Name
            //            select car;

            //foreach (var car in query.Take(10))
            //{
            //    Console.WriteLine($"{car.Manufacturer} {car.Name} : {car.Combined}");
            //}

            ////objeto anonimo
            //var a = new { nome = "asd" };


            ////join com query syntax
            //var query =
            //    from car in cars
            //    join manufacturer in manufacturers on car.Manufacturer equals manufacturer.Name
            //    orderby car.Combined descending, car.Name ascending
            //    select new
            //    {
            //        manufacturer.Headquarters,
            //        car.Name,
            //        car.Combined
            //    };

            ////join com mehtod syntax
            //var query =
            //    cars.Join(manufacturers,
            //            c => c.Manufacturer,
            //            m => m.Name,
            //            (c, m) =>
            //            new
            //            {
            //                m.Headquarters,   //pode usar o objeto inteiro aqui tbm
            //                c.Name,           // car = c,
            //                c.Combined        // manufacturer = m e depois tem q fazer projeçao com select()
            //            }).OrderByDescending(c => c.Combined)
            //            .ThenBy(c => c.Name);


            ////join com chave composta com query syntax
            //var query =
            //    from car in cars
            //    join manufacturer in manufacturers 
            //    on new { car.Manufacturer, car.Year }
            //    equals
            //    new {Manufacturer = manufacturer.Name, manufacturer.Year}
            //    orderby car.Combined descending, car.Name ascending
            //    select new
            //    {
            //        manufacturer.Headquarters,
            //        car.Name,
            //        car.Combined
            //    };

            ////join com chave composta mehtod syntax
            //var query =
            //    cars.Join(manufacturers,
            //            c => new { c.Manufacturer, c.Year },
            //            m => new { Manufacturer = m.Name, m.Year },
            //            (c, m) =>
            //            new
            //            {
            //                m.Headquarters,   //pode usar o objeto inteiro aqui tbm
            //                c.Name,           // car = c,
            //                c.Combined        // manufacturer = m e depois tem q fazer projeçao com select()
            //            }).OrderByDescending(c => c.Combined)
            //            .ThenBy(c => c.Name);
            //foreach (var car in query.Take(10))
            //{
            //    Console.WriteLine($"{car.Headquarters} {car.Name} : {car.Combined}");
            //}

            //// group by 
            //var query =
            //    from car in cars
            //    group car by car.Manufacturer.ToUpper()
            //    into m
            //    orderby m.Key
            //    select m;

            //var query =
            //    cars.GroupBy(c => c.Manufacturer.ToUpper())
            //    .OrderBy(g => g.Key);


            //foreach (var group in query)
            //{
            //    Console.WriteLine(group.Key);
            //    foreach (var car in group.OrderByDescending(c => c.Combined).Take(2))
            //    {
            //        Console.WriteLine($"\t{car.Name} : {car.Combined}");
            //    }
            //}


            /////groupby com join
            //var query =
            //    from manu in manufacturers
            //    join car in cars on manu.Name equals car.Manufacturer
            //    into carGroup
            //    select new
            //    {
            //        Manufacturer = manu,
            //        Cars = carGroup
            //    };

            //var query =
            //    manufacturers.GroupJoin(cars,
            //    m => m.Name, c => c.Manufacturer,
            //    (m, g) => new
            //    {
            //        Manufacturer = m,
            //        Cars = g
            //    }).OrderBy(m => m.Manufacturer.Name);

            //foreach (var group in query)
            //{
            //    Console.WriteLine($"{group.Manufacturer.Name} : {group.Manufacturer.Headquarters}");
            //    foreach (var car in group.Cars.OrderByDescending(c => c.Combined).Take(2))
            //    {
            //        Console.WriteLine($"\t{car.Name} : {car.Combined}");
            //    }
            //}

            var query =
                cars.Join(manufacturers, c => c.Manufacturer, m => m.Name, (c, m) => new
                {
                    c.Name,
                    c.Combined,
                    m.Headquarters
                }).GroupBy(g => g.Headquarters);

            foreach (var group in query)
            {
                Console.WriteLine(group.Key);
                foreach (var car in group.OrderByDescending(c => c.Combined).Take(2))
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }



        }

        private static List<Manufacturer> ProcessManufacturers(string v)
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
