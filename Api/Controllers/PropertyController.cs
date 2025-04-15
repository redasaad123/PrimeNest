using Core.Interfaces;
using Core.Models;
using Core.Servises;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectApi.DTOS;

using ProjectApi.NewFolder;

namespace ProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly Service service;
        private readonly IUnitOfWork<Property> propertyUnitOfWork;
        private readonly UserManager<AppUser> userManager;

        public PropertyController(Service service, IUnitOfWork<Property> PropertyUnitOfWork, UserManager<AppUser> userManager)
        {
            this.service = service;
            propertyUnitOfWork = PropertyUnitOfWork;
            this.userManager = userManager;
        }

        [HttpGet("GetAllProperty")]

        public async Task<IActionResult> GetAllProperty()
        {
            var Property = await propertyUnitOfWork.Entity.GetAllAsync();
            if (Property.Count() == 0)
                return NotFound("Not Found Any Property");

            var map = Property.SkipWhile(x => x.IsRequested == true).Select(x => new GetPropertyDTO
            {
                Id = x.Id,
                OwnerName = userManager.Users.Where(u => u.Id == x.user).Select(u => u.Name).FirstOrDefault() ?? x.user,
                TypeContract = x.TypeContract,
                Type = x.Type,
                Address = x.Address,
                Area = x.Area,
                date = x.date,
                Description = x.Description,
                MainPhoto = x.MainPhoto,
                MoreDescription = x.MoreDescription,
                Photo = x.Photo,
                Price = x.Price,
                State = x.State ? "Availabe" : "Notavailabe"


            });
            return Ok(map);

        }

        [HttpPost("Search")]
        public async Task< IActionResult> search([FromForm]PropertySearchDto dto)
        {
            var result = await service.SearchProperties(dto);
            return Ok(result);
        }

        [HttpPost("AddProperty")]
        public async Task<IActionResult> AddProperty(AddPropertyDTO dto )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.GetUserAsync(User);
            var property = new Property();
            property.Id = Guid.NewGuid().ToString();

            var resalt = await service.Help(property, dto, user);
            if (resalt == null)
                return BadRequest();

            await propertyUnitOfWork.Entity.AddAsync(property);
            propertyUnitOfWork.Save();
            return Ok(resalt);
        }


        [HttpPut("UpdateProperty/{propertyId}")]
        public async Task<IActionResult> UpdateProperty(string propertyId, UpdatePropertyDTO dto)
        {
            var property = await propertyUnitOfWork.Entity.GetAsync(propertyId);
            if (property == null)
                return BadRequest();
            var user = await userManager.GetUserAsync(User);

            var resalt = await service.Help(property, dto, user);
            if (resalt == null)
                return BadRequest();

            await propertyUnitOfWork.Entity.UpdateAsync(property);
            propertyUnitOfWork.Save();
            return Ok(resalt);

        }

        [HttpDelete("DeleteProperty/{propertyId}")]
        public async Task<IActionResult> DeleteProperty(string propertyId)
        {
            var property = await propertyUnitOfWork.Entity.GetAsync(propertyId);
            if (property == null)
                return BadRequest();

            propertyUnitOfWork.Entity.Delete(property);
            propertyUnitOfWork.Save();
            return Ok(property);
        }


    }
}
