
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMBINATION.Modules
{
    class SQLITE3
    {

        private string connectionStr = string.Empty;
        private SQLiteConnection conn;
        private SQLiteCommand cmd;

        private SQLiteDataReader rdr;

        private List<SQLiteParameter> parameters;

        CommonModule cm = new CommonModule();

        public bool nState { get; set; }

        public string sql_raise_error_msg = string.Empty;
        public DataTable result { get; set; }


        public SQLITE3(string dbPath)
        {
            try
            {
                this.connectionStr = dbPath;
            }
            catch (Exception ex) 
            {

            }
        }


        public bool ExecuteSql(string Query)
        {
            try
            {
                DataSet ds = new DataSet();

                this.sql_raise_error_msg = string.Empty;

                using (conn = new SQLiteConnection(connectionStr))
                {

                    this.result = ds.Tables[0];
                }

                this.nState = true;

                conn.Close();

                return this.nState;

            }
            catch (SQLiteException Sqlex)
            {
                this.nState = false;

                this.sql_raise_error_msg = Sqlex.Message;

                Console.WriteLine(Sqlex);

                cmd.Parameters.Clear();

                this.result = null;

                return nState;
            }
            catch (Exception ex)
            {
                return this.nState;
            }
        }



        public bool ExecuteNonSqlForInsert(List<ProductionRecords> dataList)
        {
            try
            {
                DataSet ds = new DataSet();

                this.sql_raise_error_msg = string.Empty;

                string DeleteQuery = "DELETE FROM production_records";

                string initSequenceQuery = "UPDATE SQLITE_SEQUENCE SET seq = 0 WHERE name = 'production_records'";

                string InsertQuery = "INSERT INTO production_records (production_date, lot_number, item_code, item_name, good_quantity) VALUES ($production_date, $lot_number, $item_code, $item_name, $good_quantity)";

                using (conn = new SQLiteConnection(connectionStr))
                {
                    conn.Open();

                    using (var tran = conn.BeginTransaction())
                    {
                        cmd = new SQLiteCommand(DeleteQuery, conn);

                        cmd.ExecuteNonQuery();

                        cmd = new SQLiteCommand(initSequenceQuery, conn);

                        cmd.ExecuteNonQuery();

                        cmd = new SQLiteCommand(InsertQuery, conn);

                        var production_date = cmd.CreateParameter();

                        production_date.ParameterName = "$production_date";

                        cmd.Parameters.Add(production_date);

                        var lot_number = cmd.CreateParameter();

                        lot_number.ParameterName = "$lot_number";

                        cmd.Parameters.Add(lot_number);

                        var item_code = cmd.CreateParameter();

                        item_code.ParameterName = "$item_code";

                        cmd.Parameters.Add(item_code);

                        var item_name = cmd.CreateParameter();

                        item_name.ParameterName = "$item_name";

                        cmd.Parameters.Add(item_name);

                        var good_quantity = cmd.CreateParameter();

                        good_quantity.ParameterName = "$good_quantity";

                        cmd.Parameters.Add(good_quantity);


                        foreach (ProductionRecords rec in dataList)
                        {
                            production_date.Value = rec.GetProduction_date();

                            lot_number.Value = rec.GetIot_number();

                            item_code.Value = rec.Getitem_code();

                            item_name.Value = rec.Getitem_name();

                            good_quantity.Value = rec.Getgood_quantity();

                            cmd.ExecuteNonQuery();
                        }

                        tran.Commit();

                    }

                    conn.Close();
                }

                this.nState = true;

                return this.nState;
            }
            catch (SQLiteException Sqlex)
            {
                this.nState = false;

                this.sql_raise_error_msg = Sqlex.Message;

                cmd.Parameters.Clear();

                this.result = null;

                return nState;  
            }  
            catch (Exception ex)
            {
                this.nState = false;

                this.sql_raise_error_msg = ex.Message;

                cmd.Parameters.Clear();

                this.result = null;

                return nState;
            }
        }


        public void Close()
        {
            try
            {
                conn.Close();
            }
            catch (Exception ex)
            {
                cm.writeLog($"db Close Error occurs : {ex.Message}");
            }
        }


 
    }
}
