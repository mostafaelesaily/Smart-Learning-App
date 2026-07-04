using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Smart_Learning_App.Data;
using Smart_Learning_App.Data.Models;
using Smart_Learning_App.Data.Seed;
using Smart_Learning_App.Extensions;
using Smart_Learning_App.Integrations.Ai;
using Smart_Learning_App.Integrations
.Ai.Implementations;
using Smart_Learning_App.Middleware;
using Smart_Learning_App.Repo_Uow;
using Smart_Learning_App.Repo_Uow.Base;
using Smart_Learning_App.Repo_Uow.Main;
using Smart_Learning_App.Services.IService;
using Smart_Learning_App.Services.MainService;
using Smart_Learning_App.Services.MainService.
Smart_Learning_App.Services.MainService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Smart_Learning_App")));

// Repository + UOW
builder.Services.AddScoped<IUow, MainUow>();
builder.Services.AddScoped(typeof(IRepo<>), typeof(MainRepo<>));

// Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICacheServicecs, CacheService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICouresService, CourseService>();
builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmetService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<IProgressService, ProgressService>();
builder.Services.AddScoped<IInstructorRequestService,
InstructorRequestService>();
builder.Services.AddHttpClient<IAiService, GeminiIntegration>();
builder.Services.AddCustomJwtAuth(builder.Configuration);
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "SmartLearningApp_";
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var roleManager =
    scope.ServiceProvider.
    GetRequiredService
    <RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRolesAsync(roleManager);
}
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();