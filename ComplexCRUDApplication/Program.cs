using AutoMapper;
using ComplexCRUDApplication.Helper;
using ComplexCRUDApplication.Models;
using ComplexCRUDApplication.Repos;
using ComplexCRUDApplication.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

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
builder.Services.AddTransient<IRefreshHandler, RefreshHandler>();
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));

var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingHandler()));
IMapper mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);

// register the basic authentication
// builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

// register the JWT authentication
var securityKey = builder.Configuration.GetSection("JwtSettings:SecurityKey").Value;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)),
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

// declare the cors policy
/*builder.Services.AddCors(cors => cors.AddPolicy("corspolicy", build => {
    build.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
}));*/

// declare the default cors, allow for any domain
builder.Services.AddCors(cors => cors.AddDefaultPolicy(build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddRateLimiter(opt =>
    opt.AddFixedWindowLimiter(policyName: "fixedWindow", options =>
    {
        options.Window = TimeSpan.FromSeconds(1);
        options.PermitLimit = 1;
        options.QueueLimit = 1;
        options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
    }).RejectionStatusCode = 600
);

var _jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(_jwtSettings);

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

app.UseRateLimiter();

// enable the authentication
app.UseAuthentication();

// enable this middleware to enable the static files
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
