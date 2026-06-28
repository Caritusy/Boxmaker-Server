namespace BoxMaker_Server
{
    public class Program
    {
        public static DateTime serverBeginDT = DateTime.MinValue;
        public static void Main(string[] args)
        {
            System.Console.Title = "Boxmaker.Server";
            BoxmakerProxy proxy = new BoxmakerProxy();

            AccountManager.serverMaps = AccountManager.GetServerMapList();
            AccountManager.RepairServerMapReplayIndexes();
            AccountManager.RebuildAllPlayerStates();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddSession();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.MapRazorPages();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            Console.WriteLine("[green][SERVER] Boxmaker.LocalServer is running. Check the configured ASP.NET Core endpoint to access the web console.");

            serverBeginDT = DateTime.Now;

			app.Run();

        }
    }
}