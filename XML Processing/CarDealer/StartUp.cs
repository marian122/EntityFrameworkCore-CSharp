namespace CarDealer
{
    using System;
    using CarDealer.Data;
    using System.Xml.Serialization;
    using CarDealer.Dtos.Import;
    using System.Collections.Generic;
    using System.IO;
    using AutoMapper;
    using CarDealer.Models;
    using System.Linq;
    using CarDealer.Dtos.Export;
    using System.Text;
    using System.Xml;
    using AutoMapper.QueryableExtensions;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile<CarDealerProfile>());

            using (var db = new CarDealerContext())
            {              
                //var inputXml = File.ReadAllText("./../../../Datasets/sales.xml");

                var result = GetLocalSuppliers(db);

                Console.WriteLine(result);
            }
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ImportSupplierDto>), new XmlRootAttribute("Suppliers"));

            List<ImportSupplierDto> supplierDtos;

            using (var reader = new StringReader(inputXml))
            {
                supplierDtos = (List<ImportSupplierDto>)xmlSerializer.Deserialize(reader);
            }

            var suppliers = Mapper.Map<List<Supplier>>(supplierDtos);

            context.Suppliers.AddRange(suppliers);

            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }


        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ImportPartsDto>), new XmlRootAttribute("Parts"));

            var partDtos = xmlSerializer.Deserialize(new StringReader(inputXml));

            var parts = Mapper.Map<List<Part>>(partDtos);   

            var supplierIds = context.Suppliers.Select(x => x.Id).ToHashSet();
            var validParts = parts.Where(p => supplierIds.Contains(p.SupplierId));

            context.Parts.AddRange(validParts);

            context.SaveChanges();

            return $"Successfully imported {parts.Count}";

        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ImportCarDto>), new XmlRootAttribute("Cars"));

            var carDtos = (List<ImportCarDto>)xmlSerializer.Deserialize(new StringReader(inputXml));
            var cars = new List<Car>();

            foreach (var carDto in carDtos)
            {
                var car = Mapper.Map<Car>(carDto);

                foreach (var part in carDto.Parts)
                {
                    var parForCarExist = car.PartCars
                        .FirstOrDefault(x => x.PartId == part.PartId) != null;

                    if (!parForCarExist && context.Parts.Any(p => p.Id == part.PartId))
                    {
                        var partCar = new PartCar
                        {
                            CarId = car.Id,
                            PartId = part.PartId
                        };

                        car.PartCars.Add(partCar);
                    }
                }
                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {context.Cars.Count()}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ImportCustomersDto>), new XmlRootAttribute("Customers"));

            var customerDtos = xmlSerializer.Deserialize(new StringReader(inputXml));

            var customers = Mapper.Map<List<Customer>>(customerDtos);

            context.Customers.AddRange(customers);

            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ImportSalesDto>), new XmlRootAttribute("Sales"));

            var saleDtos = (List<ImportSalesDto>)xmlSerializer.Deserialize(new StringReader(inputXml));

            var carId = context.Cars.Select(c => c.Id);

            var validSales = new List<ImportSalesDto>();

            foreach (var sale in saleDtos)
            {
                if (carId.Contains(sale.CarId))
                {
                    validSales.Add(sale);
                }
            }

            var sales = Mapper.Map<List<Sale>>(validSales);

            context.Sales.AddRange(sales);

            context.SaveChanges();

            return $"Successfully imported {context.Sales.Count()}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var carDtos = context
                .Cars
                .Where(c => c.TravelledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ProjectTo<ExportCarDto>()
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCarDto[]), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            xmlSerializer.Serialize(new StringWriter(sb), carDtos, namespaces);

            return sb.ToString().TrimEnd();


        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var bmwCarDtos = context
                .Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ProjectTo<ExportBmwCarDto>()
                .ToList();

            XmlSerializer serializer = new XmlSerializer(typeof(List<ExportBmwCarDto>), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), bmwCarDtos, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var localSuppliersDto = context
                .Suppliers
                .Where(s => s.IsImporter == false)
                .ProjectTo<ExportLocalSuppliersDto>()
                .ToList();

            XmlSerializer serializer = new XmlSerializer(typeof(List<ExportLocalSuppliersDto>), new XmlRootAttribute("suppliers"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), localSuppliersDto, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carWithPartsDto = context
                .Cars               
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ProjectTo<ExportCarWithTheirListOfPartsDto>()
                .ToList();

            foreach (var part in carWithPartsDto)
            {
               part.Parts = part.Parts.OrderByDescending(x => x.Price).ToList();
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<ExportCarWithTheirListOfPartsDto>), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), carWithPartsDto, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}