using ABC.Web.Middlewares;
using ABC.Web.Models.Profile;
using ABC.Web.Services.Implementations;
using ABC.Web.Services.Interfaces;
using NLog;
using NLog.Web;
using System.Reflection.Metadata;

var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
try
{
    logger.Debug("Application starting");
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.AddAutoMapper(typeof(ContentMappingProfile));
    builder.Services.AddSingleton<IContentService, ContentService>();

    var app = builder.Build();

    //app.Use(async (context, next) =>
    //{
    //    var contentService = context.RequestServices.GetRequiredService<IContentService>();
    //    bool isPrefaceExist = contentService.IsPrefaceExist();

    //    if (isPrefaceExist)
    //    {
    //        context.Response.Redirect("/Home/Index");
    //        return;
    //    }

    //    await next();
    //});

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();
    app.UseMiddleware<ErrorHandlingMiddleware>();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch(Exception ex)
{
    logger.Error(ex, "Application stopped due to an exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
