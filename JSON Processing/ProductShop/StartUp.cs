using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.Dto_s;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile<ProductShopProfile>());

            using (var db = new ProductShopContext())
            {
                //var inputJson = File.ReadAllText("./../../../Datasets/categories-products.json");

                var result = GetUsersWithProducts(db);

                Console.WriteLine(result);
            }
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var result = JsonConvert.DeserializeObject<List<User>>(inputJson);

            context.Users.AddRange(result);

            context.SaveChanges();

            return $"Successfully imported {result.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var result = JsonConvert.DeserializeObject<List<Product>>(inputJson);

            context.Products.AddRange(result);

            context.SaveChanges();

            return $"Successfully imported {result.Count}";

        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var result = JsonConvert.DeserializeObject<List<Category>>(inputJson)
                .Where(c => c.Name != null);

            context.Categories.AddRange(result);

            context.SaveChanges();

            return $"Successfully imported {result.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var result = JsonConvert.DeserializeObject<List<CategoryProduct>>(inputJson);

            context.CategoryProducts.AddRange(result);

            context.SaveChanges();

            return $"Successfully imported {result.Count}";


        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    seller = p.Seller.FirstName + " " + p.Seller.LastName
                })
                .Where(p => p.price >= 500 && p.price <= 1000)
                .OrderBy(p => p.price);

            var json = JsonConvert.SerializeObject(products, Formatting.Indented);

            return json;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var userSoldProducts = context.Users
                 .Where(u => u.ProductsSold.Count > 0
                 && u.ProductsSold.Any(ps => ps.Buyer != null))
                 .OrderBy(u => u.LastName)
                 .ThenBy(u => u.FirstName)
                 .Include(u => u.ProductsSold)
                 .ToList();

            var mapper = Mapper.Map<IEnumerable<User>,
                IEnumerable<UserWithSalesDto>>(userSoldProducts);



            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var json = JsonConvert.SerializeObject(mapper, new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });

            return json;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .OrderByDescending(c => c.CategoryProducts.Count)
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoryProducts.Count,
                    averagePrice = $"{c.CategoryProducts.Average(cp => cp.Product.Price):f2}",   
                    totalRevenue = $"{c.CategoryProducts.Sum(cp => cp.Product.Price):f2}"
                });

            var json = JsonConvert.SerializeObject(categories, Formatting.Indented);

            return json;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderByDescending(u => u.ProductsSold.Count(p => p.Buyer != null))
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = new
                    {
                        count = u.ProductsSold.Count(p => p.Buyer != null),

                        products = u.ProductsSold
                        .Where(p => p.Buyer != null)
                        .Select(p => new
                        {
                            name = p.Name,
                            price = p.Price
                        })

                    }
                });

            var resultJson = new
            {
                usersCount = users.Count(),
                users
            };

            var json = JsonConvert.SerializeObject(resultJson, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });


            return json;
        }
    }
}