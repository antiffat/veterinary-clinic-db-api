using VeterinaryClinicDatabaseApi.Service.Models;
using System.Data;
using System.Data.SqlClient;

namespace VeterinaryClinicDatabaseApi.Service.Repositories;

public class AnimalRepository : IAnimalRepository
{
    private readonly string _connectionString;

    public AnimalRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }
    }

    public async Task<List<Animal>> GetAllAnimalAsync(string orderBy)
    {
        List<Animal> animals = new List<Animal>();
        var allowedColumns = new List<string> { "Name", "Description", "Category", "Area" };
        if (!allowedColumns.Contains(orderBy))
        {
            throw new ArgumentException("Invalid order by parameter");
        }
        
        string sql = $"SELECT * FROM Animal ORDER BY {orderBy}";
        
        using (SqlConnection connection = new(_connectionString))
        {
            SqlCommand command = new SqlCommand(sql, connection);
            await connection.OpenAsync();

            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    animals.Add(new Animal
                    {
                        IdAnimal = reader.GetInt32("IdAnimal"),
                        Name = reader.GetString("Name"),
                        Description = reader.GetString("Description"),
                        Category = reader.GetString("Category"),
                        Area = reader.GetString("Area"),
                    });
                }
            }
        }

        return animals;
    }
}