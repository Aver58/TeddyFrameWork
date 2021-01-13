using System;
using System.IO;
using HiProtobuf.Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Google.Protobuf;

namespace HiProtobuf.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestExport()
        {
            Manager.Export();
        }

        [TestMethod]
        public void TestRead()
        {
            //Excel_Example example;
            //using (var input = File.OpenRead(path))
            //{
            //    example = Excel_Example.Parser.ParseFrom(input);
            //    var data = example.Data;
            //    var t1 = data[1];
            //}
        }
    }
}
