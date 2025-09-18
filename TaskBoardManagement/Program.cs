using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog;
using System.Reflection;
using System.Text;
using TaskBoardManagement;
using TaskBoardManagement.AutoMapper;
using TaskBoardManagement.Data;
using TaskBoardManagement.ExceptionMiddleware;
using TaskBoardManagement.Middleware;
using TaskBoardManagement.Models;
using TaskBoardManagement.Repositories;
using TaskBoardManagement.Service;
using TaskBoardManagement.UnitofWork;



var builder = WebApplication.CreateBuilder(args);




// ✅ Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console() // human-readable in terminal
    .WriteTo.File("logs/taskboard-.txt", rollingInterval: RollingInterval.Day) // per-day file logs
                                                                               //.WriteTo.Seq("http://localhost:5341") // if using Seq locally
    .CreateLogger();

builder.Host.UseSerilog(); // replace default logging with Serilog



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "TaskBoard API",
        Version = "v1"
    });
    options.OperationFilter<RemoveResponseContentFilter>();

    // ✅ Add JWT Auth to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});




//Dependency Injection for DbContext    
builder.Services.AddDbContext<AppDbContext>(options=>
options.UseSqlServer(builder.Configuration.GetConnectionString("TaskBoardConnectionString")));


builder.Services.AddTransient<GlobalExceptionMiddleware>();


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

builder.Services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();
builder.Services.AddScoped<ITaskAssignmentRepository, TaskAssignmentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITaskTagRepository, TaskTagRepository>();
builder.Services.AddAutoMapper(typeof(MappingProfile)); // AutoMapper


// ✅ Add JWT Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,

//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
//        };

//        options.Events = new JwtBearerEvents
//        {
//            // Token is malformed, expired, wrong signature, etc.
//            OnAuthenticationFailed = context =>
//            {
//                if (!context.Response.HasStarted)
//                {
//                    context.NoResult();       // stop default behavior
//                    context.Response.Clear(); // clear partially written response
//                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//                    context.Response.ContentType = "application/json";

//                    var result = new
//                    {
//                        error = context.Exception is SecurityTokenExpiredException
//                            ? "Token has expired"
//                            : "Invalid token",
//                        details = context.Exception.Message
//                    };

//                    return context.Response.WriteAsJsonAsync(result);
//                }

//                return Task.CompletedTask;
//            },

//            // Token missing completely
//            OnChallenge = context =>
//            {
//                context.HandleResponse(); // stop default challenge

//                if (!context.Response.HasStarted)
//                {
//                    context.Response.Clear();
//                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//                    context.Response.ContentType = "application/json";

//                    var result = new
//                    {
//                        error = "Not authenticated"
//                    };

//                    return context.Response.WriteAsJsonAsync(result);
//                }

//                return Task.CompletedTask;
//            },

//            // Token valid but user lacks required role/claim
//            OnForbidden = context =>
//            {
//                if (!context.Response.HasStarted)
//                {
//                    context.Response.Clear();
//                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
//                    context.Response.ContentType = "application/json";

//                    var result = new
//                    {
//                        error = "Not authorized"
//                    };

//                    return context.Response.WriteAsJsonAsync(result);
//                }

//                return Task.CompletedTask;
//            }
//        };
//    });

//Add Authorization with Role-based policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireSuperAdmin", policy => policy.RequireRole("SuperAdmin"));
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireManager", policy => policy.RequireRole("Manager"));
    options.AddPolicy("RequireDeveloper", policy => policy.RequireRole("Developer"));

    // Custom example: ProjectOwnerOrManager
    options.AddPolicy("ProjectOwnerOrManager", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Manager") ||
            context.User.HasClaim(c => c.Type == "ProjectOwner" && c.Value == "true")));
});


//builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();

var app = builder.Build();
//app.UseMiddleware<GlobalExceptionMiddleware>();


//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    DbInitializer.Seed(db);
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseGlobalExceptionHandling();
app.UseRequestLogging();

app.MapControllers();

app.Run();
