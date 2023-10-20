﻿using LostButFound.API.DAL.Interfaces;
using LostButFound.API.Domian;
using Microsoft.EntityFrameworkCore;

namespace LostButFound.API.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        public ApplicationDbContext _db;

        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(User entity)
        {
            _db.Users.Add(entity);
            _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Delete(User entity)
        {
            _db.Users.Remove(entity);
            _db.SaveChangesAsync();

            return true;
        }

        public async Task<User> Get(int id)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User> GetByName(string name)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.Login == name);
        }

        public async Task<List<User>> Select()
        {
            return await _db.Users.ToListAsync();
        }
    }
}
