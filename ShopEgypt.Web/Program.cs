using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Stripe;
using Web.Entites.Mappings;
using Web.Entites.ModelsValidation.CategoryValidations;
using Web.Entites.ModelsValidation.ProductValidations;
using Web.Entites.ModelsValidation.UserValidations;
using Web.Entites.ViewModels.ProductVMs;
using Web.Entites.ViewModels.UsersVMs;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
      .ReadFrom.Configuration(builder.Configuration)
      .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddHybridCache();

builder.Services.AddOptions<EmailSettings>()
            .Bind(builder.Configuration.GetSection(nameof(EmailSettings)))
            .ValidateOnStart();

TypeAdapterConfig.GlobalSettings.Scan(typeof(CategoryMapping).Assembly);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(
	options =>options.UseSqlServer(connectionString)
	);

builder.Services.Configure<StripeData>(builder.Configuration.GetSection("StripeData"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(op =>
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
	.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
	{
		options.LoginPath = "/Auths/Login";
		options.LogoutPath = "/Auths/Logout";
		options.AccessDeniedPath = "/Auths/AccessDenied";
		options.ExpireTimeSpan = TimeSpan.FromDays(2);
	});


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();


builder.Services.AddScoped<IValidator<CreateCategoryVM>, CreateCategoryVMValidator>();
builder.Services.AddScoped<IValidator<EditCategoryVM>, EditCategoryVMValidator>();


builder.Services.AddScoped<IValidator<CreateProductVM>, CreateProductVMValidator>();


builder.Services.AddScoped<IValidator<ConfirmEmailVM>, ConfirmEmailVMValidator>();
builder.Services.AddScoped<IValidator<ResendEmailConfirmationVM>, ResendEmailConfirmationVMValidator>();

builder.Services.AddScoped<GeneralRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

var app = builder.Build();

using(var scope=app.Services.CreateScope())
{
	var dbContext= scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
	if(dbContext.Database.GetPendingMigrations().Any())
		await dbContext.Database.MigrateAsync();
}
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
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
