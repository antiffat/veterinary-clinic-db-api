using VeterinaryClinicDatabaseApi.Service.Models;

namespace VeterinaryClinicDatabaseApi.Service.Repositories;

public interface IAnimalRepository
{
    Task<List<Animal>> GetAllAnimalAsync(string orderBy);
}