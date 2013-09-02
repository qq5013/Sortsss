using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.AS.Dal;

namespace THOK.AS.Schedule
{
   public class UploadSortingData
    {
       UploadSortingDal udal = new UploadSortingDal();

       public UploadSortingData()
       {

       }

       /// <summary>
       /// 上报分拣数据 郑小龙-20110903 修改
       /// </summary>
       /// <param name="orderDate"></param>
       /// <param name="userName"></param>
       /// <param name="uploadIp"></param>
       public void UploadSorting(string orderDate, string userName,string uploadIp)
       {
           try
           {
               ProcessState.Status = "PROCESSING";
               ProcessState.TotalCount = 5;
               ProcessState.Step = 1;

               DataTable masterTable= udal.FindOrderMaster(orderDate);
               udal.UploadSortingOrderMaster(masterTable);
               udal.UpdateOrderMasterState(orderDate);
               ProcessState.CompleteCount = 1;

               DataTable detailTable = udal.FindOrderDetail(orderDate);
               udal.UploadSortingOrderDetail(detailTable);
               udal.UpdateOrderDeatilState(orderDate);
               ProcessState.CompleteCount = 2;

               DataTable idpsTable = udal.FindSortingIdps();
               udal.UploadSortingIdps(idpsTable);
               ProcessState.CompleteCount = 3;

               udal.InsertUploadRecord(userName, uploadIp, orderDate);
               ProcessState.CompleteCount = 4;
              
           }
           catch (Exception e)
           {
               ProcessState.Status = "ERROR";
               ProcessState.Message = e.Message;
           }
       }

       /// <summary>
       /// 获取上报状态
       /// </summary>
       /// <param name="orderDate"></param>
       /// <returns></returns>
       public DataTable GetUploadState(string orderDate)
       {
           return udal.GetUploadState(orderDate);
       }
    }
}
