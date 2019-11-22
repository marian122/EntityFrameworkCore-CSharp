using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile<CarDealerProfile>());

            var db = new CarDealerContext();

            var inputJson = File.ReadAllText("./../../../Datasets/sales.json");

            var result = ImportSales(db, inputJson);

            Console.WriteLine(result);


        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);

            context.Suppliers.AddRange(suppliers);

            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var inputParts = JsonConvert.DeserializeObject<List<Part>>(inputJson);

            var supplierIds = context.Suppliers
                .Select(s => s.Id)
                .ToList();

            var parts = new List<Part>();

            foreach (var partt in inputParts)
            {
                if (supplierIds.Contains(partt.SupplierId))
                {
                    parts.Add(partt);
                }
            }

            context.AddRange(parts);

            context.SaveChanges();

            return $"Successfully imported {parts.Count}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carsImport = JsonConvert.DeserializeObject<CarImportDto[]>(inputJson);
            var carsToAdd = Mapper.Map<CarImportDto[], Car[]>(carsImport);

            context.AddRange(carsToAdd);
            context.SaveChanges();

            HashSet<int> partIds = context.Parts.Select(x => x.Id).ToHashSet();

            HashSet<PartCar> carPartsToAdd = new HashSet<PartCar>();

            foreach (var car in carsImport)
            {
                car.PartsId = car
                    .PartsId
                    .Distinct()
                    .ToList();

                Car currentCar = context.
                    Cars
                    .Where(x => x.Make == car.Make
                    && x.Model == car.Model
                    && x.TravelledDistance == car.TravelledDistance)
                    .FirstOrDefault();

                if (currentCar == null)
                {
                    continue;
                }

                foreach (var id in car.PartsId)
                {
                    if (!partIds.Contains(id))
                    {
                        continue;
                    }

                    PartCar partCar = new PartCar
                    {
                        CarId = currentCar.Id,
                        PartId = id
                    };

                    if (!carPartsToAdd.Contains(partCar))
                    {
                        carPartsToAdd.Add(partCar);
                    }
                }

                if (carPartsToAdd.Count > 0)
                {
                    currentCar.PartCars = carPartsToAdd;
                    context.PartCars.AddRange(carPartsToAdd);
                    carPartsToAdd.Clear();
                }
            }

            context.SaveChanges();

            return $"Successfully imported {context.Cars.ToList().Count}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson);

            context.Customers.AddRange(customers);

            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson);

            context.Sales.AddRange(sales);

            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }
    }
}