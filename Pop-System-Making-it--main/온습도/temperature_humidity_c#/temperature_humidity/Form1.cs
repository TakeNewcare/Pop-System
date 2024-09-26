using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace temperature_humidity
{

    // https://www.youtube.com/watch?v=ObaXL193xlw
    // 48:00

    

    public partial class Form1 : Form
    {
        server sv = new server();


        double current_t = 0;
        double current_h = 0;
        int x_position = 0;
        int[] produc = new int[4];
        List<Label> labels = new List<Label>();


        public Form1()
        {
            InitializeComponent();

            string qry_getdata = "select LineID, max(ActualProduction) as LineMaxPro from Production_Line_Status group by LineID";
            DataSet ds = sv.GetData(qry_getdata);

            labels.Add(label9);
            labels.Add(label10);
            labels.Add(label11);
            labels.Add(label12);

            for (int i=0; i<4; i++)
            {
                try{
                    produc[i] = (int)ds.Tables[0].Rows[i][1];
                    labels[i].Text = produc[i].ToString();
                }
                catch (Exception ex)
                {
                    produc[i] = 0;
                    labels[i].Text = "0";
                }

            }


            

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (textBox1.Text != "" && !serialPort1.IsOpen)
            {
                 // 온습도
                serialPort1.PortName = textBox1.Text.ToString();

                try {
                    serialPort1.Open();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                MessageBox.Show("오픈!");


            }

            timer1.Start();

        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

            if (serialPort1.IsOpen)
            {
                byte[] buffer = new byte[4];
                serialPort1.Read(buffer, 0, 4);
                serialPort1.Write(buffer, 0, 4);



                current_t = ((buffer[0] * 256) + buffer[1]) / 100.0f;
                current_h = ((buffer[2] * 256) + buffer[3]) / 100.0f;


             //   label5.Text = current_t.ToString("F2") + "c";
             //   label6.Text = current_h.ToString("F2") + "%";




                // 데이터 집합 생성
                ListViewItem lvi = new ListViewItem();
                lvi.Text = DateTime.Now.ToString();

                lvi.SubItems.Add(current_t.ToString("F2"));        // 리스트뷰의 첫번째 아이탬은 text로 넣고 다음 요소들은 subitems.add로 넣는다.
                lvi.SubItems.Add(current_h.ToString("F2"));

                listView1.Items.Add(lvi);

                drawchart();

            }
        }

        // 데이터 전송
        private void sendDataFlask(double current_t, double current_h)
        {
            //sv.DataToflask(current_t, current_h);

            string qry = $"insert into Com_Data (Temperature, Humidity) values ({current_t}, {current_h})";

            sv.SetData(qry);
        }


        


        // 차트 그리기
        private void drawchart()
        {
            chart1.Series[0].Points.AddXY(x_position, current_t);
            chart1.Series[1].Points.AddXY(x_position, current_h);

            // 데이터가 들어오면서 그리면 데이터가 많아지면서 화면에 가득차면 점점 찌그러지는 그래프가 그려진다.
            // =>실시간 데이터를 구현하기 위해서는 오래된 값들이 삭제되면서 구현되어야 한다.

            // 그래프에 그릴 데이터 최대 갯수를 설정 후 오래된 값 삭제
            if (chart1.Series[0].Points.Count >10)
            {
                chart1.Series[0].Points.RemoveAt(0);
            }

            // 데이터를 삭제한 만큼 축을 이동 시켜줘야한다!!!!
            // ChartAreas[0] : 한 chart에 여러개의 차트를 표현할 수 있다.
            chart1.ChartAreas[0].AxisX.Maximum = x_position;                            // 최대 위치는 1씩 증가
            chart1.ChartAreas[0].AxisX.Minimum = chart1.Series[0].Points[0].XValue;     // 첫 데이터의 x 값으로 설정

            x_position++;

        }

        // 중지
        private void button2_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }

        // 파일 출력
        private void button3_Click(object sender, EventArgs e)
        {
            string path = "output.csv";

            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);     // 쓸 준비 완료
            

            for (int i = 0; i<listView1.Items.Count; i++)
            {
                // listView1.Items[i].SubItems[0]
                sw.WriteLine(listView1.Items[i].SubItems[0].Text + "," + listView1.Items[i].SubItems[1].Text + "," + listView1.Items[i].SubItems[2].Text);
            }
            MessageBox.Show("csv파일로 저장 완료");

            sw.Close();
            sw.Dispose();
            fs.Close();
            fs.Dispose();
        }



        // label9~12
        private void button4_Click(object sender, EventArgs e)
        {
            produc[0]++;
            labels[0].Text = produc[0].ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            produc[1]++;
            labels[1].Text = produc[1].ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            produc[2]++;
            labels[2].Text = produc[2].ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            produc[3]++;
            labels[3].Text = produc[3].ToString();
        }
        

        
        string updateData_qry;

        private void timer1_Tick(object sender, EventArgs e)
        {

            for (int i =0; i<4; i++)
            {
                updateData_qry = $"insert into Production_Line_Status (LineID, ActualProduction) values ('Line{i + 1}', {produc[i]})";

                sv.SetData(updateData_qry);

            }

            sendDataFlask(current_t, current_h);


        }
    }
}
