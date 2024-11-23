using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Web;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddDbContext<DatabaseContext>();
var app = builder.Build();

Security.Initialize();
EmailService.Initialize();  
DatabaseAccess.Instantiate();
PrefixManager.Instantiate();
UserManager.Instantiate();

app.UseHttpsRedirection();
app.UseHsts();
app.MapRazorPages();
app.UseStaticFiles();
app.Run();