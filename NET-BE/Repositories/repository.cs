using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NET_BE.Data;
using NET_BE.Model;

namespace NET_BE.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AttendanceDbContext _context;

        public Repository(AttendanceDbContext context)
        {
            _context = context;
        }

        public async Task<PagedModel<T>> GetPagedAsync(int pageIndex, int pageSize)
        {
            var query = _context.Set<T>().AsQueryable();
            var totalCount = await query.CountAsync();
            var data = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedModel<T>(totalCount, data, pageIndex, pageSize);
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with ID {id} not found.");
            }

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}