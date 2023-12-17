using Dadata;
using Dadata.Model;
using LostButFound.API.Domian;
using LostButFound.API.Domian.ViewModels;
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
            var token = "7068cf85baa1b43c53d92e9eedbb1e7bdec395d1";
            var secret = "55996817b6f14154aa342ea77480143bbcf9d4c7";
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
    }

    

    
}
