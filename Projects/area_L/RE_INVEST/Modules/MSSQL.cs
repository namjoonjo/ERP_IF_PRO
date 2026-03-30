using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace area_L.Modules
{
    class MSSQL
    {

        /***********************
         * 
         * MSSQL 모듈 적용방법
         * 
         * 1. MSSQL 객체 선언
           ex) MSSQL db = new MSSQL();
         * 
         * 2. strSql 지정 
           ex) string strSql = "ERP_2.dbo.TEMP_PROC";
         * 
         * 3. 파라미터 지정 (파라미터가 없는 경우 시행하지 않아도됨.)
           ex) db.Parameter("프로시저 매개변수", "매개변수에 들어갈 값");
         *
         * 4. db.ExcuteSql(strSql) 혹은 db.ExecuteNonSql(strSql) 실행.
           ( SELECT 처럼 결과값이 있는경우 db.ExcuteSql실행. INSERT, DELETE, UPDATE 처럼 결과값이 없는경우 db.NonExcuteSql 실행.)
         *
         * 5. select의 경우 db.result 객체에 결과값이 있습니다.
         ***********************/

        private string connectionStr = string.Empty;
        private SqlConnection conn;
        private SqlCommand cmd;
        private List<SqlParameter> parameters;

        CommonModule cm = new CommonModule();

        public bool nState { get; set; }

        public string sql_raise_error_msg = string.Empty;
        public DataTable result { get; set; }

        public MSSQL(string DataBaseName)
        {
            try
            {
                // 프로젝트의 App.config 파일에 ERP_DB_TEST에 대한 경로가 있습니다. 
                this.connectionStr = ConfigurationManager.ConnectionStrings[DataBaseName].ConnectionString;

                parameters = new List<SqlParameter>();

            }
            catch (Exception ex)
            {
                cm.writeLog($"DB_CONNECTION ERR OCCUR : {ex.Message}");
            }
        }


        public bool ExecuteSql(string Sp_name)
        {
            try
            {
                DataSet ds = new DataSet();

                this.sql_raise_error_msg = string.Empty;

                using (conn = new SqlConnection(connectionStr))
                {
                    conn.Open();

                    cmd = new SqlCommand(Sp_name, conn);

                    cmd.CommandTimeout = 0;
                     
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    foreach (SqlParameter sp in parameters)
                    {
                        cmd.Parameters.Add(sp);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);

                    da.Fill(ds);

                    this.result = ds.Tables[0];
                }
                 
                this.nState = true;

                ClearParameters();

                conn.Close();

                return this.nState;

            }
            catch (SqlException Sqlex)
            {
                this.nState = true;

                this.sql_raise_error_msg = Sqlex.Message;

                ClearParameters();

                this.result = null;

                return nState;
            }
            catch (Exception ex)
            {
                ClearFields();

               // cm.writeLog($"db ExecuteSql Error occurs : {ex.Message}");

                return this.nState;
            }
        }



        public bool ExecuteNonSql(string Sp_name)
        {
            try
            {
                DataSet ds = new DataSet();

                using (conn = new SqlConnection(connectionStr))
                {
                    conn.Open();

                    cmd = new SqlCommand(Sp_name, conn);

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    foreach (SqlParameter sp in parameters)
                    {
                        cmd.Parameters.Add(sp);
                    }

                    cmd.CommandTimeout = 0;

                    cmd.ExecuteNonQuery();
                }

                ClearParameters();

                conn.Close();

                return this.nState;
            }
            catch (SqlException Sqlex)
            {
                this.nState = true;

                this.sql_raise_error_msg = Sqlex.Message;

                ClearParameters();

                this.result = null;

                return nState;
            }
            catch (Exception ex)
            {
                ClearFields();

                //cm.writeLog($"db ExecuteNonSql Error occurs : {ex.Message}");

                return this.nState;
            }
        }

        public bool ExecuteNonSql(string Sp_name, Dictionary<string, string> outputValues)
        {
            try
            {
                DataSet ds = new DataSet();

                outputValues.Clear();

                using (conn = new SqlConnection(connectionStr))
                {
                    conn.Open();

                    cmd = new SqlCommand(Sp_name, conn);

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    foreach (SqlParameter sp in parameters)
                    {
                        cmd.Parameters.Add(sp);
                    }

                    cmd.ExecuteNonQuery();

                    foreach (SqlParameter sp in parameters)
                    {
                        if (sp.Direction == ParameterDirection.Output && !outputValues.ContainsKey(sp.ParameterName)) outputValues.Add(sp.ParameterName, sp.Value.ToString());

                    }
                }

                ClearParameters();

                conn.Close();

                return this.nState;
            }
            catch (SqlException Sqlex)
            {
                this.nState = true;

                this.sql_raise_error_msg = Sqlex.Message;

                ClearParameters();

                this.result = null;

                return nState;
            }
            catch (Exception ex)
            {
                ClearFields();

                cm.writeLog($"MSSQL ExecuteNonSql Error : {ex.Message}");

                return this.nState;
            }
        }

        public void Parameter(string mapStr, int value)
        {
            try
            {
                SqlParameter sp = new SqlParameter(mapStr, SqlDbType.Int);
                sp.Value = value;

                parameters.Add(sp);
            }
            catch (Exception ex)
            {
                ClearFields();

                //cm.writeLog($"db String Parameter Error occurs : {ex.Message}");
            }
        }

        public void Parameter(string mapStr, Boolean value)
        {
            try
            {
                SqlParameter sp = new SqlParameter(mapStr, SqlDbType.Bit);
                sp.Value = value;

                parameters.Add(sp);
            }
            catch (Exception ex)
            {
                ClearFields();

                //cm.writeLog($"db String Parameter Error occurs : {ex.Message}");
            }
        }

        public void Parameter(string mapStr, string value)
        {
            try
            {
                SqlParameter sp = new SqlParameter(mapStr, SqlDbType.VarChar);
                sp.Value = value;

                parameters.Add(sp);
            }
            catch (Exception ex)
            {

                ClearFields();

                cm.writeLog($"db String Parameter Error occurs : {ex.Message}");
            }
        }

        public void Parameter(string mapStr, DateTime value)
        {
            try
            {
                SqlParameter sp = new SqlParameter(mapStr, SqlDbType.Date);
                sp.Value = value;

                parameters.Add(sp);
            }
            catch (Exception ex)
            {

                ClearFields();

                cm.writeLog($"db String Parameter Error occurs : {ex.Message}");
            }
        }

        public void Parameter(string mapStr, bool isOutput, int size)
        {
            try
            {
                SqlParameter sp = new SqlParameter(mapStr, SqlDbType.VarChar);
                sp.Direction = isOutput ? ParameterDirection.Output : ParameterDirection.Input;
                sp.Size = size;

                parameters.Add(sp);
            }
            catch (Exception ex)
            {

                ClearFields();

                cm.writeLog($"MSSQL Parameter Error : {ex.Message}");
            }
        }


        public void Close()
        {
            try
            {
                ClearFields();

                conn.Close();
            }
            catch (Exception ex)
            {
                ClearFields();

                cm.writeLog($"db Close Error occurs : {ex.Message}");
            }
        }

        private void ClearParameters()
        {
            try
            {
                this.parameters.Clear();

                this.cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                cm.writeLog($"db ClearParameters Error occurs : {ex.Message}");
            }
        }

        private void ClearFields()
        {
            try
            {
                this.nState = false;

                this.result = null;

                ClearParameters();
            }
            catch (Exception ex)
            {
                cm.writeLog($"db ClearFields Error occurs : {ex.Message}");
            }
        }
    }
}
