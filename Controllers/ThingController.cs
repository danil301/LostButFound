using Dadata;
using Dadata.Model;
using LostButFound.API.Domian;
using LostButFound.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace LostButFound.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]/{id?}")]
    public class ThingController : Controller
    {
        public IThingService _thingService;
        public ThingController(IThingService thingService)
        {
            _thingService = thingService;
        }

        [HttpPost]
        public async Task<IActionResult> EditData(string data)
        {
            var token = "8bf3f6224dfe7b74201b315ce2c1fac770a66c2f";
            var secret = "db02528010a64af0d36b932ec40f5f773da31897";
            var api = new CleanClientAsync(token, secret);
            var result = await api.Clean<Address>(data);

            List<string> metroList = new();
            foreach (var item in result.metro.ToList())
            {
                if (!metroList.Contains(item.name)) metroList.Add(item.name);
            }

            Data resultData = new Data()
            {
                Region = result.region,
                District = result.city_district,
                Street = result.street,
                Metro = metroList
            };

            return Ok(resultData);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddPost(string name, string description, string city, string district, string street, string metro)
        {
            var login = User.Identity.Name;
            Thing thing = new Thing()
            {
                City = city,
                Street = street,
                Metro = metro,
                Name = name,
                Description = description,
                UserName = login,
                IsLost = 1,
                IsApproved = 0,
                PathToIMG = "no yet",
                District = district
            };
            
           await _thingService.SetThing(thing);

            return Ok();
        }

        [HttpGet]
        public async Task<List<Thing>> GetPosts()
        {
            var response = await _thingService.GetThings();
            if (response.StatusCode == Domian.Enum.StatusCode.OK)
            {
                return response.Data;
            }
            else
            {
                return new List<Thing>();
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<List<Thing>> GetUserPosts()
        {
            var login = User.Identity.Name;
            var response = _thingService.GetThingsByUserName(login).Result;

            return response.Data;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteThing(string title)
        {
            string login = User.Identity.Name;
            var response = await _thingService.DeleteThing(login, title);

            if (response.StatusCode == Domian.Enum.StatusCode.OK)
            {
                return Ok("Post has been deleted");
            }
            else
            {
                return BadRequest(response.Data);
            }
        }
    }

    

    
}
