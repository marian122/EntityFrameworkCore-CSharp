using AutoMapper;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System.Linq;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<ImportSupplierDto, Supplier>();

            this.CreateMap<ImportPartsDto, Part>();

            this.CreateMap<ImportCarDto, Car>();

            this.CreateMap<ImportCustomersDto, Customer>();

            this.CreateMap<ImportSalesDto, Sale>();

            this.CreateMap<Supplier, ExportLocalSuppliersDto>();

            this.CreateMap<Part, ExportCarPartDto>();

            this.CreateMap<Car, ExportCarWithTheirListOfPartsDto>()
                .ForMember(x => x.Parts, y => y.MapFrom(s => s.PartCars.Select(pc => pc.Part)));
        }
    }
}
