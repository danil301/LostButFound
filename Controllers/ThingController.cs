using Dadata;
using Dadata.Model;
using LostButFound.API.Domian;
using LostButFound.API.Domian.ViewModels;
using LostButFound.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult AddPost(ThingViewModel thingViewModel)
        {
            Thing thing = new Thing()
            {
                City = thingViewModel.City,
                Street = thingViewModel.Street,
                Metro = thingViewModel.Metro,
                Name = thingViewModel.Name,
                Description = thingViewModel.Description,
                UserName = User.Identity.Name,
                IsLost = 1,
                IsApproved = 0,
                PathToIMG = "no yet",
                District = thingViewModel.District
            };
            
            _thingService.SetThing(thing);

            return Ok("Object has been added");
        }
    }

    

    
}
