using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTest
{
    public class ADUserInfo
    {
        // 登录名
        public string UserPrincipalName { get; set; }
        // 邮箱
        public string mail { get; set; }
        // 接到
        public string streetAddress { get; set; }
        public string otherTelephone { get; set; }
        // 显示名
        public string displayName { get; set; }
        // 省
        public string st { get; set; }
        // 办公室
        public string physicalDeliveryOfficeName { get; set; }
        // 电话
        public string telephoneNumber { get; set; }
        // 市县
        public string l { get; set; }
        // 国家代码
        public string c { get; set; }
        // 姓
        public string sn { get; set; }
        // 名
        public string givenName { get; set; }
        // 描述
        public string description { get; set; }
        // 即对象所在的DN
        public string distinguishedName { get; set; }
        // 创建时间
        public string whenCreated { get; set; }
        // 修改时间
        public string whenChanged { get; set; }
        // 隶属于
        public string memberOf { get; set; }
        // 帐户行为控制标志
        public string userAccountControl { get; set; }
        // 用户尝试错误密码的次数
        public string badPwdCount { get; set; }
        // 用户最后一次尝试错误密码的时间
        public string badPasswordTime { get; set; }
        // 用户最后登陆时间
        public string lastLogon { get; set; }
        // 所属组的ID
        public string primaryGroupID { get; set; }
        // 帐户到期日期
        public string accountExpires { get; set; }
        // 远程登陆名 
        public string sAMAccountName { get; set; }

        public string mailnickname { get; set; }
        public string msexchhomeservername { get; set; }
        public string physicaldeliveryofficename { get; set; }
        public string samaccountname { get; set; }
        public string name { get; set; }
    }
}
