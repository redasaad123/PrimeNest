using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectApi.ChainOfResponsibility;
using ProjectApi.DTOS;
using ProjectApi.DTOS.InterfaceDTO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using static System.Net.Mime.MediaTypeNames;

namespace Core.Servises
{
    public class Service
    {
        private readonly IUnitOfWork<Property> propertyUnitOfWork;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment hosting;

        public Service(IUnitOfWork<Property> PropertyUnitOfWork, Microsoft.AspNetCore.Hosting.IHostingEnvironment hosting)
        {
            propertyUnitOfWork = PropertyUnitOfWork;
            this.hosting = hosting;
        }


        
        public async Task<string> CompressAndSaveImageAsync(IFormFile file , string directory , int width = 800, int quality = 50)
        {

            string uploads = Path.Combine(hosting.WebRootPath, $@"{directory}");
            string filePath = Path.Combine(uploads, file.FileName);
            using (var image = await Image.LoadAsync(file.OpenReadStream()))
            {
                // ضغط الصورة
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(width, 0) 
                }));

                using var outputStream = new FileStream(filePath, FileMode.Create);
                await image.SaveAsync(outputStream, new JpegEncoder { Quality = quality });
            }

            return file.FileName;
        }

        public async Task<Property> Help(Property property, IPropertyDTO dto , AppUser user)
        {
            
            if (dto.Photo != null && dto.Photo.Count > 0)
            {
                property.Photo = new List<string>();
                foreach (var image in dto.Photo)
                {
                    if (image.Length > 0)
                    {

                        property.Photo.Add(await CompressAndSaveImageAsync(image, "Photos"));

                    }
                }
            }
            property.user = dto.owner ?? user.Id;
            property.TypeContract = dto.TypeContract ?? property.TypeContract;
            property.Type = dto.Type ?? property.Type;
            property.Address = dto.Address ?? property.Address;
            property.Area = dto.Area ?? property.Area;
            property.date = DateTime.Now;
            property.Description = dto.Description ?? property.Description;
            if (dto.MainPhoto != null )
            {
                property.MainPhoto = await CompressAndSaveImageAsync(dto.MainPhoto, "Photos");
            }
            property.MoreDescription = dto.MoreDescription ?? property.MoreDescription;
            property.Price = dto.Price ?? property.Price;

            if(dto.TypeContract != null)
                if (dto.TypeContract.ToLower().Contains("rent"))
                     property.State = false;


            return property;

        }
        public async Task<List<Property>> SearchProperties(PropertySearchDto filter)
        {
            var price = new PriceFilter();
            var location = new LocationFilter();
            var type = new TypeFilter();
            var Area = new AreaFilter();
            var Keyword = new KeywordFilter();
            var TypeContract = new TypeContractFilter();
            var Date = new DateFilter();

            // Chain them
            price.SetNext(location);
            location.SetNext(TypeContract);
            TypeContract.SetNext(type);
            type.SetNext(Area);
            Area.SetNext(Keyword);
            Keyword.SetNext(Date);

            // Start chain
            var query = await propertyUnitOfWork.Entity.GetAllAsync();
            var result = price.Apply(query, filter).ToList();

            return result;
        }    
    }
}
