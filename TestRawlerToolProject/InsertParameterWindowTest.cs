using RawlerTool.Tool;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestRawlerToolProject
{
    
    
    /// <summary>
    ///InsertParameterWindowTest のテスト クラスです。すべての
    ///InsertParameterWindowTest 単体テストをここに含めます
    ///</summary>
    [TestClass()]
    public class InsertParameterWindowTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///現在のテストの実行についての情報および機能を
        ///提供するテスト コンテキストを取得または設定します。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 追加のテスト属性
        // 
        //テストを作成するときに、次の追加属性を使用することができます:
        //
        //クラスの最初のテストを実行する前にコードを実行するには、ClassInitialize を使用
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //クラスのすべてのテストを実行した後にコードを実行するには、ClassCleanup を使用
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //各テストを実行する前にコードを実行するには、TestInitialize を使用
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //各テストを実行した後にコードを実行するには、TestCleanup を使用
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///GetParameterList のテスト
        ///</summary>
        [TestMethod()]
        public void GetParameterListTest()
        {
            InsertParameterWindow target = new InsertParameterWindow(); // TODO: 適切な値に初期化してください
            string xaml = "a=\"@@aaaaa\" b=\"@@bbbb\""; // TODO: 適切な値に初期化してください
            string[] expected = {"aaaaa","bbbb"}; // TODO: 適切な値に初期化してください
            string[] actual;
            actual = target.GetParameterList(xaml);
            Assert.AreEqual<string[]>(expected, actual);
            Assert.Inconclusive("このテストメソッドの正確性を確認します。");
        }
    }
}
