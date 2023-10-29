using CommandArgsValid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
                        .AddCommandLine(args, keyValuePairs)
                        .Build();

    // 3. 利用 Option 模式 Validate 驗證指令正確性
    var service = new ServiceCollection();
    service.AddOptions<CommandModel>()
                      .Configure(option =>
                      {
                          option.Mode = configuration["mode"] ?? String.Empty;
                          option.Help = configuration["help"] ?? String.Empty;
                      }).Validate(options => ValidateCommand("mode", options.Mode) && ValidateCommand("help", options.Help), "參數錯誤請確認");

    // 4. 取出對應值
    var options = service.BuildServiceProvider()
                        .GetRequiredService<IOptions<CommandModel>>()
                        .Value;

    Console.WriteLine($"options.Mode: {options.Mode}");
    Console.WriteLine($"options.Help: {options.Help}");

    static bool ValidateCommand(string command,string value)
    { 
        if (string.IsNullOrEmpty(command) || string.IsNullOrEmpty(value)) return false;

        switch (command)
        {
            case string i when i == "mode":

                string[] modes = new string[] { "prod", "test", "stag" };

                if (!modes.Contains(value)) return false;
                else return true;

             case string i when i == "help":
                return true;

            default: return false;
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.ToString()}");
}

