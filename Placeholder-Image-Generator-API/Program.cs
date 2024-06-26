using Microsoft.AspNetCore.ResponseCompression;
using Placeholder_Image_Generator_API.Services.PlaceholderImageService;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Dependency Injection for our Service
builder.Services.AddTransient<IPlaceholderImageService, PlaceholderImageService>();

// GZip Compression
builder.Services.AddResponseCompression();
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

//Configuring Index.html as default page
//instead of swagger documentation
var options = new DefaultFilesOptions();
options.DefaultFileNames.Clear();
options.DefaultFileNames.Add("index.html");

app.UseDefaultFiles(options);

app.UseStaticFiles();
app.UseRouting();

app.MapControllers();

app.Run();
