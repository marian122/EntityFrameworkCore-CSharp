﻿using Newtonsoft.Json;

namespace ProductShop.Dto_s
{
    public class ExportProductDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("seller")]
        public string Seller { get; set; }
    }
}