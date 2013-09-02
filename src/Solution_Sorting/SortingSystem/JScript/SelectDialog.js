// //JScript 文件
/*模式窗体*/
function SelectDialog(target,TableName,ReturnField,filterTarget,filterField)
{
   var date=new Date();
   var time=date.getMilliseconds();
  if(filterTarget!=null && filterField!=null)
  {
     var strFilterValue=document.getElementById(filterTarget).value;
     var returnvalue=window.showModalDialog("../../Common/SelectDialog.aspx?TableName="+TableName+"&ReturnField="+ReturnField+"&filterField="+filterField+"&filterValue="+strFilterValue+"&time="+time,"",
                                         "top=0;left=0;toolbar=no;menubar=no;scrollbars=no;resizable=no;location=no;status=no;dialogWidth=680px;dialogHeight=450px");
  }
  else
  {
     var returnvalue=window.showModalDialog("../../Common/SelectDialog.aspx?TableName="+TableName+"&ReturnField="+ReturnField+"&time="+time,"",
                                         "top=0;left=0;toolbar=no;menubar=no;scrollbars=no;resizable=no;location=no;status=no;dialogWidth=680px;dialogHeight=450px");
  }
   
   if(returnvalue==null)
   {
     return;
   }
   else if(returnvalue!='')
   {
        if(typeof(target)=="object")
        {
             target.value=returnvalue;
        }
        else
        {
           document.getElementById(target).value =returnvalue;
        }
   }                                     
}


function SelectDialog2(strTarget,strTableName,strReturnField)
{
   var date=new Date();
   var time=date.getMilliseconds();
   var aryTarget=strTarget.split(',');
   var aryField=strReturnField.split(',');
   if(aryTarget.length!=aryField.length)
   {
      alert('参数有错！');
      return false;
   }
   
   var localPath = location.pathname;
   var path=localPath.split('/'); 
   var num = path.length;
   rootPath = "";
//   if(path[1]=="DISCUSSBOARD" ||"MANAGEM_CLIENT"==path[1])
//      num = num-2;
//   else num = num-1;
   if(num>2)
   {
        num = num-1;  //本地测试减1，发布成网站不减
       for(var i=2;i<num;i++)
       {
         rootPath=rootPath+"../";      
        // alert(path[i]);
       }
   }
   if (window.document.all)//IE判断window.showModalDialog!=null
   {
      var returnvalue;
      returnvalue=window.showModalDialog(rootPath+"Common/SelectDialog2.aspx?TableName="+strTableName+"&ReturnField="+strReturnField+"&time="+time,"",
                                         "top=0;left=0;toolbar=no;menubar=no;scrollbars=no;resizable=no;location=no;status=no;dialogWidth=680px;dialogHeight=450px");
   
       if(returnvalue==null)
       {
          return false;
       }
       else if(returnvalue!='')
       {
           var aryValue=new Array();
           aryValue=returnvalue.split('|');
           for(var i=0;i<aryTarget.length;i++)
           {
              var e=document.getElementById(aryTarget[i]);
              if(e!=null)
              {
                e.value=aryValue[i];
              }
           }  
           return false; 
       } 
   }
   else
   {
        //参数
        var strPara = "height=450px;width=500px;help=off;resizable=off;scroll=no;status=off;modal=yes;dialog=yes";
        //打开窗口
        var url=rootPath+"Common/SelectDialog2.aspx?TableName="+strTableName+"&ReturnField="+strReturnField+"&time="+time+"&targetControls="+strTarget;
        var DialogWin = window.open(url,"myOpen",strPara,true);
   } 
                                   
}
