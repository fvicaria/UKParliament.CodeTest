
using Microsoft.EntityFrameworkCore;

using UKParliament.CodeTest.Common.Entities;
using UKParliament.CodeTest.Common.Interfaces;

namespace UKParliament.CodeTest.Data.Repositories
{
    public class EfPersonRepository : IPersonRepository
    {
        private readonly PersonManagerContext _context;

        public EfPersonRepository(PersonManagerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            // Using AsNoTracking for read-only query => performance gain
            return await _context.People
                                 .AsNoTracking()
                                 .Include(p => p.Department)
                                 .OrderBy(p => p.LastName)
                                 .ToListAsync();
        }

        public async Task<Person?> GetByIdAsync(int id)
        {
            return await _context.People
                                 .Include(p => p.Department)
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Person> AddAsync(Person person)
        {
            _context.People.Add(person);
            await _context.SaveChangesAsync();
            return person;
        }

        public async Task<Person?> UpdateAsync(Person person)
        {
            var existingPerson = await _context.People.FindAsync(person.Id);
            if (existingPerson == null)
            {
                return null; // Nothing to update
            }

            _context.Entry(existingPerson).CurrentValues.SetValues(person);
            await _context.SaveChangesAsync();
            return existingPerson;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingPerson = await _context.People.FindAsync(id);
            if (existingPerson == null)
            {
                return false; // Nothing to delete
            }

            _context.People.Remove(existingPerson);
            await _context.SaveChangesAsync();
            return true; // Successfully deleted
        }
    }
}
