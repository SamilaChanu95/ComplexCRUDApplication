using AutoMapper;
using ComplexCRUDApplication.Helper;
using ComplexCRUDApplication.Repos;
using ComplexCRUDApplication.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

/*builder.Services.AddSerilog(options => {
    options.MinimumLevel.Debug()
        .WriteTo.Console()
        .WriteTo.File(builder.Configuration.GetSection("Logging:LoggingPath").Value, rollingInterval: RollingInterval.Day);
});*/

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));

var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingHandler()));
IMapper mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
