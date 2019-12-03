$.validator.setDefaults({
	submitHandler: function(form) { 
		modelSubmit();
		return false;
	},
	ignore: ":hidden:not(select)",
	//hideErrors: function() {
		//this.addWrapper( this.toHide ).hide();
	//	alert(this.toHide);
	//},
	showErrors: function(errorMap, errorList ) {
		//alert($.toJSON(this.successList));
		/*if(errorList.length==0){
			//alert(errorList.length+"  "+event.target);
			if(event&&event.target){
				var $el = $(event.target);
				if(event.target.tagName=="SELECT"){
					$el = $(event.target).parent();
				}
				$el.attr("data-content","");
				qmsVld.removeErrorCss(event.target); 
				$el.popover('hide');
			}else{
				//清空所有error
				$el = $(".datepicker");
				//alert($el.length);
				$el.attr("data-content","");
				qmsVld.removeErrorCss($el.get(0)); 
				$el.popover('hide');
				
				
			}
		}*/
		$.each( errorList, function(i,v){
			var $el = $(v.element);
			if(v.element.tagName=="SELECT"){
				$el = $(v.element).parent();
			}
			qmsVld.addErrorCss(v.element); 
			$el.attr("data-content",v.message);
			$el.popover({
				trigger:"manual",
				placement: 'bottom' //top, bottom, left or right
			});
			$el.popover('show');
		});
		
		$.each( this.successList, function(i,v){
			var $el = $(v);
			if(v.tagName=="SELECT"){
				$el = $(v).parent();
			}
			qmsVld.removeErrorCss(v); 
			$el.attr("data-content","");
			$el.popover('hide');
		});
		
	}
});



jQuery.validator.addMethod("time", function(value, element) {
	return this.optional(element) || /^([01]\d|2[0-3])(:[0-5]\d){1,2}$/.test(value);
}, "请输入一个正确的时间, 介于 00:00 和 23:59 之间");
jQuery.validator.addMethod("time12h", function(value, element) {
	return this.optional(element) || /^((0?[1-9]|1[012])(:[0-5]\d){1,2}(\ ?[AP]M))$/i.test(value);
}, "请输入一个正确的时间 格式: 12-hour am/pm");
jQuery.validator.addMethod("ipv4", function(value, element, param) {
	return this.optional(element) || /^(25[0-5]|2[0-4]\d|[01]?\d\d?)\.(25[0-5]|2[0-4]\d|[01]?\d\d?)\.(25[0-5]|2[0-4]\d|[01]?\d\d?)\.(25[0-5]|2[0-4]\d|[01]?\d\d?)$/i.test(value);
}, "请输入一个正确的IP v4 地址");

jQuery.validator.addMethod("ipv6", function(value, element, param) {
	return this.optional(element) || /^((([0-9A-Fa-f]{1,4}:){7}[0-9A-Fa-f]{1,4})|(([0-9A-Fa-f]{1,4}:){6}:[0-9A-Fa-f]{1,4})|(([0-9A-Fa-f]{1,4}:){5}:([0-9A-Fa-f]{1,4}:)?[0-9A-Fa-f]{1,4})|(([0-9A-Fa-f]{1,4}:){4}:([0-9A-Fa-f]{1,4}:){0,2}[0-9A-Fa-f]{1,4})|(([0-9A-Fa-f]{1,4}:){3}:([0-9A-Fa-f]{1,4}:){0,3}[0-9A-Fa-f]{1,4})|(([0-9A-Fa-f]{1,4}:){2}:([0-9A-Fa-f]{1,4}:){0,4}[0-9A-Fa-f]{1,4})|(([0-9A-Fa-f]{1,4}:){6}((\b((25[0-5])|(1\d{2})|(2[0-4]\d)|(\d{1,2}))\b)\.){3}(\b((25[0-5])|(1\d{2})|(2[0-4]\d)|(\d{1,2}))\b))|(([0-9A-Fa-f]{1,4}:){0,5}:((\b((25[0-5])|(1\d{2})|(2[0-4]\d)|(\d{1,2}))\b)\.){3}(\b((25[0-5])|(1\d{2})|(2[0-4]\d)|(\d{1,2}))\b))|(::([0-9A-Fa-f]{1,4}:){0,5}((\b((25[0-5])|(1\d{2})|(2[0-4]\d)|(\d{1,2}))\b)\.){3}(\b((25[0-5])|(1\d{2})|(2[0-4]\d)|(\d{1,2}))\b))|([0-9A-Fa-f]{1,4}::([0-9A-Fa-f]{1,4}:){0,5}[0-9A-Fa-f]{1,4})|(::([0-9A-Fa-f]{1,4}:){0,6}[0-9A-Fa-f]{1,4})|(([0-9A-Fa-f]{1,4}:){1,7}:))$/i.test(value);
}, "请输入一个正确的IP v6 地址");
// 手机号码验证
jQuery.validator.addMethod("mobile", function(value, element) {
    var length = value.length;
	var mobile = /^(13[0-9]|14[0-9]|15[0-9]|18[0-9]|170|177)\d{8}$/;
    return this.optional(element) || (length == 11 && mobile.test(value));
}, "手机号码格式错误");   

jQuery.validator.addMethod("chosenRequired1", function(value, element) {
    //var length = value.length;
	//var mobile = /^(13[0-9]|14[0-9]|15[0-9]|18[0-9]|170|177)\d{8}$/;
    //return this.optional(element) || (length == 11 && mobile.test(value));
	//alert(element.name+"  "+value);
	return this.optional(element) || value;
}, "必选字段");   

// 电话号码验证   
jQuery.validator.addMethod("phone", function(value, element) {
    var flag = true;
	var a = /^(0[1-9]\d)?\d{7,8}$/;
    var b = /^(0[3-9][13579]\d)?\d{7,8}$/;
    var c = /^(0[3-9][24680]\d{2})?\d{7,8}$/;
    var e = /^(13[0-9]|14[0-9]|15[0-9]|18[0-9]|170|177)\d{8}$/;
    if (a.test(value) || b.test(value) || c.test(value) || e.test(value))
    {
        flag = true;
    } else
    {
        flag = false;
    }
    return this.optional(element) || (length == 11 && flag);
}, "电话号码格式错误");

//窗口设备验证 1|2|88
jQuery.validator.addMethod("formatDevice", function(value, element) {
	var flag = true;
	var strArr = value.split("|");
	
	$.each(strArr, function (index, item){
		if(""==item){
			flag = false;
			return false;
		}else{
			if(/^\d+$/.test(item)==false){
				flag = false;
				return false;
			}
		} 
	});
    return this.optional(element) || flag;
}, "格式错误 如:1|2|7 ");
		

// 邮政编码验证   
jQuery.validator.addMethod("zipCode", function(value, element) {
    var tel = /^[0-9]{6}$/;
    return this.optional(element) || (tel.test(value));
}, "邮政编码格式错误");

// IP分段验证   
jQuery.validator.addMethod("ipPart", function(value, element) {
    var tel = /^(([0-9]|([1-9]\d)|(1\d\d)|(2([0-4]\d|5[0-5]))))$/;
    return this.optional(element) || (tel.test(value));
}, "请输入0-255的数字");


// QQ号码验证   
jQuery.validator.addMethod("qq", function(value, element) {
    var tel = /^[1-9]\d{4,9}$/;
    return this.optional(element) || (tel.test(value));
}, "qq号码格式错误");

// IP地址验证
jQuery.validator.addMethod("ip", function(value, element) {
    var ip = /^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/;
    return this.optional(element) || (ip.test(value) && (RegExp.$1 < 256 && RegExp.$2 < 256 && RegExp.$3 < 256 && RegExp.$4 < 256));
}, "Ip地址格式错误");

//身份证验证
jQuery.validator.addMethod("idCard", function(value, element) {
    return this.optional(element) || (qmsVld.checkCard(value));
}, "身份证号码格式错误");

// 字母和数字的验证
jQuery.validator.addMethod("chrnum", function(value, element) {
    var chrnum = /^([a-zA-Z0-9]+)$/;
    return this.optional(element) || (chrnum.test(value));
}, "只能输入数字和字母(字符A-Z, a-z, 0-9)");

// 数字字符串的验证
jQuery.validator.addMethod("num", function(value, element) {
    var chrnum = /^([0-9]+)$/;
    return this.optional(element) || (chrnum.test(value));
}, "只能输入数字(字符0-9)");
//最少3位数字
jQuery.validator.addMethod("limitpwd", function(value, element) {
    var chrnum = /^\d{3}/;
    return this.optional(element) || (chrnum.test(value));
}, "最少输入3位数字密码");
//最少5位
jQuery.validator.addMethod("limitftppwd", function(value, element) {
    var chrnum = /^(?=.*[\d]+)(?=.*[a-zA-Z]+)(?=.*[^a-zA-Z0-9]+).{0,5}/;
    return this.optional(element) || (chrnum.test(value));
}, "最少输入5位密码,必须包含英文、数字、标点符号");
jQuery.validator.addMethod("defaultServeLevel", function(value, element) {
    var chrnum = /^([0-7]+)$/;
    return this.optional(element) || (chrnum.test(value));
}, "请输入0到7内的数字");
// 中文的验证
jQuery.validator.addMethod("chinese", function(value, element) {
    var chinese = /^[\u4e00-\u9fa5]+$/;
    return this.optional(element) || (chinese.test(value));
}, "只能输入中文");

// 下拉框验证
jQuery.validator.addMethod("selectNone", function(value, element) {
    return value == "";
}, "必须选择一项");

// 字节长度验证
jQuery.validator.addMethod("byteRangeLength", function(value, element, param) {
    var length = value.length;
    for (var i = 0; i < value.length; i++) {
        if (value.charCodeAt(i) > 127) {
            length++;
        }
    }
    return this.optional(element) || (length >= param[0] && length <= param[1]);
}, $.validator.format("请确保输入的值在{0}-{1}个字节之间(一个中文字算2个字节)"));


// 银行卡
jQuery.validator.addMethod("checkBankNo", function(value, element) {
    var checkBankNo = /^\d{14,30}$/;;
    return this.optional(element) || (checkBankNo.test(value));
}, "请输入正确的银行卡号");
jQuery.extend(jQuery.validator.messages, {
	required: "必填字段",
	remote: "请修正该字段",
	email: "请输入正确格式的电子邮件",
	url: "请输入合法的网址",
	date: "请输入合法的日期",
	dateISO: "请输入合法的日期 (ISO).",
	number: "请输入合法的数字",
	digits: "只能输入整数",
	creditcard: "请输入合法的信用卡号",
	equalTo: "请再次输入相同的值",
	accept: "请输入拥有合法后缀名的字符串",
	maxlength: jQuery.validator.format("请输入一个长度最多是 {0} 的字符串"),
	minlength: jQuery.validator.format("请输入一个长度最少是 {0} 的字符串"),
	rangelength: jQuery.validator.format("请输入一个长度介于 {0} 和 {1} 之间的字符串"),
	range: jQuery.validator.format("请输入一个介于 {0} 和 {1} 之间的值"),
	max: jQuery.validator.format("请输入一个最大为 {0} 的值"),
	min: jQuery.validator.format("请输入一个最小为 {0} 的值")
});


var qmsVld ={};
qmsVld.addErrorCss  = function (obj)
{
	$(obj).parent().addClass("has-error");
};
qmsVld.removeErrorCss  = function (obj)
{
	$(obj).parent().removeClass("has-error");
};

qmsVld.hideError  = function (obj)
{
	$(obj).attr("data-content","");
	qmsVld.removeErrorCss(obj); 
	$(obj).popover('hide');
};

qmsVld.showMessage  = function (tips,layout,type,obj){
	var options = {};
	options.text = tips;
	options.layout = layout;
	options.type = type;
	//居中问题	
	options.template = '<div class="noty_message" style="display:table-cell;width:300px;height:150px;vertical-align: middle;border:0px #95B8E7 solid;" ><span class="noty_text"  style="display:block;font-size:24px;word-wrap:break-word;word-break:break-all;border:0px #95B8E7 solid;"></span><div class="noty_close"></div></div>';
	if(type=="error"){
		options.timeout = 2000;
		options.speed = 500;
	}else{
		options.timeout = 600;
		options.speed = 300;
	}
	noty(options);
	if(type=="error"&&obj){
		qmsVld.addErrorCss(obj); 
		$(obj).attr("data-content",tips);
		$(obj).popover({trigger:"manual"});
		$(obj).popover('show');
	}
};

qmsVld.showInfo = function (tips){
	qmsVld.showMessage(tips,"center","information");
};
qmsVld.showError = function (tips,obj){
	qmsVld.showMessage(tips,"center","error",obj);
};

qmsVld.showPopover = function(obj,content){
    obj.popover({content:content});
    obj.popover("show");
}

//校验银行卡号
qmsVld.checkBankNo  = function (bankno)
{
    // 目前不校验位数
    return vldUtil.checkAccount(bankno.trimSpace());

    var reg = /^\d{14,19}$/;
    if (reg.test(bankno))
    {
        return true;
    } else
    {
        return false;
    }
}

//帐号：数字、字母、横线、下划线
qmsVld.checkAccount  = function (d)
{
	d = d.trimSpace()
    var a = /^[A-Za-z0-9\-_]*$/;
    if (d != "" && !a.test(d))
    {
        return false;
    } else
    {
        return true;
    }
}

// 身份证校验
qmsVld.checkCard  = function (card)
{
    // 是否为空
    if (card == "")
    {
        return false;
    }
    // 校验长度，类型
    if (qmsVld.isCardNo(card) == false)
    {
        return false;
    }
    // 检查省份
    if (qmsVld.checkProvince(card) == false)
    {
        return false;
    }
    // 校验生日
    if (qmsVld.checkBirthday(card) == false)
    {
        return false;
    }
    // 检验位的检测
    if (qmsVld.checkParity(card) == false)
    {
        return false;
    }
    return true;
};

// 检查号码是否符合规范，包括长度，类型
qmsVld.isCardNo = function (card)
{
    // 身份证号码为15位或者18位，15位时全为数字，18位前17位为数字，最后一位是校验位，可能为数字或字符X
    var reg = /(^\d{15}$)|(^\d{17}(\d|X)$)/;
    if (reg.test(card) === false)
    {
        return false;
    }
    return true;
};

var vcity = { 11: "\u5317\u4eac", 12: "\u5929\u6d25", 13: "\u6cb3\u5317", 14: "\u5c71\u897f", 15: "\u5185\u8499\u53e4", 21: "\u8fbd\u5b81", 22: "\u5409\u6797", 23: "\u9ed1\u9f99\u6c5f", 31: "\u4e0a\u6d77", 32: "\u6c5f\u82cf", 33: "\u6d59\u6c5f", 34: "\u5b89\u5fbd", 35: "\u798f\u5efa", 36: "\u6c5f\u897f", 37: "\u5c71\u4e1c", 41: "\u6cb3\u5357", 42: "\u6e56\u5317", 43: "\u6e56\u5357", 44: "\u5e7f\u4e1c", 45: "\u5e7f\u897f", 46: "\u6d77\u5357", 50: "\u91cd\u5e86", 51: "\u56db\u5ddd", 52: "\u8d35\u5dde", 53: "\u4e91\u5357", 54: "\u897f\u85cf", 61: "\u9655\u897f", 62: "\u7518\u8083", 63: "\u9752\u6d77", 64: "\u5b81\u590f", 65: "\u65b0\u7586", 71: "\u53f0\u6e7e", 81: "\u9999\u6e2f", 82: "\u6fb3\u95e8", 91: "\u56fd\u5916" };

// 取身份证前两位,校验省份
qmsVld.checkProvince = function (card)
{
    var province = card.substr(0, 2);
    if (vcity[province] == undefined)
    {
        return false;
    }
    return true;
};


// 检查生日是否正确
qmsVld.checkBirthday =  function (card)
{
    var len = card.length;
    // 身份证15位时，次序为省（3位）市（3位）年（2位）月（2位）日（2位）校验位（3位），皆为数字
    if (len == "15")
    {
        var re_fifteen = /^(\d{6})(\d{2})(\d{2})(\d{2})(\d{3})$/;
        var arr_data = card.match(re_fifteen);
        var year = arr_data[2];
        var month = arr_data[3];
        var day = arr_data[4];
        var birthday = new Date("19" + year + "/" + month + "/" + day);
        return qmsVld.verifyBirthday("19" + year, month, day, birthday);
    }
    // 身份证18位时，次序为省（3位）市（3位）年（4位）月（2位）日（2位）校验位（4位），校验位末尾可能为X
    if (len == "18")
    {
        var re_eighteen = /^(\d{6})(\d{4})(\d{2})(\d{2})(\d{3})([0-9]|X)$/;
        var arr_data = card.match(re_eighteen);
        var year = arr_data[2];
        var month = arr_data[3];
        var day = arr_data[4];
        var birthday = new Date(year + "/" + month + "/" + day);
        return qmsVld.verifyBirthday(year, month, day, birthday);
    }
    return false;
};

// 校验日期
qmsVld.verifyBirthday  = function (year, month, day, birthday)
{
    var now = new Date();
    var now_year = now.getFullYear();
    // 年月日是否合理
    if (birthday.getFullYear() == year && (birthday.getMonth() + 1) == month && birthday.getDate() == day)
    {
        // 判断年份的范围（3岁到100岁之间)
        var time = now_year - year;
        if (time >= 3 && time <= 100)
        {
            return true;
        }
        return false;
    }
    return false;
};

// 校验位的检测
qmsVld.checkParity  = function (card)
{
    // 15位转18位
    card = qmsVld.changeFivteenToEighteen(card);
    var len = card.length;
    if (len == "18")
    {
        var arrInt = new Array(7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2);
        var arrCh = new Array("1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2");
        var cardTemp = 0, i, valnum;
        for (i = 0; i < 17; i++)
        {
            cardTemp += card.substr(i, 1) * arrInt[i];
        }
        valnum = arrCh[cardTemp % 11];
        if (valnum == card.substr(17, 1))
        {
            return true;
        }
        return false;
    }
    return false;
};

// 15位转18位身份证号
qmsVld.changeFivteenToEighteen  = function (card)
{
    if (card.length == "15")
    {
        var arrInt = new Array(7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2);
        var arrCh = new Array("1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2");
        var cardTemp = 0, i;
        card = card.substr(0, 6) + "19" + card.substr(6, card.length - 6);
        for (i = 0; i < 17; i++)
        {
            cardTemp += card.substr(i, 1) * arrInt[i];
        }
        card += arrCh[cardTemp % 11];
        return card;
    }
    return card;
};
