// See https://aka.ms/new-console-template for more information
using DryIoc;
using LearnSqlSugar;
using LearnSqlSugar.Models;

Console.WriteLine("Hello, World!");

var container = new Container();

container.Register<ILogger, ConsoleLoger>(Reuse.Singleton);

//存在一点问题
//var mySqlRepository = new SqlRepository<TestTable1>(container);

//mySqlRepository.CreateTableByAssembly("LearnSqlSugar");

container.Register(
    typeof(ISqlRepository<>),
    typeof(SqlRepository<>),
    made: Made.Of(
        // 使用构造函数并传递固定的容器实例作为参数
        FactoryMethod.ConstructorWithResolvableArguments,
        Parameters.Of.Type<IContainer>(r => container)
    )
);

var service = container.Resolve<ISqlRepository<MyTestItems>>();

//await service.InsertByNavigate(
//    new MyTestItems()
//    {
//        TestGroupName = "TestSqlLite1",
//        TestItemList = new()
//        {
//            new MyTestItem() { TestGroupName = "TestSqlLite", TestItemName = "TestSqlLite01" },
//            new MyTestItem() { TestGroupName = "TestSqlLite", TestItemName = "TestSqlLite02" },
//        },
//    }
//);

var res = await service.GetByNavigate();

foreach(var item in res)
{
    Console.WriteLine(item.Id);
    foreach (var item2 in item.TestItemList)
    {
        Console.WriteLine(item2.TestItemName);
    }
}



//List<TestTable1> tables = new List<TestTable1>();

//for (int i = 0; i < 10; i++)
//{
//    tables.Add(
//        new TestTable1()
//        {
//            Id = i,
//            DataOrder = i,
//            Name = $"test-{i}",
//            UpdateTime = DateTime.Now,
//            MyData = new() { 1, 2, 3, 4, 5, 6 },
//        }
//    );
//}

//await service.InsertRangeAsync(tables);

//var res = await service.GetCountAsync();

//Console.WriteLine(res);

//service.ClearTable();

//await service.DeleteByLambdaAsync((t) => t.Key < 12);
