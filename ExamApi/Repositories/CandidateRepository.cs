using System.Reflection;
using Microsoft.EntityFrameworkCore;
namespace ExamApi.Repositories;


public class CandidateRepository(ApiContext context) : ICandidateRepository
{
    // Repository needs DbContext to access the database (DI)
    private readonly ApiContext _context = context;

    public async Task<List<Candidate>> GetAllAsync()
    {
        // SELECT * FROM Candidates
        return await _context.Candidates.ToListAsync();
    }

    public async Task<Candidate?> GetByIdAsync(int id)
    {
        // SELECT * FROM Candidates WHERE Id = @id LIMIT 1
        return await _context.Candidates
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Candidate?> GetByEmailAsync(string email)
    {
        return await _context.Candidates
            .FirstOrDefaultAsync(c => c.Email == email);
    }
    
    public async Task AddAsync(Candidate candidate)
    {
        // Add entity to EF change tracker (not database yet)
        _context.Candidates.Add(candidate);
        // INSERT happens only when SaveChangesAsync is called
        await _context.SaveChangesAsync();
    }


    public async Task DeleteAsync(int id)
    {
        // Find candidate in DB
        var candidate = await _context.Candidates
            .FirstOrDefaultAsync(c => c.Id == id);

        if (candidate == null)
            throw new Exception($"Candidate with id {id} not found");

        // Mark entity for deletion
        _context.Candidates.Remove(candidate);

        // Executes DELETE in DB
        await _context.SaveChangesAsync();

    }
}
