using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnSqlSugar.Models
{
    [SugarIndex("_Unique_TestGroupName", "TestGroupName",OrderByType.Desc)]
    [SugarTable("MyTestItem", "测试导航查询的从表")]
    public class MyTestItem
    {
        [SugarColumn(IsNullable = false,IsIdentity =true,IsPrimaryKey =true)]
        public int Id { get; set; }

        [SugarColumn(IsNullable = false)]
        public string TestItemName { get; set; }

        [SugarColumn(IsNullable = false)]
        public string TestGroupName { get; set; }
    }
}
