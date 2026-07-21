using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; // Required for Swagger fixes
using Saas_Micro_Finance.DataAccess.Data;
using Saas_Micro_Finance.DataAccess.Stores;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.API.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Saas_Micro_Finance.DataAccess.Repository;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using System.Text;
using Saas_Micro_Finance.API.Swagger;
using Saas_Micro_Finance.Utility.Services.Interface;
using Saas_Micro_Finance.Utility.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Connection Strings
var masterConnection = builder.Configuration.GetConnectionString("MasterConnection");

// 2. Register Contexts
builder.Services.AddDbContext<AdminDbContext>(options =>
    options.UseSqlServer(masterConnection));

builder.Services.AddDbContext<SaasBankDbContext>((sp, options) => {
    var tenant = sp.GetRequiredService<IMultiTenantContextAccessor<AppTenantInfo>>()
                   .MultiTenantContext?.TenantInfo;
    // This switches the connection string dynamically!
    options.UseSqlServer(tenant?.ConnectionString ?? masterConnection);
});

// 3. Finbuckle Custom Store
builder.Services.AddMultiTenant<AppTenantInfo>()
    .WithHeaderStrategy("__tenant__")
    .WithStore<MyCustomTenantStore>(ServiceLifetime.Singleton);

// 4. Identity - Point to SaasBankDbContext for multi-tenant support
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<SaasBankDbContext>() // CHANGED: Use SaasBank here
    .AddDefaultTokenProviders();

// 5. JWT Authentication
var jwtKey = builder.Configuration["JwtSettings:Key"]
    ?? throw new InvalidOperationException("JwtSettings:Key is missing from appsettings.json.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        )
    };
});

builder.Services.AddAuthorization();

// 6. Swagger with Authorize Button
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Saas Micro Finance API", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
    c.OperationFilter<TenantHeaderOperationFilter>();
});

// 7. Register Repositories and Unit of Work
builder.Services.AddScoped<IMasterUnitOfWork, MasterUnitOfWork>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IPaystackService, PaystackService>();
builder.Services.AddScoped<IFlutterwaveService, FlutterwaveService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<ILedgerService, LedgerService>();
builder.Services.AddScoped<TenantResolverService>();
builder.Services.AddScoped<TenantMigrationService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers()
    .AddApplicationPart(typeof(TenantController).Assembly);


// 8. CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowReact", p =>
        p.WithOrigins("http://localhost:5173")
         .AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials());
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 1. First, enable the routing engine
app.UseRouting();

// 2. Enable CORS
app.UseCors("AllowReact");

// 3. Multi-Tenancy (Now it knows which route is being called)
app.UseMultiTenant();

// 4. Identity & Permissions
app.UseAuthentication();
app.UseAuthorization();

// 5. Finally, execute the controller logic
app.MapControllers();

app.Run();