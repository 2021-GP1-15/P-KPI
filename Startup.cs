using IP_KPI.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace IP_KPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<db_a7baa5_ipkpiContext>();
            services.AddControllersWithViews();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => { 
                
                });
    
            var connectionString = Configuration.GetConnectionString("IpkpiContext");
            services.AddDbContext<db_a7baa5_ipkpiContext>(option => option.UseSqlServer(connectionString));

            //ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidate;

            //SSOEmployeeRef.SSOEmployeeClient EmployeeSSOClient = new SSOEmployeeRef.SSOEmployeeClient();
            //SSOEmployeeRef.Person EmployeeLoginData = new SSOEmployeeRef.Person();
            //SSOEmployeeRef.DataLayerADInput EmployeeLoginInput = new SSOEmployeeRef.DataLayerADInput();
            //EmployeeLoginInput.UserName = "UserName";
            //EmployeeLoginInput.Password = "Password";

            //EmployeeLoginData = EmployeeSSOClient.GetAuthenticationAsync(EmployeeLoginInput);

            //bool LoginActionStatus = EmployeeLoginData.actionStatus;


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
          
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Homepage}/{id?}");
            });
        }
        
        


        ////Validate Server Certificates
        //private static bool RemoteCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        //{
        //    return true;
        //}


    }
}
