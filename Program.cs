using Microsoft.EntityFrameworkCore;
using SampleMinimalAPI.Data;
using SampleMinimalAPI.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add dbContext 
builder.Services.AddDbContext<MyDataContext>(opt => opt.UseInMemoryDatabase("Student"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

//Student APIs
app.MapPost("/SaveStudent", async (Student student, MyDataContext db) =>
{
    db.Students.Add(student);
    await db.SaveChangesAsync();

    return Results.Created($"/save/{student.Id}", student);
});

app.MapGet("/GetAllStudent", async (MyDataContext db) =>
    await db.Students.ToListAsync());

app.MapGet("/GetStudentById/{id}", async (int id, MyDataContext db) =>
    await db.Students.FindAsync(id)
        is Student student
            ? Results.Ok(student)
            : Results.NotFound());

app.MapPut("/UpdateStudents/{id}", async (int id, Student studentinput, MyDataContext db) =>
{
    var student = await db.Students.FindAsync(id);

    if (student is null) return Results.NotFound();

    student.Name = studentinput.Name;
    student.Phone = studentinput.Phone;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/DeleteStudent/{id}", async (int id, MyDataContext db) =>
{
    if (await db.Students.FindAsync(id) is Student student)
    {
        db.Students.Remove(student);
        await db.SaveChangesAsync();
        return Results.Ok(student);
    }

    return Results.NotFound();
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
