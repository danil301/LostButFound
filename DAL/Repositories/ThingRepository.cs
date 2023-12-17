using LostButFound.API.DAL.Interfaces;
using LostButFound.API.Domian;
using Microsoft.EntityFrameworkCore;

namespace LostButFound.API.DAL.Repositories
{
    public class ThingRepository : IThingRepository
    {
        public ApplicationDbContext _db;

        public ThingRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(Thing entity)
        {
            try
            {
                _db.Things.Add(entity);
                await _db.SaveChangesAsync(); // Дождаться завершения сохранения изменений
                return true; // Вернуть true только если сохранение прошло успешно
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false; // Вернуть false в случае ошибки
            }
        }


        public async Task<bool> Delete(Thing entity)
        {
            _db.Things.Remove(entity);
            _db.SaveChangesAsync();

            return true;
        }

        public async Task<Thing> Get(int id)
        {
            return await _db.Things.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Thing>> Select()
        {
            return await _db.Things.ToListAsync();
        }

        public Task<bool> Update(Thing entity)
        {
            throw new NotImplementedException();
        }
    }
}
