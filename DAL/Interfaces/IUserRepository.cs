using LostButFound.API.Domian;

namespace LostButFound.API.DAL.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByName(string name);
    }
}
