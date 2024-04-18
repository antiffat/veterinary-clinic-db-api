using Microsoft.OpenApi.Models;
using VeterinaryClinicDatabaseApi.Service.Models;
using VeterinaryClinicDatabaseApi.Service.Repositories;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();  // Registering the AnimalRepository
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

app.Run();