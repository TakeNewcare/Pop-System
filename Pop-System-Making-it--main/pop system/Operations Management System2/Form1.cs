using CircularProgressBar;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using OpenCvSharp.Extensions;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Web.UI.WebControls;



namespace Operations_Management_System
{

    public partial class Form1 : Form
    {
        DBServer DB = new DBServer();
        DataSet ds = new DataSet();
        VideoCapture video;
        Thread thread;

        


        // 작동 관련 변수
        static DateTime nowTime = DateTime.Now;
        string dateString = nowTime.ToString("yyyy-MM-dd");
        int yearMonth = nowTime.Year * 100 + nowTime.Month;

        bool runningState;
        
        int[] OTLine = new int[4];  // 라인별 작동 가능 시간
        int[] TargetPoint = new int[4];  // 라인별 목표 생산량
        double[] beforePros = new double[4];  // 라인별 이전 생산량
        bool[] Rstate = new bool[4];  // 각 라인별 상태
                                      // List<Panel> panels = new List<Panel>();
        int[] checkState = new int[4]; // 5분 전의 값 
        int check = 0;


        System.Windows.Forms.Label[] LOptime = new System.Windows.Forms.Label[4];
        System.Windows.Forms.Label[] LineUpTime = new System.Windows.Forms.Label[4];
        System.Windows.Forms.ProgressBar[] progress = new System.Windows.Forms.ProgressBar[4];
        
        string Tem = "0";
        string Hu = "0";

        int OTAvailable = 0;
        double total =0;

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            startSystem();   // 초기값

            await ConnectCamera();

            timer1.Interval = 5000;
            timer1.Start();

        }


        private async Task ConnectCamera()
        {
            bool cameraCon = false;

            // 카메라가 1개만 연결되어있고 몇번인지 모를 경우
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    video = new VideoCapture(i);
                    if (video.IsOpened())
                    {
                        cameraCon = true;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }


            if (cameraCon)
            {
                await Task.Run(() =>
                {
                    timer2.Start();
                });

            }
            else
            {
                MessageBox.Show("카메라 연결 실패!");
            }

        }


        public void delete_addData()
        {

            // 오늘 날짜와 다른 데이터는 삭제하기!
            string P_sqldel = $"delete from Production_Line_Status where EntryDate != '{nowTime.ToString("yyyy-MM-dd")}'";
            string C_sqldel = $"delete from Com_Data where EntryDate != '{nowTime.ToString("yyyy-MM-dd")}'";

            // 달이 다르면 지우기
            string T_sqldel = $"delete from Total_pro where EntryDate != {yearMonth}";

            // 처음 초기화 시 날짜를 현재 날짜로 설정
            string T_date_sqlup = $"update Total_pro set EntryDate = {yearMonth} where EntryDate != {yearMonth}";

            // 날짜가 바뀌면 합치기
            string T_sqlup = $"update Total_pro set Line1 = Line1 + {beforePros[0]}, Line2 = Line2 + {beforePros[1]}, Line3 = Line3 + {beforePros[2]}, Line4 = Line4 + {beforePros[3]}, EntryDate = {yearMonth} where EntryDate != {yearMonth}";
            // Total_pro

            DB.SetData(T_sqlup);

            int a = DB.SetData(P_sqldel);
            DB.SetData(C_sqldel);

            DB.SetData(T_date_sqlup);


            DB.SetData(T_sqldel);
            // 초기화

            if (a != 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    string P_sqladd = $"insert into Production_Line_Status values ('Line{i + 1}', 0, '{dateString}', '00:00:00')";
                    DB.SetData(P_sqladd);

                }
                string C_sqladd = $"insert into Com_Data values (0, 0, '{dateString}', '00:00:00')";
                DB.SetData(C_sqladd);
            }
 
        }



        // 돌아가는 상태 점검 후 색상 표시
        public void changeStatebar(int i, bool isRunning = true) // isru -> 운행중 or 지연
        {
            Color bgColor = isRunning ? Color.Green : Color.Orange;
            string statusText = isRunning ? $"{i + 1}번 Line\n운행 중" : $"{i + 1}번 Line\n지연";

            if (!Rstate[i])
            {
                 bgColor = Color.Maroon;
                 statusText = $"{i + 1}번 Line\n운행 정지";
            }

            switch (i)
            {
                case 0:
                    Line1bar.BackColor = bgColor;
                    button1.BackColor = bgColor;
                    button1.Text = statusText;
                    break;
                case 1:
                    Line2bar.BackColor = bgColor;
                    button2.BackColor = bgColor;
                    button2.Text = statusText;
                    break;
                case 2:
                    Line3bar.BackColor = bgColor;
                    button3.BackColor = bgColor;
                    button3.Text = statusText;
                    break;
                case 3:
                    Line4bar.BackColor = bgColor;
                    button4.BackColor = bgColor;
                    button4.Text = statusText;
                    break;
            }


        }


        
        // 변동사항 라벨에 표시
        public void changeLabel(System.Windows.Forms.Label lineTarget, System.Windows.Forms.Label otaText, int i)
        {

            lineTarget.Text = TargetPoint[i].ToString();
            otaText.Text = beforePros[i].ToString();

        }


        public void startSystem() 
        {
            var lineTargets = new System.Windows.Forms.Label[] { LineOneTarget, LineTwoTarget, LineThreeTarget, LineFourTarget };
            var otaTexts = new System.Windows.Forms.Label[] { OtaOne, OtaTwo, OtaThree, OtaFour };


            // 오늘 날짜와 다른 데이터는 삭제하기!
            if (MessageBox.Show("데이터를 초기화 하시겠습니까?", "데이터 초기화", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                delete_addData();
            }

            LineUpTime[0]=label18;
            LineUpTime[1] = label20;
            LineUpTime[2] = label22;
            LineUpTime[3] = label30;

            progress[0] = LprogressBar1;
            progress[1] = LprogressBar2;
            progress[2] = LprogressBar3;
            progress[3] = LprogressBar4;

            LOptime[0] = OtaOne;
            LOptime[1] = OtaTwo;
            LOptime[2] = OtaThree;
            LOptime[3] = OtaFour;

            // 라인별 목표 생산량, 작동 가능 시간
            for (int i = 0; i < 4; i++)
            {
                checkState[i] = 0;
                beforePros[i] = 0;

                string sql = $"select Targetproduction, OperatingTimeAvailable from production_targets where Lineid = 'line{i + 1}'";
                

                ds = DB.GetData(sql);
                TargetPoint[i] = (int)ds.Tables[0].Rows[0][0];
                OTLine[i] = (int)ds.Tables[0].Rows[0][1];
                Rstate[i] = false;

                changeStatebar(i);
                changeLabel(lineTargets[i], otaTexts[i], i);
            }


            // 라dls별 실적 그래프 초기화
            int maxYvalue = TargetPoint.Max();
            PerformChart.ChartAreas[0].AxisY.Minimum = 0;
            PerformChart.ChartAreas[0].AxisY.Maximum = maxYvalue;
                        


            circularProgressBar1.Value = 0;
            circularProgressBar2.Value = 0;
            circularProgressBar3.Value = 0;
            circularProgressBar4.Value = 0;

            // 온습도 그래프 초기화 부분
            THChar.Series[0].Points.AddXY(nowTime.ToString("HH:mm:ss"), 0);
            THChar.Series[1].Points.AddXY(nowTime.ToString("HH:mm:ss"), 0);
            THChar.ChartAreas[0].AxisX.Minimum = 0;
            THChar.ChartAreas[0].AxisX.Maximum = 6;


            RateTimeChart.Series[0].Points.AddXY(nowTime.ToString("HH:mm:ss"), 0);
            RateTimeChart.Series[1].Points.AddXY(nowTime.ToString("HH:mm:ss"), 0);
            RateTimeChart.Series[2].Points.AddXY(nowTime.ToString("HH:mm:ss"), 0);
            RateTimeChart.Series[3].Points.AddXY(nowTime.ToString("HH:mm:ss"), 0);
            RateTimeChart.ChartAreas[0].AxisX.Minimum = 0;
            RateTimeChart.ChartAreas[0].AxisX.Maximum = 6;
            RateTimeChart.ChartAreas[0].AxisY.Maximum = 100;
            RateTimeChart.ChartAreas[0].AxisY.Minimum = 0;

            // 한달 생산량
            string sql_Total = "select * from Total_pro";
            ds = DB.GetData(sql_Total);

            PerformChart.Series[0].Points[0].YValues[0] = (int)ds.Tables[0].Rows[0][1];
            PerformChart.Series[0].Points[1].YValues[0] = (int)ds.Tables[0].Rows[0][2];
            PerformChart.Series[0].Points[2].YValues[0] = (int)ds.Tables[0].Rows[0][3];
            PerformChart.Series[0].Points[3].YValues[0] = (int)ds.Tables[0].Rows[0][4];
        }


        // 처음 검색 후 반복
        private void timer1_Tick(object sender, EventArgs e)
        {
            nowTime = DateTime.Now;

            for (int i = 0; i < 4; i++)
            {
                PerformChart.Series[1].Points[i].YValues[0] = beforePros[i];
            }

            // PerformChart.Invalidate(); // 차트 갱신

            label7.Text = nowTime.ToString();

            TimeSpan currentTime = nowTime.TimeOfDay;

            // 온습도
            string sql1 = "select top 1 Temperature, Humidity from Com_Data order by Entrytime Desc";
            THSearch(sql1);


            for (int i = 0; i < 4; i++)
            {
                // 현재 생산량
                string sql2 = $"select TOP 1 ActualProduction, EntryDate from Production_Line_Status where LineID = 'Line{i+1}' order by Entrytime DESC";

                // 작동 시간
                string sql3 = $"select min(Entrytime) as star, max(Entrytime) as cur  from Production_Line_Status where LineID = 'Line{i+1}' and ActualProduction != 0 group by LineID";


                // 생산량(차이) 가져오기
                APSearch(sql2, i);
                PerformChart.Invalidate();



                // 작동시간 비교
                // 누적 돌아간 시간을 저장해야한다!
                Opertime(sql3, i, currentTime);

            }

            total = beforePros[0] + beforePros[1] + beforePros[2] + beforePros[3];

            if (total == 0)
            {
                

                for (int i = 0; i < 4; i++)
                {
                    RateProChart.Series[0].Points[i].IsValueShownAsLabel = true;
                    double result = TargetPoint[i];
                    RateProChart.Series[0].Points[i].YValues[0] = result;
                    RateProChart.Series[0].Points[i].Label = $"{result.ToString("F2")}";

                }
            }
            else
            {

                RateProChart.ChartAreas[0].AxisY.Maximum = total;

                for (int i = 0; i < 4; i++)
                {
                    RateProChart.Series[0].Points[i].IsValueShownAsLabel = true;
                    double result = beforePros[i] * 100 / total;
                    RateProChart.Series[0].Points[i].YValues[0] = result;
                    RateProChart.Series[0].Points[i].Label = $"{result.ToString("F2") + "%"}";

                }
            }

            if (check == 0 || check == 6)
            {
                operationState();
                check = 1;
            }
            check++;

        }

        // 온습도

        public void THSearch(string sql)
        {
            ds = DB.GetData(sql);

            try {

                Tem = ds.Tables[0].Rows[0][0].ToString();
                Hu = ds.Tables[0].Rows[0][1].ToString();

            }
            catch (Exception ex)
            {
                Tem = "0";
                Hu = "0";
            }



            label5.Text = Tem;
            label6.Text = Hu;

            THChar.Series[0].Points.AddXY(nowTime.ToString("HH:mm:ss"), Tem);
            THChar.Series[1].Points.AddXY(nowTime.ToString("HH:mm:ss"), Hu);


            if (THChar.Series[0].Points.Count == 6)
            {
                THChar.Series[0].Points.RemoveAt(0);
                THChar.Series[1].Points.RemoveAt(0);
            }

   
    
            THChar.ChartAreas[0].AxisY.Maximum = ((double.Parse(Tem) + double.Parse(Hu)) / 2) + 50;

            THChar.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm:ss"; // 레이블 형식 설정
        }

        // 시간 간격 체크하기
        public TimeSpan timeGap(TimeSpan starttime, TimeSpan lasttime)
        {
            return lasttime - starttime;
        }


        // 일단 한번 켜지면 안꺼진다는 전제
        // 중간에 중지하는 기능 => 따로 돌아간 시간을 저장해야함
        public void Opertime(string sql, int i, TimeSpan currentTime)
        {


            // 최소 2개의 데이터가 쌓여야 측정하게
            ds = DB.GetData(sql);

            try {
                TimeSpan sTime = (TimeSpan)ds.Tables[0].Rows[0][0];  // 처음 -> 00:00:00

                TimeSpan ctTime = (TimeSpan)ds.Tables[0].Rows[0][1];  // 최근
                TimeSpan time = timeGap(sTime, ctTime);


                int val = Math.Max(0, Math.Min((int)(time.TotalSeconds * 100 / TimeSpan.FromHours(OTLine[i]).TotalSeconds), 100));
                LineUpTime[i].Text = val.ToString() + "%";
                progress[i].Value = val;

                RateTimeChart.Series[i].Points.AddXY(nowTime.ToString("HH:mm:ss"), val);

                if (RateTimeChart.Series[i].Points.Count == 6)
                {
                    RateTimeChart.Series[i].Points.RemoveAt(0);
                }

            }
            catch(Exception ex) {
                LineUpTime[i].Text = "0%";
                progress[i].Value = 0;

                RateTimeChart.Series[i].Points.AddXY(nowTime.ToString("HH:mm:ss"), 0);

                if (RateTimeChart.Series[i].Points.Count == 6)
                {
                    RateTimeChart.Series[i].Points.RemoveAt(0);
                }


            }

        }


        // 게이지 1씩 오르는 동작
        public void AddPro(double addVal, int i)
        {
            switch (i)
            {
                case 0:
                    if (circularProgressBar1.Value + addVal <= 100)
                    {
                        for (int j = 0; j < addVal; j++)
                        {
                            circularProgressBar1.Value++;
                        }
                    }
                    else
                    {
                        while (true)
                        {
                            if (circularProgressBar1.Value == 100)
                                break;
                            circularProgressBar1.Value++;

                        }
                    }
                    break;
                case 1:
                    if (circularProgressBar2.Value + addVal <= 100)
                    {
                        for (int j = 0; j < addVal; j++)
                        {
                            circularProgressBar2.Value++;

                        }
                    }
                    else
                    {
                        while (true)
                        {
                            if (circularProgressBar2.Value == 100)
                                break;
                            circularProgressBar2.Value++;

                        }
                    }
                    break;
                case 2:
                    if (circularProgressBar3.Value + addVal <= 100)
                    {
                        for (int j = 0; j < addVal; j++)
                        {
                            circularProgressBar3.Value++;
                        }
                    }
                    else
                    {
                        while (true)
                        {
                            if (circularProgressBar3.Value == 100)
                                break;
                            circularProgressBar3.Value++;

                        }
                    }
                    break;
                case 3:
                    if (circularProgressBar4.Value + addVal <= 100)
                    {
                        for (int j = 0; j < addVal; j++)
                        {
                            circularProgressBar4.Value++;
                        }
                    }
                    else
                    {
                        while (true)
                        {
                            if (circularProgressBar4.Value == 100)
                                break;
                            circularProgressBar4.Value++;

                        }
                    }
                    break;
            }

        }


        // 이전생산량, 현재생산량
        public void APSearch(string sql, int i)
        {
            ds = DB.GetData(sql);

            int currentVal = (int)ds.Tables[0].Rows[0][0];

            if (currentVal != 0)
            {
                Rstate[i] = true;  // 데이터가 0보다 크면 운행 중 -> true
            }

            double Val = currentVal - beforePros[i];

            beforePros[i] = currentVal;
            LOptime[i].Text = currentVal.ToString();

            double addVal = (Val * 100) / TargetPoint[i];
            AddPro(addVal, i);
            PerformChart.Series[1].Points[i].YValues[0] = currentVal;


        }

        // 운행중 표기
        public void operationState()
        {
            // 현재 값이랑 비교
            for (int i=0; i<4; i++)
            {
                if (beforePros[i] == checkState[i])
                {
                    changeStatebar(i, false);
                }
                else
                {
                    changeStatebar(i);
                }

                checkState[i] = (int)beforePros[i];
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            // 5. Frame을 저장할 Mat 생성
            Mat mat = new Mat();

            // 6. 카메라 영상 mat 저장
            video.Read(mat);

            // 7. mat에 이미지를 bitmap 으로 전환
            Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat);

            // 8. pictureBox1 에 bitmap 에 저장한 이미지 출력
            this.Invoke(new Action(() => pictureBox1.Image = bitmap));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
