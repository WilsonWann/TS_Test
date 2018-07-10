using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;

namespace SqlDependency_Test
{
    public partial class _Default : Page
    {
        string ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=aspnet-WebApplication3-20180709032144;Integrated Security=True";
        protected void Page_Load(object sender, EventArgs e)
        {
            SqlDependency.Start(ConnectionString);

            SqlDependencyWatch();

            RefreshTable();
        }
        private void SqlDependencyWatch()
        {
            //這邊用的查詢欄位不能是PK，資料表也必須是完整的像dbo.TableName
            string sql = "SELECT NAME,AGE FROM [dbo].[People]";
            using (var sqlconnection = new SqlConnection(ConnectionString))
            {
                using (var sqlcommand = new SqlCommand(sql, sqlconnection))
                {
                    sqlcommand.CommandType = System.Data.CommandType.Text;
                    sqlconnection.Open();
                    var sqlDependency = new SqlDependency(sqlcommand);

                    //這邊加入監聽事件SQLTableOnChange
                    sqlDependency.OnChange += new OnChangeEventHandler(SQLTableOnChange);
                    var reader = sqlcommand.ExecuteReader();

                }
            }
        }
        private void SQLTableOnChange(object sender, SqlNotificationEventArgs e)
        {
            //觸發後再開啟一次監聽事件    
            SqlDependencyWatch();
            //執行我自己要執行的邏輯處理
            RefreshTable();
        }
        private void RefreshTable()
        {
            string sql2 = "select * from People";
            DataTable datatable = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sql2, connection))
                {
                    using (SqlDataAdapter dr = new SqlDataAdapter(sql2, connection))
                    {
                        dr.Fill(datatable);
                        //這邊要注意，因為SqlDependency是屬於另外個執行緒
                        //所以要使用Invoke來做UI的更新
                        //this.Invoke((EventHandler)(delegate {
                            GridView1.DataSource = datatable;
                            GridView1.DataBind();
                        //}));
                    }
                }
            }
        }

       
       
    }
}