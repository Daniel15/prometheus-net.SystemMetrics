using Prometheus;
using Prometheus.SystemMetrics;

var builder = WebApplication.CreateSlimBuilder(args);

var services = builder.Services;
services.AddSystemMetrics();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.MapMetrics();
app.MapGet("/", async context =>
{
	context.Response.Redirect("/metrics");
});

app.Run();
