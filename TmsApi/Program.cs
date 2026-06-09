var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Add services to the container.

builder.Services.AddControllers();


var app = builder.Build();
app.UseRouting();
app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/api/assessments/results", () => Results.Ok(new {
    courseCode = "CS-101",
    studentId = "S-001",
    letterGrade = "A"
}));
// Configure the HTTP request pipeline.



app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

app.Run();
