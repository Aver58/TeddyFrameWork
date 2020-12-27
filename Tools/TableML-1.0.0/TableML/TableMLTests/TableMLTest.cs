using System.IO;
using TableML;
using NUnit.Framework;
using TableML.Compiler;

namespace TableMLTests
{
	[TestFixture]
    public class TableMLTest
    {
        public string TableString1 = @"Id	Value
int	string
1	hi
2	f
3	abc 
4	temp
";
        public string TableString2 = @"Id	Value
int	string
10	hi
20	f
30	abc 
40	temp
";
        public string TableString1Plus2 = @"Id	Value
int	string
1	hi
2	f
3	abc 
4	temp
10	hi
20	f
30	abc 
40	temp
";

		[SetUp]
		public void Init()
		{
//			var dllDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
//			Directory.SetCurrentDirectory(dllDir);
        }

        [Test]
        public void TestLoad()
        {
            var table = TableML.TableFile.LoadFromString(TableString1, TableString2);
            var row = table.GetByPrimaryKey("1");
            Assert.AreEqual("hi", row["Value"]);
        }

        /// <summary>
        /// 传入多个Table
        /// </summary>
        [Test]
        public void TestMultString()
        {
            var table = TableFile.LoadFromString(TableString1, TableString2);
            var row = table.GetByPrimaryKey("1");
            Assert.AreEqual("hi", row["Value"]);
        }
        [Test]
        public void TestMultStringTableObj()
        {
            var tableObj = TableObject.LoadFromString(TableString1, TableString2);
            var objRow = tableObj.GetByPrimaryKey(10);
            Assert.AreEqual("hi", objRow["Value"]);
        }
        [Test]
        public void TestWrite()
        {
            var tableObj = TableFile.LoadFromString(TableString1, TableString2);
            var writer = new TableFileWriter(tableObj);
            Assert.AreEqual(true, writer.Save("write.txt"));
            var result = File.ReadAllText("write.txt");
            var expect = TableString1Plus2.Replace("\r", "");
            Assert.AreEqual(expect, result);

        }

		[Test]
		public void TestCompileXls()
		{
			var compiler = new Compiler();
			compiler.Compile("./TestSettings/TestExcel.xls", Path.GetFullPath("./TestExcelXls.tml"), Path.GetFullPath("./"));
		    Assert.True(File.Exists("TestExcelXls.tml"));

		}
		[Test]
		public void TestCompileXlsx()
		{
			var compiler = new Compiler();
			compiler.Compile("TestSettings/TestExcel2.xlsx", Path.GetFullPath("./TestExcelXlsx.tml"), Path.GetFullPath("./"));
		    Assert.True(File.Exists("TestExcelXlsx.tml"));

		}

		[Test]
	    public void TestModifyXls()
		{
		    var file = new SimpleExcelFile("TestSettings/TestExcel.xls");
		    file.Save("TestSettings/TestExcelSave.xls");
		    Assert.True(File.Exists("TestSettings/TestExcelSave.xls"));
		}
		[Test]
	    public void TestModifyXlsx()
		{
		    var file = new SimpleExcelFile("TestSettings/TestExcel2.xlsx");
		    file.Save("TestSettings/TestExcel2Save.xlsx");
		    Assert.True(File.Exists("TestSettings/TestExcel2Save.xlsx"));
//			File.Delete("TestSettings/TestExcel2Save.xlsx"); // TODO: Save NPOI xlsx cannot open
		}


		[Test]
		public void TestBatchCompile()
		{
			var bc = new BatchCompiler();
			var results = bc.CompileTableMLAll("TestSettings", "TestSettingsResult", "TestSettings.cs.gen", DefaultTemplate.GenCodeTemplate, "AppSettings", ".tml", null, true);

			Assert.AreEqual(4, results.Count);
			Assert.True(File.Exists("TestSettings.cs.gen"));
		}
    }
}
