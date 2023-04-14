using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using tufol.Helpers;
using tufol.Interfaces;
using tufol.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace tufol
{
  public class Startup
  {

    
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;

    }

    public IConfiguration Configuration { get; }

    // Localizer
    public class LanguageRouteConstraint : IRouteConstraint
    {
      public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
      {

        if (!values.ContainsKey("culture"))
          return false;

        var culture = values["culture"].ToString();
        return culture == "en" || culture == "de";
      }
    }

    public class RouteDataRequestCultureProvider : RequestCultureProvider
    {
      public int IndexOfCulture;
      public int IndexofUICulture;

      public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
      {
        if (httpContext == null)
          throw new ArgumentNullException(nameof(httpContext));

        string culture = null;
        string uiCulture = null;

        var twoLetterCultureName = httpContext.Request.Path.Value.Split('/')[IndexOfCulture]?.ToString();
        var twoLetterUICultureName = httpContext.Request.Path.Value.Split('/')[IndexofUICulture]?.ToString();

        if (twoLetterCultureName == "id")
          culture = "id";
        else if (twoLetterCultureName == "en")
          culture = uiCulture = "en";

        if (twoLetterUICultureName == "id")
          culture = "id";
        else if (twoLetterUICultureName == "en")
          culture = uiCulture = "en";

        if (culture == null && uiCulture == null)
          return NullProviderCultureResult;

        if (culture != null && uiCulture == null)
          uiCulture = culture;

        if (culture == null && uiCulture != null)
          culture = uiCulture;

        var providerResultCulture = new ProviderCultureResult(culture, uiCulture);

        return Task.FromResult(providerResultCulture);
      }
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddSession(options => { options.IdleTimeout = TimeSpan.FromDays(1); });
      services.AddControllersWithViews();

      var appSettings = Configuration.GetSection("AppSettings");
      services.Configure<AppSettings>(appSettings);
      var defaultValue = Configuration.GetSection("DefaultValue");
      services.Configure<DefaultValue>(defaultValue);

      services.AddScoped<IMenu, MenuService>();
      services.AddScoped<IUser, UserService>();
      services.AddScoped<IUserAccessApi, UserAccessApiService>();
      services.AddScoped<IRegistration, RegistrationService>();
      services.AddScoped<IVendor, VendorService>();
      services.AddScoped<ITempCompanyRelation, CompanyService>();
      services.AddScoped<IMaster, MasterService>();
      services.AddScoped<ISetting, SettingService>();
      services.AddScoped<ITicket, TicketService>();
      services.AddScoped<IMasterTable, MasterTableService>();
      services.AddScoped<IOpenItemPo, OpenItemPoService>();
      services.AddScoped<IDashboard, DashboardService>();

      services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

      // Localizer
      services.AddLocalization(options => options.ResourcesPath = "Resources");

      services.Configure<RequestLocalizationOptions>(options =>
      {
        var supportedCultures = new List<CultureInfo>
          {
                    new CultureInfo("id"),
                    new CultureInfo("en")
          };
        options.DefaultRequestCulture = new RequestCulture(culture: "id", uiCulture: "id");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;
        options.RequestCultureProviders = new[] { new RouteDataRequestCultureProvider { IndexOfCulture = 1, IndexofUICulture = 1 } };
      });

      services.Configure<RouteOptions>(options =>
      {
        options.ConstraintMap.Add("culture", typeof(LanguageRouteConstraint));
      });

      services.AddMvc()
          .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
          .AddDataAnnotationsLocalization();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IUserAccessApi _userTest)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
      app.UseRequestLocalization(localizationOptions.Value);

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthorization();
      app.UseSession();

      app.Use(async (context, next) =>
      {
        // Console.WriteLine("Hello from middleware");
        // Console.WriteLine(context.Request.Path.StartsWithSegments("/api/rpa"));

        bool PathAPIRPA = context.Request.Path.StartsWithSegments("/api/rpa");

        // Console.Write("Path API: ");
        // Console.WriteLine(PathAPIRPA);
        
        String access_token = context.Request.Headers["access_token"];

        if(PathAPIRPA){

            if (String.IsNullOrEmpty(access_token))
            {

              Console.WriteLine("401");
            context.Response.StatusCode = 401;
              await context.Response.WriteAsJsonAsync(new
                {
                  status = 401,
                  message = "Unauthorized"
                }); 
            }
            else
            {
              var apiKey = _userTest.GetUserDetail(new Helpers.GlobalFunction().GetMD5(access_token));

              if (String.IsNullOrEmpty(apiKey.access_token))
              {
              Console.WriteLine("403");
                context.Response.StatusCode = 403;

                await context.Response.WriteAsJsonAsync(new
                {
                  status = 403,
                  message = "Forbidden"
                });
              }
            }
          }
        // await context.Response.WriteAsync("Middleware 0\n");
        if(!context.Response.HasStarted){
          await next();
        }
       
      });

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{culture=id}/{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
