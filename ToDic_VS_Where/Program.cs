// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

List<MyClass> GenerateMyClassList(int count)
{
    var list = new List<MyClass>();
    for (int i = 1; i <= count; i++)
    {
        var myClass = new MyClass
        {
            Number = i,
            Name = "Name" + i // 例如: "Name0", "Name1", ...
        };
        list.Add(myClass);
    }
    return list;
}


var mockdata =  GenerateMyClassList(10000);
var mockdataDic = mockdata.ToDictionary(x => x.Number, x => x);

var stopwatch = Stopwatch.StartNew();
Console.WriteLine(mockdata.FirstOrDefault(n => n.Number == 10000)?.Name);
stopwatch.Stop();
Console.WriteLine($"Where 方式 Elapsed time: {stopwatch.Elapsed.TotalMilliseconds} ms");


var stopwatch2 = Stopwatch.StartNew();
Console.WriteLine(mockdataDic[10000].Name);
stopwatch2.Stop();
Console.WriteLine($"ToDic 方式  Elapsed time: {stopwatch2.Elapsed.TotalMilliseconds} ms");



public class MyClass
{
    public int Number { get; set; }
    public string Name { get; set; }
}



