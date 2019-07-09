using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Oracle.ManagedDataAccess.Client;

namespace ExecutionSelectSql
{
    /// <summary>
    /// SELECT文のSQL実行
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args">未使用</param>
        static void Main(string[] args)
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                Console.WriteLine(connectionString);
                Console.WriteLine("");

                //引数無しのSQLを実行する。
                Console.WriteLine("引数無しSELECT文実行");
                using (var c = new OracleConnection(connectionString))
                {
                    var query = "select * from test_noda";
                    Console.WriteLine(query);
                    var entities = c.Query<DataEntity>(query);
                    foreach (var entity in entities)
                        Console.WriteLine("CODE:{0} VALUE:{1}", entity.CODE, entity.VALUE);
                }
                Console.WriteLine("");

                //引数有りSQLを実行する。
                Console.WriteLine("引数有りSELECT文実行");
                Console.Write("CODE:");
                var code = Console.ReadLine();
                using (var c = new OracleConnection(connectionString))
                {
                    var query = "select * from test_noda where code = :code";
                    Console.WriteLine(query);
                    var entities = c.Query<DataEntity>(query, new { CODE = code});
                    foreach (var entity in entities)
                        Console.WriteLine("CODE:{0} VALUE:{1}", entity.CODE, entity.VALUE);
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
