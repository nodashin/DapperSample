using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Oracle.ManagedDataAccess.Client;

namespace ExecutionProcedure
{
    /// <summary>
    /// プロシージャ実行
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                Console.WriteLine("接続文字列:{0}", connectionString);
                Console.WriteLine("");

                //匿名クラスを使用して実行する。
                Program.ExecuteUsingAnonymousClass(connectionString);
                Console.WriteLine("");

                //Dapper.DynamicParameterを使用して実行する。
                Program.ExecuteUsingDynamicParameters(connectionString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadKey();
            }
        }

        /// <summary>
        /// 匿名クラスを使用して実行する。
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        static void ExecuteUsingAnonymousClass(string connectionString)
        {
            Console.WriteLine("匿名クラスを使用して実行");

            using (var c = new OracleConnection(connectionString))
            {
                try
                {
                    c.Open();

                    var storedName = "SP_TEST_NODA_INSERT";
                    Console.WriteLine("ストアド実行：{0}", storedName);
                    c.Query<DataEntity>(storedName,new { in_code = "100", in_value = "value-100" },commandType: System.Data.CommandType.StoredProcedure);

                    
                    var functionName = "SF_TEST_NODA_GET_VALUE";
                    Console.WriteLine("ファンクション実行：{0}", functionName);
                    //ファンクション実行時は「select ファンクション名 from dual」の形式にする必要がある。
                    var query = string.Format("select {0}(:in_code) from dual", functionName);
                    var result = c.ExecuteScalar<string>(query, new { in_code = "100" });
                    Console.WriteLine("result:{0}", result);
                }
                finally
                {
                    c.Close();
                }
            }
        }

        /// <summary>
        /// DynamicParameterを使用して実行する。
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        static void ExecuteUsingDynamicParameters(string connectionString)
        {
            Console.WriteLine("Dapper.DynamicParameterを使用して実行");

            using (var c = new OracleConnection(connectionString))
            {
                try
                {
                    c.Open();

                    var storedName = "SP_TEST_NODA_INSERT";
                    Console.WriteLine("ストアド実行：{0}", storedName);
                    var parameters = new DynamicParameters();
                    parameters.Add("in_code", "200", System.Data.DbType.String, System.Data.ParameterDirection.Input);
                    parameters.Add("in_value", "value-200", System.Data.DbType.String, System.Data.ParameterDirection.Input);
                    c.Execute(storedName, parameters, commandType: System.Data.CommandType.StoredProcedure);


                    var functionName = "SF_TEST_NODA_GET_VALUE";
                    Console.WriteLine("ファンクション実行：{0}", functionName);
                    parameters = new DynamicParameters();
                    parameters.Add("in_code", "200", System.Data.DbType.String, System.Data.ParameterDirection.Input);
                    parameters.Add("return_value", null, System.Data.DbType.String, System.Data.ParameterDirection.ReturnValue, 1000);
                    c.Execute(functionName, parameters, commandType: System.Data.CommandType.StoredProcedure);
                    Console.WriteLine("result:{0}", parameters.Get<string>("return_value"));
                }
                finally
                {
                    c.Close();
                }
            }
        }
    }
}
