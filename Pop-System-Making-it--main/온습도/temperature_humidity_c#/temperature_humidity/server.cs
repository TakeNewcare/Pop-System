using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics.PerformanceData;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace temperature_humidity
{
    internal class server
    {
        
        // 웹과 통신
        // 보낼 주소 설정
        //private static readonly HttpClient httpClient = new HttpClient();
        // private string flaskServer = "http://43.203.128.213:8000/live-data";
        //// private string flaskServer = "http://127.0.0.1:5000/live-data";

        //// 데이터 정의
        //public async void DataToflask(double T, double H)
        //{
        //    // 한국 시간(KST)으로 변환
        //    var koreanTimeOffset = TimeSpan.FromHours(9);
        //    var utcNow = DateTimeOffset.UtcNow;
        //    var koreanTime = utcNow.ToOffset(koreanTimeOffset);

        //    // 한국 시간의 밀리초 단위 타임스탬프
        //    var timestamp = koreanTime.ToUnixTimeMilliseconds();

        //    var data = new
        //    {
        //        Timestamp = timestamp,
        //        Temperature = T,
        //        Humidity = H
        //    };

            

        //    var jsonData = JsonConvert.SerializeObject(data);
        //    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        //    await sendData(content);

        //}

        //private async Task sendData(HttpContent content)
        //{
            

        //    try {
            
        //        var response = await httpClient.PostAsync(flaskServer, content);

        //        // 서버 연걸된 상태에서 전송

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //    }


        //}
        //


        // mssql 저장
        string con_str = "Server=PROPC; database=popSys; user id=sa; password=1234;";

        public void SetData(string qry)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(con_str)) {
                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "데이터 설정 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

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
            catch(Exception ex)
            {

                MessageBox.Show(ex.ToString(), "데이터 읽기 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            return ds;



        }
        

        





    }
}
