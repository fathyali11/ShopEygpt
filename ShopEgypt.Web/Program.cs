

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Stripe;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.Configure<StripeData>(builder.Configuration.GetSection("StripeData"));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(op =>
{
	op.Lockout.MaxFailedAccessAttempts=3;
	op.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(2);
	op.Password.RequiredUniqueChars = 0;
	op.Password.RequireNonAlphanumeric = false;
	op.Password.RequireUppercase = false;
	op.Password.RequireDigit = false;
	op.Password.RequireLowercase = false;
})
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultUI()
	.AddDefaultTokenProviders();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IEmailSender, EmailSender>();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();


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
app.UseSession();
app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("StripeData:Secretkey").Get<string>();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
	name: "default",
	pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
