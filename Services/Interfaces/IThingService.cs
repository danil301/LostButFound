using LostButFound.API.Domian;
using LostButFound.API.Domian.Response;

namespace LostButFound.API.Services.Interfaces
{
    public interface IThingService
    {
        Task<BaseResponse<List<Thing>>> GetThingsByUserName(string login);

        Task<BaseResponse<List<Thing>>> GetThings();

        Task<BaseResponse<string>> SetThing(Thing thing);

        Task<BaseResponse<string>> DeleteThing(string login, string name);
    }
}
