using CampusAI.VectorPipeline.Configuration;
using CampusAI.VectorPipeline.Embeddings;
using CampusAI.VectorPipeline.Extensions;
using CampusAI.VectorPipeline.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddVectorPipeline();
builder.Services.AdModuleServices();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<PineconeOptions>(
  builder.Configuration.GetSection("VectorStore:Pinecone"));
builder.Services.AddScoped<IPineconeService, PineconeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Enable Swagger in Development (or always if you want)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// If you want Swagger always enabled, remove the if block and use:
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
