using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Configuration;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string DataTableToJSONWithJSONNet(DataTable table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);

            Console.WriteLine(JSONString);
            return JSONString;
        }

        public string DataTableToJSONWithJavaScriptSerializer(DataTable table)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }

            foreach (Dictionary<string, object> childRow2 in parentRow)
            {
                foreach (KeyValuePair<string, object> kvp in childRow2)
                {
                    ;//  Console.WriteLine("{0}, {1}", kvp.Key, kvp.Value);
                }
            }

            Console.WriteLine(jsSerializer.Serialize(parentRow));

            return jsSerializer.Serialize(parentRow);
        }

        public string DataTableToJSONWithStringBuilder(DataTable table)
        {
            var JSONString = new System.Text.StringBuilder();
            if (table.Rows.Count > 0)
            {
                JSONString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
                JSONString.Append("]");
            }

            Console.Write(JSONString.ToString());
            return JSONString.ToString();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            string oradb = "Data Source=(DESCRIPTION="
                + "(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))"
                + "(CONNECT_DATA=(SERVICE_NAME=XE)));"
                + "User Id=system;Password=password;";

            //var connection = ConfigurationManager.ConnectionStrings["SampleDataSource"].ConnectionString;

            OracleConnection conn = new OracleConnection(oradb);  // C#
            conn.Open();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select department_name,first_name, last_name ,email from hr.departments a, HR.employees b where a.manager_id = b.manager_id";
            cmd.CommandType = CommandType.Text;
            //OracleDataReader dr = cmd.ExecuteReader();
            //dr.Read();

            var dataReader = cmd.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(dataReader);

            DataTableToJSONWithStringBuilder(dataTable);
            DataTableToJSONWithJavaScriptSerializer(dataTable);
            DataTableToJSONWithJSONNet(dataTable);

            //var aa = dr.GetString(1);
            //label1.Text = dr.GetString(0);
            conn.Dispose();
        }

    }
}
