using HiFramework.Log;
using System;
using System.Runtime.ExceptionServices;

namespace HiProtobuf.UI
{
    internal class Logger : ILogHandler
    {
        internal Logger()
        {
            AppDomain.CurrentDomain.FirstChanceException += OnException;
        }

        private void OnException(object sender, FirstChanceExceptionEventArgs e)
        {
            string str = e.Exception.ToString();
            Log += "[Exception]" + str.ToString() + "\r\n";
        }

        public static string Log;
        public void Info(params object[] args)
        {
            string str = "";
            for (int i = 0; i < args.Length; i++)
            {
                str += args[i].ToString();
            }
            Log += "[Print]" + str.ToString() + "\r\n";
        }

        public void Warning(params object[] args)
        {
            string str = "";
            for (int i = 0; i < args.Length; i++)
            {
                str += args[i].ToString();
            }
            Log += "[Warning]" + str.ToString() + "\r\n";
        }

        public void Error(params object[] args)
        {
            string str = "";
            for (int i = 0; i < args.Length; i++)
            {
                str += args[i].ToString();
            }
            Log += "[Error]" + str.ToString() + "\r\n";
        }
    }
}
