using System;
using System.Collections.Generic;
using System.Text;

namespace THOK.AS.Schedule
{
    public class ProcessState
    {
        private static string userName;

        public static string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        private static string userIP;

        public static string UserIP
        {
            get { return userIP; }
            set { userIP = value;}
        }
        private static string orderDate;
        public static string OrderDate
        {
            get { return orderDate; }
            set { orderDate = value; }
        }

        private static int batchNo;
        public static int BatchNo
        {
            get { return batchNo; }
            set { batchNo = value;}
        }
        private static string status;
        public static string Status
        {
            get { return status; }
            set { status = value; }
        }

        private static int completeCount;

        public static int CompleteCount
        {
            get { return completeCount; }
            set { completeCount = value; }
        }

        private static int totalCount;
        public static int TotalCount
        {
            get { return totalCount; }
            set { totalCount = value; }
        }


        private static string message;
        public static string Message
        {
            get { return message; }
            set { message = value; }
        }

        private static int step;
        public static int Step
        {
            get { return step; }
            set { step = value; }
        }

        private static bool inProcessing = false;
        public static bool InProcessing
        {
            get { return inProcessing; }
            set { inProcessing = value; }
        }

        public static string GetMessage()
        {
            string msg = string.Format("<root><status>{0}</status><message>{1}</message><step>{2}</step><completecount>{3}</completecount><totalcount>{4}</totalcount></root>",
                status, message, step, completeCount, totalCount);
            return msg;
        }

    }
}
