using AspNetCore.RouteAnalyzer;
using MangaWeb.Areas.Identity.Data;
using MangaWeb.Authorization;
using MangaWeb.Managers;
using MangaWeb.Models.OptionModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddNewtonsoftJson(option =>
{
    option.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
});
builder.AddServices();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

builder.Services.AddRazorPages();
builder.Services.AddMvc();
builder.Services.AddRouteAnalyzer();

var connectionString = TestConnectionManager.GetLocalDataBaseConnectionString();
    builder.Services.AddDbContext<MangaWebContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString(connectionString) ?? throw new InvalidOperationException("Connection string 'MangaWebContext' not found.")));

builder.Services.AddDefaultIdentity<MangaWebUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
}
    ).AddEntityFrameworkStores<MangaWebContext>();
builder.Services.Configure<MailSenderOptions>(builder.Configuration.GetSection(nameof(MailSenderOptions)));
builder.Services.AddControllersWithViews(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanManageManga",
                    policyBuilder => policyBuilder
                        .AddRequirements(new IsMangaOwnerRequirement()));
});
var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapRazorPages();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
  
}

app.Run();


