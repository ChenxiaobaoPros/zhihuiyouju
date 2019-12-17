$.icbcQms = 
{
	/**
	 * 连接测试
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	getPinLisService: function (args)
    {
        window.external.PluginInvoke("icbcspecialuseService", "IcbcspecialusePing2JS", $.toJSON(args));
    },
    /**
	 * 重启叫号机
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	boxRestartService: function (args)
    {
        window.external.PluginInvoke("boxRestartService", "BoxRestart2JS", $.toJSON(args));
    },
    /**
	 * 叫号机日志下载
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	getLogQuery2JS: function (args)
    {
        window.external.PluginInvoke("logService", "SelectLog2JS", $.toJSON(args));
    },
	/**
	 * 本地卡BIN星级对照[增(command=31)、删(command=32)、改(command=33)、查（command=34）]
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	localcardbin2JS: function (args)
    {
        window.external.PluginInvoke("localcardbinService", "Localcardbin2JS", $.toJSON(args));
    },	
	/**
	 * 本地卡类型星级对照[增(command=31)、删(command=32)、改(command=33)、查（command=34）]
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	localcardtype2JS: function (args)
    {
        window.external.PluginInvoke("localcardtypeService", "Localcardtype2JS", $.toJSON(args));
    },	
    /**
	 * 远程卡BIN星级对照[增(command=31)、删(command=32)、改(command=33)、查（command=34）]
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	biomcardbin2JS: function (args)
    {
        window.external.PluginInvoke("localcardbinService", "Biomqmscardbin2JS", $.toJSON(args));
    },	
	/**
	 * 基础设置查询（command=34）/更新(command=33)
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	getBasicconfig2JS: function (args)
    {
      	//alert($.toJSON(args));  	
    	window.external.PluginInvoke("basicconfigService", "GetBasicconfig2JS", $.toJSON(args));
    },		
	/**
	 * 工商银行专用查询（command=34）/更新(command=33)
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	getIcbcspecialuse: function (args)
    {
        window.external.PluginInvoke("icbcspecialuseService", "Icbcspecialuse2JS", $.toJSON(args));
    },
	/**
	 * 号票配置查询（command=34）/更新(command=33)
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	getTicketsconfig: function (args)
    {
        window.external.PluginInvoke("ticketsconfigService", "Ticketsconfig2JS", $.toJSON(args));
    },	
	/**
	 * 更新配置查询（command=34）/更新(command=33)
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	updateconfig2JS: function (args)
    {
        window.external.PluginInvoke("updateconfigService", "Updateconfig2JS", $.toJSON(args));
    },	
	/**
	 * 语音配置查询（command=34）/更新(command=33)
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	voiceconfig2JS: function (args)
    {
        window.external.PluginInvoke("voiceconfigService", "Voiceconfig2JS", $.toJSON(args));
    },		
	/**
	 * 获取关于信息（command=34）
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	about2JS: function (args)
    {
        window.external.PluginInvoke("aboutService", "About2JS", $.toJSON(args));
    },	
	/**
	 * 营业厅分区
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	businesshall2JS: function (args)
    {
        window.external.PluginInvoke("businesshallService", "Businesshall2JS", $.toJSON(args));
    },				
	/**
	 * 柜台配置
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	counterconfig2JS: function (args)
    {
        window.external.PluginInvoke("counterconfigService", "Counterconfig2JS", $.toJSON(args));
    },	
	/**
	 * 业务查询
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	servicequery2JS: function (args)
    {
        window.external.PluginInvoke("servicequeryService", "Servicequery2JS", $.toJSON(args));
    },	
	/**
	 * 队列查询
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	queuequery2JS: function (args)
    {
        window.external.PluginInvoke("queuequeryService", "Queuequery2JS", $.toJSON(args));
    },
	/**
	 * 本地号码查询
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	localNumberQuery2JS: function (args)
    {
        window.external.PluginInvoke("localNumberQueryService", "LocalNumberQuery2JS", $.toJSON(args));
    },	
	/**
	 * 删废号
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	deleteabolishNumber: function (args)
    {
        window.external.PluginInvoke("deleteabolishNumberService", "DeleteabolishNumber2JS", $.toJSON(args));
    },
	/**
	 * 窗口队列调整
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	windowQueueAdjust2JS: function (args)
    {
       window.external.PluginInvoke("counterconfigService", "WindowQueueAdjust2JS", $.toJSON(args));
    },	
	/**
	 * 队列属性更新
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	queueAttributes2JS: function (args)
    {
        window.external.PluginInvoke("queuequeryService", "QueueAttributes2JS", $.toJSON(args));
    },
	/**
	 * 模块日志查询
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	getLogquery: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("XXXService", "GetLogquery", $.toJSON(args)));
    },    
	/**
	 * 联动关机
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	linkageshutdown: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("XXXService", "Linkageshutdown", $.toJSON(args)));
    },	
    /**
	 * 设备管理查询（command=34）/更新(command=33)
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	getQcmConfig2JS: function (args)
    {
      	//alert($.toJSON(args));  	
    	window.external.PluginInvoke("qcmConfigService", "GetQcmConfig2JS", $.toJSON(args));
    },	
    /**
	 * usb（command=34）/更新(command=33)
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	usb2JS: function (args)
    {
      	//alert($.toJSON(args));  	
    	window.external.PluginInvoke("usbService", "Usb2JS", $.toJSON(args));
    },
	/**
	 * 设备状态更新
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	stateUpdate: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("XXXService", "StateUpdate", $.toJSON(args)));
    }
	/**
	 * 联脱机状态接口(待定)
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	

};

$.qms = 
{	
	/*
   	* 取号机重启
   	*/
	qmsRestart:function (){
		window.external.QmsRestart();
	},
	/*
   	* 取号机设置查询
   	*/
	selectWebServer:function (){
		return window.external.SelectWebServer();
	},
	/*
   	* 取号机设置更新
   	*/
	updateWebServer:function (args){
		return window.external.UpdateWebServer(args);
	},
	/*
   	* 查看取号机日志
   	*/
	openQmsLog:function (){
		window.external.OpenQsmLog();
	},
	/*
   	* 打开叫号机日志
   	*/
	openBoxLog:function (){
		window.external.openBoxLog();
	},
	printTest:function (str){
		window.external.PrintTest(str);
	},
	/**
	 * 号票配置查询（command=34）/更新(command=33)
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */	
	getTicketsconfig: function (args)
    {
        window.external.PluginInvoke("ticketsconfigService", "Ticketsconfig2JS", $.toJSON(args));
    },	
	playSound: function (wav){
        window.external.PlaySound2JS(wav);
    },
	shutDownSystem: function(args){
        window.external.ShutDownSystem();
	},
	openCard: function(){
        window.external.ReadRawData();
	},
	closeCard: function(){
        window.external.CancelReadRawData();
	},
	getQtwdxx: function(args){
		window.external.PluginInvoke("brnocondsService", "Brnoconds2JS", $.toJSON(args));
	},
	log: function (str)
    {
        window.external.Log(str);
    },
	systemCommond: function(password){
        return window.external.SystemCommond(password);
	},
	setPopEvent: function (flag)
    {
        window.external.SetPopEvent(flag);
    },
	printTicket: function (args)
    {
		//alert($.toJSON(args));
		window.external.Print2JS($.toJSON(args));
    },	
	getTicket: function (args)
    {
		args.callback="getTicketCallback";
		window.external.PluginInvoke("custgetseqService", "Custgetseq2JS", $.toJSON(args));
    },	
	getTicket4Yyqh: function (args)
    {
		args.callback="qmsSwipeCardCallback";
		//alert($.toJSON(args));
		window.external.PluginInvoke("reservationService", "Reservation2JS", $.toJSON(args));
    },
	getQueueTemplateJsonFile: function (args)
    {	
		return window.external.GetQueueTemplateJsonFile($.toJSON(args));
    },
	setCache: function (key, value) {
        window.external.SetCache(key, value);
    },
    getCache: function (key) {
        return window.external.GetCache(key);
    },
	/**
	 * 签到（发送签到请求，返回签到信息）
	 * @param args 请求JSON
	 * @return 响应JSON（壳程序调用callback方法）
	 */
	qmsSign: function (args)
    {
		args.callback="qmsSignCallback";
        window.external.PluginInvoke("qmssignService", "GetQmssign2JS", $.toJSON(args));
    },	
	createTktFormat: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("ticketTemplateService", "CreateTktFormat", $.toJSON(args)));
    },
	
	updateTktFormat: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("ticketTemplateService", "UpdateTktFormat", $.toJSON(args)));
    },
	
	removeTktFormat: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("ticketTemplateService", "RemoveTktFormat", $.toJSON(args)));
    },
	
	getTktFormat: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("ticketTemplateService", "GetTktFormat", $.toJSON(args)));
    },
	
	getTktFormats: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("ticketTemplateService", "GetTktFormats", $.toJSON(args)));
    },
	getBackgroundImgs: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("queueTemplateService", "GetBackgroundImgs", $.toJSON(args)));
    },
	getMediaList: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("queueTemplateService", "GetMediaList", $.toJSON(args)));
    },	
	getCouponImgs: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("queueTemplateService", "GetCouponImgs", $.toJSON(args)));
    },
	getSettingsByType: function(args){
		return $.parseJSON(window.external.PluginInvoke("settingService", "GetSettingsByType", $.toJSON(args)));
	},
	
	createTicketTemplate: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("ticketTemplateService", "CreateTicketTemplate", $.toJSON(args)));
    },
	
	updateTicketTemplate: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("ticketTemplateService", "UpdateTicketTemplate", $.toJSON(args)));
    },
	
	selectCustLevel4DeleteTicketTemplate: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("ticketTemplateService", "SelectCustLevel4DeleteTicketTemplate", $.toJSON(args)));
    },
	
	removeTicketTemplate: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("ticketTemplateService", "RemoveTicketTemplate", $.toJSON(args)));
    },
	
	getTicketTemplate: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("ticketTemplateService", "GetTicketTemplate", $.toJSON(args)));
    },
	
	getTicketTemplates: function ()
    {
        var args = {};
        return $.parseJSON(window.external.PluginInvoke("ticketTemplateService", "GetTicketTemplates", $.toJSON(args)));
    },
	
	saveTicketTemplateJsonFile: function (jo)
    {
        return $.parseJSON(window.external.PluginInvoke("ticketTemplateService", "SaveTicketTemplateJsonFile", $.toJSON(jo)));
    },
	
	getTicketTemplateJsonFile: function (jo)
    {
        return $.parseJSON(window.external.PluginInvoke("ticketTemplateService", "GetTicketTemplateJsonFile", $.toJSON(jo)));
    },
	
    createTeller: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("tellerService", "CreateTeller", $.toJSON(args)));
    },
	
    getTeller: function (rowid)
    {
        var args = {};
        args.teller = {};
        args.teller.rowid = rowid;
        return $.parseJSON(window.external.PluginInvoke("tellerService", "GetTeller", $.toJSON(args)));
    },
//------------------------------------------窗口------------------------------------------------------
    saveCounters: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("counterService", "SaveCounters", $.toJSON(args)));
    },
	createCounter: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("counterService", "CreateCounter", $.toJSON(args)));
    },
	
	updateCounter: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("counterService", "UpdateCounter", $.toJSON(args)));
    },

	removeCounter: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("counterService", "RemoveCounter", $.toJSON(args)));
    },
	
	getCounter: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("counterService", "GetCounter", $.toJSON(args)));
    },
	
	getCounters: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("counterService", "GetCounters", $.toJSON(args)));
    },
//------------------------------------------客户等级------------------------------------------------------	
     createCustLevel: function (jo)
    {
        var args = {};
        args.custLevel = jo;
        return $.parseJSON(window.external.PluginInvoke("counterService", "CreateCustLevel", $.toJSON(args)));
    },
	
	updateCustLevel: function (jo)
    {
        var args = {};
        args.custLevel = jo;
        return $.parseJSON(window.external.PluginInvoke("counterService", "UpdateCustLevel", $.toJSON(args)));
    },

	removeCounter: function (rowid)
    {
        var args = {};
        args.custLevel.rowid = rowid;
        return $.parseJSON(window.external.PluginInvoke("counterService", "RemoveCustLevel", $.toJSON(args)));
    },
	
	getCustLevel: function (rowid)
    {
        var args = {};
        args.custLevel.rowid = rowid;
        return $.parseJSON(window.external.PluginInvoke("counterService", "GetCustLevel", $.toJSON(args)));
    },
	
	getCustLevels: function ()
    {
        var args = {};
        return $.parseJSON(window.external.PluginInvoke("counterService", "GetCustLevels", $.toJSON(args)));
    },
//------------------------------------------业务类型------------------------------------------------------
   createBuzType: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "CreateBuzType", $.toJSON(args)));
    },
	
	updateBuzType: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "UpdateBuzType", $.toJSON(args)));
    },
	
	moveBuzType: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "MoveBuzType", $.toJSON(args)));
    },
	
	selectBuzTypeRef4Delete: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "SelectBuzTypeRef4Delete", $.toJSON(args)));
    },

	removeBuzType: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "RemoveBuzType", $.toJSON(args)));
    },
	
	getBuzTypesForTree: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "GetBuzTypesForTree", $.toJSON(args)));
    },
	
	getBuzTypesForTreeByDateTypeId: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "GetBuzTypesForTreeByDateTypeId", $.toJSON(args)));
    },
	
	getLeafBuzTypes : function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "GetLeafBuzTypes", $.toJSON(args)));
    },	
	
	getBuzTypeCount: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "GetBuzTypeCount", $.toJSON(args)));
    },
	getBuzTypes: function ()
    {
		var args = {};
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "GetBuzTypes", $.toJSON(args)));
    },
	
	printCoupon: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "PrintCoupon", $.toJSON(args)));
    },
//------------------------------------------业务显示时间------------------------------------------------------
    createBuzTime: function (jo)
    {
        var args = {};
        args.buzTime = jo;
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "CreateBuzTime", $.toJSON(args)));
    },
	
	updateBuzTime: function (jo)
    {
        var args = {};
        args.buzTime = jo;
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "UpdateBuzTime", $.toJSON(args)));
    },

	removeBuzTime: function (rowid)
    {
        var args = {};
        args.buzTime.rowid = rowid;
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "RemoveBuzTime", $.toJSON(args)));
    },

	getBuzTimes: function (buzType,dateType)
    {
        var args = {};
        args.buzTime.buzType = buzType;
        args.buzTime.dateType = dateType;
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "GetBuzTimes", $.toJSON(args)));
    },	
//------------------------------------------窗口业务------------------------------------------------------
    createCounterBuz: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("counterService", "CreateCounterBuz", $.toJSON(args)));
    },
	

	removeBuzTime: function ()
    {
        var args = {};
        return $.parseJSON(window.external.PluginInvoke("counterService", "RemoveAllCounterBuz", $.toJSON(args)));
    },
	
	getCounterBuzsByCounterId: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("counterService", "GetCounterBuzsByCounterId", $.toJSON(args)));
    },
	
	getMrCounterBuzs: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("counterService", "GetMrCounterBuzs", $.toJSON(args)));
    },
	
	saveCounterBuzs: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("counterService", "SaveCounterBuzs", $.toJSON(args)));
    },
	
	

//--------------------------------客户级别-------------------------------
	getCustLevels: function ()
    {
        var args = {};
        return $.parseJSON(window.external.PluginInvoke("customerService", "GetCustLevels", $.toJSON(args)));
    },
	saveCustLevels: function (args)
	{
        return $.parseJSON(window.external.PluginInvoke("customerService", "SaveCustLevels", $.toJSON(args)));
	},
//--------------------------------特殊日期-------------------------------
	getSpecialDates: function ()
    {
        var args = {};
        return $.parseJSON(window.external.PluginInvoke("settingService", "GetSpecialDates", $.toJSON(args)));
    },	

	saveSpecialDates:function(args)
	{
        return $.parseJSON(window.external.PluginInvoke("settingService", "SaveSpecialDates", $.toJSON(args)));
	},
//--------------------------------柜员设置-------------------------------
	getTellers: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("tellerService", "GetTellers", $.toJSON(args)));
    },
	
	saveTellers: function (args)
	{
        return $.parseJSON(window.external.PluginInvoke("tellerService", "SaveTellers", $.toJSON(args)));
	},
//--------------------------------特殊客户-------------------------------
	getSpecialCusts: function ()
    {
		var args = {};
        return $.parseJSON(window.external.PluginInvoke("customerService", "GetSpecialCusts", $.toJSON(args)));
    },
	saveSpecialCusts: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("customerService", "SaveSpecialCusts", $.toJSON(args)));
    },

//--------------------------------业务显示-------------------------------
    getDateTypeIdByToday: function ()
    {
		var args = {};
        return $.parseJSON(window.external.PluginInvoke("settingService", "GetDateTypeIdByToday", $.toJSON(args)));
    },
	getBuzTypesForTree: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "GetBuzTypesForTree", $.toJSON(args)));
    },
	
	getCurrBuzTypesForTree: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "GetCurrBuzTypesForTree", $.toJSON(args)));
    },
	
	getBuzTimeByBuzTypeId: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "GetBuzTimeByBuzTypeId", $.toJSON(args)));
    },
	saveBizTimes: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "SaveBizTimes", $.toJSON(args)));
    },

//--------------------------------单据业务-------------------------------
	getBusTypesExceptEnabled: function ()
    {
		var args = {};
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "GetBusTypesExceptEnabled", $.toJSON(args)));
    },
	getBusTypesIdByBuzTypeId: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("buzTypeService", "GetBusTypesIdByBuzTypeId", $.toJSON(args)));
    },
	saveBuzBuses: function (args)
	{
		return $.parseJSON(window.external.PluginInvoke("buzTypeService", "SaveBuzBuses", $.toJSON(args)));
	},
//--------------------------------本地识别-------------------------------
	getCustRecs: function ()
    {
		var args = {};
        return $.parseJSON(window.external.PluginInvoke("customerService", "GetCustRecs", $.toJSON(args)));
    },
	saveCustRecs: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("customerService", "SaveCustRecs", $.toJSON(args)));
    },
	getWaitings: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("ticketService", "GetWaitingCount", $.toJSON(args)));
    },
	callTicket: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("ticketService", "CallTicket", $.toJSON(args)));
    },
    swipeAllCard: function (objId, callback, skipText)
    {
		var type = "";
		if(!app){
			//type = $.pps.getAppConfig().appConfig.peripheral.type;
		}else{
			//type = app.peripheral.type;
		}
        var args = {};
        args.objId = objId;
        args.callback = callback;
		args.tag = "A";
        args.type = type;
        args.timeout = 864000000;
        window.external.PluginInvoke("peripheralManager", "SwipeCard", $.toJSON(args));
    },
	cancelSwipeAllCard: function ()
	{
		var args = {};
		window.external.PluginInvoke("peripheralManager", "Cancel", $.toJSON(args));
	},
	playSoundCustMgr: function ()
    {
		var args = {};
        window.external.PluginInvoke("ticketService", "PlaySoundCustMgr", $.toJSON(args));
    },
    lockTicket: function (args) {
        return $.parseJSON(window.external.PluginInvoke("ticketService", "LockTicket", $.toJSON(args)));
    },
    unlockTicket: function (args) {
        return $.parseJSON(window.external.PluginInvoke("ticketService", "UnlockTicket", $.toJSON(args)));
    },
	isEffectQueueNo: function (args) {
        return $.parseJSON(window.external.PluginInvoke("ticketService", "IsEffectQueueNo", $.toJSON(args)));
    },
	yesterdayBizReport: function (args) {
        return $.parseJSON(window.external.PluginInvoke("ticketService", "YesterdayBizReport", $.toJSON(args)));
    },
	todayBizReport: function (args) {
        return $.parseJSON(window.external.PluginInvoke("ticketService", "TodayBizReport", $.toJSON(args)));
    }
};
$.pps =
{
    toPinyin: function (val)
    {
        return window.external.ToPinyin(val);
    },
    toPinyins: function (val)
    {
        return window.external.ToPinyins(val);
    },
    login: function (username, password)
    {
        var args = {};
        args.admin = {};
        args.admin.username = username;
        args.admin.password = password;
        return $.parseJSON(window.external.PluginInvoke("adminService", "Login", $.toJSON(args)));
    },
    log: function (message, type)
    {
        var args = {};
        args.log = {};
        args.log.operationData = message;
        args.log.operationType = type;

        window.external.PluginInvoke("logService", "CreateLogAsync", $.toJSON(args));
    },
    setVolume: function (val)
    {
        var args = {};
        args.settings = {};
        args.settings.volume = val;
        return $.parseJSON(window.external.PluginInvoke("systemService", "SetVolume", $.toJSON(args)));
    },
    setMute: function (val)
    {
        var args = {};
        args.settings = {};
        args.settings.isMute = val;
        return $.parseJSON(window.external.PluginInvoke("systemService", "SetMute", $.toJSON(args)));
    },
    shutdown: function ()
    {
        window.external.PluginInvoke("systemService", "Shutdown", null);
    },
    restart: function ()
    {
        window.external.PluginInvoke("systemService", "Restart", null);
    },
    appRestart: function ()
    {
        window.external.PluginInvoke("systemService", "AppRestart", null);
    },
    getBusType: function (busTypeId)
    {
        var args = {};
        args.busType = {};
        args.busType.busTypeId = busTypeId;
        return $.parseJSON(window.external.PluginInvoke("busTypeService", "GetBusType", $.toJSON(args)));
    },
    getBusTypes: function ()
    {
        return $.parseJSON(window.external.PluginInvoke("busTypeService", "GetBusTypes", null));
    },
    getBusTypesByBuzTypeId: function (buzTypeId)
    {
		var args = {};
        args.buzTypeId = buzTypeId;
        return $.parseJSON(window.external.PluginInvoke("busTypeService", "GetBusTypesByBuzTypeId", $.toJSON(args)));
    },
    getSettings: function ()
    {
        return $.parseJSON(window.external.PluginInvoke("settingService", "GetSettings", null));
    },
    saveSettings: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("settingService", "SaveSettings", $.toJSON(args)));
    },
    getStyles: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("styleService", "GetStylesByPage", $.toJSON(args)));
    },
	getCurrStyle: function (args)
    {
        return $.parseJSON(window.external.PluginInvoke("styleService", "GetCurrStyle", $.toJSON(args)));
    },
    saveStyles: function (args)
    {	
        return $.parseJSON(window.external.PluginInvoke("styleService", "SaveStyles", $.toJSON(args)));
    },
	removeStyle: function (args)
    {	
        return $.parseJSON(window.external.PluginInvoke("styleService", "RemoveStyle", $.toJSON(args)));
    },
	updateStyle: function (args)
    {	
        return $.parseJSON(window.external.PluginInvoke("styleService", "UpdateStyle", $.toJSON(args)));
    },
	createStyle: function (args)
    {	
        return $.parseJSON(window.external.PluginInvoke("styleService", "CreateStyle", $.toJSON(args)));
    },
	copyQueueTemplateJsonFiles: function (args)
    {	
        return $.parseJSON(window.external.PluginInvoke("styleService", "CopyQueueTemplateJsonFiles", $.toJSON(args)));
    },
	getQueueTemplateJsonFile: function (args)
    {	
        return $.parseJSON(window.external.PluginInvoke("styleService", "GetQueueTemplateJsonFile", $.toJSON(args)));
    },
    getAppState: function ()
    {
        return $.parseJSON(window.external.GetAppState());
    },
    getAdvConfig: function ()
    {
        return $.parseJSON(window.external.GetAdvConfig());
    },
    getAppConfig: function ()
    {
        return $.parseJSON(window.external.GetAppConfig());
    },
    saveAppConfig: function (args)
    {
        return $.parseJSON(window.external.SaveAppConfig($.toJSON(args)));
    },
    getPrintConfig: function (groupType, receiptType)
    {
        return $.parseJSON(window.external.GetPrintConfig(groupType, receiptType));
    },
    saveAndPrintConfig: function (groupType, receiptType, jo)
    {
        var args = {}
        args.mask = {};
        args.mask.message = "正在打印，请稍候...";
        args.sound = ";10:preparation.wav";
        args.groupType = groupType;
        args.receiptType = receiptType;
        args.print = jo;
        window.external.SaveAndPrintConfig($.toJSON(args));
    },
    saveAndPrintReceipt: function (args)
    {
        args.mask = {};
        args.mask.message = "正在打印，请稍候...";
        args.sound = ";10:preparation.wav";
        window.external.PluginInvoke("receiptService", "SaveAndPrintReceiptAsync", $.toJSON(args));
    },
    getLastReceipt: function (receiptType, certType, certNo, cardType, cardNo)
    {
        var args = {};
        args.query = {};
        args.query.receiptType = receiptType;
        args.query.certType = certType;
        args.query.certNo = certNo;
        args.query.cardType = cardType;
        args.query.cardNo = cardNo;
        return $.parseJSON(window.external.PluginInvoke("receiptService", "GetLastReceipt", $.toJSON(args)));
    },
    getReceiptStatListByPage: function (beginDate, endDate, rows, page)
    {
        var args = {};
        args.query = {};
        args.query.beginDate = beginDate;
        args.query.endDate = endDate;
        args.query.rows = rows;
        args.query.page = page;
        return $.parseJSON(window.external.PluginInvoke("receiptService", "GetReceiptStatByPage", $.toJSON(args)));
    },
    getCustomer: function (certType, certNo)
    {
        var args = {};
        args.customer = {};
        args.customer.certType = certType;
        args.customer.certNo = certNo;
        return $.parseJSON(window.external.PluginInvoke("customerService", "GetCustomerByCert", $.toJSON(args)));
    },
    playSound: function (wav)
    {
        window.external.PlaySound(wav);
    },
    swipeIDCard: function (objId, callback, skipText)
    {
        var args = {};
        args.mask = {};
		args.mask.timeout = 10;
		args.mask.skipText = skipText;
        args.mask.message = "请刷身份证...";
        args.objId = objId;
        args.sound = "idcard.wav;1:swipeIDCardfail.wav,4:swipeIDCardover.wav";
        args.callback = callback;
		args.type = 4;
        window.external.PluginInvoke("peripheralManager", "SwipeCard", $.toJSON(args));
    },
    swipeCard: function (objId, callback, skipText)
    {
        var args = {};
        args.mask = {};
		args.mask.timeout = 10;
        args.mask.message = "请刷银行卡...";
		args.mask.skipText = skipText;
        args.objId = objId;
        args.sound = "bankcard.wav;1:bankcarfail.wav,4:bankcarover.wav";
        args.callback = callback;
        args.type = 3;
        args.tag = "A";
        window.external.PluginInvoke("peripheralManager", "SwipeCard", $.toJSON(args));
    },
    swipeEleCard: function (objId, callback, skipText)
    {
        var args = {};
        args.mask = {};
		args.mask.timeout = 10;
        args.mask.skipText = skipText;
        args.mask.message = "请刷电子结算证...";
        args.objId = objId;
        args.sound = "insertcard.wav;1:insertcarfail.wav,4:insertcarover.wav";
        args.callback = callback;
        args.type = 2;
        args.tag = "J";
        window.external.PluginInvoke("icCardReaderWriter", "ReadAsync", $.toJSON(args));
    },
    loadKeyboard: function (obj, keyboardType, maxLen, format)
    {
        var text = document.getElementById(obj.id).value;
        window.external.ShowKeyboard(obj.id, text, maxLen, keyboardType, format);
    },
	updateFromUsb: function ()
    {
        window.external.PluginInvoke("systemService", "UpdateFromUsbAsync", null);
    }
};

/**
* 显示虚拟键盘
* @param obj 当前控件的ID
* @param keyboardType 键盘类型   0:全键盘 1:手写 2:数字键盘-全数字（账号、卡号） 
*  3:数字键盘-带小数点（金额） 4:数字键盘-带X（身份证号） 5: 电话-  6：负数
* @param maxLen 允许输入的最大字符位数
*/
function loadKeyboard(obj, keyboardType, maxLen)
{
    $.pps.loadKeyboard(obj, keyboardType, maxLen);
}

function loadInsertCard(objId, callback, skipText)
{
    $.pps.swipeEleCard(objId, callback, skipText);
}

function loadSwipeIDCard(objId, callback, skipText)
{
    $.pps.swipeIDCard(objId, callback, skipText);
}

function loadSwipeMagneticCard(objId, callback, skipText)
{
    $.pps.swipeCard(objId, callback, skipText);
}

function playSound()
{
    $.pps.playSound();
}

/*
result : Success = 0;Failure = 1;Busy = 2;Cancelled = 3;Timeout = 4;

// ------------记录用操作日志 $.pps.log(message)--------------//
// ---input----
message:string

// ---output---
{"result":0}

// ----code----
$.pps.log("用户点击了保存按钮");

// ------------设置操作系统音量 $.pps.setVolume(val)--------------
// ---input
val:int

// ---output
{"result":0}

// ---code
$.pps.setVolume(50);

// ------------获取系统设置默认值 $.pps.getSettings()--------------
// ---output
{
"result" : 0,
"settings":{
"openBank": "板桥区建设支行2",
"bankCode": "NB20023",
"province": "江苏4",
"city": "苏州5",
"volume": "25"
}
}
// ---code
var json = $.pps.getSettings();
var jo = $.parseJSON(json);
alert(jo.result);
alert(jo.settings.openBank);

// ------------保存系统设置默认值 $.pps.saveSettings(args)--------------
// ---input
{
"mask": {"message" : "正在保存，请稍候..."},
"sound": "d.wav;1:a.wav,2:b.wav,3:c.wav",
"callback" : "callback",
"settings":{
"openBank": "板桥区建设支行2",
"bankCode": "NB20023",
"province": "江苏4",
"city": "苏州5",
"volume": "25"
}
}

// ---output
{"result" : 0}

// ---code
var args = {};
var mask = {};
var settings = {};

// 遮罩层，提示信息，可选
mask.message = "正在保存，请稍候...";
args.mask = mask;

// 输入输出播放语言，可选
// ;前是输入语音文件名不加后缀，;后是输出语音，输出语音格式(返回值:语音文件,返回值:语音文件,...)，返回值参照result
args.sound = "a;0:b,1:c,2:d";

// 回调js函数，可选
args.callback = "callback";

// 可以赋值一个或多个
settings.openBank =  "板桥区建设支行8";
settings.bankCode =  "NB20028";
settings.province =  "江苏8";
settings.city =  "苏州8";
settings.volume = "28";
args.settings = settings;

var json = $.pps.saveSettings(args);
var jo = $.parseJSON(json);
alert(jo.result);

// ------------获取主题样式列表 $.pps.getStylesByPage(args)--------------
// ---input
{
"mask": {"message" : "正在查询，请稍候..."},
"sound": "d;1:a,2:b,3:c",
"callback" : "callback",
"query":{
"page": 1,
"rows": 10,
}
}

// ---output
{
"result" : 0,
"total" : 2,
"rows": [{
"styleId": 1,
"styleName": "标准版（蓝）",
"beginDate": "20150101",
"endDate": "20150105",
"checked": true,
"def": false
},
{
"styleId": 2,
"styleName": "标准版（红）",
"beginDate": "20150106",
"endDate": "20150107",
"checked": false,
"def": true
}]
}

// ---code
var args = {};
var mask = {};
var query = {};

// 遮罩层，提示信息，可选
mask.message = "正在查询，请稍候...";
args.mask = mask;

// 输入输出播放语言，可选
// ;前是输入语音文件名不加后缀，;后是输出语音，输出语音格式(返回值:语音文件,返回值:语音文件,...)，返回值参照result
args.sound = "a;0:b,1:c,2:d";

// 回调js函数，可选
args.callback = "callback";

// 分页查询条件，page当前页从1开发，rows每页多少行
query.page =  1;
query.rows =  10;
args.query = query;

var json = $.pps.getStylesByPage(args);
var jo = $.parseJSON(json);
alert(jo.result);
alert(jo.total);
alert(jo.rows);

// ------------保存设置主题样式 $.pps.saveStyles(args)--------------
// ---input
{
"mask": {"message" : "正在保存，请稍候..."},
"sound": "d;1:a,2:b,3:c",
"callback" : "callback",
"styles": [
{
"styleId": 1,
"styleName": "标准版（蓝）",
"beginDate": "20150101",
"endDate": "20150105",
"checked": true,
"def": false
},
{
"styleId": 2,
"styleName": "标准版（红）",
"beginDate": "20150106",
"endDate": "20150107",
"checked": false,
"def": true
}]
}
// ---output
{"result" : 0}

// ---code
var args = {};
var mask = {};
var styles = [];

// 遮罩层，提示信息，可选
mask.message = "正在查询，请稍候...";
args.mask = mask;

// 输入输出播放语言，可选
// ;前是输入语音文件名不加后缀，;后是输出语音，输出语音格式(返回值:语音文件,返回值:语音文件,...)，返回值参照result
args.sound = "a;0:b,1:c,2:d";

// 回调js函数，可选
args.callback = "callback";

// 获取页面主题信息，多个装载到数组当中
var item = {};
item.styleId = 1;
item.styleName = "标准版（蓝）";
item.beginDate = "20150101";
item.endDate = "20150105";
item.checked = true;
item.def = false;
styles.push(item);
styles.push(item1);
styles.push(item2);
args.styles = styles;

var json = $.pps.saveStyles(args);
var jo = $.parseJSON(json);
alert(jo.result);

// ------------获取系统配置信息 $.pps.getAppConfig()--------------
// ---output
{
"result":0,
"appConfig":{
orgCode = "10011001",
deviceNo =  "101010190",
webServerHost = "localhost",
webServerPort = 8080
}
}

// ---code
var json = $.pps.getAppConfig();
var jo = $.parseJSON(json);
alert(jo.orgCode);

// ------------保存系统配置信息 $.pps.saveAppConfig(args)--------------
// ---input
{
"mask": {"message" : "正在保存，请稍候..."},
"sound": "d;1:a,2:b,3:c",
"callback" : "callback",
"appConfig":{
orgCode = "10011001",
deviceNo =  "101010190",
webServerHost = "localhost",
webServerPort = 8080
}
}

// ---output
{"result":0}

// ---code
var json = $.pps.saveAppConfig(args);
var jo = $.parseJSON(json);
alert(jo.result);

// ------------获取单据打印配置信息 $.pps.getPrintConfig(args)--------------
// ---input
{"query" : {"busGroupCode":"personal", "busTypeCode":"withdraw"}}

// ---output
{
"family": "sans-serif",
"size": 24,
"dpi": 200,
"image": {"width":1877,"height":739},
"backImage":{"width":1827,"height":739},
"offset": {
"x": 1,
"y": 1
},
"items": [
{
"id": "receiptNo",
"text": "002AEX",
"x": 950,
"y": 80
},
{
"id": "accountNo",
"text": "002521466332",
"x": 300,
"y": 470
},
{
"id": "accountName",
"text": "张三",
"x": 720,
"y": 470
}]
}

// ---code
var json = $.pps.getPrintConfig(args);
var jo = $.parseJSON(json);

// ------------保存并打印配置信息 $.pps.saveAndPrintConfig(args)--------------
// ---input
{
"family": "sans-serif",
"size": 24,
"dpi": 200,
"image": {"width":1877,"height":739},
"backImage":{"width":1827,"height":739},
"offset": {
"x": 1,
"y": 1
},
"items": [
{
"id": "receiptNo",
"text": "002AEX",
"x": 950,
"y": 80
},
{
"id": "accountNo",
"text": "002521466332",
"x": 300,
"y": 470
},
{
"id": "accountName",
"text": "张三",
"x": 720,
"y": 470
}]
}

// ---output
{"result":0}

// ---code
var json = $.pps.saveAndPrintConfig(args);
var jo = $.parseJSON(json);

// ------------保存并打印单据信息 $.pps.saveAndPrintReceipt(args)--------------
// ---input
{
"mask": {"message" : "正在打印，请稍候..."},
"sound": "d.wav;0:d.wav;1:a.wav,2:b.wav,3:c.wav",
"callback" : "aaa",
"receipt":{
"busType":4,
"receiptType":4,
"certType":1,
"certNo":"420830198412211560",
"cardType":1,
"cardNo":"123456789012345678",
"content":"",
"queueNo":"Na0001",
"beginDate":"2015-01-01 12:30:34",
"endDate":"2015-01-01 12:32:12",
},
"customer":{
"customerName":"张三",
"certNo":"420830198412211560",
"certType":1,
"address":"南京雨花区板桥新村510",
"mailbox":"456@qq.com",
"phone":"18319821450",
"job":"教师",
"nationality":"中国"
},
"print" : {}
}

// ---output
{"result":0,"receiptNo":"MO0001"}

// ---code
var json = $.pps.saveAndPrintReceipt(args);
var jo = $.parseJSON(json);

// ------------获取最后填写的单据信息 $.pps.getLastReceipt(args)--------------
// ---input
{
"query":{
"receiptType":4,
"certType":1,
"certNo":"420830198412211560",
"cardType":1,
"cardNo":"123456789012345678"
}
}

// ---output
{
"result": 0,
"receipt":{
"busType":4,
"receiptType":4,
"certType":1,
"certNo":"420830198412211560",
"cardType":1,
"cardNo":"123456789012345678",
"content":"",
"queueNo":"Na0001",
"beginDate":"2015-01-01 12:30:34",
"endDate":"2015-01-01 12:32:12",
}
}

// ---code
var json = $.pps.getLastReceipt(args);
var jo = $.parseJSON(json);

// ------------获取客户信息 $.pps.getCustomer(args)--------------
// ---input
{
"query":{
"certType":1,
"certNo":"420830198412211560",
}
}

// ---output
{
"result": 0,
"customer":{
"customerName":"ddd",
"certType":1,
"certNo":"420830198412211560",
"address":"",
"mailbox":"123456789012345678",
"phone":"1232423434",
"job":"职员",
"nationality":"中国",
}
}

// ---code
var json = $.pps.getCustomer(args);
var jo = $.parseJSON(json);

// ------------播放声音 $.pps.playSound(wav)--------------
// ---input
wav:string

// ---code
$.pps.playSound("a.wav");

// ------------刷身份证 $.pps.swipeIDCard(args)--------------
// ---input
{
"mask": {"message" : "请刷身份证..."},
"sound": "d.wav;1:a.wav,4:c.wav",
"callback" : "aaa",
"objId": "certNo"
}

// ---output
{
"result": 0,
"objId": "certNo",
"certName":"ddd",
"gender":"男",
"nationality":"420830198412211560",
"birthday":"19881010",
"address":"南京市广州路",
"certNo":"420830198412211560",
"issuedBy":"南京市公安局",
"expDate":"20101231",
"certType":"中国"
}

// ---code
var json = $.pps.swipeIDCard(args);
var jo = $.parseJSON(json);

// ------------刷卡 $.pps.swipeCard(args)--------------
// type
// 001：磁条卡启用
// 010：IC卡启用
// 011：磁条卡、IC卡启用
// 100：吸卡器启用
// 101：吸卡器、磁条卡启用
// 110：IC卡启用、吸卡器启用
// 111：IC卡启用、吸卡器、磁条卡启用

// tag
//A： 帐号	19位
//B： 姓名
//C： 证件类型，值: 00：身份证 01：军官证 02：护照 03：入境证 04：临时身份证 05：其它
//D： 证件号码
//E： 二磁道信息（可选）	Ans
//F： 一磁道信息（可选）	Ans
//G： 余额	Ans,不带小数点
//H： 余额上限	Ans,不带小数点
//I： 应用失效日期
//J： IC卡序列号
// ---input
{
"mask": {"message" : "请刷银行卡..."},
"sound": "d.wav;0:d.wav;1:a.wav,2:b.wav,3:c.wav",
"callback" : "aaa",
"objId": "cardNo",
"type" : 1
"tag" : "A"
}

// ---output
{
"result": 0,
"objId": "certNo",
"serialNo" : "fewrwew",     // 序列号
"cardNo" = "3242345235435"  // 卡号
}

// ---code
var json = $.pps.swipeCard(args);
var jo = $.parseJSON(json);
*/
