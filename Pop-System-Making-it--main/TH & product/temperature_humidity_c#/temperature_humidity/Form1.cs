using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace temperature_humidity
{
    
    public partial class Form1 : Form
    {
        server sv = new server();

        byte[] buffer = new byte[4];

        double current_t = 0;
        double current_h = 0;
        int x_position = 0;
        int[] produc = new int[4];
        List<Label> labels = new List<Label>();
        byte[] buffer2 = new byte[4];
        int lineNum = 0;


        public Form1()
        {
            InitializeComponent();

            string qry_getdata = "select LineID, max(ActualProduction) as LineMaxPro from Production_Line_Status group by LineID";
            DataSet ds = sv.GetData(qry_getdata);

            labels.Add(label9);
            labels.Add(label10);
            labels.Add(label11);
            labels.Add(label12);

            // 온습도
            serialPort1.PortName = "COM3";

            serialPort2.PortName = "COM5";
    

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

 
            try {
                
                serialPort1.Open();
                serialPort2.Open();
                serialPort1.DiscardInBuffer();
                serialPort2.DiscardInBuffer();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            MessageBox.Show("포트 연결!");

             timer1.Start();
           //  timer2.Start();


        }


        public void Line1Up()
        {
            // 숫자 1 올리기
            produc[0]++;
            this.Invoke((MethodInvoker)delegate
            {

                labels[0].Text = produc[0].ToString();

            });
        }


        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            serialPort1.Read(buffer, 0, 4);

            if (serialPort1.IsOpen)
            {

                int combinedH = (buffer[2] * 256) + buffer[3];

                current_t = buffer[0] + buffer[1] / 100.0f;
                current_h = buffer[2] + buffer[3] / 100.0f;

                serialPort1.DiscardInBuffer();


                this.Invoke((MethodInvoker)delegate
                {

                    label5.Text = current_t.ToString("F2");
                    label6.Text = current_h.ToString("F2");


                    // 데이터 집합 생성
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = DateTime.Now.ToString("HH:mm:ss");
                    lvi.SubItems.Add(current_t.ToString("F2"));        // 리스트뷰의 첫번째 아이탬은 text로 넣고 다음 요소들은 subitems.add로 넣는다.
                    lvi.SubItems.Add(current_h.ToString("F2"));



                    listView1.Items.Insert(0, lvi);

                    if (listView1.Items.Count >= 20)
                    {
                        listView1.Items.RemoveAt(19);
                    }

                });

            }
        }

        // 데이터 전송
        private void sendDataFlask(double current_t, double current_h)
        {

            string qry = $"insert into Com_Data (Temperature, Humidity) values ({current_t}, {current_h})";

            sv.SetData(qry);
        }


        // 중지
        private void button2_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
            if (serialPort2.IsOpen)
            {
                serialPort2.Close();
            }

            timer2.Stop();
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

            labels[0].Text = produc[0].ToString();

            sendDataFlask(current_t, current_h);


        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Line1Up();
        }

        private void serialPort2_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

            serialPort2.Read(buffer2, 0, 4);



            if (buffer2[0] == 5)
            {

                // 숫자 1 올리기
                produc[0]++;

                this.Invoke((MethodInvoker)delegate
                {

                    labels[0].Text = produc[0].ToString();

                });

            }
            serialPort2.DiscardInBuffer();

     
        }


    }
}
