var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.Use(async (context, next) =>
{
    string path = context.Request.Path.Value ?? "";

    bool isLoginPage = path.StartsWith("/Login");
    bool isStaticFile = path.StartsWith("/css") ||
                        path.StartsWith("/js") ||
                        path.StartsWith("/lib") ||
                        path.StartsWith("/favicon");

    bool isLoggedIn = context.Session.GetString("Username") != null;

    if (!isLoggedIn && !isLoginPage && !isStaticFile)
    {
        context.Response.Redirect("/Login");
        return;
    }

    await next();
});

app.MapRazorPages();

app.Run();