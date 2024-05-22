using MvcCoreElastiCacheAWS.Repository;
using MvcCoreElastiCacheAWS.Services;

var builder = WebApplication.CreateBuilder(args);

// Agregar configuraci�n de Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "cachecoches.uk9dp8.ng.0001.use1.cache.amazonaws.com:6379";
    options.InstanceName = "ElastiCacheExample";
});

// Add services to the container.
builder.Services.AddTransient<RepositoryCoches>();
builder.Services.AddTransient<ServiceAWSCache>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
