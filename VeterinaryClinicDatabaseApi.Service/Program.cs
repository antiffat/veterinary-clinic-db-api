using Microsoft.OpenApi.Models;
using VeterinaryClinicDatabaseApi.Service.Models;
using VeterinaryClinicDatabaseApi.Service.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Veterinary Clinic API", Version = "v1" });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/animals", async (IAnimalRepository repo, string orderBy = "Name") =>
    await repo.GetAllAnimalAsync(orderBy))
    .WithName("GetAllAnimals")
    .Produces<List<Animal>>(StatusCodes.Status200OK);

app.MapPost("/api/animals", async (IAnimalRepository repo, Animal animal, HttpContext httpContext) =>
{
    try
    {
        var addedAnimal = await repo.AddAnimalAsync(animal);
        return Results.Created($"/api/animals/{addedAnimal.IdAnimal}", addedAnimal);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
});

app.MapGet("/api/animals/{id}", async (IAnimalRepository repo, int id) =>
{
    var animal = await repo.GetAnimalByIdAsync(id);
    if (animal == null)
    {
        return Results.NotFound($"Animal with ID {id} is not found");
    }

    return Results.Ok(animal);
}).WithName("GetAnimalById")
.Produces<Animal>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapPut("/api/animals/{id}", async (IAnimalRepository repo, Animal animal, int id, HttpContext httpContext) =>
    {
        if (animal.IdAnimal != id)
        {
            return Results.BadRequest("ID mismatch between URL and body.");
        }

        var existingAnimal = await repo.GetAnimalByIdAsync(id);
        if (existingAnimal == null)
        {
            var newAnimal = await repo.AddAnimalAsync(animal); // this was my mistake (line did not exist)
            return Results.Created($"/api/animals/{{newAnimal.IdAnimal}}", newAnimal); // now it will return 201
        }
        else
        {
            bool updated = await repo.UpdateAnimalAsync(animal);
            if (updated)
            {
                return Results.NoContent(); // it will return 204 
            }
            else
            {
                return Results.BadRequest("Update failed, check the provided data.");
            }
        }
    })
    .WithName("UpdateAnimal")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status400BadRequest);

app.Run();