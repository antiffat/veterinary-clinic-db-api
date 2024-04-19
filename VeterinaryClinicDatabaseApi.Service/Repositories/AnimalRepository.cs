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

    public async Task<Animal> AddAnimalAsync(Animal animal)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string sql = "INSERT INTO Animal (Name, Description, Category, Area) " +
                         "OUTPUT INSERTED.IdAnimal VALUES (@Name, @Description, @Category, @Area)";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Name", animal.Name);
                command.Parameters.AddWithValue("@Description", animal.Description);            
                command.Parameters.AddWithValue("@Category", animal.Category);
                command.Parameters.AddWithValue("@Area", animal.Area);

                animal.IdAnimal = (int)await command.ExecuteScalarAsync();
            }
        }

        return animal;
    }

    public async Task<List<Animal>> GetAllAnimalAsync(string orderBy)
    {
        List<Animal> animals = new List<Animal>();
        var allowedColumns = new List<string> { "Name", "Description", "Category", "Area" };
        if (!allowedColumns.Contains(orderBy))
        {
            orderBy = "Name";
        }
        
        string sql = $"SELECT IdAnimal, Name, Description, Category, Area " +
                     $"FROM Animal " +
                     $"ORDER BY {orderBy}";    
        
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(sql, connection);
            await connection.OpenAsync();

            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var animal = new Animal
                    {
                        IdAnimal = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Category = reader.GetString(3),
                        Area = reader.GetString(4)
                    };
                    animals.Add(animal);
                }
            }
        }

        return animals;
    }

    public async Task<Animal> GetAnimalByIdAsync(int idAnimal)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string sql = "SELECT * FROM Animal WHERE IdAnimal = @IdAnimal";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@IdAnimal", idAnimal);
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Animal
                        {
                            IdAnimal = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Category = reader.GetString(3),
                            Area = reader.GetString(4)
                        };
                    }
                }
            }
        }

        return null;
    }

    public async Task<bool> UpdateAnimalAsync(Animal animal)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string sql = @"UPDATE Animal SET Name = @Name, Description = @Description, Category = @Category, Area = @Area WHERE IdAnimal = @IdAnimal";
            
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@IdAnimal", animal.IdAnimal);
                command.Parameters.AddWithValue("@Name", animal.Name);
                command.Parameters.AddWithValue("@Description", animal.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Category", animal.Category);
                command.Parameters.AddWithValue("@Area", animal.Area);

                await connection.OpenAsync();
                int affectedRows = await command.ExecuteNonQueryAsync();
                return affectedRows > 0;
            }
        }
    }
}