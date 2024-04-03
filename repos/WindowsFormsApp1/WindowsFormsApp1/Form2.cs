using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        string connectstring = @"Data Source=DESKTOP-4B7JTQE\THANHSON;Initial Catalog=SQL_login;Integrated Security=True";
        SqlConnection connect;
        SqlCommand cmd;
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable dt = new DataTable();
        public Form2()
        {
            InitializeComponent();
        }
        void loaddata()
        {
            
            cmd = connect.CreateCommand();
            cmd.CommandText = "select * from AccessLog ";
            adapter.SelectCommand = cmd;
            dt.Clear();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            connect = new SqlConnection(connectstring);
            connect.Open();
            loaddata();

        }
        
        
    }
}
