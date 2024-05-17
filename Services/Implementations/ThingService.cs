using LostButFound.API.DAL.Interfaces;
using LostButFound.API.Domian;
using LostButFound.API.Domian.Response;
using LostButFound.API.Services.Interfaces;

namespace LostButFound.API.Services.Implementations
{
    public class ThingService : IThingService
    {
        public IThingRepository _thingRepository;

        public ThingService(IThingRepository thingRepository)
        {
            _thingRepository = thingRepository;
        }

        public async Task<BaseResponse<List<Thing>>> GetThingsByUserName(string login)
        {
            var things = _thingRepository.Select().Result.Where(x => x.UserName == login).ToList();
            return new BaseResponse<List<Thing>>
            {
                Data = things,
            };
        }

        public async Task<BaseResponse<List<Thing>>> GetThings()
        {
            return new BaseResponse<List<Thing>>
            {
                StatusCode = Domian.Enum.StatusCode.OK,
                Data = await _thingRepository.Select(),
            };
        }

        public async Task<BaseResponse<string>> SetThing(Thing thing)
        {
            await _thingRepository.Create(thing);
            return new BaseResponse<string>
            {
                Data = "Success",
            };
        }

        public async Task<BaseResponse<string>> DeleteThing(string login, string name)
        {
            try
            {
                var thing = _thingRepository.Select().Result.FirstOrDefault(x => x.UserName == login && x.Name == name);
                await _thingRepository.Delete(thing);

                return new BaseResponse<string>()
                {
                    StatusCode = Domian.Enum.StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<string>()
                {
                    StatusCode = Domian.Enum.StatusCode.IternalServerError,
                    Data = ex.Message,
                };
            }
        }
    }
}
