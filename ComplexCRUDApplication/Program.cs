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

// declare the cors policy
/*builder.Services.AddCors(cors => cors.AddPolicy("corspolicy", build => {
    build.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
}));*/

// declare the default cors, allow for any domain
builder.Services.AddCors(cors => cors.AddDefaultPolicy(build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

/*app.UseCors(options => {
    options.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
});*/

// Use cors policy
/*app.UseCors("corspolicy");*/

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
