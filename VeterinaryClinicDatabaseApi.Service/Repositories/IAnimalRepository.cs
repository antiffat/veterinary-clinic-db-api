using VeterinaryClinicDatabaseApi.Service.Models;

namespace VeterinaryClinicDatabaseApi.Service.Repositories;

public interface IAnimalRepository
{
    Task<Animal> AddAnimalAsync(Animal animal);
    Task<List<Animal>> GetAllAnimalAsync(string orderBy);
}