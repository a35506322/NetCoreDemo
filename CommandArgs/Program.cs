using Microsoft.Extensions.Configuration;

try
{
    // 1. 建立指令對應
    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>()
    {
        ["-mode"] = "mode",
        ["-h"] = "help"
    };

    // 2. 註冊在ConfigurationBuilder
    // 🔔 注意args 是預設字 => 使用者打的指令
    var configuration = new ConfigurationBuilder()
                        .AddCommandLine(args,keyValuePairs)
                        .Build();

    foreach (var arg in args)
    {
        Console.WriteLine($"arg:{arg}");
    }

    Console.WriteLine($"mode: {configuration["mode"]}");
}
catch (Exception ex)
{ 
    Console.WriteLine($"Error: {ex.ToString()}");
}
