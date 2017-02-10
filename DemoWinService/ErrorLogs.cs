using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoWinService
{
    class ErrorLogs
    {
        public static void LogErrorData(string stackTraceInfo, string message)
        {
            try
            {
                shortenURLEntities dc = new shortenURLEntities();
                ErrorLog objErrorLog = new ErrorLog();
                objErrorLog.StackTrace = stackTraceInfo;
                objErrorLog.ErrorMessage = message;
                objErrorLog.DateCreated = System.DateTime.Now;
                dc.ErrorLogs.Add(objErrorLog);
                dc.SaveChanges();
                string err = "";
                err = "Error Occured on:" + DateTime.Now.ToString() + "<br>";
                err += "Message: " + message;
                err += "Stack Trace:<br>" + stackTraceInfo;
                //EmailUtil.SendMail("yasodha.bitra@gmail.com", ConfigurationManager.AppSettings["Error Mail"].ToString(), "", "", "Error ShortenURL", " <br>Message : " + err);

            }
            catch (Exception ex)
            {

            }
        }
    }
}
