using Hangfire;
using WebAppApi3.Authorizations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Hangfire
builder.Services.AddHangfire(x => x.UseSqlServerStorage("Server=mssql-dev;Database=HangFire;User Id=sa;Password=SomeHardPasswordNotGonnaLie2022!;"));
builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Middleware to automatically handle virtual paths. Reference: https://github.com/HangfireIO/Hangfire/issues/1368#issuecomment-472785155
app.Use((context, next) =>
{
    var pathBase = new PathString(context.Request.Headers["X-Forwarded-Prefix"]);
    if (pathBase != null)
        context.Request.PathBase = new PathString(pathBase.Value);
    return next();
});

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
});

app.MapControllers();

app.Run();
