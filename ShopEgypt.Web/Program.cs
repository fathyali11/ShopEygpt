using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
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

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeData"));

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

builder.Services.AddOptions<GoogleSettings>()
	.Bind(builder.Configuration.GetSection(nameof(GoogleSettings)))
	.ValidateOnStart();

builder.Services.AddOptions<StripeSettings>()
	.Bind(builder.Configuration.GetSection(nameof(StripeSettings)))
	.ValidateOnStart();



builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = "Cookies";
	options.DefaultChallengeScheme = "Cookies";
})
.AddCookie("Cookies")
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
	var googleSettings = builder.Configuration.GetSection(nameof(GoogleSettings)).Get<GoogleSettings>();
	options.ClientId = googleSettings!.ClientId;
	options.ClientSecret = googleSettings.ClientSecret;
});

var stripeSettings=builder.Configuration.GetSection(nameof(StripeSettings)).Get<StripeSettings>();
StripeConfiguration.ApiKey = stripeSettings!.Secretkey;

builder.Services.Configure<CookieAuthenticationOptions>("Cookies",options =>
{
	options.LoginPath = "/Auths/Login";
	options.LogoutPath = "/Auths/Logout";
	options.AccessDeniedPath = "/Auths/AccessDenied";
	options.ExpireTimeSpan = TimeSpan.FromDays(2);
});


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();


builder.Services.AddScoped<IValidator<CreateCategoryVM>, CreateCategoryVMValidator>();
builder.Services.AddScoped<IValidator<EditCategoryVM>, EditCategoryVMValidator>();


builder.Services.AddScoped<IValidator<CreateProductVM>, CreateProductVMValidator>();


builder.Services.AddScoped<IValidator<ConfirmEmailVM>, ConfirmEmailVMValidator>();
builder.Services.AddScoped<IValidator<ResendEmailConfirmationVM>, ResendEmailConfirmationVMValidator>();
builder.Services.AddScoped<IValidator<ForgotPasswordVM>, ForgotPasswordVMValidator>();
builder.Services.AddScoped<IValidator<ResetPasswordVM>, ResetPasswordVMValidator>();

builder.Services.AddScoped<GeneralRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
	if (dbContext.Database.GetPendingMigrations().Any())
		await dbContext.Database.MigrateAsync();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

	if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
		await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

    if (!await roleManager.RoleExistsAsync(UserRoles.Customer))
        await roleManager.CreateAsync(new IdentityRole(UserRoles.Customer));
}

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}
app.UseRouting();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
