using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace LearnSqlSugar.Models
{
    [SugarIndex(
        "_Unique_Name_ID",
        nameof(TestTable1.Name),
        OrderByType.Desc,
        nameof(TestTable1.Id),
        OrderByType.Desc,
        true
    )]
    [SugarTable("TestTable1", "测试表1")]
    public class TestTable1
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Key { get; set; }

        [SugarColumn(IsNullable = false)]
        public string Name { get; set; }

        public int Id { get; set; }

        public int DataOrder { get; set; }

        [SugarColumn(IsJson = true)]
        public List<int> MyData { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
