using Microsoft.EntityFrameworkCore;
using TechMoveCRM.Data;
using TechMoveCRM.Services;
using TechMoveCRM.MVC.Services; // <-- ADD THIS (ApiService namespace)

var builder = WebApplication.CreateBuilder(args);

// Add MVC
builder.Services.AddControllersWithViews();

// Register DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register HttpClient for Currency API
builder.Services.AddHttpClient();

// ? STEP 9: Register ApiService (MVC ? API communication)
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7001/");
});

// Register our custom services (Repository pattern from Part 1)
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IContractWorkflowService, ContractWorkflowService>();

var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// ? FIX: Create uploads folder BEFORE app.Run()
var uploadsPath = Path.Combine(app.Environment.WebRootPath, "uploads", "contracts");
Directory.CreateDirectory(uploadsPath);

// Run app (ONLY ONCE)
app.Run();