using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NovelExchangeApi.Data;
using NovelExchangeApi.Model;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo {
            Title = "novelExchangeApi",
            Description = "The library of your life!",
            Version = "v1",
         }
    );
});

var connectionString = builder.Configuration.GetConnectionString("AddDbContext");

builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.Configure<JsonOptions>(o =>
{
    o.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI(c =>
   {
      c.SwaggerEndpoint("/swagger/v1/swagger.json", "Novel Exchange API V1");
   });
}

app.UseHttpsRedirection();

// Book CRUD
app.MapGet("/books", async (AppDbContext db) => {
    var books = await db.Books
        .Include(b => b.Reviews)
        .Include(b => b.Users)
        .ToListAsync(); 
    
    return Results.Ok(books);
});
app.MapGet("/books/{id}", async (Guid id, AppDbContext db) =>
{
    var book = await db.Books
        .Include(b => b.Reviews)
        .Include(b => b.Users)
        .FirstOrDefaultAsync(b => b.Id == id);

    return book is null ? Results.NotFound("Not exist!") : Results.Ok(book);
});
app.MapPost("/books", async (Book book, AppDbContext db) =>
{
    var author = await db.Authors.AnyAsync(a => a.Id == book.AuthorId);
    
    if (!author)
    {
        return Results.NotFound($"Author with ID {book.AuthorId} does not exist.");
    }

    book.CreatedAt = DateTime.UtcNow;

    db.Books.Add(book);
    await db.SaveChangesAsync();
    return Results.Created($"/books/{book.Id}", book);
});
app.MapPut("/books/{id}", async (Guid id, Book updatedBook, AppDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound();

    if (!string.IsNullOrEmpty(updatedBook.Title)) book.Title = updatedBook.Title;
    if (!string.IsNullOrEmpty(updatedBook.Volume)) book.Volume = updatedBook.Volume;
    if (updatedBook.ReleaseYear.HasValue) book.ReleaseYear = updatedBook.ReleaseYear;
    if (!string.IsNullOrEmpty(updatedBook.Description)) book.Description = updatedBook.Description;
    if (!string.IsNullOrEmpty(updatedBook.Genre)) book.Genre = updatedBook.Genre;
    if (!string.IsNullOrEmpty(updatedBook.Series)) book.Series = updatedBook.Series;

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
app.MapGet("/authors", async (AppDbContext db) => {
    var authors = await db.Authors
        .Include(a => a.Users)
        .Include(a => a.Books)
        .ToListAsync();

    return Results.Ok(authors);
});
app.MapGet("/authors/{id}", async (Guid id, AppDbContext db) =>
{
    var author = await db.Authors
        .Include(a => a.Users)
        .Include(a => a.Books)
        .FirstOrDefaultAsync(b => b.Id == id);

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

    if (!string.IsNullOrEmpty(updatedAuthor.FirstName)) author.FirstName = updatedAuthor.FirstName;
    if (!string.IsNullOrEmpty(updatedAuthor.LastName)) author.LastName = updatedAuthor.LastName;
    if (!string.IsNullOrEmpty(updatedAuthor.Description)) author.Description = updatedAuthor.Description;

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
    var user = await db.Users.AnyAsync(r => r.Id == review.UserId);
    var book = await db.Books.AnyAsync(r => r.Id == review.BookId);

    if (!user)
    {
        return Results.NotFound($"User with ID {review.UserId} does not exist.");
    }

    if (!book)
    {
        return Results.NotFound($"Book with ID {review.BookId} does not exist.");
    }

    review.CreatedAt = DateTime.UtcNow;

    db.Reviews.Add(review);
    await db.SaveChangesAsync();
    return Results.Created($"/reviews/{review.Id}", review);
});
app.MapPut("/reviews/{id}", async (Guid id, Review updatedReview, AppDbContext db) =>
{
    var review = await db.Reviews.FindAsync(id);
    if (review is null) return Results.NotFound();

    if (!string.IsNullOrEmpty(updatedReview.Title)) review.Title = updatedReview.Title;
    if (!string.IsNullOrEmpty(updatedReview.Description)) review.Description = updatedReview.Description;

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
app.MapGet("/users", async (AppDbContext db) => {
    var users = await db.Users
        .Include(u => u.Books)
        .Include(u => u.Authors)
        .Include(u => u.Reviews)
        .ToListAsync();

    return Results.Ok(users);
});
app.MapGet("/users/{id}", async (Guid id, AppDbContext db) =>
{
    var user = await db.Users
        .Include(u => u.Books)
        .Include(u => u.Authors)
        .Include(u => u.Reviews)
        .FirstOrDefaultAsync(b => b.Id == id);

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

    if (!string.IsNullOrEmpty(updatedUser.Email)) user.Email = updatedUser.Email;
    if (!string.IsNullOrEmpty(updatedUser.FirstName)) user.FirstName = updatedUser.FirstName;
    if (!string.IsNullOrEmpty(updatedUser.LastName)) user.LastName = updatedUser.LastName;

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

// One User CRUD favorite books
app.MapGet("/users/{id}/books", async (Guid id, AppDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(b => b.Id == id);

    if (user is null)
    {
        return Results.NotFound("User does not exist!");
    }

    var books = await db.Books
        .Include(b => b.Reviews)
        .Include(b => b.Users)
        .Where(b => b.Users.Any(u => u.Id == id))
        .ToListAsync(); 
    
    return Results.Ok(books);
});
app.MapPost("/users/{userId}/books/{bookId}", async (Guid userId, Guid bookId, AppDbContext db) =>
{
    var user = await db.Users.Include(u => u.Books).FirstOrDefaultAsync(u => u.Id == userId);
    var book = await db.Books.FirstOrDefaultAsync(b => b.Id == bookId);

    if (user is null)
    {
        return Results.NotFound("User does not exist!");
    }

    if (book is null)
    {
        return Results.NotFound("Book does not exist!");
    }

    if (user.Books.Any(b => b.Id == bookId))
    {
        return Results.BadRequest("This book is already in the user's collection.");
    }

    user.Books.Add(book);

    await db.SaveChangesAsync();

    return Results.NoContent();
});
app.MapDelete("/users/{userId}/books/{bookId}", async (Guid userId, Guid bookId, AppDbContext db) =>
{
    var user = await db.Users.Include(u => u.Books).FirstOrDefaultAsync(b => b.Id == userId);
    var book = await db.Books.FirstOrDefaultAsync(b => b.Id == bookId);

    if (user is null)
    {
        return Results.NotFound("User does not exist!");
    }

    if (book is null)
    {
        return Results.NotFound("Book does not exist!");
    }

    if (!user.Books.Any(b => b.Id == bookId))
    {
        return Results.BadRequest("This book is not in the user's collection.");
    }

    user.Books.Remove(book);

    await db.SaveChangesAsync();

    return Results.NoContent();
});

// One User CRUD favorite authors
app.MapGet("/users/{id}/authors", async (Guid id, AppDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(b => b.Id == id);

    if (user is null)
    {
        return Results.NotFound("User does not exist!");
    }

    var authors = await db.Authors
        .Include(a => a.Users)
        .Include(a => a.Books)
        .Where(b => b.Users.Any(u => u.Id == id))
        .ToListAsync(); 
    
    return Results.Ok(authors);
});
app.MapPost("/users/{userId}/authors/{authorId}", async (Guid userId, Guid authorId, AppDbContext db) =>
{
    var user = await db.Users.Include(u => u.Authors).FirstOrDefaultAsync(u => u.Id == userId);
    var author = await db.Authors.FirstOrDefaultAsync(a => a.Id == authorId);

    if (user is null)
    {
        return Results.NotFound("User does not exist!");
    }

    if (author is null)
    {
        return Results.NotFound("Author does not exist!");
    }

    if (user.Authors.Any(a => a.Id == authorId))
    {
        return Results.BadRequest("This author is already in the user's collection.");
    }

    user.Authors.Add(author);

    await db.SaveChangesAsync();

    return Results.NoContent();
});
app.MapDelete("/users/{userId}/authors/{authorId}", async (Guid userId, Guid authorId, AppDbContext db) =>
{
    var user = await db.Users.Include(u => u.Authors).FirstOrDefaultAsync(u => u.Id == userId);
    var author = await db.Authors.FirstOrDefaultAsync(b => b.Id == authorId);

    if (user is null)
    {
        return Results.NotFound("User does not exist!");
    }

    if (author is null)
    {
        return Results.NotFound("Author does not exist!");
    }

    if (!user.Authors.Any(b => b.Id == authorId))
    {
        return Results.BadRequest("This author is not in the user's collection.");
    }

    user.Authors.Remove(author);

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
