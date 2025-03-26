using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using BillController;
using BillController.Configurations;
using BillController.Models;
using BillController.Models.Authentication;
using BillController.Repository;
using BillController.Repository.Realisation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers().AddJsonOptions(e=> e.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddTransient<ProblemRecorder>();

builder.Services.AddControllers();




 
 
builder.Services.Configure<DatabaseScopeConfigurations>(
    builder.Configuration.GetSection(DatabaseScopeConfigurations.SectionName));
     
builder.Services.AddExceptionHandler(e => e.ExceptionHandler = async (context) =>
{
    var exh = context.Features.GetRequiredFeature<IExceptionHandlerFeature>();
    var problem = new ProblemDetails()
    {
        Title = "We are working on problem",
        Detail = exh.Error.Message,
        Extensions = new Dictionary<string, object?>()
        {
            { "Current Activity",Activity.Current.OperationName}
        }
    };
    await Results.Problem(problem).ExecuteAsync(context);
});
var SerilogConfiguration = new LoggerConfiguration().WriteTo
    .File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs\\MainLog.txt"), LogEventLevel.Information)
    .WriteTo.Console(LogEventLevel.Information).Enrich.FromLogContext().CreateLogger();

builder.Host.UseSerilog(SerilogConfiguration);

builder.Services.AddDbContextPool<DataContext>(e =>

    e.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<IdentityDbContextTest>(e =>
    e.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

 
builder.Services.AddIdentityCore<AppUser>(e =>
    {
        e.Password = new PasswordOptions()
        {
            RequireDigit = false,
            RequireUppercase = false,
            RequiredLength = 10,
            RequireNonAlphanumeric = false
        };
      
         

    })
    .AddEntityFrameworkStores<IdentityDbContextTest>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(b =>
{
    var audience = builder.Configuration["JwtConfig:ValidAudiences"];
    var issuer = builder.Configuration["JwtConfig:ValidIssuer"];
    var secret = builder.Configuration["JwtConfig:Secret"];
    if (audience is null || issuer is null || secret is null)
        throw new ApplicationException("Some Token data is null");
    b.SaveToken = true;
    b.UseSecurityTokenValidators = true;
    b.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidAudience = audience,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidateIssuer = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
    };

});
  
//builder.Services.AddDbContext<IdentityDbContextTest>(e => e.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

builder.Services.AddTransient(typeof(IRepository<Category>), typeof(CategoryRepository));
builder.Services.AddTransient(typeof(IRepository<Bill>), typeof(BillRepository));
builder.Services.AddTransient(typeof(IRepository<CurrentAccount>), typeof(CurrentAccountRepository));

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CustomMiddleware>();
app.UseRouting();
app.Services.UseSomething();
app.UseExceptionHandler( );

 




app.UseSwagger();
app.UseSwaggerUI();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
