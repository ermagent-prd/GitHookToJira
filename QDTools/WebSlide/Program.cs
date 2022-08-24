var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

//https://weblog.west-wind.com/posts/2020/Mar/13/Back-to-Basics-Rewriting-a-URL-in-ASPNET-Core

/*
/// <summary>
/// rewrite
/// </summary>

app.Use(async (context, next) =>
{
    var url = context.Request.Path.Value;

    // Rewrite to index
    if (url.Contains("/Privacy"))
    {
        // rewrite and continue processing
        context.Request.Path = "/";
    }

    await next();
});
*/

/// <summary>
/// Redirect
/// </summary>
app.Use(async (context, next) =>
{
    var url = context.Request.Path.Value;

    // Redirect to an external URL
    if (url.Contains("/Privacy"))
    {
        context.Response.Redirect("https://www.prometeia.com/it/home");
        return;   // short circuit
    }

    await next();
});

app.Run();
