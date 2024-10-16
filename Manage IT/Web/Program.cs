using EFModeling.EntityProperties.DataAnnotations.Annotations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddDbContext<DatabaseContext>();
var app = builder.Build();

app.MapRazorPages();
app.Run();
