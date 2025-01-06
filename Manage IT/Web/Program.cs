using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Web;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".ManageIT.Session";
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.IsEssential = true;
});


builder.Services.AddRazorPages();
builder.Services.AddDbContext<DatabaseContext>();
builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
var app = builder.Build();

Security.Initialize();
EmailService.Initialize();  
DatabaseAccess.Instantiate();
UserManager.Instantiate();
ProjectManager.Instantiate();
MeetingManager.Instantiate();
TaskListManager.Instantiate();
TaskManager.Instantiate();

app.UseHttpsRedirection();
app.UseHsts();
app.UseStaticFiles();
app.UseAuthorization();
app.UseRouting();
app.UseSession();
app.MapRazorPages();
app.Run();