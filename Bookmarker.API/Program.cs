using System.Data;
using Bookmarker.API.Services;
using Bookmarker.Contracts.Base.Bookmark.Interfaces;
using Bookmarker.Contracts.Base.Bookmark.Search;
using Bookmarker.Contracts.Base.Category.Interfaces;
using Bookmarker.Data.Repositories;
using Bookmarker.Search;
using Bookmarker.Workers;
using Meilisearch;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add the controllers
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy  =>
        {
            policy.AllowAnyOrigin();
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
        });
});

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddTransient<IDbConnection>(_ => new NpgsqlConnection("Server=localhost;Port=5432;Database=bookmarker;User Id=postgres;Password=password;"));

// Repositories
builder.Services.AddTransient<IBookmarkRepository, BookmarkRepository>();
builder.Services.AddTransient<IBookmarkIndexRepository, BookmarkIndexRepository>();

builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();

// Services
builder.Services.AddTransient<IBookmarkService, BookmarkService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();

// MeiliSearch
builder.Services.AddTransient(_ => new MeilisearchClient("http://localhost:7700", "verysecurekey"));
builder.Services.AddTransient(serviceProvider => new IndexWrapper<BookmarkSearchable>(serviceProvider.GetRequiredService<MeilisearchClient>().Index("bookmark")));

// Workers
builder.Services.AddHostedService<BasicIndexingWorker>();
builder.Services.AddHostedService<IndexingStatusUpdateWorker>();
builder.Services.AddHostedService<BrokenLinkWorker>();
builder.Services.AddHostedService<DuplicatedLinkWorker>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();