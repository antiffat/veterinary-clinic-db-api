using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using VeterinaryClinicDatabaseApi.Service.Helpers;
using VeterinaryClinicDatabaseApi.Service.Models;
using VeterinaryClinicDatabaseApi.Service.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<VeterinaryClinicContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Veterinary Clinic API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/animals", async (IAnimalRepository repo, string orderBy = "Name") =>
{
    try
    {
        var animals = await repo.GetAllAnimalAsync(orderBy);
        return Results.Ok(animals);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in GET /api/animals: {ex}");
        return Results.Problem(ex.Message, statusCode: 500);
    }
});

app.MapPost("/api/animals", async (IAnimalRepository repo, Animal animal) =>
{
    try
    {
        var addedAnimal = await repo.AddAnimalAsync(animal);
        return Results.Created($"/api/animals/{addedAnimal.IdAnimal}", addedAnimal);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in POST /api/animals: {ex}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
        }
        return Results.Problem(ex.Message, statusCode: 500);
    }
});

app.MapGet("/api/animals/{id}", async (IAnimalRepository repo, int id) =>
{
    try
    {
        var animal = await repo.GetAnimalByIdAsync(id);
        if (animal == null)
        {
            return Results.NotFound($"Animal with ID {id} is not found");
        }
        return Results.Ok(animal);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in GET /api/animals/{id}: {ex}");
        return Results.Problem(ex.Message, statusCode: 500);
    }
});

app.MapPut("/api/animals/{id}", async (IAnimalRepository repo, Animal animal, int id) =>
{
    if (animal.IdAnimal != id)
    {
        return Results.BadRequest("ID mismatch between URL and body.");
    }

    var existingAnimal = await repo.GetAnimalByIdAsync(id);
    if (existingAnimal == null)
    {
        return Results.NotFound($"Animal with ID {id} is not found.");
    }

    bool updated = await repo.UpdateAnimalAsync(animal);
    if (updated)
    {
        return Results.NoContent();
    }

    return Results.BadRequest("Update failed, check the provided data.");
});


app.Run();