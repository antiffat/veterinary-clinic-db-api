namespace VeterinaryClinicDatabaseApi.Service.Models;

public class Animal
{
    public int IdAnimal { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; }
    public string Area { get; set; }
}