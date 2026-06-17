using AhorraYa.WebClient.Services;

namespace AhorraYa.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddHttpClient();  
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ApiService>();
            builder.Services.AddSession();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Login/Error");
            //    app.UseHsts();
            //}

            app.UseDeveloperExceptionPage();


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();   // middleware

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Logins}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
