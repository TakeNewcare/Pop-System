using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Operations_Management_System
{
    public class DBServer
    {
        static string con_str ="Server=PROPC; database=popSys; user id=sa; password=1234;";


        public DataSet GetData(string qry)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(con_str))
                {

                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "검색 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ds;
        }

        public int SetData(string qry)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(con_str))
                {
                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        con.Open();
                        return cmd.ExecuteNonQuery();
                    }
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "데이터 설정 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

    }


}
