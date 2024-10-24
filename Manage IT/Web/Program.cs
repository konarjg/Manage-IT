using EFModeling.EntityProperties.DataAnnotations.Annotations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddDbContext<DatabaseContext>();
var app = builder.Build();

DatabaseAccess.Instantiate();
PrefixManager.Instantiate();
UserManager.Instantiate();

app.UseHttpsRedirection();
app.UseHsts();
app.MapRazorPages();
app.UseStaticFiles();
app.Run();