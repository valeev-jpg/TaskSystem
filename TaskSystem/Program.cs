using System.Text.Json.Serialization;
using TaskSystem.DataAccessLayer;
using TaskSystem.Extensions;
using TaskSystem.Middleware;


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddJwtSwagger(builder.Configuration);
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);


const string origin = "MyAllowSpecificOrigins";
builder.Services.AddCorsPolicy(builder.Configuration, origin);

builder.Services.AddSerilogLogging();
builder.Services.AddFluentValidation();
builder.Services.AddAutoMapper(typeof(Program).Assembly);


var app = builder.Build();

// Configure the HTTP request pipeline.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ServiceDbContext>();
    DbInitializer.Initialize(context);
}

app.UseCors(origin);

app.UseSwagger();
app.UseSwaggerUI();

app.UseCustomLoggingHandler();
app.UseCustomExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();