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
                string plainText = "Hello, World!";

                context.Response.ContentType = "text/plain";
                context.Response.ContentLength = plainText.Length;
                await context.Response.WriteAsync(plainText);
            });

            app.Run();
        }
    }
}