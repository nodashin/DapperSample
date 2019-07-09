using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Dynamic;
using Oracle.ManagedDataAccess.Types;

namespace ExecutionUsingOracleDynamicParameter.Extensions
{
    /// <summary>
    /// OracleDynamicParametersの拡張メソッド
    /// </summary>
    /// <remarks>
    /// https://gist.github.com/ttlatex/10ce7f2cd1f44815727925f4474dd34c
    /// 将来的にこのクラスだけ抜き出してGitHubにUpする。
    /// </remarks>
    public static class OracleDynamicParametersExtensions
    {
        /// <summary>
        /// 数値を取得する。
        /// </summary>
        /// <param name="parameters">OracleDynamicParameters</param>
        /// <param name="name">パラメータ名</param>
        /// <returns>Value</returns>
        public static int GetIntValue(this OracleDynamicParameters parameters, string name)
        {
            var value = parameters.Get<OracleDecimal>(name);
            return value.ToInt32();
        }

        /// <summary>
        /// 文字列値を取得する。
        /// </summary>
        /// <param name="parameters">OracleDynamicParameters</param>
        /// <param name="name">パラメータ名</param>
        /// <returns>Value</returns>
        public static string GetStringValue(this OracleDynamicParameters parameters, string name)
        {
            var value = parameters.Get<OracleString>(name);

            if (value.IsNull)
                return "";

            return value.ToString();
        }

        /// <summary>
        /// カーソルを取得する。
        /// </summary>
        /// <typeparam name="T">カーソルをマッピングするクラスの型</typeparam>
        /// <param name="parameters">OracleDynamicParameters</param>
        /// <param name="name">パラメータ名</param>
        /// <returns>Value</returns>
        public static List<T> GetRefCursorValue<T>(this OracleDynamicParameters parameters, string name)
        {
            var value = parameters.Get<OracleRefCursor>(name);

            if (value.IsNull)
                return new List<T>();

            var reader = value.GetDataReader();
            var columnNames = Enumerable.Range(0, reader.FieldCount).Select(i => reader.GetName(i)).ToList();

            var result = new List<T>();
            while(reader.Read())
            {
                var t = Activator.CreateInstance<T>();
                foreach (var p in typeof(T).GetProperties())
                    if (columnNames.Any(c => c == p.Name))
                        p.SetValue(t, reader[p.Name]);

                result.Add(t);
            }
            
            return result;
        }
    }
}
