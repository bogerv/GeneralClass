using Gen.EntityFramework.Context;
using Gen.EntityFramework.Entitities.TleEntities;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;

namespace GenGuidDate.Windows
{
    /// <summary>
    /// Interaction logic for TleWindow.xaml
    /// </summary>
    public partial class TleWindow
    {
        private static TLEContext dbContext = new TLEContext();
        private static List<UserInfo> userInfos = dbContext.Users.ToList();
        private static List<Product> products = dbContext.Products.ToList();
        private Action _action = default(Action);

        public TleWindow(Action action)
        {
            InitializeComponent();
            this._action = action;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tleFlyout.IsOpen = true;
        }


        private void SyncUserLevel_Click(object sender, RoutedEventArgs e)
        {
            var controller = this.ShowProgressAsync("Please wait...", "正在生成文件,请不要关闭程序!");
            Task.Run(() => WriteXmlFile(controller));
        }

        #region TLE

        protected async Task WriteXmlFile(Task<ProgressDialogController> controller)
        {
            var pos = await controller;
            Dictionary<string, int> DicProductID = new Dictionary<string, int>();
            List<Guid> ProductIDList =
                products.OrderBy(i => i.CreateTime).Select(u => u.ProductID).ToList();
            //按创建时间降序排列productID
            if (ProductIDList != null && ProductIDList.Count > 0)
            {
                for (var i = 0; i < ProductIDList.Count; i++)
                {
                    DicProductID.Add(ProductIDList[i].ToString(), i + 1);
                }
            }

            #region UserLevels.xml
            DateTime time = DateTime.Parse(DateTime.Now.AddYears(-1).AddMonths(-2).ToShortDateString());
            DateTime now = DateTime.Parse(DateTime.Now.AddDays(-1).ToShortDateString());

            var fileName = "TLE_UserLevels_" + GetTimeStamp() + ".xml";
            List<UserLevel> UserLevelList = dbContext.UserLevels.Where(u => (u.CreateTime > time && u.CreateTime < now) || (u.UpdateTime > time && u.UpdateTime < now)).ToList();
            StringBuilder xmlContent = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            StringBuilder xmlModalityLevels = new StringBuilder();
            StringBuilder xmlProductLevels = new StringBuilder();
            List<string> userIdList = new List<string>();
            string userEmail = string.Empty;

            if (UserLevelList != null)
            {
                foreach (UserLevel u in UserLevelList)
                {
                    if (!userIdList.Contains(u.UserID.ToString()))//获取userlevel去重后的userid
                    {
                        userIdList.Add(u.UserID.ToString());
                    }
                }
                xmlContent.Append("<UserLevels>");
                xmlContent.Append("<Header>");
                xmlContent.AppendFormat("<Recordsize>{0}</Recordsize>", userIdList.Count);
                xmlContent.Append("</Header>");
                xmlContent.Append("<Contents>");

                for (int s = 0; s < userIdList.Count; s++)
                {
                    userEmail = userInfos.Where(i => i.UserID.ToString() == userIdList[s]).Select(i => i.Email).FirstOrDefault();
                    List<UserLevel> userLevelList2 = UserLevelList.Where(e => e.UserID.ToString() == userIdList[s]).ToList();//根据去重后的userid查找userlevel
                    foreach (UserLevel u in userLevelList2)
                    {
                        string levelName = dbContext.Levels.Where(l => l.LevelID == u.LevelID).Select(c => c.LevelName).FirstOrDefault();

                        if (!string.IsNullOrEmpty(u.ProductID.ToString()))
                        {
                            //   Product productModel = productService.LoadEntites(p => p.ProductID == u.ProductID).FirstOrDefault();
                            if (u.UpdateTime > time || u.CreateTime > time)
                            {
                                xmlProductLevels.Append("<ProductLevel>");
                                string modalityName = dbContext.Modalities.Where(d => d.ModalityID == u.ModalityID).Select(c => c.ModalityName).FirstOrDefault();
                                if (u.CreateTime > now)
                                {
                                    xmlProductLevels.AppendFormat("<Operate>{0}</Operate>", "0");
                                }
                                else
                                {
                                    xmlProductLevels.AppendFormat("<Operate>{0}</Operate>", "1");
                                }
                                xmlProductLevels.AppendFormat("<ModalityName>{0}</ModalityName>", modalityName);
                                xmlProductLevels.AppendFormat("<ProductID>{0}</ProductID>", DicProductID[u.ProductID.ToString()]);
                                xmlProductLevels.AppendFormat("<ProductLevelName>{0}</ProductLevelName>", "Product " + levelName);
                                xmlProductLevels.AppendFormat("<ProductLevelDueDate>{0}</ProductLevelDueDate>", u.DueDate.ToString().Replace("/", "-"));
                                xmlProductLevels.Append("</ProductLevel>");
                            }
                        }
                        else
                        {
                            Modality modalityModel = dbContext.Modalities.Where(d => d.ModalityID == u.ModalityID).FirstOrDefault();
                            if (modalityModel.UpdateTime > time || modalityModel.CreateTime > time)
                            {
                                xmlModalityLevels.Append("<ModalityLevel>");
                                if (modalityModel.CreateTime > now)
                                {
                                    xmlModalityLevels.AppendFormat("<Operate>{0}</Operate>", "0");
                                }
                                else
                                {
                                    xmlModalityLevels.AppendFormat("<Operate>{0}</Operate>", "1");
                                }
                                xmlModalityLevels.AppendFormat("<ModalityName>{0}</ModalityName>", modalityModel.ModalityName);
                                xmlModalityLevels.AppendFormat("<ModalityLevelName>{0}</ModalityLevelName>", levelName);
                                xmlModalityLevels.AppendFormat("<ModalityLevelDueDate>{0}</ModalityLevelDueDate>", u.DueDate.ToString().Replace("/", "-"));
                                xmlModalityLevels.Append("</ModalityLevel>");
                            }
                        }
                    }
                    if (userEmail.Equals("Haihua.Meng@philips.com"))
                    {
                        Console.WriteLine();
                    }
                    xmlContent.Append("<Content>");
                    xmlContent.Append("<UserProfile>");
                    xmlContent.AppendFormat("<UserEmail>{0}</UserEmail>", userEmail);
                    xmlContent.Append("</UserProfile>");
                    xmlContent.Append("<ModalityLevels>");
                    xmlContent.Append(xmlModalityLevels.ToString());
                    xmlContent.Append("</ModalityLevels>");
                    xmlContent.Append("<ProductLevels>");
                    xmlContent.Append(xmlProductLevels.ToString());
                    xmlContent.Append("</ProductLevels>");
                    xmlContent.Append("</Content>");

                    xmlProductLevels.Clear();
                    xmlModalityLevels.Clear();
                    // 设置弹出 Dialog 的进度条
                    pos.SetProgress(s * 1.0 / userIdList.Count);
                }
                xmlContent.Append("</Contents>");
                xmlContent.Append("</UserLevels>");
            }

            var fpath = "D:\\";
            if (!Directory.Exists(fpath))
            {
                Directory.CreateDirectory(fpath);
            }
            fileName = fpath + "/" + fileName;
            File.WriteAllText(fileName, xmlContent.ToString());

            // 关闭弹出的 Dialog
            await pos.CloseAsync();

            #endregion
        }

        public static string GetTimeStamp()
        {
            return DateTime.Now.GetDateTimeFormats('s')[0].ToString().Trim().Replace("-", "").Replace(":", "");
        }

        protected static int TimeSpan(DateTime? time)
        {
            if (time == null)
            {
                return 0;//product时间段为空
            }
            DateTime d1 = (DateTime)time;
            DateTime d2 = DateTime.Now;
            TimeSpan d3 = d1.Subtract(d2);
            if (d3.Days >= -1)
            {
                return 1;//一天内的数据
            }
            else
            {
                return 2;//不满足一天内
            }
        }

        #endregion

        /// <summary>
        /// TLE 窗口关闭之后, 激活主窗口中打开TLE窗口的按钮
        /// </summary>
        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            _action();
        }
    }
}
