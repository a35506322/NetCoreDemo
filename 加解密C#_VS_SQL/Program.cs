using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using AutoMapper;
using Dapper;
using DecryptionComparison;

String dbConn = string.Empty;
string messageTemplate = "{0} 方式 Elapsed time: {1} ms\r\n";
Console.WriteLine("此程式是測試C# Function解密以及SQL組件解密速度比較\r\n");

while (true)
{
    while (String.IsNullOrEmpty(dbConn))
    {
        Console.Write("請輸入DB連線字串: ");
        dbConn = Console.ReadLine();
        try
        {
            using var conn = new SqlConnection(dbConn);
            var result = conn.QueryFirstOrDefault<int>("SELECT 1");
            if (result == 1)
            {
                Console.WriteLine("連線字串正確!\r\n");
            }
        }
        catch (Exception ex)
        {
            dbConn = string.Empty;
            Console.WriteLine("連線字串錯誤請重新填寫!\r\n");
        }
    }

    Console.WriteLine("請選擇操作 (輸入號碼即可)");
    Console.WriteLine("(1) 測試 C# Function解密 (Foreach) 查詢");
    Console.WriteLine("(2) 測試 C# Function解密 (Select) 查詢");
    Console.WriteLine("(3) 測試 SQL組件解密 查詢");
    Console.WriteLine("(4) 測試 C# Function解密 (AutoMapper) 查詢\r\n");

    string command = string.Empty;
    while (String.IsNullOrEmpty(command))
    {
        Console.Write("請輸入:");
        command = Console.ReadLine();

        string commandName = ToChangeCommandName(command);

        if (commandName == "錯誤指令")
        {
            command = string.Empty;
            Console.WriteLine("無此指令請重新輸入\r\n");
        }
        else
        {
            Console.WriteLine($"選擇的是 {command}號，執行操作為{commandName}\r\n");
        }
    }

    try
    {

        if (command == "1")
        {
            string sql = @"SELECT [id],[name],[tw_id] 
                            FROM [LabDemo].[dbo].[TestDecrypt]";

            var stopwatch = Stopwatch.StartNew();

            using var conn = new SqlConnection(dbConn);
            var TestDecryptQueryResult = conn.Query<TestDecryptModel>(sql);

            List<TestDecryptModel> decryptResult = new List<TestDecryptModel>();
            foreach (var data in TestDecryptQueryResult)
            {
                data.name = SecurityHelper.DecryptData(data.name);
                data.tw_id = SecurityHelper.DecryptData(data.tw_id);
                decryptResult.Add(data);
            }

            stopwatch.Stop();

            Console.WriteLine(messageTemplate, ToChangeCommandName(command), stopwatch.Elapsed.TotalMilliseconds);
        }
        else if (command == "2")
        {
            string sql = @"SELECT [id]
                                ,[name]
                                ,[tw_id] 
                            FROM [LabDemo].[dbo].[TestDecrypt]";

            var stopwatch = Stopwatch.StartNew();

            using var conn = new SqlConnection(dbConn);
            var TestDecryptQueryResult = conn.Query<TestDecryptModel>(sql);

            var decryptResult = TestDecryptQueryResult.Select(data => new TestDecryptModel
            {
                id = data.id,
                name = SecurityHelper.DecryptData(data.name),
                tw_id = SecurityHelper.DecryptData(data.tw_id)
            });

            stopwatch.Stop();

            Console.WriteLine(messageTemplate, ToChangeCommandName(command), stopwatch.Elapsed.TotalMilliseconds);
        }
        else if (command == "3")
        {
            string sql = @"SELECT [id],
                                    [dbo].[Decipher]([name],2),
                                    [dbo].[Decipher]([tw_id],2)
                                    FROM [LabDemo].[dbo].[TestDecrypt]";

            var stopwatch = Stopwatch.StartNew();

            using var conn = new SqlConnection(dbConn);
            var TestDecryptQueryResult = conn.Query<TestDecryptModel>(sql);

            stopwatch.Stop();

            Console.WriteLine(messageTemplate, ToChangeCommandName(command), stopwatch.Elapsed.TotalMilliseconds);
        }
        else if (command == "4")
        {
            string sql = @"SELECT [id]
                                ,[name]
                                ,[tw_id] 
                            FROM [LabDemo].[dbo].[TestDecrypt]";

            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<TestDecryptModel, TestDecryptDto>()
                .ForMember(to => to.name, from => from.MapFrom(f => SecurityHelper.DecryptData(f.name)))
                .ForMember(to => to.tw_id, from => from.MapFrom(f => SecurityHelper.DecryptData(f.tw_id)))
                );
            var mapper = config.CreateMapper();

            var stopwatch = Stopwatch.StartNew();

            using var conn = new SqlConnection(dbConn);
            var TestDecryptQueryResult = conn.Query<TestDecryptModel>(sql);
            var mapperResult = mapper.Map<IEnumerable<TestDecryptDto>>(TestDecryptQueryResult);
            
            stopwatch.Stop();

            Console.WriteLine(messageTemplate, ToChangeCommandName(command), stopwatch.Elapsed.TotalMilliseconds);

        }

    }
    catch (Exception ex)
    {
        Console.WriteLine("執行DB查詢有誤");
        Console.WriteLine(ex);
        Console.WriteLine();
    }

}

string ToChangeCommandName(string command) => command switch
{
    "1" => "測試 C# Function解密 (Foreach) 查詢",
    "2" => "測試 C# Function解密 (Select) 查詢",
    "3" => "測試 SQL組件解密 查詢",
    "4" => "測試 C# Function解密 (AutoMapper) 查詢",
    _ => "錯誤指令"
};

class TestDecryptModel
{
    public int id { get; set; }
    public string name { get; set; }
    public string tw_id { get; set; }
    public void Deconstruct(out int id, out string name, out string tw_id)
    {
        id = this.id;
        name = this.name;
        tw_id = this.tw_id;
    }
}

class TestDecryptDto
{
    public int id { get; set; }
    public string name { get; set; }
    public string tw_id { get; set; }
    public void Deconstruct(out int id, out string name, out string tw_id)
    {
        id = this.id;
        name = this.name;
        tw_id = this.tw_id;
    }
}
