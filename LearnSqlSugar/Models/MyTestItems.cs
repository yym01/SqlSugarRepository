using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnSqlSugar.Models
{
    [SugarTable("MyTestItems", "测试导航查询的主表")]
    public class MyTestItems
    {
        [SugarColumn(IsIdentity = true,IsPrimaryKey =true)]
        public int Id { get; set; }

        [SugarColumn(IsNullable =false)]
        public string TestGroupName { get; set; }

        [Navigate(NavigateType.OneToMany,nameof(MyTestItem.TestGroupName),nameof(TestGroupName))]
        public List<MyTestItem> TestItemList { get; set; }
    }
}
