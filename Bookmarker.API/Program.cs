using Bookmarker.API.Services;
using Bookmarker.Contracts.Base;
using Bookmarker.Data.Repositories;
using Bookmarker.Model;
using Bookmarker.Search;
using Bookmarker.Workers;
using Meilisearch;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddDbContext<BookmarkerContext>(
    options => options.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") 
                                 ?? throw new ArgumentNullException(nameof(options))), ServiceLifetime.Transient);

// Repositories
builder.Services.AddTransient<BookmarkRepository>();
builder.Services.AddTransient<BookmarkIndexRepository>();

builder.Services.AddTransient<CategoryRepository>();

// Services
builder.Services.AddTransient<BookmarkService>();
builder.Services.AddTransient<CategoryService>();

// MeiliSearch
builder.Services.AddTransient(_ => new MeilisearchClient(Environment.GetEnvironmentVariable("MEILISEARCH_URL"), Environment.GetEnvironmentVariable("MEILISEARCH_KEY")));
builder.Services.AddTransient(serviceProvider => new IndexWrapper<SearchBookmark>(serviceProvider.GetRequiredService<MeilisearchClient>().Index("bookmark")));

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