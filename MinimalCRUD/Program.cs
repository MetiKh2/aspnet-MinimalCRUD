using Microsoft.EntityFrameworkCore;
using MinimalCRUD;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
async Task<List<SuperHero>> GetAllHeroes(DataContext context) =>
    await context.SuperHeroes.ToListAsync();
async Task<SuperHero> GetHero(DataContext context, int id) =>
    await context.SuperHeroes.FirstOrDefaultAsync(p => p.Id == id);
app.UseHttpsRedirection();
app.MapGet("/", () => "hey");
app.MapGet("/superHeros", async (DataContext context) =>
{
    return Results.Ok(await GetAllHeroes(context));
});
app.MapGet("/superHero/{id}", async (DataContext context, int id) =>
{
    var hero = await GetHero(context, id);
    if (hero == null) Results.NotFound();
   return Results.Ok(hero);
});

app.MapPost("/superHero", async (DataContext context, SuperHero hero) =>
{
    await context.AddAsync(hero);
    await context.SaveChangesAsync();
    return Results.Ok(hero);
});
app.MapPut("/superHero/{id}", async (DataContext context, int id, SuperHero hero) =>
{
    if (id != hero.Id)return Results.NotFound();
    context.Update(hero);
    await context.SaveChangesAsync();
    return Results.Ok(hero);
});
app.Run();