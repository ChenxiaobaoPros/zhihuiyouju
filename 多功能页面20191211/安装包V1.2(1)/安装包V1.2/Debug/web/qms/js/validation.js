var vldUtil ={};
//判空(value=字段的值)
vldUtil.isEmpty  = function (value)
{
	if($.string($.trim(value)).blank())
    {
        return true;
    } else
    {
        return false;
    }
}

//判空,加载错误样式（显示右边）(value=字段的值，name=字段的中文名称,obj=字段的对象)
vldUtil.validateEmpty  = function (value, name, obj,type)
{
	if($.string($.trim(value)).blank())
    {
		if("checkSelect"==type){
			vldUtil.addErrCss(obj, "<span>请选择" + name + "</span>");
		}else{
			vldUtil.addErrCss(obj, "<span>请输入" + name + "</span>");
		}
        return true;
    } else
    {
        return false;
    }
}

//判空,加载错误样式(显示下边)(value=字段的值，name=字段的中文名称,obj=字段的对象,index=错误显示表格位置)
vldUtil.validateUnderEmpty  = function (value, name, obj, index,type)
{
	if($.string($.trim(value)).blank())
    {
		if("checkSelect"==type){
			vldUtil.addUnderErrCss(obj, "<span>请选择" + name + "</span>", index);
		}else{
			vldUtil.addUnderErrCss(obj, "<span>请输入" + name + "</span>", index);
		}
        return true;
    } else
    {
        return false;
    }
}

//正则表达式 (value=字段的值，name=字段的中文名称,obj=字段的对象,reg=正则表达式,index=错误显示表格位置)
vldUtil.validateByRegular  = function (value, name, obj, reg, index)
{
    if (!reg.test(value))
    {
        if (index == 0)
        {
            vldUtil.addErrorCss(obj, name);
        } else
        {
            vldUtil.addUnderErrorCss(obj, name, index);
        }
        return true;
    }
    else
    {
        if (index == 0)
        {
            vldUtil.removeErrorCss(obj);
        } else
        {
            vldUtil.removeUnderErrorCss(obj, index);
        }
        return false;
    }
}

//添加错误样式（显示右边）(name=字段的中文名称,obj=字段的对象)
vldUtil.addErrCss  = function (obj, name)
{
    obj.addClass("error");
    obj.parent().next().html(name);
}

//添加错误样式（显示下边）(name=字段的中文名称,obj=字段的对象,index=错误显示表格位置)
vldUtil.addUnderErrCss  = function (obj, name, index)
{
    obj.addClass("error");
    obj.parent().parent().next().children().eq(index).html(name);
}

//添加错误样式（显示右边）(name=字段的中文名称,obj=字段的对象)
vldUtil.addErrorCss  = function (obj, name,type)
{
    obj.addClass("error");
	var alt = "";
	if("checkSelect"==type){
		alt = "<span>请选择" + name + "</span>";
	}else{
		alt = "<span>请输入正确的" + name + "</span>";
	}
    obj.parent().next().html(alt);
}

//添加错误样式(显示下边)(name=字段的中文名称,obj=字段的对象,index=错误显示表格位置)
vldUtil.addUnderErrorCss  = function (obj, name, index,type)
{
    obj.addClass("error");
	var alt = "";
	if("checkSelect"==type){
		alt = "<span>请选择" + name + "</span>";
	}else{
		alt = "<span>请输入正确的" + name + "</span>";
	}
    obj.parent().parent().next().children().eq(index).html(alt);
}

//移除错误样式(移除右边)(obj=字段的对象)
vldUtil.removeErrorCss  = function (obj)
{
    obj.removeClass("error");
    obj.parent().next().html("");
}

//移除错误样式(移除下边)(obj=字段的对象,index=错误显示表格位置)
vldUtil.removeUnderErrorCss  = function (obj, index)
{
    obj.removeClass("error");
    obj.parent().parent().next().children().eq(index).html("");
}

//正整数，并加载错误样式(value=字段的值，name=字段的中文名称,obj=字段的对象,index=错误显示表格位置)
vldUtil.validateInteger  = function (value, name, obj, index)
{
    var reg = /^\d{1,8}?$/;
    return vldUtil.validateByRegular(value, name, obj, reg, index);
}

//正实数，可保留两位小数，并加载错误样式(value=字段的值，name=字段的中文名称,obj=字段的对象,index=错误显示表格位置)
vldUtil.validateDouble  = function (value, name, obj, index)
{
    var reg = /^\d{1,8}(\.\d{1,2})?$/;
    return vldUtil.validateByRegular(value, name, obj, reg, index);
}

vldUtil.toChineseNumber  = function (num)
{
    if (!/^\d*(\.\d*)?$/.test(num)) throw (new Error(-1, "Number is wrong!"));
    var AA = new Array("零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖");
    var BB = new Array("", "拾", "佰", "仟", "万", "億", "圆", "");
    var CC = new Array("角", "分", "厘");
    var a = ("" + num).replace(/(^0*)/g, "").split("."), k = 0, re = "";
    for (var i = a[0].length - 1; i >= 0; i--)
    {
        switch (k)
        {
            case 0: re = BB[7] + re; break;
            case 4: if (!new RegExp("0{4}\\d{" + (a[0].length - i - 1) + "}$").test(a[0]))
                    re = BB[4] + re; break;
            case 8: re = BB[5] + re; BB[7] = BB[5]; k = 0; break;
        }
        if (k % 4 == 2 && a[0].charAt(i) == "0" && a[0].charAt(i + 2) != "0") re = AA[0] + re;
        if (a[0].charAt(i) != 0) re = AA[a[0].charAt(i)] + BB[k % 4] + re; k++;
    }
    if (a.length > 1)
    {
        re += BB[6];
        for (var i = 0; i < a[1].length; i++)
        {
            re += AA[a[1].charAt(i)] + CC[i];
            if (i == 2) break;
        }
        if (a[1].charAt(0) == "0" && a[1].charAt(1) == "0")
        {
            re += "元整";
        }
    } else
    {
        re += "元整";
    }
    return re;
}

//校验手机号
vldUtil.checkPhone  = function (d)
{
	d = d.trimSpace();
    var a = /^(13[0-9]|14[0-9]|15[0-9]|18[0-9]|170|177)\d{8}$/;
    if (!a.test(d))
    {
        return false;
    } else
    {
        return true;
    }
}

//分隔电话号码
vldUtil.getSplitPhone  = function (splitPhone)
{
    if (vldUtil.checkPhone(splitPhone))
    {
        return (splitPhone.substring(0, 3) + " " +
        		splitPhone.substring(3, 7) + " " +
        		splitPhone.substring(7, splitPhone.length));
    } else
    {
        return splitPhone;
    }
}

//校验电话号码: 如：0101234567,1234567,12345678,15151586394
//匹配：区号+电话，电话,手机号码
vldUtil.checkTel  = function (d)
{
    var a = /^(0[1-9]\d)?\d{7,8}$/;
    var b = /^(0[3-9][13579]\d)?\d{7,8}$/;
    var c = /^(0[3-9][24680]\d{2})?\d{7,8}$/;
    var e = /^(13[0-9]|14[0-9]|15[0-9]|18[0-9]|170|177)\d{8}$/;
    if (a.test(d) || b.test(d) || c.test(d) || e.test(d))
    {
        return true;
    } else
    {
        return false;
    }
}

//正实数，可保留两位小数（适用金额）
vldUtil.checkMoney  = function (d)
{
	d = vldUtil.backChangeMoney(d);
    var reg = /^\d{1,8}(\.\d{1,2})?$/;
    if (!reg.test(d))
    {
        return false;
    } else
    {
        return true;
    }
}


//验证邮编
vldUtil.checkPostalcode  = function (d)
{
    var a = /^[0-9][0-9]{5}$/;
    if (!a.test(d))
    {
        return false;
    } else
    {
        return true;
    }
}

//校验字符串中含中文
vldUtil.checkCnText = function (str)
{
    // var reg = /[\u4E00-\u9FA5]/g;
    var reg = /^[\u4E00-\uFFE5]+$/;
    if (reg.test(str))
    {
        return true;
    } else
    {
        return false;
    }
}

//校验字符串中含中英文
vldUtil.checkCnOrEnValid  = function (str)
{
    // var reg = /[\u4E00-\u9FA5A-Za-z]/g;
    var reg = /^[\u0391-\uFFE5A-Za-z]+$/;
    if (reg.test(str))
    {
        return true;
    } else
    {
        return false;
    }
}

//校验文本输入字段有效性（只允许输入中文、英文、数字），校验地址
vldUtil.checkTextValid  = function (str)
{
    var reg = /^[\u4E00-\u9FA5A-Za-z0-9]+$/;
    if (reg.test(str))
    {
        return true;
    } else
    {
        return false;
    }
}

//校验字母输入字段（只允许输入英文和空格）
vldUtil.checkLetter  = function (str)
{
    var reg = /^[ A-Za-z]*$/;
    if (reg.test(str))
    {
        return true;
    } else
    {
        return false;
    }
}

// 身份证校验
vldUtil.checkCard  = function (card)
{
    // 是否为空
    if (card == "")
    {
        return false;
    }
    // 校验长度，类型
    if (vldUtil.isCardNo(card) == false)
    {
        return false;
    }
    // 检查省份
    if (vldUtil.checkProvince(card) == false)
    {
        return false;
    }
    // 校验生日
    if (vldUtil.checkBirthday(card) == false)
    {
        return false;
    }
    // 检验位的检测
    if (vldUtil.checkParity(card) == false)
    {
        return false;
    }
    return true;
}

// 检查号码是否符合规范，包括长度，类型
vldUtil.isCardNo = function (card)
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
vldUtil.checkProvince = function (card)
{
    var province = card.substr(0, 2);
    if (vcity[province] == undefined)
    {
        return false;
    }
    return true;
};


// 检查生日是否正确
vldUtil.checkBirthday =  function (card)
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
        return vldUtil.verifyBirthday("19" + year, month, day, birthday);
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
        return vldUtil.verifyBirthday(year, month, day, birthday);
    }
    return false;
};

// 校验日期
vldUtil.verifyBirthday  = function (year, month, day, birthday)
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
vldUtil.checkParity  = function (card)
{
    // 15位转18位
    card = vldUtil.changeFivteenToEighteen(card);
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
vldUtil.changeFivteenToEighteen  = function (card)
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

//校验银行卡号
vldUtil.checkBankNo  = function (bankno)
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
vldUtil.checkAccount  = function (d)
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

/**
*  只验证ISNULL
*/
vldUtil.checkSelect  = function (str)
{
	return true;
}

//校验邮箱
vldUtil.checkEmail  = function (str)
{
    var reg = /^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/;
    if (reg.test(str))
    {
        return true;
    } else
    {
        return false;
    }
}

//校验日期yyyyMMdd;  checkDateStr：不比较日期; checkDateStr1:是比当前日期小；checkDateStr2：是比当前日期大
vldUtil.checkDateStr  = function (str,type) {
    var reg = /^(([0-9]{3}[1-9]|[0-9]{2}[1-9][0-9]{1}|[0-9]{1}[1-9][0-9]{2}|[1-9][0-9]{3})(((0[13578]|1[02])(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)(0[1-9]|[12][0-9]|30))|(02(0[1-9]|[1][0-9]|2[0-8]))))|((([0-9]{2})(0[48]|[2468][048]|[13579][26])|((0[48]|[2468][048]|[3579][26])00))[02][29])$/;
	if (reg.test(str)) {
		var result = true;
		if(type == "checkDateStr1"){
			result = vldUtil.compareTime(str,new Date().toString("yyyyMMdd"));
		}
		if(type == "checkDateStr2"){
			result = vldUtil.compareTime(new Date().toString("yyyyMMdd"),str);
		}
		return result;
	} else {
		return false;
	}	  
}

// 校验日期格式是否为YYYY-MM-DD，且符合要求
vldUtil.checkDateStrFormat  = function (str)
{
    if (str.length != 10)
    {
        return false;
    } else
    {
        var year = str.substr(0, 4);
        var month = str.substr(5, 2);
        var date = str.substr(8, 2);
        year = parseInt(year);
        if (year > 2100 || year < 1900)
        {
            return false;
        }

        month = parseInt(month);
        if (month > 12 || month < 1)
        {
            return false;
        }

        date = parseInt(date);
        if (date > 31 || date < 1)
        {
            return false;
        }

        return true;
    }
}

// 校验日期格式是否为YYYYMMDD，且符合要求
vldUtil.checkDateFormat  = function (str)
{
    if (str.length != 8)
    {
        return false;
    } else
    {
        var year = str.substr(0, 4);
        var month = str.substr(4, 2);
        var date = str.substr(6, 2);

        year = parseInt(year);
        if (year > 2100 || year < 1900)
        {
            return false;
        }

        month = parseInt(month);
        if (month > 12 || month < 1)
        {
            return false;
        }

        date = parseInt(date);
        if (date > 31 || date < 1)
        {
            return false;
        }

        return true;
    }
}

/*
* id 需要校验的id
* type 校验类型  checkPhone 校验手机号, checkTel 校验联系电话,checkMoney  金额, checkPostalcode 验证邮编,
* checkCnText  中文,checkCnOrEnValid  中英文,checkTextValid  中文、英文、数字,checkLetter 英文和空格,
* checkCard 身份证校验,checkBankNo  校验银行卡号,checkEmail  校验邮箱 checkDateStr 日期（YYYY-MM-DD）
* isNull  是否需要非空校验 1 需要 ,0不需要
* tips  字段的中文名称
*/
vldUtil.checkDesignatedDataInfos  = function (id, type, isNull, tips, checkid)
{
	checkid = $('#' + checkid);
	
    id = $('#' + id);
    var value = $.trim(id.val());
    var result = true;
    if (isNull == 1)
    {
        if (vldUtil.isEmpty(value))
        {
            id.addClass("error");
			if(vldUtil.isEmpty(checkid.attr("errorid"))){
				
				var alt = "<span>请输入" + tips + "</span>";
				checkid.html(alt);
				checkid.attr("errorid",id.attr("id"));
			}	
            return false;
        }
        return vldUtil.checkAll(result, type, value, tips, id,checkid);
    } else
    {
        if (!vldUtil.isEmpty(value))
        {
            return vldUtil.checkAll(result, type, value, tips, id,checkid);
        } else
        {
            id.removeClass("error");
			//标记 报错的元素id
			if(checkid.attr("errorid")==id.attr("id")){
				checkid.html("");
				checkid.attr("errorid","");
			}
            return true;
        }
    }
}

//校验函数
/*
* id 需要校验的id
* type 校验类型  checkPhone 校验手机号, checkTel 校验联系电话,checkMoney  金额, checkPostalcode 验证邮编,
* checkCnText  中文,checkCnOrEnValid  中英文,checkTextValid  中文、英文、数字,checkLetter 英文和空格,
* checkCard 身份证校验,checkBankNo  校验银行卡号,checkEmail  校验邮箱 checkDateStr 日期（YYYY-MM-DD）
* isNull  是否需要非空校验 1 需要 ,0不需要
* tips  字段的中文名称
*/
vldUtil.checkDataInfos  = function (id, type, isNull, tips)
{
    id = $('#' + id);
    var value = $.trim(id.val());
    var result = true;
    if (isNull == 1)
    {
        if (vldUtil.isEmpty(value))
        {
            id.addClass("error");
            var alt = "<span>请输入" + tips + "</span>";
            id.parent().next().html(alt);
            return false;
        }
        return vldUtil.checkAll(result, type, value, tips, id);
    } else
    {
        if (!vldUtil.isEmpty(value))
        {
            return vldUtil.checkAll(result, type, value, tips, id);
        } else
        {
            id.removeClass("error");
            id.parent().next().html("");
            return true;
        }
    }
}

vldUtil.checkAll  = function (result, type, value, tips, id,checkid)
{
    if ("checkPhone" == type)
    {
        result = result && vldUtil.checkPhone(value);
    } else if ("checkTel" == type)
    {
        result = result && vldUtil.checkTel(value);
    } else if ("checkMoney" == type)
    {
        result = result && vldUtil.checkMoney(value);
    } else if ("checkPostalcode" == type)
    {
        result = result && vldUtil.checkPostalcode(value);
    } else if ("checkCnText" == type)
    {
        result = result && vldUtil.checkCnText(value);
    } else if ("checkCnOrEnValid" == type)
    {
        result = result && vldUtil.checkCnOrEnValid(value);
    } else if ("checkTextValid" == type)
    {
        result = result && vldUtil.checkTextValid(value);
    } else if ("checkLetter" == type)
    {
        result = result && vldUtil.checkLetter(value);
    } else if ("checkCard" == type)
    {
        result = result && vldUtil.checkCard(value);
    } else if ("checkBankNo" == type)
    {
        result = result && vldUtil.checkBankNo(value);
    } else if ("checkEmail" == type)
    {
        result = result && vldUtil.checkEmail(value);
    }else if("checkDateStr0" == type || "checkDateStr1" == type || "checkDateStr2" == type){
	    
    	result = result && vldUtil.checkDateStr(value,type);
	}else if("checkSelect"==type)
	{
		result = result && vldUtil.checkSelect(value);
	}
    if (!result)
    {
		var alt = "";
		if("checkSelect"==type){
			alt = "<span>请选择" + tips + "</span>";
		}else{
			alt = "<span>请输入正确的" + tips + "</span>";
		}
		
        id.addClass("error");
		if(checkid){
			if(vldUtil.isEmpty(checkid.attr("errorid"))){
				checkid.html(alt);
				checkid.attr("errorid",id.attr("id"));
			}
		}else{
			id.parent().next().html(alt);
			return false;
		}
        
    } else
    {
        id.removeClass("error");
		if(checkid){
			if(checkid.attr("errorid")==id.attr("id")){
				checkid.html("");
				checkid.attr("errorid","")
			}
		}else{
			id.parent().next().html("");
		}
        return true;
    }
}

















//判空,加载错误样式(显示下边)(value=字段的值，name=字段的中文名称,obj=字段的对象,index=错误显示表格位置)
vldUtil.validateEmpty  = function (value, tips, obj,type)
{
	if($.string($.trim(value)).blank())
    {
		if("checkSelect"==type){
			vldUtil.addUnderErrCss(obj, "<span>请选择" + tips + "</span>", index);
		}else{
			vldUtil.addUnderErrCss(obj, "<span>请输入" + tips + "</span>", index);
		}
        return true;
    } else
    {
        return false;
    }
}
//特殊较验
vldUtil.checkDataInfos  = function (id, checkType, isNull, tips){
	id = $('#' + id);
    var value = $.trim(id.val());
    var result = true;
	if (isNull == 1)
    {
        if (vldUtil.validateEmpty(value, tips, id,checkType))
        {
            return false;
        }
        return vldUtil.checkAllUnder(result, checkType, value, tips, id);
    }
	else
    {
        if (!vldUtil.isEmpty(value))
        {
            return vldUtil.checkAllUnder(result, type, value, tips, id, index);
        } else
        {
            vldUtil.removeUnderErrorCss(id, index);
            return true;
        }
    }
}


//校验函数
/*
* id 需要校验的id
* type 校验类型  checkSelect 所有选择器  checkPhone 校验手机号, checkTel 校验电话号码,checkMoney  金额, checkPostalcode 验证邮编,
* checkCnText  中文,checkCnOrEnValid  中英文,checkTextValid  中文、英文、数字,checkLetter 英文和空格,
* checkCard 身份证校验,checkBankNo  校验银行卡号,checkEmail  校验邮箱 checkDateStr 日期（YYYY-MM-DD）
* isNull  是否需要非空校验 1 需要 ,0不需要, 9 必填时，判断不为空，但是输入格式任意  比如通讯地址
* tips  字段的中文名称 
*/
vldUtil.checkUnderDataInfos  = function (id, type, isNull, tips, index)
{
    id = $('#' + id);
    var value = $.trim(id.val());
    var result = true;
    if (isNull == 1)
    {
        if (vldUtil.validateUnderEmpty(value, tips, id, index,type))
        {
            return false;
        }
        return vldUtil.checkAllUnder(result, type, value, tips, id, index);
    }else if(isNull == 9){
    	if (vldUtil.validateUnderEmpty(value, tips, id, index,type))
        {
            return false;
        }
    	vldUtil.removeUnderErrorCss(id,index);
    	return true;
    }else
    {
        if (!vldUtil.isEmpty(value))
        {
            return vldUtil.checkAllUnder(result, type, value, tips, id, index);
        } else
        {
            vldUtil.removeUnderErrorCss(id, index);
            return true;
        }
    }

}

vldUtil.checkAllUnder  = function (result, type, value, tips, id, index)
{
    if ("checkPhone" == type)
    {
        result = result && vldUtil.checkPhone(value);
    } else if ("checkTel" == type)
    {
        result = result && vldUtil.checkTel(value);
    } else if ("checkMoney" == type)
    {
        result = result && vldUtil.checkMoney(value);
    } else if ("checkPostalcode" == type)
    {
        result = result && vldUtil.checkPostalcode(value);
    } else if ("checkCnText" == type)
    {
        result = result && vldUtil.checkCnText(value);
    } else if ("checkCnOrEnValid" == type)
    {
        result = result && vldUtil.checkCnOrEnValid(value);
    } else if ("checkTextValid" == type)
    {
        result = result && vldUtil.checkTextValid(value);
    } else if ("checkLetter" == type)
    {
        result = result && vldUtil.checkLetter(value);
    } else if ("checkCard" == type)
    {
        result = result && vldUtil.checkCard(value);
    } else if ("checkBankNo" == type)
    {
        result = result && vldUtil.checkBankNo(value);
    } else if ("checkEmail" == type)
    {
        result = result && vldUtil.checkEmail(value);
    } else if ("checkDateStr" == type || "checkDateStr1" == type || "checkDateStr2" == type)
    {
        result = result && vldUtil.checkDateStr(value,type);
    } else if("checkSelect"==type)
	{
		result = result && vldUtil.checkSelect(value);
	}
	

    if (!result)
    {
        vldUtil.addUnderErrorCss(id, tips, index,type);
        return false;
    } else
    {
        vldUtil.removeUnderErrorCss(id, index);
        return true;
    }
}



//金额大小写转换（数字转大小金额）
vldUtil.digitalToMoney  = function (moneyStr)
{
    var money = new Number(moneyStr);
    var monee = Math.round(money * 100).toString(10);
    var len = monee.length;
    var monval = "";
    for (var i = 0; i < len; i++)
    {
        monval = monval + vldUtil.to_upper(monee.charAt(i)) + vldUtil.to_mon(len - i - 1);
    }
	if(monval=="零分"){
		monval="零元";
	}
    return vldUtil.repace_acc(monval);
}

vldUtil.repace_acc  = function (money)
{
    money = money.replace("零分", "");
    money = money.replace("零角", "零");
    var yy = 0;
    var outmoney;
    outmoney = money;
    while (true)
    {
        var lett = outmoney.length;
        outmoney = outmoney.replace("零元", "元");
        outmoney = outmoney.replace("零万", "万");
        outmoney = outmoney.replace("零亿", "亿");
        outmoney = outmoney.replace("零仟", "零");
        outmoney = outmoney.replace("零佰", "零");
        outmoney = outmoney.replace("零零", "零");
        outmoney = outmoney.replace("零拾", "零");
        outmoney = outmoney.replace("亿万", "亿零");
        outmoney = outmoney.replace("万仟", "万零");
        outmoney = outmoney.replace("仟佰", "仟零");
        yy = outmoney.length;
        if (yy === lett)
        {
            break;
        }
    }
    yy = outmoney.length;
    if (outmoney.charAt(yy - 1) == '零')
    {
        outmoney = outmoney.substring(0, yy - 1);
    }
    yy = outmoney.length;
    if (outmoney.charAt(yy - 1) == '元')
    {
        outmoney = outmoney + '整';
    }
	
	if(money=="零元"){
		outmoney="零元整";
	}
 
    return outmoney;
}

vldUtil.to_upper  = function (a)
{
    switch (a)
    {
        case '0':
            return '零';
            break;
        case '1':
            return '壹';
            break;
        case '2':
            return '贰';
            break;
        case '3':
            return '叁';
            break;
        case '4':
            return '肆';
            break;
        case '5':
            return '伍';
            break;
        case '6':
            return '陆';
            break;
        case '7':
            return '柒';
            break;
        case '8':
            return '捌';
            break;
        case '9':
            return '玖';
            break;
        default:
            return '';
    }
}

vldUtil.to_mon  = function (a)
{
    if (a > 10)
    {
        a = a - 8;
        return (vldUtil.to_mon(a));
    }
    switch (a)
    {
        case 0:
            return '分';
            break;
        case 1:
            return '角';
            break;
        case 2:
            return '元';
            break;
        case 3:
            return '拾';
            break;
        case 4:
            return '佰';
            break;
        case 5:
            return '仟';
            break;
        case 6:
            return '万';
            break;
        case 7:
            return '拾';
            break;
        case 8:
            return '佰';
            break;
        case 9:
            return '仟';
            break;
        case 10:
            return '亿';
            break;
    }
}

//格式化金额：XX.XX
vldUtil.getChangMoney  = function (id,value)
{
    //var value = $("#" + id).val();
    //	if(value == "" || (value-0) < 1 || value.split(".").length >= 3){
    //		$("#" + id).val("");
    //		$("#" + id).addClass("error");
    //         $("#" + id).parent().next().html("<span>请输入金额，至多8位</span>");
    //		return;
    //	}
	//if(value - 0 <= 0){
	//	$("#"+id).val("");
	//	return;
	//}
    var frontValue = value, behindValue = "";
    if (-1 != value.indexOf("."))
    {
        var amtArray = value.split(".");
        frontValue = amtArray[0];

        if ("" == amtArray[0])
        {
            return;
        } else if (amtArray[1].length == 0)
        {
            behindValue = ".00";
        } else if (amtArray[1].length == 1)
        {
            behindValue = "." + amtArray[1] + "0";
        } else if (amtArray[1].length == 2)
        {
            behindValue = "." + amtArray[1];
        } else if (amtArray[1].length >= 3)
        {
            behindValue = "." + amtArray[1].substring(0, 2);
        }
    } else
    {
        behindValue = ".00";
    }

    frontValue = frontValue - 0; //减0作用：自动转化为金额数字，以便自动去除数字前的0
    $("#" + id).val(frontValue + behindValue);

    return (frontValue + behindValue);
}

/**
* 12345格式化为12,345.00 
* 12345.6格式化为12,345.60 
* 12345.67格式化为 12,345.67 
* s : 原始值 n : 保留的小数位
*/
vldUtil.getChangMoneyComma  = function (s, n)   
{
   if(s != ""){
	   n = n > 0 && n <= 20 ? n : 2;   
	   s = parseFloat((s + "").replace(/[^\d\.-]/g, "")).toFixed(n) + "";   
	   var l = s.split(".")[0].split("").reverse(),   
	   r = s.split(".")[1];   
	   t = "";   
	   for(i = 0; i < l.length; i ++ )   
	   {   
		  t += l[i] + ((i + 1) % 3 == 0 && (i + 1) != l.length ? "," : "");   
	   }   
	   return t.split("").reverse().join("") + "." + r;   
   }else{
       return "";
   }     
}

//还原格式化后的金额
vldUtil.backChangeMoney  = function (s)   
{   
   return parseFloat(s.replace(/[^\d\.-]/g, ""));   
}  

/*与当前日期比较大小
* value=待比较的日期,obj=待比较的对象,index=错误提示显示位置,minName=小的日期名称,maxName=大的日期名称
*/
vldUtil.compareNewDate  = function (value, obj, index, minName, maxName, flag)
{
    var year = value.substr(0, 4);
    var month = value.substr(4, 2);
    var date = value.substr(6, 2);
    var sdate = new Date(year, month, date);
    var stime = sdate.getTime();

    var nobj = new Date();
    var ndatestr = nobj.toString("yyyyMMdd");
    var nyear = ndatestr.substr(0, 4);
    var nmonth = ndatestr.substr(4, 2);
    var ndate = ndatestr.substr(6, 2);

    var nobjdate = new Date(nyear, nmonth, ndate);
    var etime = nobjdate.getTime();
    if (flag == 1)
    {
        if (stime > etime)
        {
            obj.addClass("error");
            obj.parent().parent().next().children().eq(index).html("<span>" + minName + "不能大于" + maxName + "</span>");
            return true;
        } else
        {
            return false;
        }
    } else
    {
        if (stime < etime)
        {
            obj.addClass("error");
            obj.parent().parent().next().children().eq(index).html("<span>" + minName + "不能大于" + maxName + "</span>");
            return true;
        } else
        {
            return false;
        }
    }
}
//日期比对 
//上面那个日期对比方法， 不全，新写的。--chenhaiyang
vldUtil.compareTime  = function (bt,et) {
	var byear = bt.substr(0,4);
	var bmonth = bt.substr(4,2);
	var bday = bt.substr(6,2);
	var eyear = et.substr(0,4);
	var emonth = et.substr(4,2);
	var eday = et.substr(6,2);
    var beginTime = new Date(byear,bmonth,bday);
    var endTime = new Date(eyear,emonth,eday);
    var a = endTime.getTime() - beginTime.getTime();
    if ( a < 0 ) {
        return false;
    } else {
    	return true;
    }
}

/*
 * 全角转半角 
 */
vldUtil.SBC2DBC  = function (str){ 
	var res = ""; 
	for(var i=0;i<str.length;i++){ 
		if(str.charCodeAt(i)>65248&&str.charCodeAt(i)<65375){ 
			res += String.fromCharCode(str.charCodeAt(i)-65248); 
		}else{ 
			res += String.fromCharCode(str.charCodeAt(i)); 
		} 
	} 
	return res; 
}
/*
* 拆分身份证地址字符串为    省   市   县(区)  详细地址  四段  
*/	
vldUtil.getAddressArray  = function (str) {	
	var tmpStr = str;
	var tmpArr = new Array();
	var isZxsFalg = false;
	var cs = new Array("省","自治区","地区","市","区","县"); //还有州 类的划分
	//"广州","沈阳","南京","武汉","成都","西安","长春","哈尔滨","济南","杭州"  副省级城市 可不带省
	var zxs = new Array("北京市","天津市","上海市","重庆市","香港特别行政区","澳门特别行政区","广州市","沈阳市","南京市","武汉市","成都市","西安市","长春市","哈尔滨市","济南市","杭州市");
	var fshcs = [["广东","广州"],["辽宁","沈阳"],["江苏","南京"],["湖北","武汉"],["四川","成都"],["陕西","西安"],["吉林","长春"],["黑龙江","哈尔滨"],["山东","济南"],["浙江","杭州"]];
	var resArr = new Array();
	for (var i = 0; i < cs.length; i++){
	   tmpStr = tmpStr.replace(cs[i].toString(),"|");
	}
	
	for (var j = 0; j < zxs.length; j++){
	   if(str.indexOf(zxs[j].toString())>=0){
			isZxsFalg = true;
	   }
	}
	
	if(isZxsFalg){
		resArr = new Array("").concat(tmpStr.split("|"));
		//补全省名
		if(""==resArr[0]){
			for (var k = 0; k < fshcs.length; k++){
			   if(resArr[1]==fshcs[k][1].toString()){
					resArr[0]=fshcs[k][0];
			   }
			}
		}
	}else{
		tmpArr = tmpStr.split("|");
		if(3==tmpArr.length){
			for(var i = 0;i<tmpArr.length;i++){
				if(i==1){
					resArr.push("");
				}
				resArr.push(tmpArr[i]);
			}
		}else{
			resArr = tmpArr;
		}
		
		//补全地级市名
		if(""==resArr[1]){
			
		}
	}
	return resArr;
}
/**
 * 字符串前后空格去掉
 */
String.prototype.trim = function(){
	return this.replace(/(^\s+)|(\s+$)/g, "");
};

/**
 * 字符串左空格去掉
 */
String.prototype.leftTrim = function(){
	return this.replace(/^\s+/, "");
};
/**
 * 去除字符串两边空格,以及中间的空格
 */
String.prototype.trimSpace = function(){
	return  this.replace(/\s/g,'');
};

/**
 * 字符串后空格去掉
 */
String.prototype.rightTrim = function(){
	return this.replace(/\s+$/, "");
};
/**
 * 将数字型字符串进行千分位格式化
 */
String.prototype.formatNum = function(){
	var num = this;
	if(!/^(\+|-)?(\d+)(\.\d+)?$/.test(num)){return num;}
	var a = RegExp.$1, b = RegExp.$2, c = RegExp.$3;
	var re=/(\d+)(\d{3})/;
	while(re.test(b)){ 
		b=b.replace(re,"$1,$2");
	} 
	return a +""+ b +""+c; 
};
/**
 * 将千分位格式化后的数字型字符串进行还原
 */
String.prototype.reFormateNum = function(){
	var x = this.split(',');
	x = x.join("");
	if(/^(\+|-)?(\d+)(\.\d+)?$/.test(x)){
		return x;
	}else{
		return this;
	}	
};

/*
*  格式化银行卡  四位加一空格
*/	
vldUtil.formatBankNo = function(str){ 
	return str.replace(/\s/g,'').replace(/(\d{4})(?=\d)/g,"$1 ");
}

/*
*  手机号  三  四位加一空格
*/	
vldUtil.formatPhone = function(str){
    var temp = str.replace(/\s/g, '')
    var newAcc = "";
    for (var n = 0; n < temp.length; n++)
    {
		if(n==2||n==6){
			newAcc += temp.substring(n,n+1) + " ";
		}else{
			newAcc += temp.substring(n,n+1);
		}
    }
    return newAcc;
}
/**
*   获取离obj最近的标签名为指定name的父层节点
*/
vldUtil.getParentByTagName = function(obj,name){
    while(obj=obj.parentNode){
      if(obj.tagName==name.toUpperCase()){
        return obj;
      }
    }
    return null;
};

/**
*  选择框公用方法（radio单选可空）
*/
vldUtil.selectIsNullRadio = function (id,value) {
	if (!value) {
		$("img[id^='"+id+"']").each(function(index, item) {
			this.src = "../../images/chkbox.png";
		});
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		return;
	}
	$("img[id^='"+id+"']").each(function(index, item) {
		this.src = "../../images/chkbox.png";
	});
	if ($("#"+id).val() == value) {
		$("#"+id + value).attr("src", "../../images/chkbox.png");
		$("#"+id).val("");
		$("#"+id+"Name").val("");
	} else {
		$("#"+ id + value).attr("src", "../../images/chkbox_c.png");
		$("#"+ id).val(value);
		$("#"+id+"Name").val($("#"+id + value).parent().text());
	}
}
/**
*  选择框公用方法（radio单选非空）
*/
vldUtil.selectIsNotNullRadio = function (id,value) {
	if (!value) {
		$("img[id^='"+id+"']").each(function(index, item) {
			this.src = "../../images/chkbox.png";
		});
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		return;
	}
	$("img[id^='"+id+"']").each(function(index, item) {
		this.src = "../../images/chkbox.png";
	});
	
	$("#"+ id + value).attr("src", "../../images/chkbox_c.png");
	$("#"+ id).val(value);
	$("#"+id+"Name").val($("#"+id + value).parent().text());
}
/**
*  选择框公用方法（radio单选可空）
*/
vldUtil.selectIsNullRadioQt = function (id,value,qtId) {
	if (!value) {
		$("img[id^='"+id+"']").each(function(index, item) {
			this.src = "../../images/chkbox.png";
		});
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		return;
	}
	if (value != qtId) {
		$("#"+id+"NameQt").val("");
	}
	$("img[id^='"+id+"']").each(function(index, item) {
		this.src = "../../images/chkbox.png";
	});
	if ($("#"+id).val() == value) {
		$("#"+id + value).attr("src", "../../images/chkbox.png");
		$("#"+id).val("");
		$("#"+id+"Name").val("");
	} else {
		$("#"+ id + value).attr("src", "../../images/chkbox_c.png");
		$("#"+ id).val(value);
		$("#"+id+"Name").val($("#"+id + value).parent().text());
	}
}
/**
*  选择框公用方法（radio单选非空）
*/
vldUtil.selectIsNotNullRadioQt = function (id,value,qtId) {
	if (!value) {
		$("img[id^='"+id+"']").each(function(index, item) {
			this.src = "../../images/chkbox.png";
		});
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		return;
	}
	if (value != qtId) {
		$("#"+id+"NameQt").val("");
	}
	$("img[id^='"+id+"']").each(function(index, item) {
		this.src = "../../images/chkbox.png";
	});
	
	$("#"+ id + value).attr("src", "../../images/chkbox_c.png");
	$("#"+ id).val(value);
	$("#"+id+"Name").val($("#"+id + value).parent().text());
}
/**
*  选择框公用方法（radioSymbolQt单选可空）
*/
vldUtil.selectIsNullRadioSymbol = function (id,value,symbol) {
	if (!value) {
		$("img[id^='"+id+"']").each(function(index, item) {
			this.src = "../../images/chkbox.png";
		});
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		$("#"+id+"Symbol").val("");
		return;
	}
	$("img[id^='"+id+"']").each(function(index, item) {
		this.src = "../../images/chkbox.png";
	});
	if ($("#"+id).val() == value) {
		$("#"+id + value).attr("src", "../../images/chkbox.png");
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		$("#"+id+"Symbol").val("");
	} else {
		$("#"+ id + value).attr("src", "../../images/chkbox_c.png");
		$("#"+ id).val(value);
		$("#"+id+"Name").val($("#"+id + value).parent().text());
		$("#"+id+"Symbol").val(symbol);
	}
}
/**
*  选择框公用方法（radioSymbol单选非空）
*/
vldUtil.selectIsNotNullRadioSymbol = function (id,value,symbol) {
	if (!value) {
		$("img[id^='"+id+"']").each(function(index, item) {
			this.src = "../../images/chkbox.png";
		});
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		$("#"+id+"Symbol").val("");
		return;
	}
	$("img[id^='"+id+"']").each(function(index, item) {
		this.src = "../../images/chkbox.png";
	});
	$("#"+ id + value).attr("src", "../../images/chkbox_c.png");
	$("#"+ id).val(value);
	$("#"+id+"Name").val($("#"+id + value).parent().text());
	$("#"+id+"Symbol").val(symbol);
}
/**
*  选择框公用方法（radioSymbolQt单选可空）
*/
vldUtil.selectIsNullRadioSymbolQt = function (id,value,symbol,qtId) {
	if (!value) {
		$("img[id^='"+id+"']").each(function(index, item) {
			this.src = "../../images/chkbox.png";
		});
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		$("#"+id+"NameQt").val("");
		$("#"+id+"Symbol").val("");
		return;
	}
	if (value != qtId) {
		$("#"+id+"NameQt").val("");
	}
	$("img[id^='"+id+"']").each(function(index, item) {
		this.src = "../../images/chkbox.png";
	});
	if ($("#"+id).val() == value) {
		$("#"+id + value).attr("src", "../../images/chkbox.png");
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		$("#"+id+"Symbol").val("");
	} else {
		$("#"+ id + value).attr("src", "../../images/chkbox_c.png");
		$("#"+ id).val(value);
		$("#"+id+"Name").val($("#"+id + value).parent().text());
		$("#"+id+"Symbol").val(symbol);
	}
}
/**
*  选择框公用方法（radioSymbolQt单选非空）
*/
vldUtil.selectIsNotNullRadioSymbolQt = function (id,value,symbol,qtId) {
	if (!value) {
		$("img[id^='"+id+"']").each(function(index, item) {
			this.src = "../../images/chkbox.png";
		});
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		$("#"+id+"NameQt").val("");
		$("#"+id+"Symbol").val("");
		return;
	}
	if (value != qtId) {
		$("#"+id+"NameQt").val("");
	}
	$("img[id^='"+id+"']").each(function(index, item) {
		this.src = "../../images/chkbox.png";
	});
	$("#"+ id + value).attr("src", "../../images/chkbox_c.png");
	$("#"+ id).val(value);
	$("#"+id+"Name").val($("#"+id + value).parent().text());
	$("#"+id+"Symbol").val(symbol);
}

/*  biz
*/

/**
*  选择框公用方法（radio单选可空）
*/
vldUtil.selectIsNullRadio4biz = function (id,value) {
	if (!value) {
		$("img[id^='"+id+"']").each(function(index, item) {
			this.src = "../../../images/chkbox.png";
		});
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		return;
	}
	$("img[id^='"+id+"']").each(function(index, item) {
		this.src = "../../../images/chkbox.png";
	});
	if ($("#"+id).val() == value) {
		$("#"+id + value).attr("src", "../../../images/chkbox.png");
		$("#"+id).val("");
		$("#"+id+"Name").val("");
	} else {
		$("#"+ id + value).attr("src", "../../../images/chkbox_c.png");
		$("#"+ id).val(value);
		$("#"+id+"Name").val($("#"+id + value).parent().text());
	}
}
/**
*  选择框公用方法（radio单选非空）
*/
vldUtil.selectIsNotNullRadio4biz = function (id,value) {
	if (!value) {
		$("img[id^='"+id+"']").each(function(index, item) {
			this.src = "../../../images/chkbox.png";
		});
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		return;
	}
	$("img[id^='"+id+"']").each(function(index, item) {
		this.src = "../../../images/chkbox.png";
	});
	
	$("#"+ id + value).attr("src", "../../../images/chkbox_c.png");
	$("#"+ id).val(value);
	$("#"+id+"Name").val($("#"+id + value).parent().text());
}
/**
*  选择框公用方法（radio单选可空）
*/
vldUtil.selectIsNullRadioQt4biz = function (id,value,qtId) {
	if (!value) {
		$("img[id^='"+id+"']").each(function(index, item) {
			this.src = "../../../images/chkbox.png";
		});
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		return;
	}
	if (value != qtId) {
		$("#"+id+"NameQt").val("");
	}
	$("img[id^='"+id+"']").each(function(index, item) {
		this.src = "../../../images/chkbox.png";
	});
	if ($("#"+id).val() == value) {
		$("#"+id + value).attr("src", "../../../images/chkbox.png");
		$("#"+id).val("");
		$("#"+id+"Name").val("");
	} else {
		$("#"+ id + value).attr("src", "../../../images/chkbox_c.png");
		$("#"+ id).val(value);
		$("#"+id+"Name").val($("#"+id + value).parent().text());
	}
}
/**
*  选择框公用方法（radio单选非空）
*/
vldUtil.selectIsNotNullRadioQt4biz = function (id,value,qtId) {
	if (!value) {
		$("img[id^='"+id+"']").each(function(index, item) {
			this.src = "../../../images/chkbox.png";
		});
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		return;
	}
	if (value != qtId) {
		$("#"+id+"NameQt").val("");
	}
	$("img[id^='"+id+"']").each(function(index, item) {
		this.src = "../../../images/chkbox.png";
	});
	
	$("#"+ id + value).attr("src", "../../../images/chkbox_c.png");
	$("#"+ id).val(value);
	$("#"+id+"Name").val($("#"+id + value).parent().text());
}
/**
*  选择框公用方法（radioSymbolQt单选可空）
*/
vldUtil.selectIsNullRadioSymbol4biz = function (id,value,symbol) {
	if (!value) {
		$("img[id^='"+id+"']").each(function(index, item) {
			this.src = "../../../images/chkbox.png";
		});
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		$("#"+id+"Symbol").val("");
		return;
	}
	$("img[id^='"+id+"']").each(function(index, item) {
		this.src = "../../../images/chkbox.png";
	});
	if ($("#"+id).val() == value) {
		$("#"+id + value).attr("src", "../../../images/chkbox.png");
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		$("#"+id+"Symbol").val("");
	} else {
		$("#"+ id + value).attr("src", "../../../images/chkbox_c.png");
		$("#"+ id).val(value);
		$("#"+id+"Name").val($("#"+id + value).parent().text());
		$("#"+id+"Symbol").val(symbol);
	}
}
/**
*  选择框公用方法（radioSymbol单选非空）
*/
vldUtil.selectIsNotNullRadioSymbol4biz = function (id,value,symbol) {
	if (!value) {
		$("img[id^='"+id+"']").each(function(index, item) {
			this.src = "../../../images/chkbox.png";
		});
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		$("#"+id+"Symbol").val("");
		return;
	}
	$("img[id^='"+id+"']").each(function(index, item) {
		this.src = "../../../images/chkbox.png";
	});
	$("#"+ id + value).attr("src", "../../../images/chkbox_c.png");
	$("#"+ id).val(value);
	$("#"+id+"Name").val($("#"+id + value).parent().text());
	$("#"+id+"Symbol").val(symbol);
}
/**
*  选择框公用方法（radioSymbolQt单选可空）
*/
vldUtil.selectIsNullRadioSymbolQt4biz = function (id,value,symbol,qtId) {
	if (!value) {
		$("img[id^='"+id+"']").each(function(index, item) {
			this.src = "../../../images/chkbox.png";
		});
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		$("#"+id+"NameQt").val("");
		$("#"+id+"Symbol").val("");
		return;
	}
	if (value != qtId) {
		$("#"+id+"NameQt").val("");
	}
	$("img[id^='"+id+"']").each(function(index, item) {
		this.src = "../../../images/chkbox.png";
	});
	if ($("#"+id).val() == value) {
		$("#"+id + value).attr("src", "../../../images/chkbox.png");
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		$("#"+id+"Symbol").val("");
	} else {
		$("#"+ id + value).attr("src", "../../../images/chkbox_c.png");
		$("#"+ id).val(value);
		$("#"+id+"Name").val($("#"+id + value).parent().text());
		$("#"+id+"Symbol").val(symbol);
	}
}
/**
*  选择框公用方法（radioSymbolQt单选非空）
*/
vldUtil.selectIsNotNullRadioSymbolQt4biz = function (id,value,symbol,qtId) {
	if (!value) {
		$("img[id^='"+id+"']").each(function(index, item) {
			this.src = "../../../images/chkbox.png";
		});
		$("#"+id).val("");
		$("#"+id+"Name").val("");
		$("#"+id+"NameQt").val("");
		$("#"+id+"Symbol").val("");
		return;
	}
	if (value != qtId) {
		$("#"+id+"NameQt").val("");
	}
	$("img[id^='"+id+"']").each(function(index, item) {
		this.src = "../../../images/chkbox.png";
	});
	$("#"+ id + value).attr("src", "../../../images/chkbox_c.png");
	$("#"+ id).val(value);
	$("#"+id+"Name").val($("#"+id + value).parent().text());
	$("#"+id+"Symbol").val(symbol);
}
/**
 * PlaceHolder组件
 * $(input).placeholder({
 *   word:     // string 提示文本
 *   color:    // string 文本颜色
 *   evtType:  // string focus|keydown 触发placeholder的事件类型
 * })
 *
 * NOTE：
 *   evtType默认是focus，即鼠标点击到输入域时默认文本消失，keydown则模拟HTML5 placeholder属性在Firefox/Chrome里的特征，光标定位到输入域后键盘输入时默认文本才消失。
 *   此外，对于HTML5 placeholder属性，IE10+和Firefox/Chrome/Safari的表现形式也不一致，因此内部实现不采用原生placeholder属性
 */
$.fn.placeholder = function(option, callback) {
    var settings = $.extend({
        word: '',
        color: '#ccc',
        evtType: 'focus'
    }, option)

    function bootstrap($that) {
        // some alias 
        var word    = settings.word
        var color   = settings.color
        var evtType = settings.evtType
        // default
        var defColor = $that.css('color')
        var defVal   = $that.val()
        if (defVal == '' || defVal == word) {
            $that.css({color: color}).val(word)
        } else {
            $that.css({color: defColor})
        }

        function switchStatus(isDef) {
            if (isDef) {
                $that.val('').css({color: defColor})    
            } else {
                $that.val(word).css({color: color})
            }
        }

        function asFocus() {
            $that.bind(evtType, function() {
                var txt = $that.val()
                if (txt == word) {
                    switchStatus(true)
                }
            }).bind('blur', function() {
                var txt = $that.val()
                if (txt == '') {
                    switchStatus(false)
                }
            })

        }

        function asKeydown() {
            $that.bind('focus', function() {
                var elem = $that[0]
                var val  = $that.val()
                if (val == word) {
                    setTimeout(function() {
                        // 光标定位到首位
                        $that.setCursorPosition({index: 0})
                    }, 10)                  
                }
            })
        }

 

        if (evtType == 'focus') {
            asFocus()
        } else if (evtType == 'keydown') {
            asKeydown()
        }

        // keydown事件里处理placeholder
        $that.keydown(function() {
            var val = $that.val()
            if (val == word) {
                switchStatus(true)
            }
        }).keyup(function() {
            var val = $that.val()
            if (val == '') {
                switchStatus(false)
                $that.setCursorPosition({index: 0})
            }
        })
    }

    return this.each(function() {
        var $elem = $(this)
        bootstrap($elem)
        if ($.isFunction(callback)) callback($elem)
    })

}



