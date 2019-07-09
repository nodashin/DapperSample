using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Dynamic; //Dapper.Oracle.DynamicParameterの名前空間
using ExecutionUsingOracleDynamicParameter.Extensions;

namespace ExecutionUsingOracleDynamicParameter
{
    /// <summary>
    /// Dapper.Oracle.DynamicParameterを使用してプロシージャを実行する。
    /// </summary>
    /// <remarks>
    /// Dapper.Oracle.DynamicParameterを使うとカーソルを取得することができる！
    /// </remarks>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                Console.WriteLine("接続文字列:{0}", connectionString);
                Console.WriteLine("");

                using (var c = new OracleConnection(connectionString))
                {
                    try
                    {
                        c.Open();

                        //プロシージャを実行する。
                        {
                            var storedName = "SP_TEST_NODA_GET";
                            Console.WriteLine("プロシージャ名：{0}", storedName);

                            var parameters = new OracleDynamicParameters();
                            parameters.Add("in_code", "100", OracleDbType.Varchar2, System.Data.ParameterDirection.Input);
                            parameters.Add("out_data", null, OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                            parameters.Add("out_state", null, OracleDbType.Int32, System.Data.ParameterDirection.Output);
                            parameters.Add("out_message", null, OracleDbType.Varchar2, System.Data.ParameterDirection.Output, 1000);

                            c.Execute(storedName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                            var data = parameters.GetRefCursorValue<OutData>("out_data");
                            var state = parameters.GetIntValue("out_state");
                            var message = parameters.GetStringValue("out_message");
                            

                            Console.WriteLine("state:{0}", state);
                            Console.WriteLine("message:{0}",message);
                            if (state == 0)
                                Console.WriteLine("code:{0} value:{1}", data[0].CODE, data[0].VALUE);
                        }
                    }
                    finally
                    {
                        c.Close();
                    }
                }
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
    }
}
