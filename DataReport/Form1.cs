using NPOI.OpenXml4Net.OPC;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DataReport
{
    public delegate void ShowSelectDataDelegate(DataTable str);
    public delegate void ShowUpdateDataDelegate(bool istrue);

    public delegate void ShowLoadingDataTimes(int currentnum,int count,string name);

    public delegate void MyInvoke(string str, int type);
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void btnLoginDataBase_Click(object sender, EventArgs e)
        {
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            this.btnExcelImport.Enabled = false;
            this.toolStripStatusLabel1.Text = "正在运行……";
            ReadExcel.Instance.IsRepate = cbExcelIsRepate.Checked;
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(importExceltoSqlite));
            th.Start(this);
        }

        private void importExceltoSqlite(object obj)
        {

            ReadExcel.Instance.myLoadingDataTimesEvent -= re_myLoadingDataTimesEvent;
            ReadExcel.Instance.myLoadingDataTimesEvent += re_myLoadingDataTimesEvent;
            ReadExcel.Instance.ReadExcelFile(obj);
            Console.WriteLine("数据导入数据库完成！！");
            Lv.Log.Write("数据导入数据库完成！！ " , Lv.Log.MessageType.Info);
            MyInvoke mi = new MyInvoke(SetTxt);
            BeginInvoke(mi, new object[] { "数据导入数据库完成！", 1 });
        }

        void re_myLoadingDataTimesEvent(int currentnum, int count, string name)
        {
            this.Invoke((EventHandler)delegate 
            {
                this.toolStripProgressBar1.Maximum = count;
                this.toolStripProgressBar1.Value = currentnum;
                this.toolStripStatusLabel2.Text = currentnum.ToString() + "/" + count.ToString() + " 来自:" + name;

            });
        }

        public void SetTxt(string str, int type)
        {
            if (type == 1)
            {
                this.btnExcelImport.Enabled = true;
                this.toolStripProgressBar1.Maximum = 0;
                this.toolStripProgressBar1.Value = 0;
                this.toolStripStatusLabel2.Text = "";
            }
            else if (type == 2)
            {
                //第三个铵钮显示出来
                //第三个因该常亮才对
                // this.btnBuild.Enabled = true;
            }
            else if (type == 3)
            {
                btnOutExcel.Enabled = true;
            }
            else if (type == 4)
            {
                btnFilter.Enabled = true;
            }
            else if (type == 5)
            {
                btnFilterSelect.Enabled = true;
            }
            else if (type == 6)
            {
                btnSelect.Enabled = true;
            }
            else if (type == 7)
            {
                btnSelectInsert.Enabled = true;
            }
            else if (type == 8)
            {
                btnInsert.Enabled = true;
            }
            this.toolStripStatusLabel1.Text = str;
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            //this.btnBuild.Enabled = false;
            //this.toolStripStatusLabel1.Text = "正在运行……";
            //System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(buildSummary));
            //th.Start();
        }

        private void buildSummary(object obj)
        {
            Summary sum = new Summary();
            sum.BuilderStart();
            Console.WriteLine("数据导入Summary完成！！");
            MyInvoke mi = new MyInvoke(SetTxt);
            BeginInvoke(mi, new object[] { "数据导入汇总表完成！", 2 });
        }

        private void btnOutExcel_Click(object sender, EventArgs e)
        {
            btnOutExcel.Enabled = false;
            this.toolStripStatusLabel1.Text = "正在运行……";
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "xlsx files|*.xlsx|csv files|*.csv";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(OutExcel));
                th.Start((object)sfd.FileName);
            }
        }
     

        private void OutExcel(object obj)
        {
            StringBuilder selectSql = new StringBuilder();
            this.Invoke((EventHandler)(delegate
                {
                    int filedindex = 0;
                    string and = "";

                    if (cbDay.Text != "")
                    {
                        if (filedindex > 0)
                        {
                            and = " and ";

                        } filedindex++;
                        selectSql.Append(and + "[日期]='" + cbDay.Text + "'");
                    }
                    if (cbWeek.Text != "")
                    {
                        if (filedindex > 0)
                        {
                            and = " and ";

                        } filedindex++;
                        selectSql.Append(and + "[星期]='" + cbWeek.Text + "'");
                    }
                    if (cbSource.Text != "")
                    {
                        if (filedindex > 0)
                        {
                            and = " and ";

                        } filedindex++;
                        selectSql.Append(and + "[广告来源]='" + cbSource.Text + "'");
                    }
                    if (cbDevice.Text != "")
                    {
                        if (filedindex > 0)
                        {
                            and = " and ";

                        } filedindex++;
                        selectSql.Append(and + "[设备类型]='" + cbDevice.Text + "'");
                    }
                    if (cbSerice.Text != "")
                    {
                        if (filedindex > 0)
                        {
                            and = " and ";

                        } filedindex++;
                        selectSql.Append(and + "[广告系列]='" + cbSerice.Text + "'");
                    }
                    if (cbGroup.Text != "")
                    {
                        if (filedindex > 0)
                        {
                            and = " and ";

                        } filedindex++;
                        selectSql.Append(and + "[广告分组]='" + cbGroup.Text + "'");
                    }
                    if (cbKey.Text != "")
                    {
                        if (filedindex > 0)
                        {
                            and = " and ";

                        } filedindex++;
                        selectSql.Append(and + "[广告关键词]='" + cbKey.Text + "'");
                    }

                }));

            if (selectSql.ToString() != "")
            {
                selectSql.Insert(0, " WHERE ");
            }
            string path = (string)obj;
            sqlite.myLoadingDataTimesEvent -= re_myLoadingDataTimesEvent;
            sqlite.myLoadingDataTimesEvent += re_myLoadingDataTimesEvent;
            if (path.EndsWith("csv") == true)
            {
                sqlite.OutPutExcelCSV(path, selectSql.ToString());
            }
            else
            {
                sqlite.OutPutExcel(path, selectSql.ToString());
            }
            Console.WriteLine("数据导出EXCEL完成！！");
            MyInvoke mi = new MyInvoke(SetTxt);
            BeginInvoke(mi, new object[] { "数据导出EXCEL完成！", 3 });
            Lv.Log.Write("数据导出EXCEL完成 ", Lv.Log.MessageType.Info);
        }
        DataBaseSqlite sqlite = DataBaseSqlite.getInstance();

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            btnFilter.Enabled = false;
            this.toolStripStatusLabel1.Text = "正在运行……";

            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(OutFilter));
            th.Start();
        }

        private void OutFilter(object obj)
        {
           
            this.Invoke((EventHandler)(delegate
                {
                    UpdateUISelectComboxItem();
                }));
            MyInvoke mi = new MyInvoke(SetTxt);
            BeginInvoke(mi, new object[] { "更新筛选列表完成", 4 });
        }

        private void UpdateUISelectComboxItem()
        {
            FilterField filter = new FilterField();
            filter.GetDataTableList();
            cbDay.Items.Clear();
            cbWeek.Items.Clear();
            cbSource.Items.Clear();
            cbDevice.Items.Clear();
            cbSerice.Items.Clear();
            cbGroup.Items.Clear();
            cbKey.Items.Clear();

            cbDaySelect.Items.Clear();
            cbWeekSelect.Items.Clear();
            cbSourceSelect.Items.Clear();
            cbDeviceSelect.Items.Clear();
            cbSericeSelect.Items.Clear();
            cbGroupSelect.Items.Clear();
            cbKeySelect.Items.Clear();

            foreach (var item in filter.Listday)
            {
                cbDay.Items.Add(item);
                cbDaySelect.Items.Add(item);
            }
            foreach (var item in filter.Listweek)
            {
                cbWeek.Items.Add(item);
                cbWeekSelect.Items.Add(item);
            }
            foreach (var item in filter.Listsource)
            {
                cbSource.Items.Add(item);
                cbSourceSelect.Items.Add(item);
            }
            foreach (var item in filter.Listdevice)
            {
                cbDevice.Items.Add(item);
                cbDeviceSelect.Items.Add(item);
            }
            foreach (var item in filter.Listserice)
            {
                cbSerice.Items.Add(item);
                cbSericeSelect.Items.Add(item);
            }
            foreach (var item in filter.Listgroup)
            {
                cbGroup.Items.Add(item);
                cbGroupSelect.Items.Add(item);
            }
            foreach (var item in filter.Listkey)
            {
                cbKey.Items.Add(item);
                cbKeySelect.Items.Add(item);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            btnFilterSelect.Enabled = false;

            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(OutFilterSelect));
            th.Start();
        }

        private void OutFilterSelect(object obj)
        {
            this.Invoke((EventHandler)(delegate
            {
                UpdateUISelectComboxItem();
               

            }));
            MyInvoke mi = new MyInvoke(SetTxt);
            BeginInvoke(mi, new object[] { "更新筛选列表完成", 5 });
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            btnSelect.Enabled = false;
            this.toolStripStatusLabel1.Text = "正在运行……";
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Select));
            th.Start();
        }

        void sqlite_mySelectDataevent(DataTable data)
        {
            this.Invoke((EventHandler)delegate
            {
                dataGridView1.DataSource = data;
            });
        }

        private void Select(object obj)
        {
            StringBuilder selectSql = new StringBuilder();
            this.Invoke((EventHandler)(delegate
            {
                int filedindex = 0;
                string and = "";

                if (cbDaySelect.Text != "")
                {
                    if (filedindex > 0)
                    {
                        and = " and ";

                    } filedindex++;
                    selectSql.Append(and + "[日期]='" + cbDaySelect.Text + "'");
                }
                if (cbWeekSelect.Text != "")
                {
                    if (filedindex > 0)
                    {
                        and = " and ";

                    } filedindex++;
                    selectSql.Append(and + "[星期]='" + cbWeekSelect.Text + "'");
                }
                if (cbSourceSelect.Text != "")
                {
                    if (filedindex > 0)
                    {
                        and = " and ";

                    } filedindex++;
                    selectSql.Append(and + "[广告来源]='" + cbSourceSelect.Text + "'");
                }
                if (cbDeviceSelect.Text != "")
                {
                    if (filedindex > 0)
                    {
                        and = " and ";

                    } filedindex++;
                    selectSql.Append(and + "[设备类型]='" + cbDeviceSelect.Text + "'");
                }
                if (cbSericeSelect.Text != "")
                {
                    if (filedindex > 0)
                    {
                        and = " and ";

                    } filedindex++;
                    selectSql.Append(and + "[广告系列]='" + cbSericeSelect.Text + "'");
                }
                if (cbGroupSelect.Text != "")
                {
                    if (filedindex > 0)
                    {
                        and = " and ";

                    } filedindex++;
                    selectSql.Append(and + "[广告分组]='" + cbGroupSelect.Text + "'");
                }
                if (cbKeySelect.Text != "")
                {
                    if (filedindex > 0)
                    {
                        and = " and ";

                    } filedindex++;
                    selectSql.Append(and + "[广告关键词]='" + cbKeySelect.Text + "'");
                }

            }));

            if (selectSql.ToString() != "")
            {
                selectSql.Insert(0, " WHERE ");
            }
            //Summary sum = new Summary();
            //sum.Select(selectSql.ToString());

            DataBaseSqlite sqlite = DataBaseSqlite.getInstance();
            sqlite.mySelectDataEvent -= sqlite_mySelectDataevent;
            sqlite.mySelectDataEvent += sqlite_mySelectDataevent;
            sqlite.SelectData(selectSql.ToString());
            Console.WriteLine("数据查询完成！！");
            MyInvoke mi = new MyInvoke(SetTxt);
            BeginInvoke(mi, new object[] { "数据查询完成！", 6 });
            Lv.Log.Write("数据查询完成！ " , Lv.Log.MessageType.Info);
        }

        private void btnSelectInsert_Click(object sender, EventArgs e)
        {
            btnSelectInsert.Enabled = false;
            this.toolStripStatusLabel1.Text = "正在运行……";
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(SelectInsert));
            th.Start();
        }

        private void SelectInsert(object obj)
        {
            StringBuilder selectSql = new StringBuilder();
            this.Invoke((EventHandler)(delegate
            {
                int filedindex = 0;
                string and = "";

                if (txtSearchID.Text != "")
                {
                    if (filedindex > 0)
                    {
                        and = " and ";

                    } filedindex++;
                    selectSql.Append(and + "[ID]='" + txtSearchID.Text + "'");
                }
                else
                {
                    MessageBox.Show("请输入ID信息");
                    return;
                }
                if (txtSearchDay.Text != "")
                {
                    if (filedindex > 0)
                    {
                        and = " and ";

                    } filedindex++;
                    selectSql.Append(and + "[日期]='" + txtSearchDay.Text + "'");
                }
                else
                {
                    MessageBox.Show("请输入日期信息");
                    return;
                }
            }));

            if (selectSql.ToString() != "")
            {
                selectSql.Insert(0, " WHERE ");
            }
            //Summary sum = new Summary();
            //sum.Select(selectSql.ToString());

            DataBaseSqlite sqlite = DataBaseSqlite.getInstance();
            sqlite.mySelectDataEvent -= sqlite_myInsertevent;
            sqlite.mySelectDataEvent += sqlite_myInsertevent;
            sqlite.SelectData(selectSql.ToString());
            Console.WriteLine("数据查询完成！！");
            MyInvoke mi = new MyInvoke(SetTxt);
            BeginInvoke(mi, new object[] { "数据查询完成！", 7 });
            Lv.Log.Write("数据查询完成！ ", Lv.Log.MessageType.Info);
        }

        private void sqlite_myInsertevent(DataTable data)
        {
            this.Invoke((EventHandler)delegate
            {
                if (data == null)
                {
                    return;
                }
                if (data.Rows.Count < 1)
                {
                    return;
                }
                DataRow dr = data.Rows[0];
                labId.Text = dr[0].ToString();
                txtDay.Text = dr[2].ToString();
                txtWeek.Text = dr[3].ToString();
                txtSource.Text = dr[4].ToString();
                txtDevice.Text = dr[5].ToString();
                txtSerice.Text = dr[6].ToString();
                txtGroup.Text = dr[7].ToString();
                txtKey.Text = dr[8].ToString();
                txtShow.Text = dr[11].ToString();
                txtClick.Text = dr[12].ToString();
                txtConsumption.Text = dr[13].ToString();
                txtCTR.Text = dr[14].ToString();
                txtAvgPriceInsert.Text = dr[15].ToString();
                txtLogin.Text = dr[16].ToString();
                txtReg.Text = dr[17].ToString();
                txtAppDown.Text = dr[18].ToString();
                txtRegSuccess.Text = dr[19].ToString();
                txtFristPama.Text = dr[20].ToString();

            });
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            btnSelectInsert.Enabled = false;
            string result = CheckData();
            if (result != "")
            {
                MessageBox.Show(result);
                return;
            }
            this.toolStripStatusLabel1.Text = "正在运行……";
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(InsertData));
            th.Start();
        }

        private string CheckData()
        {
            string result = "";
            string date = Common.GetWeekDate(txtDay.Text);
            if (date == "")
            {
                result += "日期输入不对，例 2015/10/19\r\n";
            }
            if (txtSource.Text == "")
            {
                result += "广告来源不能为空";
            }
            if (txtDevice.Text == "")
            {
                result += "设备类型不能为空";
            }
            if (txtSerice.Text == "")
            {
                result += "广告系列不能为空";
            }
            if (txtGroup.Text == "")
            {
                result += "广告分组不能为空";
            }
            if (txtKey.Text == "")
            {
                result += "广告关键词不能为空";
            }
            return result;
        }

        private void InsertData(object obj)
        {
            StringBuilder sql = new StringBuilder();
            int type = 0;
            this.Invoke((EventHandler)(delegate
            {
                if (labId.Text == "")
                {
                    //插入新数据
                    sql = InsertData();
                    type = 1;
                }
                else
                {
                    sql = updateData();
                    //更新数据
                }

            }));

            DataBaseSqlite sqlite = DataBaseSqlite.getInstance();
            sqlite.myUpdateDataEvent -= sqlite_myUpdateevent;
            sqlite.myUpdateDataEvent += sqlite_myUpdateevent;
            sqlite.UpdateDataOption(sql.ToString(), type);
            Console.WriteLine("数据查询完成！！");
            MyInvoke mi = new MyInvoke(SetTxt);
            BeginInvoke(mi, new object[] { "数据查询完成！", 7 });
            Lv.Log.Write("数据查询完成！ ", Lv.Log.MessageType.Info);
        }

        private void sqlite_myUpdateevent(bool istrue)
        {
            this.Invoke((EventHandler)delegate
            {
                if (istrue == true)
                {
                    MessageBox.Show("数据更新成功！");
                }
                else
                {
                    MessageBox.Show("数据更新失败！");
                }

            });
        }

        private StringBuilder updateData()
        {
            StringBuilder update = new StringBuilder();



            {
                update.Append("[日期] = '" + txtDay.Text + "', ");
            }

            {
                update.Append("[星期] = '" + txtWeek.Text + "', ");
            }

            {
                update.Append("[广告来源] = '" + txtSource.Text + "', ");
            }

            {
                update.Append("[设备类型] = '" + txtDevice.Text + "', ");
            }

            {
                update.Append("[广告系列] = '" + txtSerice.Text + "', ");
            }

            {
                update.Append("[广告分组] = '" + txtGroup.Text + "', ");
            }

            {
                update.Append("[广告关键词] = '" + txtKey.Text + "', ");
            }

            {
                update.Append("[展现量] = '" + txtShow.Text + "', ");
            }

            {
                update.Append("[点击量] = '" + txtClick.Text + "', ");
            }

            {
                update.Append("[消费] = '" + txtConsumption.Text + "', ");
            }

            {
                update.Append("[点击率] = '" + txtCTR.Text + "', ");
            }

            {
                update.Append("[平均点击价格] = '" + txtAvgPriceInsert.Text + "', ");
            }

            {
                update.Append("[登陆] = '" + txtLogin.Text + "', ");
            }

            {
                update.Append("[注册] = '" + txtReg.Text + "', ");
            }

            {
                update.Append("[APP下载] = '" + txtAppDown.Text + "', ");
            }

            {
                update.Append("[注册成功] = '" + txtRegSuccess.Text + "', ");
            }

            {
                update.Append("[首参] = '" + txtFristPama.Text + "' ");
            }
            string updatesql = update.ToString().Trim().Trim(',');
            update = new StringBuilder();
            update.Append(updatesql + " where id = '" + labId.Text + "'");
            // keymd5  2# 3#
            /*
            if (txtDay.Text != "")
            {
                update.Append("[日期] = '" + txtDay.Text + "' and ");
            }
            if (txtDay.Text != "")
            {
                update.Append("[日期] = '" + txtDay.Text + "' and ");
            }
            if (txtDay.Text != "")
            {
                update.Append("[日期] = '" + txtDay.Text + "' and ");
            }
            if (txtDay.Text != "")
            {
                update.Append("[日期] = '" + txtDay.Text + "' and ");
            }
            
            */
            return update;
        }

        private StringBuilder InsertData()
        {

            StringBuilder insert = new StringBuilder();

            insert.Append("([日期],[星期],[广告来源],[设备类型],[广告系列],[广告分组],[广告关键词],[展现量],[点击量],[消费],[点击率],[平均点击价格],[登陆],[注册],[APP下载],[注册成功],[首参],[keymd5],[#2],[#3])values(");

            {
                insert.Append("'" + txtDay.Text + "', ");
            }

            {
                insert.Append("'" + txtWeek.Text + "', ");
            }

            {
                insert.Append("'" + txtSource.Text + "', ");
            }

            {
                insert.Append("'" + txtDevice.Text + "', ");
            }

            {
                insert.Append("'" + txtSerice.Text + "', ");
            }

            {
                insert.Append("'" + txtGroup.Text + "', ");
            }

            {
                insert.Append("'" + txtKey.Text + "', ");
            }

            {
                insert.Append("'" + txtShow.Text + "', ");
            }

            {
                insert.Append("'" + txtClick.Text + "', ");
            }

            {
                insert.Append("'" + txtConsumption.Text + "', ");
            }

            {
                insert.Append("'" + txtCTR.Text + "', ");
            }

            {
                insert.Append("'" + txtAvgPriceInsert.Text + "', ");
            }

            {
                insert.Append("'" + txtLogin.Text + "', ");
            }

            {
                insert.Append("'" + txtReg.Text + "', ");
            }

            {
                insert.Append("'" + txtAppDown.Text + "', ");
            }

            {
                insert.Append("'" + txtRegSuccess.Text + "', ");
            }

            {
                insert.Append("'" + txtFristPama.Text + "',");
            }

            insert.Append("'" + Common.GetWeekDate(txtDay.Text) + "',");
            insert.Append("'" + txtSerice.Text + "#" + txtGroup.Text + "',");
            insert.Append("'" + txtSerice.Text + "#" + txtGroup.Text + "#" + txtKey.Text + "');");


            return insert;
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            txtDay.Text = "";
            txtWeek.Text = "";
            txtSource.Text = "";
            txtDevice.Text = "";
            txtSerice.Text = "";
            txtGroup.Text = "";
            txtKey.Text = "";
            txtShow.Text = "";
            txtClick.Text = "";
            txtConsumption.Text = "";
            txtCTR.Text = "";
            txtAvgPriceInsert.Text = "";
            txtLogin.Text = "";
            txtReg.Text = "";
            txtAppDown.Text = "";
            txtRegSuccess.Text = "";
            txtFristPama.Text = "";
            labId.Text = "";
        }

        /// <summary>
        /// ＡＣＣＥＳＳ导入数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAccessImport_Click(object sender, EventArgs e)
        {
            this.btnAccessImport.Enabled = false;
            this.toolStripStatusLabel1.Text = "正在运行……";
            AccessImport.Instance.IsRepate = cbIsAccessRepate.Checked;
            string[] tables = txtAccessTables.Text.Split('\r');
            AccessImport.Instance.ListTables.Clear();
            foreach (string item in tables)
            {
                string tablename = item.Trim('\n');
                if (tablename == "")
                {
                    continue;
                }
                AccessImport.Instance.ListTables.Add(tablename);
            }
            if (AccessImport.Instance.DataBasePath == "" || AccessImport.Instance.ListTables.Count<1)
            {
                MessageBox.Show("请指定打开的数据库及表!");
                return;
            }
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(importAccesstoSqlite));
            th.Start();
        }

        private void importAccesstoSqlite(object obj)
        {
            AccessImport.Instance.myLoadingDataTimesEvent -= re_myLoadingDataTimesEvent;
            AccessImport.Instance.myLoadingDataTimesEvent += re_myLoadingDataTimesEvent;
            AccessImport.Instance.Access2007Import();
            Console.WriteLine("Access数据导入数据库完成！！");
            Lv.Log.Write("Access数据导入数据库完成！！ ", Lv.Log.MessageType.Info);

            this.Invoke((EventHandler)delegate
            {
                this.toolStripStatusLabel1.Text = "Access数据导入数据库完成！";
                this.btnAccessImport.Enabled = true;
                this.toolStripProgressBar1.Maximum = 0;
                this.toolStripProgressBar1.Value = 0;
                this.toolStripStatusLabel2.Text = "";
            });
        }

        private void btnModfly_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                this.tabControl2.SelectedIndex = 1;
                //设置界面数据
                DataGridViewRow dr = dataGridView1.SelectedRows[0];
                txtSearchID.Text = dr.Cells[0].Value.ToString();
                txtSearchDay.Text = dr.Cells[2].Value.ToString();

                txtDay.Text = dr.Cells[2].Value.ToString();
                txtWeek.Text = dr.Cells[3].Value.ToString();
                txtSource.Text = dr.Cells[4].Value.ToString();
                txtDevice.Text = dr.Cells[5].Value.ToString();
                txtSerice.Text = dr.Cells[6].Value.ToString();
                txtGroup.Text = dr.Cells[7].Value.ToString();
                txtKey.Text = dr.Cells[8].Value.ToString();
                txtShow.Text = dr.Cells[11].Value.ToString();
                txtClick.Text = dr.Cells[12].Value.ToString();
                txtConsumption.Text = dr.Cells[13].Value.ToString();
                txtCTR.Text = dr.Cells[14].Value.ToString();
                txtAvgPriceInsert.Text = dr.Cells[15].Value.ToString();
                txtLogin.Text = dr.Cells[16].Value.ToString();
                txtReg.Text = dr.Cells[17].Value.ToString();
                txtAppDown.Text = dr.Cells[18].Value.ToString();
                txtRegSuccess.Text = dr.Cells[19].Value.ToString();
                txtFristPama.Text = dr.Cells[20].Value.ToString();

                labId.Text = dr.Cells[0].Value.ToString();
            }
            else
            {
                MessageBox.Show("请查询后选择一行！");
            }
        }

        private void btnWeekCalc_Click(object sender, EventArgs e)
        {
            if (txtDay.Text != "")
            {
                this.txtWeek.Text = Common.GetWeekDate(txtDay.Text);
            }
            else
            {
                MessageBox.Show("必须输入日期");
            }
        }

        private void btnClickPercent_Click(object sender, EventArgs e)
        {
            if (this.txtClick.Text == "" || this.txtClick.Text.Trim() == "0" || this.txtShow.Text == "" || this.txtShow.Text == "0")
            {
                MessageBox.Show("点击量，展现量不能为空或0");
            }
            try
            {
                this.txtCTR.Text = (((double)(Convert.ToInt64(txtClick.Text.Trim()))) / ((double)(Convert.ToInt64(txtShow.Text.Trim())))).ToString();
            }
            catch (Exception)
            {

                MessageBox.Show("计算错误");
            }
            
        }

        private void btnAvgPrice_Click(object sender, EventArgs e)
        {
            if (this.txtClick.Text == "" || this.txtClick.Text.Trim() == "0" || this.txtConsumption.Text == "" || this.txtConsumption.Text == "0")
            {
                MessageBox.Show("点击量，销费不能为空或0");
            }
            try
            {
                this.txtAvgPriceInsert.Text = (((double)(Convert.ToInt64(txtConsumption.Text.Trim()))) / ((double)(Convert.ToInt64(txtClick.Text.Trim())))).ToString();
            }
            catch (Exception)
            {

                MessageBox.Show("计算错误");
            }
        }

        private void btnOpenAccess_Click(object sender, EventArgs e)
        {
            this.btnOpenAccess.Enabled = false;
            this.toolStripStatusLabel1.Text = "正在运行……";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.accdb|*.accdb";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                AccessImport.Instance.DataBasePath = ofd.FileName;
                AccessImport.Instance.ListTables = AccessImport.Instance.GetListTable();
                StringBuilder sb = new StringBuilder();
                foreach (var item in AccessImport.Instance.ListTables)
	            {
                    sb.AppendLine(item);
	            }
                this.txtAccessTables.AppendText(sb.ToString());
            }
            this.btnOpenAccess.Enabled = true;
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            this.btnReport.Enabled = false;
            this.toolStripStatusLabel1.Text = "正在运行……";
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(OutPutDataReport));
            th.Start();
        }

        private void OutPutDataReport(object obj)
        {
            DataBaseSqlite.Instance.OutPutDataRepot();
            this.Invoke((EventHandler)delegate { this.toolStripStatusLabel1.Text = "报表数据导出完成！"; this.btnReport.Enabled = true; });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str = "\"2015-11-24\",\"想抄股票,怎么开户\",\"股户-疑问\",\"Baidu-财道2132946\",\"广泛-通用词-股户\",1,0,0.00,0.00%,0.00,0,0,10.00";
            string a = "13.50%";
            string clicknumstr = "2";
            string shownumstr = "15";
            string result = "";
            int clicknum = 0;
            int shownum = 0;
            int.TryParse(clicknumstr, out clicknum);
            int.TryParse(shownumstr, out shownum);
            if (shownum == 0 || clicknum == 0)
            {
                result = "0%";
            }
            else
            {
                result = (((double)clicknum / (double)shownum)*100).ToString() + "%";
            }
            Console.WriteLine(result);
        }
        //end access read


    }
}
