namespace HelloWorld
{
    class Program
    {
        static void Main(String[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.MapGet("/", () => { });
            app.MapGet("/plain", async context =>
            {
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Hello, World!");
            });

            app.Run();
        }
    }
}