<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MainPage.aspx.cs" Inherits="MainPage" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server"> 
<style type="text/css">
img {
   behavior: url("//JScript/pngbehavior.htc");
}
</style>
    
    <script type="text/javascript" language="javascript">     
     var thisTimeOut
     function carGrid()
    {           
     
    }
    </script>
    <title>无标题页</title>
    <link href="Css/css.css?t=88" type="text/css" rel="stylesheet" />
<script type="text/javascript" language="javascript">
  function SetNewColor(source)
  {
            _oldColor=source.style.backgroundColor;
            source.style.backgroundColor='#C0E4EE';
          
  }
  function SetOldColor(source)
  {
         source.style.backgroundColor=_oldColor;
  }
  </script>
</head>
<body bgcolor="#F8FCFF" style="margin-top:30px;" onload = "carGrid()">
    <form id="form1" runat="server">
    <div style="background-color:Red;">
    </div>
    </form>
</body   >
</html>
