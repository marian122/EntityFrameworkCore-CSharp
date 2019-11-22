using System.Collections.Generic;

namespace ProductShop.Dto_s
{
    public class UserWithSalesDto
    {
        public UserWithSalesDto()
        {
            this.SoldProducts = new HashSet<ProductDto>(); 
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }


        public ICollection<ProductDto> SoldProducts { get; set; }
    }
}
