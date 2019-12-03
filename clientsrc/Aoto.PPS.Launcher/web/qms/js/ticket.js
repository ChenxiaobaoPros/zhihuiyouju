var app = {};
app.mode = 2;
app.ticketBtnTimeout = 3;
app.ppsPopUpTimeout = 5;
app.ticketBtnTimeout = app.ticketBtnTimeout ? app.ticketBtnTimeout:3; 
app.ppsPopUpTimeout = app.ppsPopUpTimeout ? app.ppsPopUpTimeout:5; 
var ppsPopUpTimeout = app.ppsPopUpTimeout+1;
var qmsIp = "";
var channel = "1";
var $biz1 = $("#biz1");
var $biz2 = $("#biz2");
var $yyqh = $("#yyqh");
var $error = $("#error");  
var $qtwdxx = $("#qtwdxx");  
var $biz11 = $("#biz1");
var $loading = $("#loading");
var pageTimeoutIntervalT = null;
var pageTimeout = 30;
var keyboardTimeoutIntervalT = null;
var keyboardTimeout = 60;
var devStatusArr = null;
var signJo = null;
var isBusPage = false;
var styleData = null;
var xWidth = screen.width;
var yHeight = screen.height;
var perPage = 20;
var $divLogin = $("#divLogin");
var $divValidate = $("#divValidate");
var $divFill = $("#divFill");
var	$biz = $("#biz");
var $main = $("#main");
var settings = null;
var lwzt = "0";//联网状态
var zszt = "1";//值守状态
var disableOfflineGetTicketFlag = "1";
var dutyFlag = "1";
var enFlag = "1";
var pageSize = 5;
var adminStr = "";
var printStatus = "0000";

/*
---------------------------------------------------  
	 日期格式化  
	 格式 YYYY/yyyy/YY/yy 表示年份  
	 MM/M 月份  
	 W/w 星期  
	 dd/DD/d/D 日期  
	 hh/HH/h/H 时间  
	 mm/m 分钟  
	 ss/SS/s/S 秒  
--------------------------------------------------- 
*/
Date.prototype.Format = function(formatStr){   
    var str = formatStr;   
    var Week = ['日','一','二','三','四','五','六'];  
  
    str=str.replace(/yyyy|YYYY/,this.getFullYear());   
    str=str.replace(/yy|YY/,(this.getYear() % 100)>9?(this.getYear() % 100).toString():'0' + (this.getYear() % 100));   
  
    str=str.replace(/MM/,(this.getMonth()+1)>9?(this.getMonth()+1).toString():'0' + (this.getMonth()+1));   
    str=str.replace(/M/g,this.getMonth());   
  
    str=str.replace(/w|W/g,Week[this.getDay()]);   
  
    str=str.replace(/dd|DD/,this.getDate()>9?this.getDate().toString():'0' + this.getDate());   
    str=str.replace(/d|D/g,this.getDate());   
  
    str=str.replace(/hh|HH/,this.getHours()>9?this.getHours().toString():'0' + this.getHours());   
    str=str.replace(/h|H/g,this.getHours());   
    str=str.replace(/mm/,this.getMinutes()>9?this.getMinutes().toString():'0' + this.getMinutes());   
    str=str.replace(/m/g,this.getMinutes());   
  
    str=str.replace(/ss|SS/,this.getSeconds()>9?this.getSeconds().toString():'0' + this.getSeconds());   
    str=str.replace(/s|S/g,this.getSeconds());   
  
    return str;   
};

$(function (){
	  initPage();
		/*
		rtn = {
		  "biom": {
			"body": {
			  "addr": "（已屏蔽）",
			  "birthday": "（已屏蔽）",
			  "cardFlag": "3",
			  "certNo": "3209111988050****X",
			  "certType": "0",
			  "custName": "智白*",
			  "custTime": "",
			  "image": "",
			  "indate": "20360625",
			  "leftCipher": "",
			  "nation": "汉族",
			  "office": "南京市公安局雨花台分局",
			  "printtemp": "",
			  "secgs": "",
			  "secondTrack": "",
			  "sex": "男",
			  "signDate": "20160625",
			  "thirdTrack": ""
			},
			"head": {
			  "retCode": "7",
			  "retMsg": "||0||----------------★★★☆☆☆☆----------------||----------------○○○○○○○○----------------|2017-01-09 17:12:08|测试网点,|3|"
			}
		  }
		}
		alert($.toJSON(rtn));
		qmsSwipeCardCallback($.toJSON(rtn));*/
});
function initPage(){
    $.qms.setCache("signFlag", "1");
	  //alert($.qms.getCache("signFlag"));
	  $.qms.setCache("isYyqhPage","0");
	  showLoading();//显示等待加载
	  screenResolutionCss();//屏幕样式
	  signJo = $.parseJSON($.qms.getCache("sign"));	//从壳中取出键为sign的值，然后将json格式的值转为js对象
	  //alert($.toJSON(signJo));
	  //1主  2备  3联脱机  4禁止脱机取号  5打印机状态  
	  var devStatus = $.qms.getCache("devStatus");

	  //alert(devStatus);
	  if(devStatus){
			devStatusArr = devStatus.split("|");
			printStatus = devStatusArr[4] ? signJo.biom.body.printStatus:"0000";
	  }else{
			devStatusArr = (signJo.biom.body.devStatus).split("|");
			printStatus = signJo.biom.body.printStatus ? signJo.biom.body.printStatus:"0000";
	  }
	  disableOfflineGetTicketFlag = $.qms.getCache("disableOfflineGetTicketFlag");
	  if(!disableOfflineGetTicketFlag){
			disableOfflineGetTicketFlag = devStatusArr[3];
	  }
	  lwzt = devStatusArr[2];//联网状态
	  //zszt = "1";
	  zszt = $.qms.getCache("dutyStatus");//值守状态
	  enFlag = signJo.biom.body.remindFlag ? signJo.biom.body.remindFlag:"1";
	  //enFlag = "0";
	  dutyFlag = signJo.biom.body.dutyFlag ? signJo.biom.body.dutyFlag:"1";
	  
	  //signJo.biom.body.pageTimeout = 3;
	  pageTimeout = signJo.biom.body.pageTimeout ? signJo.biom.body.pageTimeout:30;
	  keyboardTimeout = signJo.biom.body.pageTimeout ? signJo.biom.body.pageTimeout:30;
	  var args = {};
	  args.queueTemplate = {};
	  //args.queueTemplate.screenResolution = (app.screenResolution ? app.screenResolution : "1366x768");
	  args.queueTemplate.screenResolution = xWidth+"x"+yHeight;


      //获取界面信息
	  var rtn = $.parseJSON($.qms.getQueueTemplateJsonFile(args));
	  var retCode = rtn.biom.head.retCode;
	  var body = rtn.biom.body;
	  if (rtn.biom.head.retCode == 0){
			styleData = body.queueTemplate.print;
			addLevel1();//加载首页的菜单页面
			initErrorDiv();//加载错误页面
			initQtwdxxDiv();//加载其他信息二级菜单页面
			initLoadingDiv();//加载等等加载提示页面
			initYyqhDiv();//加载预约取号二级菜单页面
			/*setInterval(function ()
			{
			  $("label[name='systemTime']").text((new Date()).Format('yyyy年MM月dd日 hh:mm:ss 星期W'));
			}, 1000);*/
			
			var flag = disableOfflineGetTicket();
			if(flag) flag = devStatusGetTicket();
			if(flag){
				setCardStatus("1");  //back() 中已经包含了setCardStatus("1");
				//back();
			}else{
				setCardStatus("0");
			}
			$main.show();
			hideLoading();
	  }
}
function showLoading(cnRetMsg,enRetMsg){
	if(cnRetMsg){
		$("#loadingMessage").find("label").eq(0).text(cnRetMsg);
		$("#loadingMessage").find("label").eq(1).text(enRetMsg);
	}else{
		$("#loadingMessage").find("label").eq(0).text("正在处理中,请稍候......");
		$("#loadingMessage").find("label").eq(1).text("Queuing parameters are being obtained. Please wait…");
	}
	$loading.show();
}
function screenResolutionCss(){
    qmsIp = $.qms.getCache("qmsIp");
	var args = getZoomJo();
	//alert($.toJSON(args));
	$main.css("transform-origin","0 0");
	$main.css("transform","scale("+args.zoomX+","+args.zoomY+")");
	xWidth = args.width;
	yHeight = args.height;
	$biz.css({"width":xWidth,"height":yHeight});
	$loading.css({"width":xWidth,"height":yHeight});
}
function addLevel1(){
	$biz1.empty().html("");//删除id为biz1下面的所有子标签，为后面动态的重新加入样式做准备
	var body = signJo.biom.body;
	var type = null;
	var $spn,divLength;
	var flag = -1;
	var itemStyleData = null;
	var items = styleData.level1; 
	//alert(body.perShowFlag+ " " +body.cusShowFlag+" "+body.ptShowFlag+" "+body.ptShowFlag);
	
	//显示  个人无卡折户菜单  grwkzh
	//alert("个人客户状态："+body.perShowFlag);
	if(body.perShowFlag&&body.perShowFlag == "1"){
		flag ++;
		itemStyleData = items[flag];
		type = "grwkzh";    
		
		var itemData = {};
		itemData.list = body.sctNoBusiList? body.sctNoBusiList.sctNoBusi:null;
		//1-个人客户，2-对公客户，3-无卡无折，4-预约客户，5-外围接口，6-公积金，7-手机预约取号
		itemData.busiType1 = "3";
        itemData.cardFlag = "5";
        //初始化显示首页菜单
		initBuzType($biz1.get(0),itemStyleData,type,itemData);//根据type初始化进入个人无卡折户二级菜单
	}
	//显示  对公客户菜单   
	//alert("对公客户状态："+body.cusShowFlag);
	if(body.cusShowFlag&&body.cusShowFlag == "1"){
		flag ++;
		itemStyleData = items[flag];
		if(body.cusLevelShowFlag&&body.cusLevelShowFlag=="1"){
			type = "dgwkh";
		}else{
			type = "dgkh";
		}
		var itemData = {};
		
		itemData.list = body.sctCusBusiList.sctCusBusi;
		//1-个人客户，2-对公客户，3-无卡无折，4-预约客户，5-外围接口，6-公积金，7-手机预约取号
		itemData.busiType1 = "2";
		itemData.cardFlag = "5";
		initBuzType($biz1.get(0),itemStyleData,type,itemData);//根据type初始化进入对公无卡折户二级菜单
	}
	
	//显示  预约客户取号菜单。 1(联机)：显示，0(脱机)：隐藏
	//if(body.ptShowFlag == "1"){
	//alert("联网状态："+lwzt);
	if(lwzt == "1"){
		flag ++;
		itemStyleData = items[flag];
		type = "yyqh";
		var itemData = {};
		itemData.list = body.sctNoBusiList.sctNoBusi;
		//1-个人客户，2-对公客户，3-无卡无折，4-预约客户，5-外围接口，6-公积金，7-手机预约取号
        itemData.busiType1 = "4";
        //初始化显示首页菜单
		initBuzType($biz1.get(0),itemStyleData,type,itemData);//根据type初始化进入预约取号二级菜单		
	}else{
		$biz1.find("div[name='yyqh']").hide();
	}
	
	//显示其他网点信息菜单。 1(联机)：显示，0(脱机)：隐藏
	//alert("联网状态："+lwzt);
	if(lwzt == "1"){
		flag ++;
		itemStyleData = items[flag];
		type = "qtwdxx";
		var itemData = null;
		initBuzType($biz1.get(0),itemStyleData,"qtwdxx",itemData);//根据type初始化进入其他网点信息二级菜单
	}else{	
		$biz1.find("div[name='qtwdxx']").hide();
	}
	
	//非业务按扭区域
	if(styleData&&styleData.level1Items&&styleData.level1Items.length>0){
		$.each(styleData.level1Items, function (index, item){
			if(item.type=="status"){ //状态处
				$spn = $('<div class="site" name = "status" style="background-size: 100% 100%; left: 650px; top: 50px; width: 350px; height: 50px; z-index: 10; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-color: rgba(182, 215, 168, 0); overflow: hidden;"></div>');
				$spn.data("itemStyleData",item);
				$spn.appendTo($biz1);
				initItemStyle($spn.get(0),1);
			}else if(item.type=="pageTimeout"){ //超时处
				$spn = $('<div class="site" name = "pageTimeout" style="background-size: 100% 100%; left: 650px; top: 70px; width: 300px; height: 90px; z-index: 10; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-color: rgba(182, 215, 168, 0); overflow: hidden;"></div>');
				$spn.data("itemStyleData",item);
				$spn.appendTo($biz1);
				initItemStyle($spn.get(0),1);
			}else if(item.type=="remind"){ //滚动信息处
				$spn = $('<div class="site" name = "remind" style="background-size:100% 100%;left:0px;top:1000px;width:100%;height: 80px; z-index: 10; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-color: rgba(50, 50, 50, 0.6); overflow: hidden;"></div>');
				$spn.data("itemStyleData",item);
				$spn.appendTo($biz1);
				initItemStyle($spn.get(0),1);
			}else if(item.type=="message"){
				$spn = $('<div class ="site" name="message" style="display:none;background-size: 100% 100%; left: 100px; top: 350px; width: 80%; height: 150px; z-index: 10; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-color: rgba(182, 215, 168, 0); overflow: hidden;"></div>');
				$spn.data("itemStyleData",item);
				$spn.appendTo($biz1);
				initItemStyle($spn.get(0),1);
			}else if(item.type=="warmPrompt"){ //温馨提示处
				$spn = $('<div class="site" name = "warmPrompt" style="background-size: 100% 100%; left: 250px; top: 1100px; width: 50%; height: 150px; z-index: 10; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-color: rgba(182, 215, 168, 0); overflow: hidden;"></div>');
				$spn.data("itemStyleData",item);
				$spn.appendTo($biz1);
				initItemStyle($spn.get(0),1);
			}else if(item.type=="swipeTip"){
				$spn = $('<div class="site" name = "swipeTip"	style="background-size: 100% 100%; left: 150px; top: 250px; width: 80%; height: 150px; z-index: 10; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-color: rgba(182, 215, 168, 0); overflow: hidden;"></div>');
				$spn.data("itemStyleData",item);
				$spn.appendTo($biz1);
				initItemStyle($spn.get(0),1);
			}else if(item.type=="queueNo"){
				$spn = $('<div class="site" name = "queueNo" style="background-size: 100% 100%; left: 10px; top: 1240px; width: 250px; height: 50px; z-index: 10; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-color: rgba(182, 215, 168, 0); overflow: hidden;"></div>');
				$spn.data("itemStyleData",item);
				$spn.appendTo($biz1);
				initItemStyle($spn.get(0),1);
			}else if(item.type=="login"){ //登录系统设置出 
				$spn = $('<div class="site" name = "login" id="login" style="border: 0px solid #ffffff;background-size: 100% 100%; left: 200px; top: 150px; width: 620px; height: auto; z-index: 999; border-radius:16px;box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg);font-size:40px;font-weight:bold;font-family: Microsoft YaHei; background-color: rgba(218, 231, 242, 1); overflow: hidden;padding-top:50px;padding-bottom:50px;display:none;">');
				$spn.data("itemStyleData",item);
				$spn.appendTo($biz1);
				initItemStyle($spn.get(0),1);
			}else if(item.type=="blockUI"){
				$spn = $('<div name="blockUI" style="display:none;position: absolute; top: 0%; left: 0%; width: 100%; height: 100%; background-color: black; z-index:11; -moz-opacity: 0.7; opacity:.70; filter: alpha(opacity=70);overflow:hidden;"></div>');
				$spn.appendTo($biz1);
			}
		});
	}
	//$.qms.log($("#body111").html());
}
function initErrorDiv(){
	$error.empty().html("");
	if(styleData&&styleData.errorItems&&styleData.errorItems.length>0){
		//分流提示	remindFlag
		$.each(styleData.errorItems, function (index, item){
			if(item.type=="errorMessage"){
				$spn = $('<div class ="site" name="errorMessage" style="background-size: 100% 100%; left: 100px; top: 350px; width: 80%; height: 150px; z-index: 10; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-color: rgba(182, 215, 168, 0); overflow: hidden;">'
								+'<li style="width:80%;display:inline-block; vertical-align: middle;font-size: 40px;text-align:center; color: #ffffff;">'
									+'<label style="display:block;text-align:center;" id="errorMessage"></label>'
									+'<label style="display:'+(enFlag=='1'? 'block':'none')+';text-align:center;"></label>'
								+'</li>'
								+'<li onclick="goGetTicket(this,1);" id="errorBtn1" style="border-radius:8px;display:inline-block;width:50%;text-align:center;background-color:rgba(122,175,59,1);padding:20px;font-size:30px;margin-top:30px;" >'
									+'<label style="display:block;text-align:center;"  >取号并撤销预约</label>'
									+'<label style="display:'+(enFlag=='1'? 'block':'none')+';text-align:center;">Get No. & cancel</label>'
								+'</li>'	
								+'<li onclick="goGetTicket(this,2);" id="errorBtn2" style="border-radius:8px;display:inline-block;width:50%;text-align:center;background-color:#4875f7;padding:20px;font-size:30px;margin-top:30px;" >'
									+'<label style="display:block;text-align:center;" >取号不撤销预约</label>'
									+'<label style="display:'+(enFlag=='1'? 'block':'none')+';text-align:center;">Get No. Not cancel</label>'
								+'</li>'			
						+'</div>');   
				$spn.data("itemStyleData",item);
				$spn.appendTo($error);
				initItemStyle($spn.get(0),1);
			}else if(item.type=="pageTimeout"){
				$spn = $('<div class="site" name = "pageTimeout" style="background-size: 100% 100%; left: 650px; top: 70px; width: 300px; height: 90px; z-index: 10; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-color: rgba(182, 215, 168, 0); overflow: hidden;"></div>');
				$spn.data("itemStyleData",item);
				$spn.appendTo($error);
				initItemStyle($spn.get(0),1);
			}else if(item.type=="backBtn"){
			
				//<div class="site" onclick="back();" name = "backBtn" style="z-index: 1000; background-size: 100% 100%; left: 820px; top: 1070px; width: 92px; height: 149px; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-image:url(../../images/q/1/out.png); overflow: hidden;"></div>
				$spn = $('<div class="site" name = "backBtn" onclick="back();" style="cursor:pointer;z-index: 1000; background-size: 100% 100%; left: 820px; top: 1070px; width: 92px; height: 149px; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-image:url(../../images/q/1/out.png); overflow: hidden;"></div>');
				//$spn.data("itemStyleData",item);
				$spn.appendTo($error);
				//initItemStyle($spn.get(0),1);
			}
		});
	}	
}
function initQtwdxxDiv(){
	$qtwdxx.empty().html("");
	if(styleData&&styleData.qtwdxxItems&&styleData.qtwdxxItems.length>0){
		//分流提示	remindFlag
		$.each(styleData.qtwdxxItems, function (index, item){
			if(item.type=="qtwdxx"){
				$spn = $('<div class ="site" name="qtwdxx" style="background-size: 100% 100%; left: 100px; top: 250px;  z-index: 10; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); background-color: rgba(182, 215, 168, 0); overflow: hidden;">'
								+'<div style="color:#ffffff;font-family: Microsoft YaHei;text-align:center;font-size:45px;"><span style="display:block;line-height:60px;">附近网点</span><span style="display:'+(enFlag=='1'? 'block':'none')+';line-height:60px;font-size:40px;">Other bank outlet info</span></div>'
								+'<div id="dataList" class="table-c" style="margin-top:20px;height:600px;"> '
									+'<table>'
										+'<thead>'
											+'<tr>'
												+'<th><span style="line-height:60px;">网点名称</span><span style="font-size:25px;line-height:40px;display:'+(enFlag=='1'? 'block':'none')+';">Outlet name</span></th>'
												+'<th><span style="line-height:60px;">等待客户数</span><span style="font-size:25px;line-height:40px;display:'+(enFlag=='1'? 'block':'none')+';">Number of waiting customer</span></th>'
											+'</tr>'
										+'</thead>'
										+'<tbody>'
											+'<tr><td>北京网动行</td><td>1</td></tr>'
											+'<tr><td>北京网动行</td><td>1</td></tr>'
											+'<tr><td>北京网动行</td><td>1</td></tr>'
											+'<tr><td>北京网动行</td><td>1</td></tr>'
											+'<tr><td>北京网动行</td><td>1</td></tr>'
											+'<tr><td>北京网动行</td><td>1</td></tr>'
										+'</tbody>'
									+'</table>'
								+'</div>'
								+'<div class="pages" style="display:block;">'
									+'<div id="Pagination">'
										+'<a href="#" class="first" data-action="first">&laquo;</a>'
										+'<a href="#" class="previous" data-action="previous">上一页</a>'
										+'<input type="text" readonly="readonly" data-max-page="40" />'
										+'<a href="#" class="next" data-action="next">下一页</a>'
									+'</div>'
									+'<div class="searchPage" style="display:none;">'
									  +'<span class="page-sum">共<strong class="allPage">15</strong>页</span>'
									  +'<span class="page-go">跳转<input type="text">页</span>'
									  +'<a href="javascript:;" class="page-btn">GO</a>'
									+'</div>'
								+'</div>'
						+'</div>');
				$spn.data("itemStyleData",item);
				$spn.appendTo($qtwdxx);
				initItemStyle($spn.get(0),1);
			}else if(item.type=="pageTimeout"){
				$spn = $('<div class="site" name = "pageTimeout" style="background-size: 100% 100%; left: 650px; top: 70px; width: 300px; height: 90px; z-index: 10; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-color: rgba(182, 215, 168, 0); overflow: hidden;"></div>');
				$spn.data("itemStyleData",item);
				$spn.appendTo($qtwdxx);
				initItemStyle($spn.get(0),1);
			}else if(item.type=="backBtn"){
			
				//<div class="site" onclick="back();" name = "backBtn" style="z-index: 1000; background-size: 100% 100%; left: 820px; top: 1070px; width: 92px; height: 149px; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-image:url(../../images/q/1/out.png); overflow: hidden;"></div>
				$spn = $('<div class="site" name = "backBtn" onclick="back();" style="cursor:pointer;z-index: 1000; background-size: 100% 100%; left: 820px; top: 1070px; width: 92px; height: 149px; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-image:url(../../images/q/1/out.png); overflow: hidden;"></div>');
				//$spn.data("itemStyleData",item);
				$spn.appendTo($qtwdxx);
				//initItemStyle($spn.get(0),1);
			}
		});
	}	
}
function initLoadingDiv(){
	$loading.empty().html("");
	if(styleData&&styleData.loadingItems&&styleData.loadingItems.length>0){
		//分流提示	remindFlag
		$.each(styleData.loadingItems, function (index, item){
			if(item.type=="loading"){
				$spn = $('<div class ="site" name="loading" style="position:absolute;z-index:1001;text-align:center;background-size: 100% 100%; left:10%; top: 40%; width: 80%; height: auto;box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-color: rgba(218, 231, 242, 0); overflow: hidden;">'
							+'<li style="background-size: 100% 100%; vertical-align: middle;display:block;width:152px;height:152px;text-align:center;margin:0 auto;background-image: url(../../images/q/1/dd.gif);font-size:50px;color:#ffffff;" >'
							+'</li>'
							+'<li id="loadingMessage" style="margin:0 auto;margin-top:50px;vertical-align: middle;display:inline-block;text-align:center;background-color:rgba(122,175,59,0);font-size:50px;color:#ffffff;" >'
								+'<label style="display:block;text-align:center;">正在处理中,请稍候......</label>'
								+'<label style="display:'+(enFlag=='1'? 'block':'none')+';text-align:center;">Queuing parameters are being obtained. Please wait…</label>'
							+'</li>'	
						+'</div>');
				$spn.data("itemStyleData",item);
				$spn.appendTo($loading);
			}
		});
	}		
}
function initYyqhDiv(){
	$yyqh.empty().html("");
	if(styleData&&styleData.yyqhItems&&styleData.yyqhItems.length>0){
		//分流提示	remindFlag
		$.each(styleData.yyqhItems, function (index, item){
			if(item.type=="keyboard"){
				$spn = $('<div class="site" name = "keybord" style="border: 0px solid #ffffff;display:block;background-size: 100% 100%; left: 200px; top: 550px; width: 600px; height: auto; z-index: 10; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg);font-size:40px;font-weight:bold;font-family: Microsoft YaHei; background-color: rgba(182, 215, 168, 0); overflow: hidden;">'
							+'<li style="display:block;box-sizing:border-box;margin:0;padding:0;"><input id="phoneNo" type="text" maxlength="11" style="font-weight:bold;margin:0px 1px 8px 1px;width:92%;height:80px;border-radius:8px;background-size: 100% 100%;background-color:rgba(194,231,240,1);"/></li>'
							+'<li style="display:block;box-sizing:border-box;margin:0;padding:0;margin-top:20px;" name="keys">'
								+'<input type="button" onclick="cal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:30%;height:80px;border-radius:8px;background-size: 100% 100%;background-color:rgba(194,231,240,1);" value="1"/>'
								+'<input type="button" onclick="cal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:30%;height:80px;border-radius:8px;background-size: 100% 100%;background-color:rgba(194,231,240,1);" value="2"/>'
								+'<input type="button" onclick="cal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:30%;height:80px;border-radius:8px;background-size: 100% 100%;background-color:rgba(194,231,240,1);" value="3"/>'
								+'<input type="button" onclick="cal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:30%;height:80px;border-radius:8px;background-size: 100% 100%;background-color:rgba(194,231,240,1);" value="4"/>'
								+'<input type="button" onclick="cal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:30%;height:80px;border-radius:8px;background-size: 100% 100%;background-color:rgba(194,231,240,1);" value="5"/>'
								+'<input type="button" onclick="cal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:30%;height:80px;border-radius:8px;background-size: 100% 100%;background-color:rgba(194,231,240,1);" value="6"/>'
								+'<input type="button" onclick="cal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:30%;height:80px;border-radius:8px;background-size: 100% 100%;background-color:rgba(194,231,240,1);" value="7"/>'
								+'<input type="button" onclick="cal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:30%;height:80px;border-radius:8px;background-size: 100% 100%;background-color:rgba(194,231,240,1);" value="8"/>'
								+'<input type="button" onclick="cal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:30%;height:80px;border-radius:8px;background-size: 100% 100%;background-color:rgba(194,231,240,1);" value="9"/>'
								+'<input type="button" onclick="cal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:30%;height:80px;border-radius:8px;background-size: 100% 100%;background-color:rgba(194,231,240,1);" value="清除"/>'
								+'<input type="button" onclick="cal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:30%;height:80px;border-radius:8px;background-size: 100% 100%;background-color:rgba(194,231,240,1);" value="0"/>'
								+'<input type="button" onclick="cal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:30%;height:80px;border-radius:8px;background-size: 100% 100%;background-color:rgba(122,175,59,1);border-color: rgba(122,175,59,1);" value="确认"/>'
							+'</li>'
						+'</div>');
				$spn.data("itemStyleData",item);
				$spn.appendTo($yyqh);
				initItemStyle($spn.get(0),1);
			}else if(item.type=="yySwipeTip"){
				$spn = $('<div class="site" name = "yySwipeTip" style="display:block;background-size: 100% 100%; left: 120px; top: 250px; width: 80%; height: auto; z-index: 10; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-color: rgba(182, 215, 168, 0); overflow: hidden;">'
								+'<li style="width:80%;display:inline-block; vertical-align: middle;font-size: 40px;text-align:center; color: #ffffff;">'
									+'<label style="display:block;text-align:left;cursor: default;">请使用您的卡、存折或身份证进行取号 或请输入预约手机号取号</label>'
									+'<label style="display:'+(enFlag=='1'? 'block':'none')+';text-align:left;cursor: default;">Please use your card, passbook or ID to obtain an access number</label>'
								+'</li>'																			      			
								+'<li style="background-size: 100% 100%;width:18%;height:100px;background-image: url(../../../qms/images/q/1/jt.gif);display:inline-block; vertical-align: middle;">'
								+'</li>'
						+'</div>');
				$spn.data("itemStyleData",item);
				$spn.appendTo($yyqh);
				initItemStyle($spn.get(0),1);
			}else if(item.type=="pageTimeout"){
				$spn = $('<div class="site" name = "pageTimeout" style="background-size: 100% 100%; left: 650px; top: 70px; width: 300px; height: 90px; z-index: 10; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-color: rgba(182, 215, 168, 0); overflow: hidden;"></div>');
				$spn.data("itemStyleData",item);
				$spn.appendTo($yyqh);
				initItemStyle($spn.get(0),1);
			}else if(item.type=="backBtn"){
			
				//<div class="site" onclick="back();" name = "backBtn" style="z-index: 1000; background-size: 100% 100%; left: 820px; top: 1070px; width: 92px; height: 149px; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-image:url(../../images/q/1/out.png); overflow: hidden;"></div>
				$spn = $('<div class="site" name = "backBtn" onclick="back();" style="cursor:pointer;z-index: 1000; background-size: 100% 100%; left: 820px; top: 1070px; width: 92px; height: 149px; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-image:url(../../images/q/1/out.png); overflow: hidden;"></div>');
				//$spn.data("itemStyleData",item);
				$spn.appendTo($yyqh);
				//initItemStyle($spn.get(0),1);
			}
		});
	}	
}

//根据联网状态来判断“预约取号”、“其他网点信息”的显示和隐藏
function disableOfflineGetTicket(){
	var flag = true;
	//alert("联网状态："+lwzt);
	if(lwzt == "1"){
		$biz1.find("div[name='yyqh']").show();//联机时“预约取号”、“其他网点信息”显示。
		$biz1.find("div[name='qtwdxx']").show();
		$error.find("div[name='backBtn']").show();
	}else{
		$biz1.find("div[name='yyqh']").hide();//脱机时“预约取号”、“其他网点信息”隐藏。
		$biz1.find("div[name='qtwdxx']").hide();
		
		if(disableOfflineGetTicketFlag=="1"){ //如果设置了“禁止脱机取号”，则执行以下内容。
			//showLoading();
			//显示 排队机当前不允许脱机取号，请咨询大堂经理及英文
			clearInterval(pageTimeoutIntervalT);
			$("#errorMessage").text("排队机当前不允许脱机取号， 请咨询大堂经理");
			
			if(enFlag == "0"){
				$("#errorMessage").next().text("");
			}else{
				$("#errorMessage").next().text("Queuing machine is unavailable for obtaining the access number offline currently. Please consult the lobby manager. ");
			}	
			$("#errorBtn1").hide();
			$("#errorBtn2").hide();
			$error.find("div[name='pageTimeout']").hide();
			showDiv($error.get(0));
			$error.find("div[name='backBtn']").hide();
			hideLoading();
			flag = false;
		}else{
			$error.find("div[name='backBtn']").show();
		}
	}
	return flag;
}
//打印机状态  
function devStatusGetTicket(){
	var flag = true;
	//正常
	if(printStatus == "0000"){
		$error.find("div[name='backBtn']").show();
	}else{
		flag = false;
		clearInterval(pageTimeoutIntervalT);
		
		if(printStatus == "0002"){
			$("#errorMessage").text("打印机无纸， 请咨询大堂经理");
			$("#errorMessage").next().text("打印机无纸. Please consult the lobby manager. ");
		}else if(printStatus == "0003"){
			$("#errorMessage").text("打印机故障， 请咨询大堂经理");
			$("#errorMessage").next().text("打印机故障. Please consult the lobby manager. ");
		}else if(printStatus == "9999"){
			$("#errorMessage").text("打印机未安装， 请咨询大堂经理");
			$("#errorMessage").next().text("打印机未安装. Please consult the lobby manager. ");
		}
		if(enFlag == "0"){
			$("#errorMessage").next().text("");
		}
		$("#errorBtn1").hide();
		$("#errorBtn2").hide();
		$error.find("div[name='pageTimeout']").hide();
		showDiv($error.get(0));
		$error.find("div[name='backBtn']").hide();
		hideLoading();
	}
	return flag;
}
function setCardStatus(popEvent){
    if (popEvent && popEvent == "1") {
        popEvent = 5;
		$.qms.openCard(popEvent);
	}else{
		$.qms.closeCard("0");
	}
}
function hideLoading(){
	$loading.hide();
}

//竖屏  1024x1280   横屏  1366x768
function getZoomJo(){
	var args = {};
	//横屏
	//if(screen.width>screen.height){
	//	args.zoomX = screen.width/1366;
	//	args.zoomY = screen.height/768;
	//	args.width = 1366;
	//	args.height = 768;
	//竖屏	
	//}else{
		//使用1024x1280
		//alert(screen.width+"  "+screen.height);
		args.zoomX = screen.width/1024;
		args.zoomY = screen.height/1280;
		args.width = 1024;
		args.height = 1280;
	//}
	return args;
}

//初始化显示首页菜单
function initBuzType(obj,itmeStyle,type,itemData){
   //如果不显示英文,则英文内容为空  颜色 图标信息
   var cnname,enname,backgroundColor,icon; 
   var level1BuzTypes = styleData.level1BuzTypes;
   
   cnname = getCnnameByBuzCode(type,level1BuzTypes);
   if(enFlag=="1"){
		enname = getEnnameByBuzCode(type,level1BuzTypes);	
   }else{
	    enname = "";
   }
   backgroundColor = getBackgroundColorByBuzCode(type,level1BuzTypes);	
   backgroundColor = backgroundColor? backgroundColor:"#1fbde5";
   icon = getIconByBuzCode(type,level1BuzTypes);
   icon	= icon? icon:"yyqh";	
   
   //首页四个菜单的样式，动态的。
   var $spn = $("<div class=\"site\" name=\""+type+"\" style=\"background-size: 100% 100%; left:"+itmeStyle.x+"px; top: "+itmeStyle.y+"px; width: "+itmeStyle.width+"px; height: "+itmeStyle.height+"px; position: absolute; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-color: "+backgroundColor+"; overflow: hidden;\">"
		+"<ul style=\"display:table-cell;padding:0;margin:0;transform: rotateZ(0deg); text-align:left;font-style: normal; font-weight: normal; color: rgb(255, 255, 255);cursor: pointer;\" onclick=\"addLevel2(this);\">"
			+"<li style=\"display:block; vertical-align: middle;font-size: 41px;text-align:left; padding:10px 15px 0px 15px;\">"+cnname+"</li>"
			+"<li style=\"display:block; font-size: 30px;text-align:left; padding-left:15px;padding-right:15px;\">"+enname+"</li>"
			+"<li style=\"display:block;background-size: 100% 100%; left: 165px; top: 100px; width: 100px; height: 100px; position: absolute;background-image: url(../../images/q/1/buzType/"+icon+".png);\"></li>"
		+"</ul>"
		+"<ul style=\"padding:0;margin:0;display:none;\">"
			+"<li style=\"z-index: 99;display:inline-block;line-height:225px;vertical-align: middle;text-align:center;font-style: normal; font-weight: bold; font-size: 30px;display:block;background-size: 100% 100%;width:100%;height:100%;position: absolute;left: 0px; top: 0px;color: rgba(255, 255, 255, 0.8); background-color: rgba(0, 0, 0, 0.8);\">暂停服务</li>"
		+"</ul>"
	+"</div>");
	//alert($.toJSON(itemData));
	$spn.data("itemData",itemData);
	
	$spn.appendTo($(obj));
}
function getCnnameByBuzCode(buzCode,data){
	for (var i = 0; i < data.length; i++) {
		if (buzCode == data[i].buzCode) {
			return data[i].cnname;
		}
	}
}
function getEnnameByBuzCode(buzCode,data){
	for (var i = 0; i < data.length; i++) {
		if (buzCode == data[i].buzCode) {
			return data[i].enname;
		}
	}
}
function getBackgroundColorByBuzCode(buzCode,data){
	for (var i = 0; i < data.length; i++) {
		if (buzCode == data[i].buzCode) {
			return data[i].backgroundColor;
		}
	}
}
function getIconByBuzCode(buzCode,data){
	for (var i = 0; i < data.length; i++) {
		if (buzCode == data[i].buzCode) {
			return data[i].icon;
		}
	}
}

function initItemStyle(obj,times){
	if(!times) times = 1;
	var item = $(obj).data("itemStyleData");
	var $spn = $(obj);
	//alert(item.width*times+"  "+item.height*times);
	if(item){   
		if(item.type=="status"){
			
			$spn.empty().html('<li style="width:100%;display:inline-block; vertical-align: middle;font-size: 24px;text-align:center; color: #ffffff;">'
								+'主机<img id="status1" style="vertical-align: middle;width:20px;height:20px;" src="../../images/q/1/'+getLightColor(devStatusArr[0])+'.png"/>&nbsp;|&nbsp;备机<img id="status2" style="vertical-align: middle;width:20px;height:20px;" src="../../images/q/1/'+getLightColor(devStatusArr[1])+'.png"/>&nbsp;|&nbsp;<span style="display:inline-block;" id="status3">'+(devStatusArr[2]== 1 ? "联机":"脱机")+'</span>&nbsp;|&nbsp;<span style="display:inline-block;" id="status4">'+(zszt== "1" ? "值守":"非值守")+'</span>'
							+'</li>');	
			$spn.find("li").css("cursor","default");
			setIndex($spn.get(0));
		}else if(item.type=="pageTimeout"){
			$spn.empty().html('<li name="systemTime" style="display: table-cell; vertical-align: middle; background-size: 100% 100%; left: 0px; top: 0px; width: 100%; height: 100%; cursor: default; font-style: normal; font-weight: normal; font-size: 26px; color: #ffffff;">页面停留时间<span style="display:inline-block;width:60px;font-size:40px;color: rgba(134, 234, 234, 0.6);">'+pageTimeout+'</span>秒</li>');	
			$spn.find("li").css("cursor","default");
			setIndex($spn.get(0));
		}else if(item.type=="remind"){
			var remind = signJo.biom.body.remind;
			if(enFlag=='1'){
				remind += signJo.biom.body.remindEn;
			}
			if(item.effect=="marquee"){
				$spn.empty().html('<li style="display:table-cell;vertical-align:middle; background-size:100% 100%;left:0px;top:0px;width:100%;height:100%;"><marquee style="display: table-cell;vertical-align: middle;" width=100% height=100% scrollAmount='+item.scrollAmount+'  scrollDelay=100 behavior="scroll" direction="'+item.direction+'" >'+remind+'</marquee></li>');
			}else{
				$spn.empty().html('<li style="display:table-cell;vertical-align:middle; background-size:100% 100%;left:0px;top:0px;width:100%;height:100%;">'+remind+'</li>');
			}
			$spn.find("li").css("cursor","default");
			setIndex($spn.get(0));
		}else if(item.type=="message"){
			$spn.empty().html('<li style="width:80%;display:inline-block; vertical-align: middle;font-size: 40px;text-align:center; color: #ffffff;"><label style="display:block;text-align:left;">排队机当前不允许脱机取号，请咨询大堂经理!</label><label style="display:'+(enFlag=='1'? 'block':'none')+';text-align:left;">Queuing machine is unavailable for obtaining the access number offline currently. Please consult the lobby manager.</label></li>');	
			$spn.find("li").css("cursor","default");
			setIndex($spn.get(0));
		}else if(item.type=="swipeTip"){
			var cnRetMsg = "您好，个人客户请刷卡（折）或刷身份证取号";
			var enRetMsg = "Please swipe your card(passbook) or ID card to obtain an access number.";
			var body = signJo.biom.body;  
			if(body.cusShowFlag&&body.cusShowFlag == "1"){   
				if(body.cusLevelShowFlag&&body.cusLevelShowFlag=="1"){
					cnRetMsg = "您好，请刷卡（折）或刷身份证取号";
					enRetMsg = "Please swipe your card(passbook) or ID card to obtain an access number.";
				}
			}
			$spn.empty().html('<li style="width:80%;display:inline-block; vertical-align: middle;font-size: 40px;text-align:center; color: #ffffff;"><label style="display:block;text-align:left;cursor: default;">'+cnRetMsg+'</label><label style="display:'+(enFlag=='1'? 'block':'none')+';text-align:left;cursor: default;">'+enRetMsg+'</label></li><li style="background-size: 100% 100%;width:18%;height:100px;background-image: url(../../../qms/images/q/1/jt.gif);display:inline-block; vertical-align: middle;"></li>');	
			$spn.find("label").css("cursor","default");
			setIndex($spn.get(0));
		}else if(item.type=="warmPrompt"){
			$spn.empty().html('<li style="width:100%;display:inline-block; vertical-align: middle;font-size: 26px;text-align:center; color: #ffffff;"><label style="display:block;text-align:left;">温馨提示 : 如有疑问 , 请咨询大堂经理</label><label style="display:'+(enFlag=='1'? 'block':'none')+';text-align:left;">warm prompt:for any questions,please consult the lobby manager</label></li>');	
			$spn.find("label").css("cursor","default");
			setIndex($spn.get(0));
		}else if(item.type=="queueNo"){
			$spn.empty().html('<li style="width:100%;display:inline-block; vertical-align: middle;font-size: 24px;text-align:center; color: #ffffff;"><label style="display:inline-block;text-align:left;" id="queueNo"></label><label style="display:inline-block;text-align:left;">&nbsp;&nbsp;<span id="star"></span>&nbsp;<span id="code"></span></label></li>');	
			$spn.find("li").css("cursor","default");
			setIndex($spn.get(0));
		
		//二级页面的元素
		}else if(item.type == "popRemind"){
			var htmlStr = '<li style="box-shadow:0 0 3px 3px rgba(235,190,240,0.6);border-radius:16px;background-size: 100% 100%;width:80%;display:inline-block; vertical-align: middle;font-size: 40px;text-align:center; background-color:rgba(194,231,240,1); color: #000000;padding:40px;">'
				+'<label style="display:block;text-align:center;">分流提示</label>'
				+'<label style="display:'+(enFlag=='1'? 'block':'none')+';text-align:center;">Prompt for queue separation</label>'
				+'<label style="display:block;text-align:left;font-size: 30px;margin-top:20px;">为了节省您的时间，如果您需要办理存款或者20000元以内取款业务，建议到我行自动取款机或者存取款一体机办理。</label>'
				+'<label style="display:'+(enFlag=='1'? 'block':'none')+';text-align:left;font-size: 26px;">To save your time, if you need to conduct deposit or withdrawal within RMB20,000, we suggest you conduct such business with the self-service ATM or CRS of ICBC.</label>'
			+'</li>'
			+'<li onclick="getTicket(this)" style="border-radius:8px;display:inline-block;width:50%;text-align:center;background-color:rgba(122,175,59,1);padding:20px;font-size:30px;margin-top:30px;" >'
				+'<label style="display:block;text-align:center;">继续取号</label>'
				+'<label style="display:'+(enFlag=='1'? 'block':'none')+';text-align:center;">Continue get No.</label>'
			+'<li onclick="hidePopRemind()" style="border-radius:8px;display:inline-block;width:50%;text-align:center;background-color:#4875f7;padding:20px;font-size:30px;margin-top:30px;" >'
				+'<label style="display:block;text-align:center;">返&nbsp;&nbsp;&nbsp;&nbsp;回</label>'
				+'<label style="display:'+(enFlag=='1'? 'block':'none')+';text-align:center;">Return</label>'
			+'</li>';
			$spn.empty().html(htmlStr);	
			$spn.find("li").css("cursor","default");
			setIndex($spn.get(0));
		}else if(item.type == "login"){
		
			var htmlStr = '<li style="display:block;margin:0;padding:0;">密码键盘</li>'
				+'<li style="display:block;margin:0;padding:0;font-size:20px;color:red;">'+(dutyFlag=='1'? '启用值守':'禁用值守')+'</li>'
				+'<li id="loginError" style="display:inline-block;margin-top:20px;box-shadow:0 0 3px 3px rgba(235,190,240,0.6);border-radius:16px;background-size: 100% 100%;width:80%; vertical-align: middle;font-size: 40px;text-align:center; background-color:rgba(194,231,240,1); color: #000000;padding:40px;">'
					+'<label style="display:block;text-align:left;font-size: 30px;"></label>'
					+'<label style="display:'+(enFlag=='1'? 'block':'none')+';text-align:left;font-size: 26px;"></label>'
				+'</li>'
				+'<li style="display:block;margin:0;padding:0;margin-top:20px;"><input id="password" type="password" maxlength="11" style="font-weight:bold;margin:0px 1px 8px 1px;width:450px;height:80px;border-radius:8px;background-size: 100% 100%;background-color:rgba(194,231,240,1);"/></li>'
				+'<li style="display:block;margin:0;padding:0;margin-top:20px;" name="keys">'
					+'<input type="button" onclick="loginCal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:150px;height:80px;border-radius:8px;background-size: 100% 100%;background-color:#4875f7;" value="1"/>'
					+'<input type="button" onclick="loginCal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:150px;height:80px;border-radius:8px;background-size: 100% 100%;background-color:#4875f7;" value="2"/>'
					+'<input type="button" onclick="loginCal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:150px;height:80px;border-radius:8px;background-size: 100% 100%;background-color:#4875f7;" value="3"/>'
					+'<input type="button" onclick="loginCal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:150px;height:80px;border-radius:8px;background-size: 100% 100%;background-color:#4875f7;" value="4"/>'
					+'<input type="button" onclick="loginCal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:150px;height:80px;border-radius:8px;background-size: 100% 100%;background-color:#4875f7;" value="5"/>'
					+'<input type="button" onclick="loginCal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:150px;height:80px;border-radius:8px;background-size: 100% 100%;background-color:#4875f7;" value="6"/>'
					+'<input type="button" onclick="loginCal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:150px;height:80px;border-radius:8px;background-size: 100% 100%;background-color:#4875f7;" value="7"/>'
					+'<input type="button" onclick="loginCal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:150px;height:80px;border-radius:8px;background-size: 100% 100%;background-color:#4875f7;" value="8"/>'
					+'<input type="button" onclick="loginCal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:150px;height:80px;border-radius:8px;background-size: 100% 100%;background-color:#4875f7;" value="9"/>'
					+'<input type="button" onclick="loginCal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:150px;height:80px;border-radius:8px;background-size: 100% 100%;background-color:#4875f7;" value="清除"/>'
					+'<input type="button" onclick="loginCal(this);" style="font-weight:bold;margin:0px 1px 8px 1px;width:150px;height:80px;border-radius:8px;background-size: 100% 100%;background-color:#4875f7;" value="0"/>'
					+'<input type="button" onclick="login();" style="font-weight:bold;margin:0px 1px 8px 1px;width:150px;height:80px;border-radius:8px;background-size: 100% 100%;background-color:#4875f7;" value="确认"/>'
				+'</li>'
				+'<li onclick="closeLogin(1);" style="border-radius:8px;display:inline-block;width:50%;text-align:center;background-color:rgba(122,175,59,1);padding:20px;font-size:30px;margin-top:30px;" >'
					+'<label style="display:block;text-align:center;">返&nbsp;&nbsp;&nbsp;&nbsp;回<span style="display:inline-block;width:50px;color:#4875f7" id="keyboardTimeout">30</span></label>'
					+'<label style="display:none;text-align:center;">Return</label>'
				+'</li>'
			$spn.empty().html(htmlStr);	
			$spn.find("li").css("cursor","default");
			setIndex($spn.get(0));
		}
		
		var $label = $spn.find("li");
		
		if (item.family)
		{
			$spn.css("font-family", item.family);
		}
		if (item.style)
		{
			if(item.style=="normal"){
				$label.css("font-style", "normal").css("font-weight","normal");
			}else if(item.style=="italic"){
				$label.css("font-style", "italic").css("font-weight","normal");
			}else if(item.style=="bold"){
				$label.css("font-style", "normal").css("font-weight","bold");
			}else if(item.style=="boldAndItalic"){
				$label.css("font-style","italic").css("font-weight","bold");
			}
		}else{
			$label.css("font-style", "normal").css("font-weight","normal");
		}

		if (item.size)
		{
			$label.css("font-size", item.size*times + "px");
		}

		if (item.width)
		{
			$spn.css("width", item.width*times + "px");
		}
		
		if (item.height)
		{
			$spn.css("height", item.height*times + "px");
		}			
		
		if (item.x)
		{
			$spn.css("left", item.x*times + "px");
		}
		
		if (item.y)
		{
			$spn.css("top", item.y*times + "px");
		}
		
		if(item.backgroundColor)
		{
			$spn.css("backgroundColor",item.backgroundColor);
		}else{
			$spn.css("backgroundColor","");
		}
		
		if(item.fontColor)
		{
			$label.css("color",item.fontColor);
		}
		
		if(item.backgroundImage){
			$spn.css("backgroundImage","url("+item.backgroundImage+")");
		}else{
			$spn.css("backgroundImage","");
		}
		$spn.css("overflow","hidden");
		
	}
}
function setIndex(obj){
	var itemData = $(obj).data("itemStyleData");
	var orgType = itemData.orgType? itemData.orgType:2;
	if(orgType==1){
		$(obj).css("z-index", 10);
	}else if(orgType == 2){
		if(itemData.type=="buzType"){
			$(obj).css("z-index", 1);
		}else if(itemData.type=="login"){   
			$(obj).css("z-index", 99);
		}else if(itemData.type=="popRemind"){   
			$(obj).css("z-index", 99);
		}else{
			$(obj).css("z-index", 5);
		}
	}
}

function showDiv(obj){
	$biz.find("div[name='biz']").hide();
	$(obj).show();
}

function login(password){
    var password = $("#password").val();
    if (0 == $.trim(password).length){
        showLoginError("请输入密码","Please enter your password");
        return;
    }else{
		$("#password").val("");
	}
	showLoading(); //让后台开启
    var jsonString = $.qms.systemCommond($.trim(password));
	if(jsonString){
		var rtn = $.parseJSON(jsonString);
		var retCode = rtn.biom.head.retCode;
		var retMsg = rtn.biom.head.retMsg;
		var retMsgArr = retMsg.split("|");
		var cnRetMsg = retMsgArr[0];
		var enRetMsg = retMsgArr.length>1 ? retMsgArr[1]:"";
		if(enFlag == "0"){
			enRetMsg = "";
		}
		if(27 != retCode){
			/*  
				关机密码	25
				取号界面退出密码	26
				联机脱机切换密码	27
				值守/非值守模式切换密码	28
				取号界面最小化  29
			*/
			//失败
			if (1 == retCode){
				showLoginError(cnRetMsg,enRetMsg);
			//关机密码	25	
			}else if(25 == retCode){
			//取号界面退出密码	26	
			}else if(26 == retCode){	
			//值守/非值守模式切换密码	28	
			}else if(28 == retCode){
				zszt = $.qms.getCache("dutyStatus");
				$("#status4").text((zszt== "1" ? "值守":"非值守"));
				showLoginError(cnRetMsg,enRetMsg);
				closeLogin(1);
			}else if(29 == retCode){
				closeLogin(1);
			}
			hideLoading();
		}
	}
}
function showLoginError(cnRetMsg,enRetMsg){
	$("#loginError").show();
	$("#loginError").find("label").eq(0).text(cnRetMsg);
	$("#loginError").find("label").eq(1).text(enRetMsg);
}

function back(){
	$.qms.setCache("isYyqhPage","0");
	$biz2.hide();
	$yyqh.hide();
	$error.hide();  
	$qtwdxx.hide();
	$biz1.show();
	closeLogin();
	setCardStatus("1");
	$loading.hide();
}
function closeLogin(flag){
    $biz1.find("div[name='login']").hide();
	  $biz1.find("div[name='blockUI']").hide();
    adminStr = '';
	keyboardTimeout = (signJo.biom.body.pageTimeout ? signJo.biom.body.pageTimeout:20);
	clearInterval(keyboardTimeoutIntervalT);
	if(flag){
		disableOfflineGetTicket();
	}
}

function cal(obj){
	var $lastValue = $(obj).parent().prev().find("input").eq(0);
	var maxlength = $lastValue.attr("maxlength");
	if($(obj).val()=="清除"){
		$lastValue.val("");
	}else if($(obj).val()=="确认"){  //预约取号  密码登录  密码退出  密码关机 密码关系统
		//alert($lastValue.val());
		if($.trim($lastValue.val())==""||$.trim($lastValue.val())=="请输入手机号"){
			$lastValue.val("请输入手机号");
		}else{
			if(!vldUtil.checkPhone($.trim($lastValue.val()))){
				$lastValue.val("请输入正确的手机号");
				return;
			}
			//预约取号
			getTicket4Yyqh($.trim($lastValue.val()));
		}
	}else{
		if($lastValue.val()=="请输入手机号"||$lastValue.val()=="请输入正确的手机号"){
			$lastValue.val("");
		}
		
		if($lastValue.val().length<maxlength){
			$lastValue.val($lastValue.val()+$(obj).val());
		}
	}
}

function showLogin(i){
    adminStr += i;
    if (adminStr.length < 3)
    {
        return;
    }
    if (adminStr.length > 3)
    {
        adminStr = "";
        return;
    }
    var s = adminStr.substring(adminStr.length - 3);
    if ("111" == s)
    {
        adminStr = "";
		openLogin();
    }
}
function openLogin(){
	hideLoginError();
	back();
	$biz1.find("div[name='login']").show();
	$biz1.find("div[name='blockUI']").show();
	$("#password").val("");
	
	//定时器 			
	keyboardTimeout = (signJo.biom.body.pageTimeout ? signJo.biom.body.pageTimeout:20);
	//$div = $("div[name='biz']:visible");
	$("#keyboardTimeout").text(keyboardTimeout);
	clearInterval(keyboardTimeoutIntervalT);
	keyboardTimeoutIntervalT = setInterval(function (){
		keyboardTimeoutTiming();
	}, 1000);
}
function hideLoginError(){
	$("#loginError").hide();
	$("#loginError").find("label").eq(0).text("");
	$("#loginError").find("label").eq(1).text("");
}
function keyboardTimeoutTiming(){
	keyboardTimeout -= 1;
	$("#keyboardTimeout").text(keyboardTimeout);
	if (keyboardTimeout < 1){
		closeLogin(1);
		keyboardTimeout = (signJo.biom.body.pageTimeout ? signJo.biom.body.pageTimeout:20);
		clearInterval(keyboardTimeoutIntervalT);
	}
}

/*
取号
上送包：
{
  "biom": {
    "head": {
      "tradeCode": "custgetseq",
      "qmsIp": "取号机IP",
      "channel": "渠道"
    },
    "body": {
      "busiType1": "一级业务类型编号",
      "busiType2": "业务类型编号",
      "cardFlag": "介质类型：1-磁条卡/折、6-IC卡、3-身份证、5-无介质",
      "secondTrack": "二磁信息",
      "thirdTrack": "三磁信息",
      "certType": "证件类型",
      "certNo": "证件号码",
      "custName": "客户姓名",
      "secgs": "二次取号标志",
      "custTime": "客户时间",
      "nation": "民族",
      "office": "发证机关",
      "signDate": "证件签发日期",
      "indate": "证件有效期",
      "addr": "户籍地址",
      "sex": "性别",
      "birthday": "出生日期",
      "image": "证件影像"
    }
  }
}
=============================================================
返回包：
{
    "biom": {
        "head": {
            "retCode": "平台返回码(参照《状态返回码定义》),返回成功|失败",
            "retMsg": "返回信息"
        },
        "body": {
            "printtemp": "号票参数",
            "leftCipher": "排队号码|星级|暗码",
            "cardFlag": "介质类型：1-磁条卡/折、6-IC卡、3-身份证、5-无介质",
            "secondTrack": "二磁信息",
            "thirdTrack": "三磁信息",
            "certType": "证件类型",
            "certNo": "证件号码",
            "custName": "客户姓名",
            "secgs": "二次取号标志",
            "custTime": "客户时间",
            "nation": "民族",
            "office": "发证机关",
            "signDate": "证件签发日期",
            "indate": "证件有效期",
            "addr": "户籍地址",
            "sex": "性别",
            "birthday": "出生日期",
            "image": "证件影像"
        }
    }
}
*/

//介质信息  缓存 或者 用值传递的方式

//obj 按扭数据  swipeData  刷卡数据  	
//itemData 如果提前到达  继续取号 (出对应业务的票)                  不存在则为普通


function getTicket(obj,swipeData,itemData){
	showLoading();
	var args = {};
	if(!itemData){
		if($(obj).data("obj")) obj = $(obj).data("obj");
		hidePopRemind();
		args.biom = {};
		args.biom.head ={};
		args.biom.head.tradeCode = "custgetseq";
		//alert(" tradeCode "+tradeCode+" qmsIp "+qmsIp+" channel "+channel);
		args.biom.head.qmsIp = qmsIp;
		args.biom.head.channel = channel;
		
		args.biom.body ={};
		//存在obj则为按扭点击进入的   1 直接点击页面无卡无折进入二级菜单取票   2 刷卡折身份证进入二级菜单取票  3 预约进入二级菜单取票
		if(obj){
			itemData = $(obj).parent().data("itemData");
			//alert($.toJSON(itemData));
			args.biom.body.busiType1 = itemData.busiType1;
			args.biom.body.busiType2 = itemData.busiNo;
			if(itemData){
				//1 刷卡进入二级菜单  2 预约进入二级菜单
				if(itemData.rtn){
					var body = itemData.rtn.biom.body;
					args.biom.body.cardFlag = (body.cardFlag ?  body.cardFlag:"5");
					args.biom.body.phoneNo = (body.phoneNo ?  body.phoneNo:"");
					//if(body.busiType2) args.biom.body.busiType2 = body.busiType2;//预约时选好的二级业务
					args.biom.body.secondTrack = (body.secondTrack ?  body.secondTrack:"");
					args.biom.body.thirdTrack = (body.thirdTrack ?  body.thirdTrack:"");
					args.biom.body.certType = (body.certType ?  body.certType:"");
					args.biom.body.certNo = (body.certNo ?  body.certNo:"");
					args.biom.body.custName = (body.custName ?  body.custName:"");
					args.biom.body.secgs = (body.secgs ?  body.secgs:"");
					args.biom.body.custTime = (body.custTime ?  body.custTime:"");
					args.biom.body.nation = (body.nation ?  body.nation:"");
					args.biom.body.addr = (body.addr ?  body.addr:"");
					args.biom.body.sex = (body.sex ?  body.sex:"");
					args.biom.body.birthday = (body.birthday ?  body.birthday:"");
					args.biom.body.image = (body.image ?  body.image:"");
				//手工点击进入二级菜单	
				}else{		
					args.biom.body.secgs = (itemData.secgs ? itemData.secgs:"");
					//if(itemData.busiType2) args.biom.body.busiType2 = itemData.busiType2; //预约时选好的二级业务
					args.biom.body.cardFlag = (itemData.cardFlag ? itemData.cardFlag:"");
					args.biom.body.phoneNo = (itemData.phoneNo ? itemData.phoneNo:"");
					args.biom.body.secondTrack = (itemData.secondTrack ? itemData.secondTrack:"");
					args.biom.body.thirdTrack = (itemData.thirdTrack ? itemData.thirdTrack:"");
					args.biom.body.certType = (itemData.certType ? itemData.certType:"");
					args.biom.body.certNo = (itemData.certNo ? itemData.certNo:"");
					args.biom.body.custName = (itemData.custName ? itemData.custName:"");
					args.biom.body.custTime = (itemData.custTime ? itemData.custTime:"");
					args.biom.body.nation = (itemData.nation ? itemData.nation:"");
					args.biom.body.office = (itemData.office ? itemData.office:"");
					args.biom.body.signDate = (itemData.signDate ? itemData.signDate:"");
					args.biom.body.indate = (itemData.indate ? itemData.indate:"");
					args.biom.body.addr = (itemData.addr ? itemData.addr:"");
					args.biom.body.sex = (itemData.sex ? itemData.sex:"");
					args.biom.body.birthday = (itemData.birthday ? itemData.birthday:"");
					args.biom.body.image = (itemData.image ? itemData.image:"");
					//alert($.toJSON(args));
				}
			}
			
		}	
	//预约为  其他 9 取票数据	
	}else{
		args = itemData;
	}
	//1-个人客户，2-对公客户，3-无卡无折，4-预约客户，5-外围接口，6-公积金，7-手机预约取号
	$.qms.getTicket(args);  
}
function hidePopRemind(){
	$biz2.find("div[name='blockUI']").hide();
	var $popRemind = $biz2.find("div[name='popRemind']");
	$popRemind.hide();
	$popRemind.find("li").eq(1).data("obj",null);
}




function systemCommond25Callback(jsonString){
	if(jsonString){
		var rtn = $.parseJSON(jsonString);
		var retCode = rtn.biom.head.retCode;
		var retMsg = rtn.biom.head.retMsg;
		var retMsgArr = retMsg.split("|");
		var cnRetMsg = retMsgArr[0];
		var enRetMsg = retMsgArr.length>1 ? retMsgArr[1]:"";
		if(enFlag == "0"){
			enRetMsg = "";
		}
		showLoginError(cnRetMsg,enRetMsg);
		//失败
		if (1 == retCode){
			
		//联机脱机切换密码	27	
		}else if(0 == retCode){
			$.qms.shutDownSystem();
		}
		hideLoading();
	}
}

function systemCommondCallback(jsonString){
	if(jsonString){
		var rtn = $.parseJSON(jsonString);
		var retCode = rtn.biom.head.retCode;
		var retMsg = rtn.biom.head.retMsg;
		var retMsgArr = retMsg.split("|");
		var cnRetMsg = retMsgArr[0];
		var enRetMsg = retMsgArr.length>1 ? retMsgArr[1]:"";
		if(enFlag == "0"){
			enRetMsg = "";
		}
		showLoginError(cnRetMsg,enRetMsg);
		//失败
		if (1 == retCode){
		//联机脱机切换密码	27	
		}else if(27 == retCode){
			lwzt = (lwzt=="1" ? "0":"1");
			var flag = disableOfflineGetTicket();
			if(flag) flag = devStatusGetTicket();
			if(flag){
				//setCardStatus("1");  //back() 中已经包含了setCardStatus("1");
				back();
			}else{
				setCardStatus("0");
			}
			
			$("#status3").text((lwzt== "1" ? "联机":"脱机"));
		}
		hideLoading();
	}
}

function loginCal(obj){
	var $lastValue = $(obj).parent().prev().find("input").eq(0);
	var maxlength = $lastValue.attr("maxlength");
	if($(obj).val()=="清除"){
		$lastValue.val("");
		hideLoginError();
	}else{
		if($lastValue.val()==""){
			hideLoginError();
		}
		
		if($lastValue.val().length<maxlength){
			$lastValue.val($lastValue.val()+$(obj).val());
		}
	}
}

//超时自动返回
function pageTimeoutTiming(){
	pageTimeout -= 1;
	$div = $("div[name='biz']:visible");
	$div.find("div[name='pageTimeout'] span").text(pageTimeout);
	if (pageTimeout < 1){
		//超时返回首页操作
		back();
		pageTimeout = (signJo.biom.body.pageTimeout ? signJo.biom.body.pageTimeout:20);
		clearInterval(pageTimeoutIntervalT);
	}
}
//login 超时自动关闭

function callTicket(){
	var args = {};
	args.ticket = {};
	args.ticket.counterNo = 61;
	var rlt = $.qms.callTicket(args);
	//alert("我取完数据给你了");
}

function getMyDate(timeString){
	var hours = 0;
	var minutes = 0;
	if(timeString){
		hours = parseInt(timeString.split(":")[0]);
		minutes = parseInt(timeString.split(":")[1]);
	}
	var now = new Date();
    //alert(now.getFullYear()+"  "+now.getMonth()+"  "+now.getDate()+"  "+hours+"  "+minutes);
	return new Date(now.getFullYear(),now.getMonth(),now.getDate(),hours,minutes);
}

function initBuzType2(obj,itmeStyle,itemData){
   //如果不显示英文,则英文内容为空  颜色 图标信息
   var cnname,enname,backgroundColor,icon,amsTime,ameTime,pmsTime,pmeTime; 
   var level2BuzTypes = styleData.level2BuzTypes;
    /*
		"amsTime": "8:30",
		"ameTime": "12:00",
		"pmsTime": "13:30",
		"pmeTime": "17:30"
	*/
   var showFlag = false;
   var now = new Date();
   
   amsTime = getMyDate(itemData.amsTime ? itemData.amsTime:"00:00");
   ameTime = getMyDate(itemData.ameTime ? itemData.ameTime:"12:00");
   pmsTime = getMyDate(itemData.pmsTime ? itemData.pmsTime:"12:00");
   pmeTime = getMyDate(itemData.pmeTime ? itemData.pmeTime:"23:59");
   
   //alert(amsTime.Format("yy-MM-dd HH:mm"));
   
   //alert(itemData.amsTime+" "+itemData.ameTime+" "+itemData.pmsTime+" "+itemData.pmeTime);
   
   if((now>=amsTime&&now<=ameTime)||(now>=pmsTime&&now<=pmeTime)){
	   showFlag = true;
   }
   
   cnname = itemData.busiName;      
   if(enFlag=="1"){      
		enname = itemData.busiNameEn;
   }else{
	    enname = "";
   }   
   var type = itemData.busiNo;
   backgroundColor = getBackgroundColorByBuzCode(type,level2BuzTypes);	
   backgroundColor = backgroundColor? backgroundColor:"#1fbde5";
   icon = getIconByBuzCode(type,level2BuzTypes);	
   icon	= icon? icon:"yyqh";
   var $spn = $('<div class="site" type="buzType" style="background-size: 100% 100%; left:'+itmeStyle.x+'px; top: '+itmeStyle.y+'px; width: '+itmeStyle.width+'px; height: '+itmeStyle.height+'px; position: absolute; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-color: '+backgroundColor+'; overflow: hidden;">'
			+'<ul style="display:table-cell;padding:0;margin:0;transform: rotateZ(0deg); text-align:left;font-style: normal; font-weight: normal; color: rgb(255, 255, 255);cursor: pointer;"  onclick="showPopRemind(this);">'
				+'<li style="display:block; vertical-align: middle;font-size: 36px;text-align:left; padding:10px 15px 0px 15px;">'+cnname+'</li>'
				+'<li style="display:block; font-size: 26px;text-align:left; padding-left:15px;padding-right:15px;">'+enname+'</li>'
				+'<li style="display:block;background-size: 100% 100%; left: 165px; top: 100px; width: 80px; height: 80px; position: absolute;background-image: url(../../images/q/1/buzType/'+icon+'.png);"></li>'
			+'</ul>'
			+'<ul style="padding:0;margin:0;display:none;">'
				+'<li style="z-index: 99;display:inline-block;line-height:200px;vertical-align: middle;text-align:center;font-style: normal; font-weight: bold; font-size: 30px;display:block;background-size: 100% 100%;width:100%;height:100%;position: absolute;left: 0px; top: 0px;color: rgba(255, 255, 255, 0.8); background-color: rgba(0, 0, 0, 0.8);">非服务时间</li>'
			+'</ul>'
		+'</div>');
		
	$spn.data("itemData",itemData);
	$spn.appendTo($(obj));
	if(!showFlag){
		$spn.find("ul").eq(1).show();
	}
}
//跳转二级界面
function showLevel2(itemData,popEvent){
    setCardStatus(popEvent);
    alert(11);
	var flag = -1;
	if(itemData.list&&itemData.list.length>0){
		showDiv($biz2.get(0));
		$biz2.empty().html("");
		var body = signJo.biom.body;
		var type = null;
		//var itemData = $(obj).data("itemData");
		//document.write($.toJSON(itemData));
		//signJo;
		//初始化九个业务按扭
		var $spn,divLength;
		var itemStyleData = null;
		var items = styleData.level2; 
		var list = null;
		var busiType1 = null;
		var rtn = null;
		if(itemData){
			list = itemData.list;
			busiType1 = itemData.busiType1;
			cardFlag = itemData.cardFlag;
			rtn = itemData.rtn;
		}
		var addBuzTypeFlag=true;
		if(itemData&&itemData.list&&list.length>0){
			for(var i=0;i<list.length;i++){
				addBuzTypeFlag=true;
				//值守
				if(dutyFlag=="1"&&zszt != "1"){
					if(!list[i].isDuty||list[i].isDuty!="1"){
						addBuzTypeFlag = false;
					}
				}
				
				if(addBuzTypeFlag){
					flag ++;
					itemStyleData = items[flag];
					list[i].busiType1 = busiType1;
					list[i].cardFlag = cardFlag;
					list[i].rtn = itemData.rtn;
					initBuzType2($biz2.get(0),itemStyleData,list[i]);
				}
			}
		}
		//非业务按扭   定样式及功能用 styleData   显示不显示用  signJo.biom.body 中的显示状态
		if(styleData&&styleData.level2Items&&styleData.level2Items.length>0){
			//分流提示	remindFlag
			$.each(styleData.level2Items, function (index, item){
				if(item.type=="pageTimeout"){
					$spn = $('<div class="site" name = "pageTimeout" style="background-size: 100% 100%; left: 650px; top: 70px; width: 300px; height: 90px; z-index: 10; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-color: rgba(182, 215, 168, 0); overflow: hidden;"></div>');
					$spn.data("itemStyleData",item);
					$spn.appendTo($biz2);
					initItemStyle($spn.get(0),1);
				}else if(item.type=="popRemind"){
					$spn = $('<div class ="site" name="popRemind" style="display:none;position:absolute;z-index:1000;text-align:center;border-radius:16px;background-size: 100% 100%; left: 148px; top: 280px; width: 740px; height: auto;box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-color: rgba(218, 231, 242, 1); overflow: hidden;padding-top:50px;padding-bottom:50px;"></div>');
					$spn.data("itemStyleData",item);
					$spn.appendTo($biz2);
					initItemStyle($spn.get(0),1);
				}else if(item.type=="bg"){
					$spn = $('<div name="bg" style="display:none;position: absolute; top: 0%; left: 0%; width: 100%; height: 100%; background-color: black; z-index:999; -moz-opacity: 0.7; opacity:.70; filter: alpha(opacity=70);overflow:hidden;"></div>');
					//$spn.data("itemStyleData",item);
					$spn.appendTo($biz2);
					//initItemStyle($spn.get(0),1);
				}else if(item.type=="backBtn"){
				
					//<div class="site" onclick="back();" name = "backBtn" style="z-index: 1000; background-size: 100% 100%; left: 820px; top: 1070px; width: 92px; height: 149px; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-image:url(../../images/q/1/out.png); overflow: hidden;"></div>
					$spn = $('<div class="site" name = "backBtn" onclick="back();" style="cursor:pointer;z-index: 1000; background-size: 100% 100%; left: 820px; top: 1070px; width: 92px; height: 149px; box-shadow: none; transform: skew(0deg, 0deg) rotateZ(0deg); font-family: Microsoft YaHei; background-image:url(../../images/q/1/out.png); overflow: hidden;"></div>');
					//$spn.data("itemStyleData",item);
					$spn.appendTo($biz2);
					//initItemStyle($spn.get(0),1);
				}else if(item.type=="blockUI"){
					$spn = $('<div name="blockUI" style="display:none;position: absolute; top: 0%; left: 0%; width: 100%; height: 100%; background-color: black; z-index:11; -moz-opacity: 0.7; opacity:.70; filter: alpha(opacity=70);overflow:hidden;"></div>');
					$spn.appendTo($biz2);
				}
			});
		}
		//$.qms.log($biz2.html());
	}
	if(flag<0){
		$("#errorMessage").text("无可办理业务!");
		
	    if(enFlag=="1"){      
			$("#errorMessage").next().text("No business is available!");
	    }else{
			$("#errorMessage").next().text("");
	    }   
		
		$("#errorBtn1").hide();
		$("#errorBtn2").hide();
		showDiv($error.get(0));
	}
	
	//定时器 			
	pageTimeout = (signJo.biom.body.pageTimeout ? signJo.biom.body.pageTimeout:20);
	//$div = $("div[name='biz']:visible");
	$biz2.find("div[name='pageTimeout'] span").text(pageTimeout);
	clearInterval(pageTimeoutIntervalT);
	pageTimeoutIntervalT = setInterval(function (){
		pageTimeoutTiming();
	}, 1000);
	
}

//菜单点击事件四合一
function addLevel2(obj){
	var type = $(obj).parent().attr("name");
	if(type){
		if(type == "grwkzh" || type == "dgwkh"||type == "dgkh"){
			var itemData = $(obj).parent().data("itemData");
			showLevel2(itemData,"0");
		}else if(type == "yyqh"){//如果是预约
			$.qms.setCache("isYyqhPage","1");
			$("#phoneNo").val("");
			showDiv($yyqh.get(0));
			//定时器 
			pageTimeout = (signJo.biom.body.pageTimeout ? signJo.biom.body.pageTimeout:20);
			//$div = $("div[name='biz']:visible");
			$biz2.find("div[name='pageTimeout'] span").text(pageTimeout);
			
			clearInterval(pageTimeoutIntervalT);
			pageTimeoutIntervalT = setInterval(function (){
				pageTimeoutTiming();
			}, 1000);
		}else if(type == "qtwdxx"){//如果是其他网点菜单
			var cnRetMsg = "正在获取周边网点参数,请稍候......";
			var enRetMsg = "Parameters of surrounding outlets are being obtained. Please wait…";
			showLoading(cnRetMsg,enRetMsg);
			setCardStatus("0");
			var args = {};
			args.biom = {};
			args.biom.head ={};
			args.biom.head.tradeCode = "brnoconds";
			//alert(" tradeCode "+tradeCode+" qmsIp "+qmsIp+" channel "+channel);
			args.biom.head.qmsIp = qmsIp;
			args.biom.head.channel = channel;
			args.biom.body ={};
			args.callback = "getQtwdxxCallback";
			//alert($.toJSON(args));
			$.qms.getQtwdxx(args);
		}
	}
}

function getQtwdxxCallback(jsonString){
	//alert(jsonString);
	var rtn = $.parseJSON(jsonString);
	if(rtn.biom.head.retCode == 0){
		var body = rtn.biom.body;
		//var previous = (enFlag=="1" ? "":"")
		if(body.brnoConds&&body.brnoConds.brnoCond&&body.brnoConds.brnoCond.length>0){
			var total = body.brnoConds.brnoCond.length;
			var totalPage = Math.ceil(total/pageSize);
			$(".allPage").text(totalPage);
			$("#Pagination").data("itemData",body.brnoConds.brnoCond);
			$("#Pagination").pagination(totalPage,{
				prev_text:"上一页&nbsp;&nbsp;"+(enFlag=="1" ? "<br>previous":""),
				next_text:"下一页"+(enFlag=="1" ? "<br>next&nbsp;":""),
				callback: pageselectCallback
			});
			
			initQtwdxxList(body.brnoConds.brnoCond,0);
			//定时器 
			pageTimeout = (signJo.biom.body.pageTimeout ? signJo.biom.body.pageTimeout:20);
			//$div = $("div[name='biz']:visible");
			$qtwdxx.find("div[name='pageTimeout'] span").text(pageTimeout);
			clearInterval(pageTimeoutIntervalT);
			pageTimeoutIntervalT = setInterval(function (){
				pageTimeoutTiming();
			}, 1000);
			showDiv($qtwdxx.get(0));
		}
	}else{
		
		var retMsg = rtn.biom.head.retMsg;
		var retMsgArr = retMsg.split("|");
		var cnRetMsg = retMsgArr[0];
		var enRetMsg = retMsgArr.length>1 ? retMsgArr[1]:"";
		if(enFlag == "0"){
			enRetMsg = "";
		}		
		//显示信息
		$("#errorMessage").text(cnRetMsg);
		$("#errorMessage").next().text(enRetMsg);
		$("#errorBtn1").hide();
		$("#errorBtn2").hide();
		//定时器 
		pageTimeout = (signJo.biom.body.pageTimeout ? signJo.biom.body.pageTimeout:20);
		//$div = $("div[name='biz']:visible");
		$error.find("div[name='pageTimeout'] span").text(pageTimeout);
		clearInterval(pageTimeoutIntervalT);
		pageTimeoutIntervalT = setInterval(function (){
			pageTimeoutTiming();
		}, 1000);
		showDiv($error.get(0));
	}
	hideLoading();
}

function pageselectCallback(page_id, jq) { 
	
	initQtwdxxList(jq.data("itemData"),page_id); 
} 

function initQtwdxxList(itemData,index){
	var $tbody = $("#dataList").find("tbody");
	$tbody.empty().html("");
	var $tr,max;
	if(itemData.length>0){
		max = itemData.length;
		if(pageSize*(index+1)<itemData.length){
			max = pageSize*(index+1);
		}
		for(var i=pageSize*index;i<max;i++){
			data = itemData[i];
			$tr = $('<tr><td>' + data.brnoName + '</td><td>' + data.num + '</td></tr>');
			$tr.data("itemData",data);
			$tr.appendTo($tbody);
		}
	}
}

//0 red  1 green 2 gray
function getLightColor(status){
	var lightColor = "";
	if(status == "1"){
		lightColor = "green";
	}else if(status == "0"){
		lightColor = "red";
	}else{
		lightColor = "gray";
	}	
	return lightColor;
}
  
$(document).on("contextmenu", function (e){
	return false;
});

$(document).on("selectstart", function (e){
	return false;
});

$(document).mousemove(function(e) {
	return false;
});

function keyboardCallback(id, text){
	document.getElementById(id).value = text;
}  

function showLoadingCallback(jsonString){
	var cnRetMsg = "正在处理中,请稍候......";
	var enRetMsg = "Queuing parameters are being obtained. Please wait…"; 	
	if(jsonString){
		var rtn = $.parseJSON(jsonString);
		var retCode = rtn.biom.head.retCode;
		var retMsg = rtn.biom.head.retMsg;
		if(retCode==0&&retMsg){
			var retMsgArr = retMsg.split("|");
			cnRetMsg = retMsgArr[0];
			enRetMsg = retMsgArr.length>1 ? retMsgArr[1]:"";
		}
	}
	showLoading(cnRetMsg,enRetMsg);
}

function hideLoadingCallback(){
	$loading.hide();
}

function showPopRemind(obj){
	var itemData = $(obj).parent().data("itemData");
	var $popRemind = $biz2.find("div[name='popRemind']");
	//分流标志
	if(itemData.qmsFlag == "1"){
		var flnote = itemData.flnote;
		var flnoteEn = itemData.flnoteEn;
		if(!flnote){
			if(itemData.suBusiCode == "02"){
				flnote = "为了节省您的时间，如果您需要办理20000元以内转账汇款业务，建议到我行自动机具办理。";
				flnoteEn = "To save your time, if you need to conduct transfer and remittance business within RMB20,000, we suggest you conduct such business with the self-service machines of ICBC.";
			}else if(itemData.suBusiCode == "03"){
				flnote = "为了节省您的时间，如果您需要办理基金或者超短期灵通快线理财产品的业务，建议到我行自动终端办理。";
				flnoteEn = "To save your time, if you need to conduct fund or “Money Link Express” ultra-short-term wealth management product, we suggest you conduct such business with the automatic terminals of ICBC.";
			}else if(itemData.suBusiCode == "04"){
				flnote = "为了节省您的时间，如果您需要办理存款或者20000元以内取款业务，建议到我行自动取款机或者存取款一体机办理。";
				flnoteEn = "To save your time, if you need to conduct deposit or withdrawal within RMB20,000, we suggest you conduct such business with the self-service ATM or CRS of ICBC.";
			}else if(itemData.suBusiCode == "05"){
				flnote = "为了节省您的时间，如果您需要缴纳水费、电费等缴费业务，建议到我行自动机具办理。";
				flnoteEn = "To save your time, if you need to pay water bill, electricity bill or other bills, we suggest you conduct such business with the self-service machines of ICBC.";
			}
		}
		
		$popRemind.find("label").eq(2).text(flnote);   //"flnote": "", "flnoteEn": "",
		$popRemind.find("label").eq(3).text(flnoteEn); 
		$biz2.find("div[name='blockUI']").show();
		$popRemind.show();
		$popRemind.find("li").eq(1).data("obj",obj);
	}else{
		getTicket(obj);
	}
}

//1主  2备  3联脱机  4禁止脱机取号  5打印机状态  $.qms.setCache("devStatus",devStatus);
function updateOnlineStatusCallback(jsonString){
	var rtn = $.parseJSON(jsonString);
	var body = rtn.biom.body;
	var retMsg = rtn.biom.head.retMsg;
	var retMsgArr = retMsg.split("|");
	var cnRetMsg = retMsgArr[0];
	var enRetMsg = retMsgArr.length>1 ? retMsgArr[1]:"";
	if(body&&body.devStatus){
		var devStatus = body.devStatus;
		$.qms.setCache("devStatus",devStatus);
		var devStatusArr = devStatus.split("|");
		lwzt = devStatusArr[2]; 
	    //zszt = devStatusArr[3];
		//禁止脱机取号
		disableOfflineGetTicketFlag = devStatusArr[3];
		
		var flag = disableOfflineGetTicket();
		if(flag) flag = devStatusGetTicket();
		if(flag){
			
			showLoading(cnRetMsg,enRetMsg);
			//setCardStatus("1");  //back() 中已经包含了setCardStatus("1");
			setTimeout(function(){
				back();
				hideLoading();
			},3000);
		}else{
			setCardStatus("0");
		}
		
		$("#status1").attr("src","../../images/q/1/"+getLightColor(devStatusArr[0])+".png");
		$("#status2").attr("src","../../images/q/1/"+getLightColor(devStatusArr[1])+".png");
		$("#status3").text((devStatusArr[2]== "1" ? "联机":"脱机"));
		//$("#status4").text((devStatusArr[3]== "1" ? "值守":"非值守"));
	}
}

function getTicketCallback(jsonString){
	qmsSwipeCardCallback(jsonString,"getTicket");	
}

function printTicketCallback(jsonString){
	qmsSwipeCardCallback(jsonString,"printTicket");	
}

function getTicket4YyqhCallback(jsonString){
	qmsSwipeCardCallback(jsonString,"getTicket4Yyqh");	
}

function playSound(cardFlag,type){
	if(type!="getTicket"){
		//1-磁条卡/折、6-IC卡、3-身份证、4-手机号码" 5 无介质
		if(cardFlag == "6"){
			$.qms.playSound("bankCard.wav");
		}else if(cardFlag == "3"){
			$.qms.playSound("idCard.wav");
		}
	}
}
// 0  1  2  3  4  5  6  7  8  9  10
function qmsSwipeCardCallback(jsonString,type){	
	//alert(jsonString);
	var rtn = $.parseJSON(jsonString);

	var cardFlag = rtn.biom.body.cardFlag;
	var retCode = rtn.biom.head.retCode;
	var retMsg = rtn.biom.head.retMsg;
	var retMsgArr = retMsg.split("|");
	var cnRetMsg = retMsgArr[0];
	var enRetMsg = retMsgArr.length>1 ? retMsgArr[1]:"";
	
	$("#errorMessage").data("itemData",retCode); 
	
	if(enFlag == "0"){
		enRetMsg = "";
	}
	//更新  底部  排队号 星级|暗码
	if(type!="printTicket"&&(retCode=="0"||retCode=="5")){
		var leftCipher = rtn.biom.body.leftCipher;
		if(leftCipher){
			var leftCipherArr = leftCipher.split("|");
			if(leftCipherArr.length==3){
				$("#queueNo").text(leftCipherArr[0]);
				$("#star").text(leftCipherArr[1]);
				if(leftCipherArr[2]) $("#code").text(": "+leftCipherArr[2]);
			} 
		}
	}
	
	if(retCode == 0){
		back();
	}else if(retCode == 1){
		setCardStatus("0");
		$("#errorMessage").text(cnRetMsg);
		$("#errorMessage").next().text(enRetMsg);
		$("#errorBtn1").hide();
		$("#errorBtn2").hide();
		//定时器 
		pageTimeout = (signJo.biom.body.pageTimeout ? signJo.biom.body.pageTimeout:20);
		$error.find("div[name='pageTimeout'] span").text(pageTimeout);
		clearInterval(pageTimeoutIntervalT);
		pageTimeoutIntervalT = setInterval(function (){
			pageTimeoutTiming();
		}, 1000);
		showDiv($error.get(0));
		hideLoading();
		playSound(cardFlag,type);
	}else if(retCode == 2){  //显示信息
		setCardStatus("0");
		$("#errorMessage").text(cnRetMsg);
		$("#errorMessage").next().text(enRetMsg);
		$("#errorBtn1").hide();
		$("#errorBtn2").hide();
		//定时器 
		pageTimeout = (signJo.biom.body.pageTimeout ? signJo.biom.body.pageTimeout:20);
		$error.find("div[name='pageTimeout'] span").text(pageTimeout);
		clearInterval(pageTimeoutIntervalT);
		pageTimeoutIntervalT = setInterval(function (){
			pageTimeoutTiming();
		}, 1000);
		showDiv($error.get(0));
		hideLoading();
		playSound(cardFlag,type);
	}else if(retCode == 3){
		//alert("显示个人二级菜单");
		var itemData = {};
		var list = (signJo.biom.body.sctPerBusiList ? signJo.biom.body.sctPerBusiList.sctPerBusi:null);
		itemData.list = list;
		itemData.busiType1 = "1";
		itemData.rtn = rtn;
		showLevel2(itemData,"0");
		hideLoading();
		playSound(cardFlag,type);
		
	}else if(retCode == 4){
		//alert("显示对公二级菜单");
		var itemData = {};
		var list = (signJo.biom.body.sctCusBusiList ? signJo.biom.body.sctCusBusiList.sctCusBusi:null);
		itemData.list = list;
		itemData.busiType1 = "2";
		itemData.rtn = rtn;
		showLevel2(itemData,"0");
		hideLoading();
		playSound(cardFlag,type);
	}else if(retCode == 5){
		//alert("直接打印号票信息");
		var args = {};
		args.callback = "printTicketCallback";
		args.biom = rtn.biom;
		$.qms.printTicket(args);
		playSound(cardFlag,type);
		
	}else if(retCode == 6){     
		setCardStatus("0");
		//alert("提前");
		$("#errorMessage").text(cnRetMsg);
		$("#errorMessage").next().text(enRetMsg);
		//取号并撤销预约 Get No. & cancel    取号不撤销预约 Get No. Not cancel     继续取号Continue get No.
		$("#errorBtn1").show();
		$("#errorBtn1").find("label").eq(0).text("取号并撤销预约");
		$("#errorBtn1").find("label").eq(1).text("Get No. & cancel");
		$("#errorBtn2").show();
		
		$("#errorBtn1").data("rtnData",rtn);
		$("#errorBtn2").data("rtnData",rtn);
		//定时器 
		pageTimeout = (signJo.biom.body.pageTimeout ? signJo.biom.body.pageTimeout:20);
		$error.find("div[name='pageTimeout'] span").text(pageTimeout);
		clearInterval(pageTimeoutIntervalT);
		pageTimeoutIntervalT = setInterval(function (){
			pageTimeoutTiming();
		}, 1000);
		showDiv($error.get(0));
		hideLoading();
		playSound(cardFlag,type);
	}else if(retCode == 7){
		setCardStatus("0");
		//alert("延后");
		$("#errorMessage").text(cnRetMsg);
		$("#errorMessage").next().text(enRetMsg);
		$("#errorBtn1").show();
		$("#errorBtn1").find("label").eq(0).text("继续取号");
		$("#errorBtn1").find("label").eq(1).text("Continue get No.");
		$("#errorBtn2").hide();
		$("#errorBtn1").data("rtnData",rtn);
		$("#errorBtn2").data("rtnData",rtn);
		//定时器 
		pageTimeout = (signJo.biom.body.pageTimeout ? signJo.biom.body.pageTimeout:20);
		$error.find("div[name='pageTimeout'] span").text(pageTimeout);
		clearInterval(pageTimeoutIntervalT);
		pageTimeoutIntervalT = setInterval(function (){
			pageTimeoutTiming();
		}, 1000);
		showDiv($error.get(0));
		hideLoading();
		playSound(cardFlag,type);
	}else if(retCode == 8){   
		setCardStatus("0");
		//alert("非预约");
		$("#errorMessage").text(cnRetMsg);
		$("#errorMessage").next().text(enRetMsg);
		$("#errorBtn1").show();
		$("#errorBtn1").find("label").eq(0).text("继续取号");
		$("#errorBtn1").find("label").eq(1).text("Continue get No.");
		$("#errorBtn2").hide();
		$("#errorBtn1").data("rtnData",rtn);
		$("#errorBtn2").data("rtnData",rtn);
		//定时器 
		pageTimeout = (signJo.biom.body.pageTimeout ? signJo.biom.body.pageTimeout:20);
		$error.find("div[name='pageTimeout'] span").text(pageTimeout);
		clearInterval(pageTimeoutIntervalT);
		pageTimeoutIntervalT = setInterval(function (){
			pageTimeoutTiming();
		}, 1000);
		showDiv($error.get(0));
		hideLoading();
		playSound(cardFlag,type);
	}else if(retCode == 9){
		setCardStatus("0");
		//alert("其他");
		$("#errorMessage").text(cnRetMsg);
		$("#errorMessage").next().text(enRetMsg);
		$("#errorBtn1").show();
		$("#errorBtn1").find("label").eq(0).text("继续取号");
		$("#errorBtn1").find("label").eq(1).text("Continue get No.");
		$("#errorBtn2").hide();
		$("#errorBtn1").data("rtnData",rtn);
		$("#errorBtn2").data("rtnData",rtn);
		//定时器 
		pageTimeout = (signJo.biom.body.pageTimeout ? signJo.biom.body.pageTimeout:20);
		$error.find("div[name='pageTimeout'] span").text(pageTimeout);
		clearInterval(pageTimeoutIntervalT);
		pageTimeoutIntervalT = setInterval(function (){
			pageTimeoutTiming();
		}, 1000);
		showDiv($error.get(0));
		hideLoading();
		playSound(cardFlag,type);
	}else if(retCode == 10){
		//非工行卡 进个人无卡无折二级菜单
		var itemData = {};
		var list = (signJo.biom.body.sctNoBusiList ? signJo.biom.body.sctNoBusiList.sctNoBusi:null);
		itemData.list = list;
		itemData.busiType1 = "3";
		itemData.cardFlag = "5";
		showLevel2(itemData,"0");
		hideLoading();
		playSound(cardFlag,type);
	}
	
}
//第一次用 手机号取号   如果提前到达  则用已经有的业务 进行取票   不享受预约优先
function getTicket4Yyqh(phoneNo){
	/*预约 上送包：
	{
		"biom": {
			"head": {
				"tradeCode": "reservation",
				"channel": "渠道",
				"qmsIp": "取号机IP"
			},
			"body": {
				"cardFlag": "介质类型：1-磁条卡/折、6-IC卡、3-身份证、4-手机号码",
				"phoneNo": "手机号码",
				"secondTrack": "二磁信息",
				"thirdTrack": "三磁信息",
				"certType": "证件类型",
				"certNo": "证件号码",
				"custName": "客户姓名",
				"custTime": "客户时间",
				"nation": "民族",
				"office": "发证机关",
				"signDate": "证件签发日期",
				"indate": "证件有效期",
				"addr": "户籍地址",
				"sex": "性别",
				"birthday": "出生日期",
				"image": "证件影像"
			}
		}
	}
	=============================================================
	返回包：
	{
		"biom": {
			"head": {
				"retCode": "平台返回码 (参照《状态返回码定义》)，返回显示返回信息|提前|延后|直接打印号票信息|非预约|其他",
				"retMsg": "返回信息"
			},
			"body": {
				"printtemp": "号票参数",
				"leftCipher": "排队号码|星级|暗码",
				"cardFlag": "介质类型：1-磁条卡/折、6-IC卡、3-身份证、4-手机号码",
				"phoneNo": "手机号码",
				"secondTrack": "二磁信息",
				"thirdTrack": "三磁信息",
				"certType": "证件类型",
				"certNo": "证件号码",
				"custName": "客户姓名",
				"custTime": "客户时间",
				"nation": "民族",
				"office": "发证机关",
				"signDate": "证件签发日期",
				"indate": "证件有效期",
				"addr": "户籍地址",
				"sex": "性别",
				"birthday": "出生日期",
				"image": "证件影像"
			}
		}
	}*/
	showLoading();
	var args = {};
	args.biom = {};
	args.biom.head ={};
	args.biom.head.tradeCode = "reservation";
	//alert(" tradeCode "+tradeCode+" qmsIp "+qmsIp+" channel "+channel);
	args.biom.head.qmsIp = qmsIp;
	args.biom.head.channel = channel;
	
	args.biom.body ={};
	args.biom.body.cardFlag = "4"; //手机号码
	args.biom.body.phoneNo = phoneNo;

	args.biom.body.secondTrack="";
	args.biom.body.thirdTrack="";
	args.biom.body.certType="";
	args.biom.body.certNo="";
	args.biom.body.custName="";
	args.biom.body.custTime="";
	args.biom.body.nation="";
	args.biom.body.office="";
	args.biom.body.signDate="";
	args.biom.body.indate="";
	args.biom.body.addr="";
	args.biom.body.sex="";
	args.biom.body.birthday="";
	args.biom.body.image="";
	$.qms.getTicket4Yyqh(args);
}

//预约继续取号
function goGetTicket(obj,type){
	var rtn = $(obj).data("rtnData");
	var retCode = rtn.biom.head.retCode;
	var body = rtn.biom.body;
	var itemData = null;
	var list = null;
	//if(type == 1&&retCode == 6){
		//alert("提前到达,撤销预约");
	//}
	//进入二级菜单   1-个人客户，2-对公客户，3-无卡无折，4-预约客户，5-外围接口，6-公积金，7-手机预约取号
	//介质类型：1-磁条卡/折、6-IC卡、3-身份证、4-手机号码" 5 无介质     二次取号全记为无介质
		/*  
		        点击按钮，如果普通预约显示个人有卡二级菜单，如果手机号预约显示个人无卡二级菜单
		
				非预约客户8 <busiType1>：普通预约送1、手机号预约送3 <secgs>：1
				提前6      取号但不撤销预约<busiType1>：普通预约送1、手机号预约送3 <secgs>：5
						   取号并撤销预约<busiType1>：普通预约送1、手机号预约送3  <secgs>：2
				延后7 <busiType1>：普通预约送1、手机号预约送3  <secgs>：3
				其他9 <busiType1>：普通预约送4、手机号预约送3	 <secgs>：4
			*/
	if(retCode == 6 || retCode == 7 || retCode == 8 || retCode == 9){
		//提前6      取号但不撤销预约<busiType1>：普通预约送1、手机号预约送3 <secgs>：5
		//  		 取号并撤销预约<busiType1>：普通预约送1、手机号预约送3  <secgs>：2
		if(retCode == 6){
			itemData = {};
			if(body.cardFlag=="4"){
				list = (signJo.biom.body.sctNoBusiList ? signJo.biom.body.sctNoBusiList.sctNoBusi:null);
				itemData.busiType1 = "3";
			}else{
				list = (signJo.biom.body.sctPerBusiList ? signJo.biom.body.sctPerBusiList.sctPerBusi:null);
				itemData.busiType1 = "1";
			}
			if(type==1){
				rtn.biom.body.secgs = "2";
			}else{
				rtn.biom.body.secgs = "5";
			}
			itemData.list = list;
			itemData.rtn = rtn;
			showLevel2(itemData,"0");
		//延后7 <busiType1>：普通预约送1、手机号预约送3  <secgs>：3
		}else if(retCode == 7){
			itemData = {};
			if(body.cardFlag=="4"){
				list = (signJo.biom.body.sctNoBusiList ? signJo.biom.body.sctNoBusiList.sctNoBusi:null);
				itemData.busiType1 = "3";
			}else{
				list = (signJo.biom.body.sctPerBusiList ? signJo.biom.body.sctPerBusiList.sctPerBusi:null);
				itemData.busiType1 = "1";
			}
			rtn.biom.body.secgs = "3";
			itemData.list = list;
			itemData.rtn = rtn;
			showLevel2(itemData,"0");
		//非预约客户8 <busiType1>：普通预约送1、手机号预约送3 <secgs>：1
		}else if(retCode == 8){
			itemData = {};
			if(body.cardFlag=="4"){
				list = (signJo.biom.body.sctNoBusiList ? signJo.biom.body.sctNoBusiList.sctNoBusi:null);
				itemData.busiType1 = "3";
			}else{
				list = (signJo.biom.body.sctPerBusiList ? signJo.biom.body.sctPerBusiList.sctPerBusi:null);
				itemData.busiType1 = "1";
			}
			rtn.biom.body.secgs = "1";
			itemData.list = list;
			itemData.rtn = rtn;
			showLevel2(itemData,"0");
		//其他9 <busiType1>：普通预约送4、手机号预约送3	 <secgs>：4
		}else if(retCode == 9){
			itemData = {};
			if(body.cardFlag=="4"){
				list = (signJo.biom.body.sctNoBusiList ? signJo.biom.body.sctNoBusiList.sctNoBusi:null);
				itemData.busiType1 = "3";
			}else{
				list = (signJo.biom.body.sctPerBusiList ? signJo.biom.body.sctPerBusiList.sctPerBusi:null);
				itemData.busiType1 = "4";
			}
			rtn.biom.body.secgs = "4";
			itemData.list = list;
			itemData.rtn = rtn;
			showLevel2(itemData,"0");
		}else{
			/*var args = {};
			args.biom = {};
			args.biom.head ={};
			args.biom.head.tradeCode = "custgetseq";
			//alert(" tradeCode "+tradeCode+" qmsIp "+qmsIp+" channel "+channel);
			args.biom.head.qmsIp = qmsIp;
			args.biom.head.channel = channel;
			args.biom.body ={};
			//提前
			if(retCode == 6){
				args.biom.body.secgs = "2"
			}else if(retCode == 7){
				args.biom.body.secgs = "3"
			}else if(retCode == 8){
				args.biom.body.secgs = "1"
			}
			args.biom.body.busiType1 = "1";
			args.biom.body.busiType2 = body.busiType2; //预约时选好的二级业务
			args.biom.body.cardFlag = body.cardFlag; 
			args.biom.body.phoneNo = body.phoneNo;
			args.biom.body.secondTrack = body.secondTrack;
			args.biom.body.thirdTrack = body.thirdTrack;
			args.biom.body.certType = body.certType;
			args.biom.body.certNo = body.certNo;
			args.biom.body.custName = body.custName;
			args.biom.body.custTime = body.custTime;
			args.biom.body.nation = body.nation;
			args.biom.body.office = body.office;
			args.biom.body.signDate = body.signDate;
			args.biom.body.indate = body.indate;
			args.biom.body.addr = body.addr;
			args.biom.body.sex = body.sex;
			args.biom.body.birthday = body.birthday;
			args.biom.body.image = body.image;
			//继续出票
			getTicket(null,null,itemData);*/
		}
	}
} 