using LostButFound.API.Domian;
using Microsoft.EntityFrameworkCore;

namespace LostButFound.API.DAL.Repositories
{
    public class ThingRepository
    {
        public ApplicationDbContext _db;

        public ThingRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(Thing entity)
        {
            _db.Things.Add(entity);
            _db.SaveChangesAsync();

            return true;
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
    }
}
