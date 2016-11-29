using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using GenGuidDate.Windows;
using Gen.EntityFramework;
using System.ComponentModel;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using Microsoft.Exchange.WebServices.Data;

namespace GenGuidDate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private static GenDbContext dbContext = new GenDbContext();
        private static string LIMandatory = "4DBDDE8C-D924-4E82-AAA5-5A543D9D7607";
        private static string CPMandatory = "73E3512B-AA60-4DD1-9079-8CD369814911";
        private static readonly string somcTeamId = "d9be54db-983d-4c4f-bfe1-59e23b6e1d29";
        private static List<Team> allTeam = dbContext.Team.ToList();
        private static Team somcTeam = allTeam.FirstOrDefault(t => t.TeamId.Equals(somcTeamId));

        private static List<CI_Area_Map> allCP = dbContext.CI_Area_Map.ToList();
        private static List<II_Class> allLI = dbContext.II_Class.ToList();
        private static List<Area_Pack> allAreaPack = dbContext.Area_Pack.ToList();
        private static List<II_Class> allIIClass = dbContext.II_Class.ToList();

        private static List<Job_ReqOnAreaPack> allJobCPMap = dbContext.Job_ReqOnAreaPack.ToList();
        private static List<II_CI_Area_Grade_Mapping> allLICPMap = dbContext.II_CI_Area_Grade_Mapping.ToList();
        private static List<II_Instance> allIIInstance = dbContext.II_Instance.ToList();

        //用于发送邮件的变量
        private static string enabled = ConfigurationManager.AppSettings["Enabled"];
        private static string useExchange = ConfigurationManager.AppSettings["UseExchange"];
        private static string isDomain = ConfigurationManager.AppSettings["IsDomain"];
        private static string domain = ConfigurationManager.AppSettings["Domain"];

        //Exchange发送邮件
        private static string asmx = ConfigurationManager.AppSettings["ASMX"];
        private static string exchangeAccount = ConfigurationManager.AppSettings["ExchangeAccount"];
        private static string exchangePassWord = ConfigurationManager.AppSettings["Password"];

        //SMTP发送邮件
        private static string from = ConfigurationManager.AppSettings["UserEmail"];
        private static string passWord = ConfigurationManager.AppSettings["UserPassword"];
        private static string host = ConfigurationManager.AppSettings["Host"];
        private static string smtp = ConfigurationManager.AppSettings["SMTP"];
        private static int port = int.Parse(ConfigurationManager.AppSettings["Port"]);

        private BackgroundWorker bgWorker = new BackgroundWorker();
        public MainWindow()
        {
            InitializeComponent();

            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += DoWork_Handler;
            bgWorker.ProgressChanged += ProgressChanged_Handler;
            bgWorker.RunWorkerCompleted += RunWorkerCompleted_Handler;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            processBtn.IsEnabled = false;
            var content1 = this.contentList_1.Text;
            var content2 = this.contentList_2.Text;
            var content3 = this.contentList_3.Text;
            var content4 = this.contentList_4.Text;
            var cpAddTime = contentList_1.Text.Trim();

            if (!bgWorker.IsBusy)
            {
                bgWorker.RunWorkerAsync(cpAddTime);
            }

            // Test_Specification
            //GetTestStandardData();

            // Job
            //GetJobData();

            // II_Class
            //GetIIClassData();
        }

        private void ProgressChanged_Handler(object sender, ProgressChangedEventArgs e)
        {
            processBtn.Count = e.UserState.ToString();
            if (e.ProgressPercentage.Equals(0))
            {
                guidList.ItemsSource = e.UserState as List<object>;
            }
        }

        private void RunWorkerCompleted_Handler(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("后台任务已经被取消。", "消息");
            }
            else
            {
                //MessageBox.Show("后台任务正常结束。", "消息");
                processBtn.IsEnabled = true;
                processBtn.Count = "Generate";
            }
        }

        private void DoWork_Handler(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            var cpAddTime = string.IsNullOrEmpty(e.Argument.ToString()) ? DateTime.Now : DateTime.Parse(e.Argument.ToString());
            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                List<Team> allSubTeam = new List<Team>();
                allSubTeam.Add(somcTeam);
                GetAllChildTeamsById(somcTeamId, allSubTeam);
                var somcSubTeamIds = new List<string>();
                allSubTeam.ForEach(t => somcSubTeamIds.Add(t.TeamId));

                worker.ReportProgress(1, "Get persons.");
                // Active 的人
                var persons = dbContext.Personal_Profile.Where(p => p.IsActive.Equals("Yes") && somcSubTeamIds.Contains(p.Team)).ToList();
                List<string> personIds = new List<string>();
                persons.ForEach(p =>
                {
                    personIds.Add(p.PersonalProfileId);
                });

                // 最后存储结果
                // 主数据
                List<Dictionary<string, List<string>>> person_job_cp_ids = new List<Dictionary<string, List<string>>>();
                List<Dictionary<string, List<string>>> person_job_cp_li_ids = new List<Dictionary<string, List<string>>>();
                // 用于比对的数据
                List<Dictionary<string, List<string>>> person_cp_ids = new List<Dictionary<string, List<string>>>();
                List<Dictionary<string, List<string>>> person_li_ids = new List<Dictionary<string, List<string>>>();

                // 所有人关联的Job ( 1 对 n )
                GetPersonJobMappedData(worker, personIds, person_job_cp_ids, person_job_cp_li_ids);

                // 所有人申请的CP ( 1 对 n )--------------------------用于比对的数据
                GetPersonAppliedCPData(worker, personIds, person_cp_ids);

                // 所有人申请的LI ( 1 对 n )--------------------------用于比对的数据
                GetPersonAppliedLIData(worker, personIds, person_li_ids);

                List<string> allSql = new List<string>();
                worker.ReportProgress(5, "Combine add personal_ci sql....");
                List<Dictionary<string, List<string>>> newAddDic = CompareCPDifferent(person_job_cp_ids, person_cp_ids, allSql);
                worker.ReportProgress(6, "Combine add ii_instance sql.....");
                CompareLIDifferent(person_job_cp_li_ids, person_li_ids, allSql);
                worker.ReportProgress(7, "Combine update personal_ci sql.....");
                UpdateCPStatus(person_job_cp_ids, person_li_ids, cpAddTime, newAddDic, allSql);

                var finalResult = new List<object>();
                foreach (var item in allSql)
                {
                    finalResult.Add(new { Id = item });
                }
                worker.ReportProgress(0, finalResult);
            }
        }

        private void UpdateCPStatus(List<Dictionary<string, List<string>>> person_job_cp_ids, List<Dictionary<string, List<string>>> person_li_ids, DateTime cpAddTime, List<Dictionary<string, List<string>>> newAddDic, List<string> allSql)
        {
            List<Dictionary<string, List<string>>> forUpdateCPs = new List<Dictionary<string, List<string>>>();

            foreach (var pcp in person_job_cp_ids)
            {
                var firstPcp = pcp.FirstOrDefault();
                Dictionary<string, List<string>> tempPersonCP = new Dictionary<string, List<string>>();
                List<string> updateCPs = new List<string>();

                foreach (var cp in firstPcp.Value)
                {
                    // 获得一个 cp 所对应的所有 li 的 Id
                    var cpMappedLis = allLICPMap.Where(cl => cl.CIAreaMapId != null && cl.CIAreaMapId.Equals(cp) && cl.IsIIMandatory != null && cl.IsIIMandatory.Equals(LIMandatory)).ToList();
                    //var cpMappedLis = allLICPMap.Where(cl => cl.CIAreaMapId != null && cl.CIAreaMapId.Equals(cp)).ToList();
                    List<string> cpMappedLiIds = new List<string>();
                    cpMappedLis.ForEach(c => cpMappedLiIds.Add(c.IIClassId));
                    // 获得 这个人已经申请了的 li
                    var thisPerson = person_li_ids.FirstOrDefault(p => p.FirstOrDefault().Key.Equals(firstPcp.Key));
                    var personAppliedLIIds = thisPerson.FirstOrDefault().Value;
                    var personAppliedLIs = allIIInstance.Where(ii_i => ii_i.IIClassId != null && personAppliedLIIds.Contains(ii_i.IIClassId) && ii_i.User != null && ii_i.User.Equals(firstPcp.Key)).ToList();
                    // 以 cp 对应的 li 为主, 循环比较
                    bool res = false;
                    foreach (var liid in cpMappedLiIds)
                    {
                        var li = personAppliedLIs.FirstOrDefault(l => l.IIClassId.Equals(liid));
                        if (li != null && li.IIInstanceStatusId.Equals("Passed"))
                        {
                            // 保证所有的 LI 都要通过
                            res = true;
                            continue;
                        }
                        // 有一个没通过或未申请, 直接跳出循环
                        res = false;
                        break;
                    }
                    if (res)
                        updateCPs.Add(cp);
                }

                tempPersonCP.Add(firstPcp.Key, updateCPs);
                forUpdateCPs.Add(tempPersonCP);
            }

            // 获得更新 CP 的语句
            List<string> tempSql = new List<string>();
            foreach (var cpUpdate in forUpdateCPs)
            {
                var itemFirst = cpUpdate.FirstOrDefault();
                foreach (var cpId in itemFirst.Value)
                {
                    var cpEntity = allCP.FirstOrDefault(c => c.CIAreaMapId.Equals(cpId));
                    if (cpEntity != null)
                    {
                        var personalCI = dbContext.Personal_CI.FirstOrDefault(p => p.CITreeId != null && p.CITreeId.Equals(cpEntity.CITreeId) && p.AreaPackId != null && p.AreaPackId.Equals(cpEntity.AreaPackId) && p.PersonalProfileId != null && p.PersonalProfileId.Equals(itemFirst.Key));
                        if (personalCI != null)
                        {
                            if (personalCI.CreatedOn.Value > (cpAddTime!=null?cpAddTime:DateTime.Parse("2016-11-02 00:00:00.000")))
                            {
                                var personalCIId = personalCI.PersonalCIId;
                                string sql = string.Format(@"update Personal_CI set [Status]='Qualified' where PersonalCIId='{0}'", personalCIId);
                                tempSql.Add(sql);
                            }
                        }
                    }
                }
            }
            tempSql = tempSql.OrderBy(c => c).ToList();
            allSql.AddRange(tempSql);
        }

        /// <summary>
        /// 递归获取一个 Team 的所有子 Team
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="allSubTeam"></param>
        public void GetAllChildTeamsById(string teamId, List<Team> allSubTeam)
        {
            List<Team> teamList = allTeam.Where(t => t.Parent.Equals(teamId) && t.TeamId != t.Parent).ToList();
            allSubTeam.AddRange(teamList);
            foreach (var team in teamList)
            {
                GetAllChildTeamsById(team.TeamId, allSubTeam);
            }
        }

        /// <summary>
        /// 获得每个人关联的 Active 的 Job
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="personIds"></param>
        /// <param name="person_job_cp_ids"></param>
        /// <param name="person_job_cp_li_ids"></param>
        private static void GetPersonJobMappedData(BackgroundWorker worker, List<string> personIds, List<Dictionary<string, List<string>>> person_job_cp_ids, List<Dictionary<string, List<string>>> person_job_cp_li_ids)
        {
            worker.ReportProgress(2, "Get person mapped jobs..");
            var personJobMaps = dbContext.Person_Job_Map.Where(jm => personIds.Contains(jm.PersonalProfileId) && jm.JobId != null && jm.IsActive.Equals("Yes")).ToList();
            // 按照人将Job分组 ( 1 对 n )
            var personJobGroup = personJobMaps.GroupBy(pjm => pjm.PersonalProfileId).ToList();
            personJobGroup = personJobGroup.OrderBy(g => g.Key).ToList();
            foreach (IGrouping<string, Person_Job_Map> group in personJobGroup)
            {
                var key = group.Key;
                Dictionary<string, List<string>> person_job_cp_id = new Dictionary<string, List<string>>();
                List<string> cpids = new List<string>();
                Dictionary<string, List<string>> person_job_cp_li_id = new Dictionary<string, List<string>>();
                List<string> cpliids = new List<string>();
                // 人 -> list mapped Jobs
                foreach (Person_Job_Map personJobMap in group)
                {
                    // 循环 job -> list mapped CPs
                    var tempCPs = allJobCPMap.Where(jcpm => jcpm.JobId.Equals(personJobMap.JobId) && jcpm.CIAreaMapId != null && jcpm.IsIIMandatory.Equals(CPMandatory)).ToList();

                    foreach (var tempCP in tempCPs)
                    {
                        cpids.Add(tempCP.CIAreaMapId);
                    }
                    foreach (var cpid in cpids)
                    {
                        var cpMappedLi = allLICPMap.FirstOrDefault(li => li.CIAreaMapId != null && li.CIAreaMapId.Equals(cpid) && li.IsIIMandatory != null && li.IsIIMandatory.Equals(LIMandatory));
                        if (cpMappedLi != null)
                            cpliids.Add(cpMappedLi.IIClassId);
                    }
                }
                person_job_cp_li_id.Add(key, cpliids.Distinct().ToList());
                // 获得Li最终数据
                person_job_cp_li_ids.Add(person_job_cp_li_id);

                person_job_cp_id.Add(key, cpids.Distinct().ToList());
                // 获得最终数据
                person_job_cp_ids.Add(person_job_cp_id);
            }
        }

        /// <summary>
        /// 获得每个人已经申请的所有 CP
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="personIds"></param>
        /// <param name="person_cp_ids"></param>
        private static void GetPersonAppliedCPData(BackgroundWorker worker, List<string> personIds, List<Dictionary<string, List<string>>> person_cp_ids)
        {
            worker.ReportProgress(3, "Get person mapped cps...");
            var personCPMaps = dbContext.Personal_CI.Where(pc => personIds.Contains(pc.PersonalProfileId) && pc.CITreeId != null && pc.AreaPackId != null).GroupBy(c => c.PersonalProfileId).ToList();
            personCPMaps = personCPMaps.OrderBy(g => g.Key).ToList();
            foreach (IGrouping<string, Personal_CI> group in personCPMaps)
            {
                var key = group.Key;
                Dictionary<string, List<string>> person_cp_id = new Dictionary<string, List<string>>();
                List<string> cpids = new List<string>();
                // 人 -> list mapped Jobs
                foreach (Personal_CI personCP in group)
                {
                    var temp = allCP.FirstOrDefault(cp => cp.CITreeId.Equals(personCP.CITreeId) && cp.AreaPackId.Equals(personCP.AreaPackId));
                    if (temp != null)
                        cpids.Add(temp.CIAreaMapId);
                }
                person_cp_id.Add(key, cpids.Distinct().ToList());

                person_cp_ids.Add(person_cp_id);
            }
        }

        /// <summary>
        /// 获得每个人已经申请的所有 LI
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="personIds"></param>
        /// <param name="person_li_ids"></param>
        private static void GetPersonAppliedLIData(BackgroundWorker worker, List<string> personIds, List<Dictionary<string, List<string>>> person_li_ids)
        {
            worker.ReportProgress(4, "Get person applied LIs....");
            var personAppliedLIs = dbContext.II_Instance.Where(ali => personIds.Contains(ali.User) && ali.IIClassId != null).GroupBy(c => c.User).ToList();
            personAppliedLIs = personAppliedLIs.OrderBy(g => g.Key).ToList();
            foreach (IGrouping<string, II_Instance> group in personAppliedLIs)
            {
                var key = group.Key;
                Dictionary<string, List<string>> person_li_id = new Dictionary<string, List<string>>();
                List<string> liids = new List<string>();
                // 人 -> list mapped Jobs
                foreach (II_Instance personLI in group)
                {
                    var temp = allLI.FirstOrDefault(li => li.IIClassId.Equals(personLI.IIClassId) && li.IIClassId != null);
                    if (temp != null)
                        liids.Add(temp.IIClassId);
                }
                person_li_id.Add(key, liids.Distinct().ToList());

                person_li_ids.Add(person_li_id);
            }
        }

        private List<Dictionary<string, List<string>>> CompareCPDifferent(List<Dictionary<string, List<string>>> person_job_cp_ids, List<Dictionary<string, List<string>>> person_cp_ids, List<string> allSql)
        {
            //person_cp_ids.Remove(person_cp_ids.FirstOrDefault(i => i.FirstOrDefault().Key.Equals("739b70f5-6f5b-4e05-bc29-a61e6a2588a7")));
            List<Dictionary<string, List<string>>> diffDic = new List<Dictionary<string, List<string>>>();
            foreach (var item in person_job_cp_ids)
            {
                var itemFirst = item.FirstOrDefault();
                Dictionary<string, List<string>> temp = new Dictionary<string, List<string>>();
                List<string> diffList = new List<string>();

                var thisPerson = person_cp_ids.FirstOrDefault(p => p.FirstOrDefault().Key.Equals(itemFirst.Key));
                var thisPersonValue = thisPerson.FirstOrDefault().Value;
                foreach (var id in itemFirst.Value)
                {
                    if (!thisPersonValue.Contains(id))
                    {
                        diffList.Add(id);
                    }
                }

                if (diffList.Count > 0)
                {
                    temp.Add(itemFirst.Key, diffList);
                    diffDic.Add(temp);
                }
            }
            foreach (var item in diffDic)
            {
                var itemFirst = item.FirstOrDefault();
                foreach (var cpid in itemFirst.Value)
                {
                    var cpEntity = allCP.FirstOrDefault(c => c.CIAreaMapId.Equals(cpid));
                    var areaPack = allAreaPack.FirstOrDefault(a => a.AreaPackId.Equals(cpEntity.AreaPackId));
                    var mentorId = areaPack.Expert.Split(new char[] { ',' })[0];
                    string sql = string.Format(@"insert into Personal_CI(PersonalCIId,PKName,PersonalProfileId,CITreeId,AreaPackId,[Status],Mentor,CreatedOn,IsDeleted) values(NEWID(), '{0}_{1}', '{2}', '{0}', '{1}', 'Targeting', {3}, GETDATE(), '0')", cpEntity.CITreeId, cpEntity.AreaPackId, itemFirst.Key, mentorId);
                    allSql.Add(sql);
                }
            }
            return diffDic;
        }

        private void CompareLIDifferent(List<Dictionary<string, List<string>>> person_job_cp_li_ids, List<Dictionary<string, List<string>>> person_li_ids, List<string> allSql)
        {
            List<Dictionary<string, List<string>>> diffDic = new List<Dictionary<string, List<string>>>();
            foreach (var item in person_job_cp_li_ids)
            {
                var itemFirst = item.FirstOrDefault();
                Dictionary<string, List<string>> temp = new Dictionary<string, List<string>>();
                List<string> diffList = new List<string>();

                var thisPerson = person_li_ids.FirstOrDefault(p => p.FirstOrDefault().Key.Equals(itemFirst.Key));
                var thisPersonValue = thisPerson.FirstOrDefault().Value;
                foreach (var id in itemFirst.Value)
                {
                    if (!thisPersonValue.Contains(id))
                    {
                        diffList.Add(id);
                    }
                }

                if (diffList.Count > 0)
                {
                    temp.Add(itemFirst.Key, diffList);
                    diffDic.Add(temp);
                }
            }
            foreach (var item in diffDic)
            {
                var itemFirst = item.FirstOrDefault();
                foreach (var liid in itemFirst.Value)
                {
                    var iiClass = allIIClass.FirstOrDefault(a => a.IIClassId.Equals(liid));
                    var mentorId = iiClass.Mentor.Split(new char[] { ',' })[0];
                    string sql = string.Format(@"insert into II_Instance(IIInstanceId,PKName,[User],IIClassId,IsPack,IIInstanceStatusId,Mentor,CreatedOn,IsDeleted)values(NEWID(),'{0}_{1}','{0}','{1}','0','Started',{2},GETDATE(),'0')", itemFirst.Key, liid, mentorId);
                    allSql.Add(sql);
                }
            }
        }

        /// <summary>
        /// 线程取消事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            bgWorker.CancelAsync();
        }

        private void NewWindow_Click(object sender, RoutedEventArgs e)
        {
            new NewWindow() { Owner = this }.ShowDialog();
        }

        private MetroWindow accentThemeTestWindow;

        private void ChangeAppStyleButtonClick(object sender, RoutedEventArgs e)
        {
            if (accentThemeTestWindow != null)
            {
                accentThemeTestWindow.Activate();
                return;
            }

            accentThemeTestWindow = new AccentStyleWindow();
            accentThemeTestWindow.Owner = this;
            accentThemeTestWindow.Closed += (o, args) => accentThemeTestWindow = null;
            accentThemeTestWindow.Left = this.Left + this.ActualWidth / 2.0;
            accentThemeTestWindow.Top = this.Top + this.ActualHeight / 2.0;
            accentThemeTestWindow.Show();
        }

        #region 应用全屏与还原
        private void FullClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            this.UseNoneWindowStyle = true;
            this.IgnoreTaskbarOnMaximize = true;
        }

        private void NormalClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.UseNoneWindowStyle = false;
            this.ShowTitleBar = true; // <-- this must be set to true
            this.IgnoreTaskbarOnMaximize = false;
        }
        #endregion

        #region 手动增加数据所用
        private void GetIIClassData()
        {
            var list = new List<object>();
            var errorList = new List<int>();

            var listCITree = dbContext.CI_Tree.ToList();
            var listLI = dbContext.II_Class.ToList();
            var listAreaPack = dbContext.Area_Pack.ToList();
            var listCI_Area_Map = dbContext.CI_Area_Map.ToList();

            var content1 = contentList_1.Text;
            var content2 = contentList_2.Text;

            var list1 = content1.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            // CI_Area_Map
            var list2 = content2.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            for (int i = 0; i < list1.Count(); i++)
            {
                int index = i;
                var mapArray = list2[index].Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                if (mapArray.Length > 1)
                {
                    var ciTreeCount = listCITree.Where(c => c.NameinRoot.Equals(mapArray[0])).Count();
                    if (ciTreeCount == 1)
                    {
                        var ciTree = listCITree.FirstOrDefault(c => c.NameinRoot.Equals(mapArray[0]));
                        var areaPack = listAreaPack.FirstOrDefault(c => c.Name.Equals(mapArray[1]));
                        var areaPackCount = listAreaPack.Where(c => c.Name.Equals(mapArray[1])).Count();
                        if (areaPackCount == 1)
                        {
                            if (ciTree != null && areaPack != null)
                            {
                                var testSpecification = listLI.FirstOrDefault(t => t.Name.Equals(list1[index]));
                                var ciAreaMap = listCI_Area_Map.FirstOrDefault(c => c.CITreeId.Equals(ciTree.CITreeId) && c.AreaPackId.Equals(areaPack.AreaPackId));
                                if (testSpecification != null && ciAreaMap != null)
                                {
                                    list.Add(new { LeftId = testSpecification.IIClassId, RightId = ciAreaMap.CIAreaMapId });
                                    continue;
                                }
                            }
                        }
                    }
                }
                errorList.Add(index + 1);
            }
            this.guidList.ItemsSource = list;
        }

        private void GetJobData()
        {
            var content1 = contentList_1.Text;
            var content2 = contentList_2.Text;
            var content3 = contentList_3.Text;
            var content4 = contentList_4.Text;

            var list1 = content1.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            // CI_Area_Map
            var list2 = content2.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            // HRLevel
            var list3 = content3.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            // Team
            var list4 = content4.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (list1.Count() != list2.Count())
            {
                MessageBox.Show("Count unequal.", "Error", MessageBoxButton.OK);
                return;
            }
            var list = new List<object>();
            var errorList = new List<int>();

            var listCITree = dbContext.CI_Tree.ToList();
            var listHRLevel = dbContext.HRLevel.ToList();
            var listTeam = dbContext.Team.ToList();

            var listTestSpecification = dbContext.Test_Specification.ToList();
            var listJob = dbContext.Job.ToList();
            var listLI = dbContext.II_Class.ToList();

            var listAreaPack = dbContext.Area_Pack.ToList();
            var listCI_Area_Map = dbContext.CI_Area_Map.ToList();

            for (int i = 0; i < list1.Count(); i++)
            {
                int index = i;
                var mapArray = list2[index].Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                if (mapArray.Length > 1)
                {
                    var ciTreeCount = listCITree.Where(c => c.NameinRoot.Equals(mapArray[0])).Count();
                    if (ciTreeCount == 1)
                    {
                        var ciTree = listCITree.FirstOrDefault(c => c.NameinRoot.Equals(mapArray[0]));
                        var areaPackCount = listAreaPack.Where(c => c.Name.Equals(mapArray[1])).Count();
                        if (areaPackCount == 1)
                        {
                            var areaPack = listAreaPack.FirstOrDefault(c => c.Name.Equals(mapArray[1]));
                            if (ciTree != null && areaPack != null)
                            {
                                var hrLevel = listHRLevel.FirstOrDefault(h => h.Name.Equals(list3[index]));
                                var team = listTeam.FirstOrDefault(t => t.NameinRoot.Equals(list4[index]));
                                var jobCount = listJob.Where(t => t.JobTitle.Equals(list1[index]) && t.HRLevelId.Equals(hrLevel.HRLevelId) && t.TeamId.Equals(team.TeamId)).Count();
                                if (jobCount == 1)
                                {
                                    var job = listJob.FirstOrDefault(t => t.JobTitle.Equals(list1[index]) && t.HRLevelId.Equals(hrLevel.HRLevelId) && t.TeamId.Equals(team.TeamId));
                                    var ciAreaMap = listCI_Area_Map.FirstOrDefault(c => c.CITreeId.Equals(ciTree.CITreeId) && c.AreaPackId.Equals(areaPack.AreaPackId));
                                    if (job != null && ciAreaMap != null)
                                    {
                                        list.Add(new { LeftId = job.JobId, RightId = ciAreaMap.CIAreaMapId });
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
                errorList.Add(index + 1);
            }
        }

        private void GetTestStandardData()
        {
            var list = new List<object>();
            var errorList = new List<int>();

            var listCITree = dbContext.CI_Tree.ToList();
            var listLI = dbContext.II_Class.ToList();
            var listAreaPack = dbContext.Area_Pack.ToList();
            var listCI_Area_Map = dbContext.CI_Area_Map.ToList();
            var listTestSpecification = dbContext.Test_Specification.ToList();

            var content1 = contentList_1.Text;
            var content2 = contentList_2.Text;

            var list1 = content1.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            // CI_Area_Map
            var list2 = content2.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            for (int i = 0; i < list1.Count; i++)
            {
                int index = i;
                var mapArray = list2[index].Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                if (mapArray.Length > 1)
                {
                    var ciTreeCount = listCITree.Where(c => c.NameinRoot.Equals(mapArray[0])).Count();
                    if (ciTreeCount == 1)
                    {
                        if (index == 8)
                        {
                            var s = "sfsdf";
                        }
                        var ciTree = listCITree.FirstOrDefault(c => c.NameinRoot.Equals(mapArray[0]));
                        var areaPackCount = listAreaPack.Where(c => c.Name.Equals(mapArray[1])).Count();
                        if (areaPackCount == 1)
                        {
                            var areaPack = listAreaPack.FirstOrDefault(c => c.Name.Equals(mapArray[1]));
                            if (ciTree != null && areaPack != null)
                            {

                                var testSpecification = listTestSpecification.FirstOrDefault(t => t.StandardOrgTestSpecNo.Equals(list1[index]));
                                var ciAreaMap = listCI_Area_Map.FirstOrDefault(c => c.CITreeId.Equals(ciTree.CITreeId) && c.AreaPackId.Equals(areaPack.AreaPackId));
                                if (testSpecification != null && ciAreaMap != null)
                                {
                                    list.Add(new { LeftId = testSpecification.TestSpecificationId, RightId = ciAreaMap.CIAreaMapId });
                                    continue;
                                }
                            }
                        }
                    }
                }
                errorList.Add(index + 1);
            }
        }
        #endregion

        private void sendMailBtn_Click(object sender, RoutedEventArgs e)
        {
            SendEmail("jiawei1.wang@sonymobile.com", "Bogerv Test", "Just is a test mail!");
        }

        /// <summary>
        /// 发送邮件接口。
        /// </summary>
        /// <param name="sendTo">邮箱名称，邮件的目的地</param>
        /// <param name="subject">邮件的标题</param>
        /// <param name="bodyStr">邮箱的内容</param>
        public static bool SendEmail(string sendTo, string subject, string bodyStr)
        {
            try
            {
                if (enabled != "true")
                    return false;

                if (useExchange == "true")
                {
                    ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                    service.Url = new Uri(asmx);
                    if (isDomain == "true")
                    {
                        service.Credentials = new NetworkCredential(exchangeAccount, exchangePassWord, domain);
                    }
                    else
                    {
                        service.Credentials = new NetworkCredential(exchangeAccount, exchangePassWord);
                    }
                    EmailMessage message = new EmailMessage(service);
                    message.Subject = subject;
                    message.Body = bodyStr;
                    message.ToRecipients.Add(sendTo);

                    message.SendAndSaveCopy();
                    return true;
                }
                else
                {
                    using (MailMessage newMail = new MailMessage())
                    {
                        newMail.BodyEncoding = System.Text.Encoding.UTF8;
                        newMail.IsBodyHtml = true;
                        newMail.From = new MailAddress(from);
                        newMail.To.Add(new MailAddress(sendTo));
                        newMail.Subject = subject;
                        newMail.Body = bodyStr;

                        //发送邮件的服务器
                        using (SmtpClient clint = new SmtpClient(smtp, port))
                        {
                            clint.Credentials = new NetworkCredential(from, passWord);
                            clint.EnableSsl = false;
                            clint.Timeout = 1 * 10 * 1000;

                            clint.Send(newMail);
                        }
                    }

                    //SmtpClient smptClient = new SmtpClient();
                    //smptClient.Host = "smtp.mxhichina.com";
                    //smptClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    ////SMTP 服务器要求安全连接需要设置此属性
                    //smptClient.EnableSsl = false;
                    //smptClient.UseDefaultCredentials = true;
                    //smptClient.Credentials = new System.Net.NetworkCredential(from, passWord);
                    ////smptClient.Credentials = new System.Net.NetworkCredential("postmaster@icani.net", "MaGang123456");
                    //smptClient.Timeout = 1 * 20 * 1000;

                    //MailMessage mailMessage = new MailMessage();
                    //mailMessage.From = new MailAddress(from);
                    //// 收件人
                    //mailMessage.To.Add(new MailAddress("bovawang@163.com"));
                    //// 正文编码
                    //mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                    //mailMessage.Subject = "This is test";
                    //// 正文
                    //mailMessage.Body = "Test";
                    //// Html格式
                    //mailMessage.IsBodyHtml = true;

                    //try
                    //{
                    //    smptClient.Send(mailMessage);
                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine(ex.Message.ToString());
                    //}
                    return true;
                }
            }
            catch(Exception e) {
                throw new Exception(e.ToString());
            }
        }
    }
}
