using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NovelExchangeApi.Model;
using NovelExchangeApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "novelExchangeApi", Description = "The library of your life!", Version = "v1" });
});

var connectionString = builder.Configuration.GetConnectionString("AddDbContext");

builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI(c =>
   {
      c.SwaggerEndpoint("/swagger/v1/swagger.json", "Novel Exchange API V1");
   });
}

// app.UseHttpsRedirection();

// Map root endpoint
app.MapGet("/", () => "Hello world!");

// Book CRUD
app.MapGet("/books", async (AppDbContext db) => await db.Books.ToListAsync());
app.MapGet("/books/{id}", async (Guid id, AppDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    return book is null ? Results.NotFound("Not exist!") : Results.Ok(book);
});

app.MapPost("/books", async (Book book, AppDbContext db) =>
{
    book.CreatedAt = DateTime.UtcNow;

    db.Books.Add(book);
    await db.SaveChangesAsync();
    return Results.Created($"/books/{book.Id}", book);
});

app.MapPut("/books/{id}", async (Guid id, Book updatedBook, AppDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound();

    book.Title = updatedBook.Title;
    book.Volume = updatedBook.Volume;
    book.ReleaseYear = updatedBook.ReleaseYear;
    book.Description = updatedBook.Description;
    book.Genre = updatedBook.Genre;
    book.Series = updatedBook.Series;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/books/{id}", async (Guid id, AppDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound("Not exist!");

    db.Books.Remove(book);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Author CRUD
app.MapGet("/authors", async (AppDbContext db) => await db.Authors.ToListAsync());
app.MapGet("/authors/{id}", async (Guid id, AppDbContext db) =>
{
    var author = await db.Authors.FindAsync(id);
    return author is null ? Results.NotFound("Not exist!") : Results.Ok(author);
});

app.MapPost("/authors", async (Author author, AppDbContext db) =>
{
    author.CreatedAt = DateTime.UtcNow;

    db.Authors.Add(author);
    await db.SaveChangesAsync();
    return Results.Created($"/authors/{author.Id}", author);
});

app.MapPut("/authors/{id}", async (Guid id, Author updatedAuthor, AppDbContext db) =>
{
    var author = await db.Authors.FindAsync(id);
    if (author is null) return Results.NotFound();

    author.FirstName = updatedAuthor.FirstName;
    author.LastName = updatedAuthor.LastName;
    author.Description = updatedAuthor.Description;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/authors/{id}", async (Guid id, AppDbContext db) =>
{
    var author = await db.Authors.FindAsync(id);
    if (author is null) return Results.NotFound("Not exist!");

    db.Authors.Remove(author);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Review CRUD
app.MapGet("/reviews", async (AppDbContext db) => await db.Reviews.ToListAsync());
app.MapGet("/reviews/{id}", async (Guid id, AppDbContext db) =>
{
    var review = await db.Reviews.FindAsync(id);
    return review is null ? Results.NotFound("Not exist!") : Results.Ok(review);
});

app.MapPost("/reviews", async (Review review, AppDbContext db) =>
{
    review.CreatedAt = DateTime.UtcNow;

    db.Reviews.Add(review);
    await db.SaveChangesAsync();
    return Results.Created($"/reviews/{review.Id}", review);
});

app.MapPut("/reviews/{id}", async (Guid id, Review updatedReview, AppDbContext db) =>
{
    var review = await db.Reviews.FindAsync(id);
    if (review is null) return Results.NotFound();

    review.Title = updatedReview.Title;
    review.Description = updatedReview.Description;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/reviews/{id}", async (Guid id, AppDbContext db) =>
{
    var review = await db.Reviews.FindAsync(id);
    if (review is null) return Results.NotFound("Not exist!");

    db.Reviews.Remove(review);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// User CRUD
app.MapGet("/users", async (AppDbContext db) => await db.Users.ToListAsync());
app.MapGet("/users/{id}", async (Guid id, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    return user is null ? Results.NotFound("Not exist!") : Results.Ok(user);
});

app.MapPost("/users", async (User user, AppDbContext db) =>
{
    user.CreatedAt = DateTime.UtcNow;

    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
});

app.MapPut("/users/{id}", async (Guid id, User updatedUser, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();

    user.Email = updatedUser.Email;
    user.FirstName = updatedUser.FirstName;
    user.LastName = updatedUser.LastName;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/users/{id}", async (Guid id, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound("Not exist!");

    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
