using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

// 全域設定
/*  🔔new CompactJsonFormatter()
 *  由於 Log 的欄位很多，使用 Console Sink 會比較看不出來，改用 Serilog.Formatting.Compact 來記錄 JSON 格式的 Log 訊息會清楚很多！
 */
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // 設定最小Log輸出
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning) // 設定 Microsoft.AspNetCore 訊息為 Warning 為最小輸出
    .Enrich.FromLogContext()  // 可以增加Log輸出欄位 https://www.cnblogs.com/wd4j/p/15043489.html
    .WriteTo.Console(new CompactJsonFormatter()) // 寫入Console 
    .WriteTo.File(new CompactJsonFormatter(),"logs/log-.txt", rollingInterval: RollingInterval.Day) // 寫入txt
    .CreateLogger();

try
{
    Log.Information("Starting web host");

    // Add services to the container.

    // SerilLog 
    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseSerilogRequestLogging(options =>
    {
        /* 如果不用 new CompactJsonFormatter()格式輸出
           可以在這邊自訂義格式但是可以先利用 new CompactJsonFormatter() 看Json屬性有哪些
            再來自定義
         */
        // options.MessageTemplate = "Handled {RequestPath}";

        // Emit debug-level events instead of the defaults
        // options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;

        // 如果覺得SerilLog太少，可以新增欄位
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        };
    });

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    // 紀錄你的應用程式中未被捕捉的例外 (Unhandled Exception)
    Log.Error(ex, "Something went wrong");
}
finally
{
    Log.CloseAndFlush(); // 非常重要的一段！
}
