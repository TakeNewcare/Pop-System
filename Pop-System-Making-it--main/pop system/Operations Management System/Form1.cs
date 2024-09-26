using CircularProgressBar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;



namespace Operations_Management_System
{
    
    public partial class Form1 : Form
    {
        DBServer DB = new DBServer();
        DataSet ds = new DataSet();
        

        // 작동 관련 변수
        DateTime nowTime = DateTime.Now;
        bool runningState;
        
        int[] OTLine = new int[4];  // 라인별 작동 가능 시간
        int[] TargetPoint = new int[4];  // 라인별 목표 생산량
        double[] beforePros = new double[4];  // 라인별 이전 생산량
        bool[] Rstate = new bool[4];  // 각 라인별 상태
                                      // List<Panel> panels = new List<Panel>();

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

        private void Form1_Load(object sender, EventArgs e)
        {

            startSystem();   // 초기값으로 설정해라 => 바꾸기
            timer1.Interval = 5000;
            timer1.Start();

        }

        // 작동하는지 체크(미완)
        public void isrunning()
        {
            // 1. 일정한 시간 간격으로 돌아가는거 체크하여 상태 변화
            //if (beforeDate.Substring(0, 10) == nowTime.Substring(0,10))
            //    runningState = true;
            //else
            //{
            //    runningState = false;
            //}

            // 2. 최근 기록이 현재시간 기준 5분 이상이 흘렀으면 작동 중지
        }

        public void deleteData()
        {

            // 오늘 날짜와 다른 데이터는 삭제하기!

            string P_sqldel = $"delete from Production_Line_Status where EntryDate != '{nowTime.ToString("yyyy-MM-dd")}'";
            string C_sqldel = $"delete from Com_Data where EntryDate != '{nowTime.ToString("yyyy-MM-dd")}'";

            DB.SetData(P_sqldel);
            DB.SetData(C_sqldel);
        }



        // 돌아가는 상태 점검 후 색상 표시
        public void changeStatebar(int i)
        {
            if (Rstate[i])
            {
                switch (i)
                {
                    case 0:
                        Line1bar.BackColor = Color.Green;
                        button1.BackColor = Color.Green;
                        break;
                    case 1:
                        Line2bar.BackColor = Color.Green;
                        button2.BackColor = Color.Green;

                        break;
                    case 2:
                        Line3bar.BackColor = Color.Green;
                        button3.BackColor = Color.Green;

                        break;
                    case 3:
                        Line4bar.BackColor = Color.Green;
                        button4.BackColor = Color.Green;
                        break;
                }
            
            }
            else
            {
                switch (i)
                {
                    case 0:
                        Line1bar.BackColor = Color.Maroon;
                        button1.BackColor = Color.Maroon;
                        break;
                    case 1:
                        Line2bar.BackColor = Color.Maroon;
                        button2.BackColor = Color.Maroon;
                        break;
                    case 2:
                        Line3bar.BackColor = Color.Maroon;
                        button3.BackColor = Color.Maroon;
                        break;
                    case 3:
                        Line4bar.BackColor = Color.Maroon;
                        button4.BackColor = Color.Maroon;
                        break;
                }
            }
        }

        // 변동사항 라벨에 표시
        public void changeLabel(int i)
        {
            switch (i)
            {
                case 0:
                    LineOneTarget.Text = TargetPoint[i].ToString();
                    OtaOne.Text = beforePros[i].ToString();
                    break;
                case 1:
                    LineTwoTarget.Text = TargetPoint[i].ToString();
                    OtaTwo.Text = beforePros[i].ToString();

                    break;
                case 2:
                    LineThreeTarget.Text = TargetPoint[i].ToString();
                    OtaThree.Text = beforePros[i].ToString();

                    break;
                case 3:
                    LineFourTarget.Text = TargetPoint[i].ToString();
                    OtaFour.Text = beforePros[i].ToString();
                    break;
            }
        }


        public void startSystem()  // 초기값으로 설정하기(미완)
        {
            // 오늘 날짜와 다른 데이터는 삭제하기!

            deleteData();
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

                beforePros[i] = 0;

                string sql = $"select Top 1 T.targetproduction, T.OperatingTimeAvailable from production_targets as T inner join production_line_status as L on T.LineID = L.LineID where T.Lineid = 'line{i+1}' order by L.actualproduction desc";
                
                ds = DB.GetData(sql);
                TargetPoint[i] = (int)ds.Tables[0].Rows[0][0];
                OTLine[i] = (int)ds.Tables[0].Rows[0][1];
                beforePros[i] = 0;
                Rstate[i] = false;

                changeStatebar(i);
                changeLabel(i);
            }


            // 라이별 실적 그래프 초기화
            int maxYvalue = TargetPoint.Max();
            PerformChart.ChartAreas[0].AxisY.Minimum = 0;
            PerformChart.ChartAreas[0].AxisY.Maximum = maxYvalue;
                        
            PerformChart.Series[0].Points[0].YValues[0] = TargetPoint[0];
            PerformChart.Series[0].Points[1].YValues[0] = TargetPoint[1];
            PerformChart.Series[0].Points[2].YValues[0] = TargetPoint[2];
            PerformChart.Series[0].Points[3].YValues[0] = TargetPoint[3];

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
            string sql1 = "select top 1 Temperature, Humidity from Com_Data order by EntryDate Desc";
            THSearch(sql1);


            for (int i = 0; i < 4; i++)
            {
                // 현재 생산량
                string sql2 = $"select TOP 1 ActualProduction, EntryDate from Production_Line_Status where LineID = 'Line{i+1}' order by Entrytime DESC";

                // 작동 시간
                string sql3 = $"select min(Entrytime) as star, max(Entrytime) as cur  from Production_Line_Status where LineID = 'Line{i+1}' group by LineID";


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


        }

        // 온습도

        public void THSearch(string sql)
        {
            ds = DB.GetData(sql);

            Tem = ds.Tables[0].Rows[0][0].ToString();
            Hu = ds.Tables[0].Rows[0][1].ToString();

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
            ds = DB.GetData(sql);

            TimeSpan sTime = (TimeSpan)ds.Tables[0].Rows[0][0];  // 이전
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
            
            double Val = currentVal - beforePros[i];

            beforePros[i] = currentVal;
            LOptime[i].Text = currentVal.ToString();

            double addVal = (Val * 100) / TargetPoint[i];
            AddPro(addVal, i);
            PerformChart.Series[1].Points[i].YValues[0] = currentVal;


        }
    }
}
