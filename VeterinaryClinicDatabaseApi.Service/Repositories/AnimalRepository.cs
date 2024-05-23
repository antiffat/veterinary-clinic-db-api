using VeterinaryClinicDatabaseApi.Service.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VeterinaryClinicDatabaseApi.Service.Helpers;

namespace VeterinaryClinicDatabaseApi.Service.Repositories;

public class AnimalRepository : IAnimalRepository
{
    private readonly VeterinaryClinicContext _context;

    public AnimalRepository(VeterinaryClinicContext context)
    {
        _context = context;
    }

    public async Task<Animal> AddAnimalAsync(Animal animal)
    {
        _context.Animals.Add(animal);
        await _context.SaveChangesAsync();
        return animal;
    }

    public async Task<List<Animal>> GetAllAnimalAsync(string orderBy)
    {

        if (string.IsNullOrEmpty(orderBy))
        {
            orderBy = "Name";
        }
        
        return await _context.Animals.OrderBy(e =>
            EF.Property<object>(e, orderBy)).ToListAsync();

    }

    public async Task<Animal> GetAnimalByIdAsync(int idAnimal)
    {
        return await _context.Animals.FindAsync(idAnimal);
    }

    public async Task<bool> UpdateAnimalAsync(Animal animal)
    {
        var trackedAnimal = await _context.Animals.FindAsync(animal.IdAnimal);
        if (trackedAnimal != null)
        {
            _context.Entry(trackedAnimal).State = EntityState.Detached;
        }

        _context.Entry(animal).State = EntityState.Modified;
        return await _context.SaveChangesAsync() > 0;
    }
}