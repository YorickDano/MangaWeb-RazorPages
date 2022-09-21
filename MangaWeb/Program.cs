using MangaWeb.Areas.Identity.Data;
using MangaWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Net.NetworkInformation;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    // options.Conventions.AuthorizePage("/Areas/Identity/Pages/Account/Login");
    options.Conventions.AuthorizeFolder("/");
    // options.Conventions.AuthorizeAreaPage("Account", "/Login");
    // options.Conventions.AllowAnonymousToAreaPage("Account", "/Login");
});
builder.Services.AddControllers()
    .AddNewtonsoftJson(option =>
{
    option.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Solodki lox"));
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = key
        };
    });



var internetTestHost = builder.Configuration["InternetTest:Host"];
var internetTestTimeout = int.Parse(builder.Configuration["InternetTest:Timeout"]);
var connectionString = "";
    var ping = new Ping();
try
{
    PingReply reply = ping.Send(internetTestHost, internetTestTimeout);
   
    if(reply.Status == IPStatus.Success)
    {
        connectionString = "MangaWebContext";
    } 
}
catch { connectionString = "MangaWebContextLocal"; }

    builder.Services.AddDbContext<MangaWebContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString(connectionString) ?? throw new InvalidOperationException("Connection string 'MangaWebContext' not found.")));

builder.Services.AddDefaultIdentity<MangaWebUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
}
    ).AddEntityFrameworkStores<MangaWebContext>();
var app = builder.Build();


app.UseExceptionHandler("/Error");
app.UseStatusCodePagesWithReExecute("/Error");
// app.UseHsts();




app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();


app.Run();


