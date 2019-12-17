$(document).ready(function(){
    $(function (){
        var info = window.external.LoadOverView();
        //$('.abc').html(info)
        var obj= JSON.parse(info);
        var html ="";
        for(var i=0; i<obj.length; i++){
             var deives=obj[i].deiveList;
            html = html + '<div class="seb">';
                html = html + '<h2>' + obj[i].appName + '</h2>';
            html = html + '<p class="appID">' + obj[i].appID + '</p>';
                html = html + '<div class="content_div clearfix">';
                    html = html + '<div class="zc_left_zong">';
                        html = html + '<div class="content_zc">';
                            html = html + '<p><span class="zc_span2">'+ obj[i].online +'/</span><span class="zc_span3">'+obj[i].all +'</span></p>';
                            html = html + '<p>物联网设备正常数</p>';
                        html = html + '</div>';
                        html = html + '<div class="content_zc3">';
                            html = html + '<h2>设备状态</h2>';
                            html = html + '<div class="sbzx line">';
                            for (j=0; j<deives.length; j++){
                                html = html + '<div>';
                                    html = html + '<ul class="">';
                                         html = html + '<li>';
                                            html = html + '<p class="clearfix"><span class="deviceName">'+ deives[j].deviceName +'：</span><i class="ciyao">'+ deives[j].state +'</i></p>';
                                         html = html + '</li>';
                                     html = html + '</ul>';
                               html = html + '</div>';
                            }
                            html = html + '</div>';
                        html = html + '</div>';
                    html = html + '</div>';
                    html = html + '<div class="zc_right_zong" datas="'+obj[i].appID+'">';
                         html = html + '<img src="../img/yjian.jpg">';
                    html = html + '</div>';
                html = html + '</div>';
            html = html + '</div>';
        }
        $('.acc').html(html);


        $(function(){
            //文字滚动
            $(".line").slideUp();
        });

        $(function(){

            $(".zc_right_zong").on("click",function(){
                $(".content").hide();
                $(".xiangqing").show();
                var app_id = $(this).attr("datas")
                var details =window.external.LoadManager(app_id);
                var kongtiao= JSON.parse(details);
                //$('.abc').html(details)
                var html2 ="";
                //获取插排
                for (var z=0; z<kongtiao.LoRaPlug.length; z++){
                    //alert(kongtiao.LoRaAirPanel[z].data)
                    var chapai= kongtiao.LoRaPlug[z].data;
                    //alert(chapai.deive_name)
                    html2 = html2 + '<div class="seb2">';
                        html2 = html2 + '<h2> '+ kongtiao.LoRaPlug[z].group_name + '</h2>';
                        html2 = html2 + '<div class="clearfix content_div2">';
                            html2 = html2 + '<div class="zc_left_zong">';
                                 html2 = html2 + '<div class="content_zc2">';
                                    html2 = html2 + '<p>物联网设备一键开启</p>';
                                                if(kongtiao.LoRaPlug[z].Online_State ==false){
                                                        html2 = html2 + '<div class="lixian">设备离线</div>';
                                                }else if(kongtiao.LoRaPlug[z].Open_State ==true){

                                                    html2 = html2 + '<div class="toggle">';
                                                        html2 = html2 + '<div class="toggle-text-off">开启</div>';
                                                        html2 = html2 + '<div class="glow-comp"></div>';
                                                        html2 = html2 + '<div class="toggle-button"></div>';
                                                        html2 = html2 + '<div class="toggle-text-on">关闭</div>';
                                                    html2 = html2 + '</div>';
                                                }else {
                                                    html2 = html2 + '<div class="toggle toggle-on">';
                                                        html2 = html2 + '<div class="toggle-text-off">开启</div>';
                                                        html2 = html2 + '<div class="glow-comp"></div>';
                                                        html2 = html2 + '<div class="toggle-button"></div>';
                                                        html2 = html2 + '<div class="toggle-text-on">关闭</div>';
                                                    html2 = html2 + '</div>';
                                                }

                                 html2 = html2 + '</div>';
                                 html2 = html2 + '<div class="content_zc4">';
                                                if(kongtiao.LoRaPlug[z].lastDate ==""){
                                                    html2 = html2 + '<h4 class="data_h4" style="color: red;">设备离线</h4>';
                                                }else {
                                                    html2 = html2 + '<h4 class="data_h4">'+ kongtiao.LoRaPlug[z].lastDate + '</h4>';
                                                }
                                     html2 = html2 + '<div class="sbzx2">';
                                        html2 = html2 + '<p class="clearfix"><span class="deviceName2">设备名称：</span><i class="ciyao2">'+ chapai.deive_name +'</i></p>';
                                        html2 = html2 + '<p class="clearfix"><span class="deviceName2">电压：</span><i class="ciyao2">'+ chapai.deive_v +'</i></p>';
                                        html2 = html2 + '<p class="clearfix"><span class="deviceName2">电流：</span><i class="ciyao2">'+ chapai.deive_i +'</i></p>';
                                     html2 = html2 + '</div>';
                                 html2 = html2 + '</div>';
                            html2 = html2 + '</div>';
                            html2 = html2 + '<div class="zc_right_zong2">';
                                html2 = html2 + '<img src="../img/cp.jpg">';
                            html2 = html2 + '</div>';
                        html2 = html2 + '</div>';
                    html2 = html2 + '</div>';
                };
                //获取温度
                for (var b=0; b<kongtiao.LoRaTempHumid.length; b++){
                    var wendu= kongtiao.LoRaTempHumid[b].data;
                    html2 = html2 + '<div class="seb2">';
                        html2 = html2 + '<h2> '+ kongtiao.LoRaTempHumid[b].group_name + '</h2>';
                        html2 = html2 + '<div class="clearfix content_div2">';
                            html2 = html2 + '<div class="zc_left_zong">';
                                                html2 = html2 + '<div class="content_zc5">';
                                                if(kongtiao.LoRaTempHumid[b].lastDate ==""){
                                                    html2 = html2 + '<h4 class="data_h4" style="color: red;">设备离线</h4>';
                                                }else {
                                                    html2 = html2 + '<h4 class="data_h4">'+ kongtiao.LoRaTempHumid[b].lastDate + '</h4>';
                                                }
                                                    html2 = html2 + '<div class="sbzx2">';
                                                    html2 = html2 + '<p class="clearfix"><span class="deviceName2">设备名称：</span><i class="ciyao2">'+ wendu.deive_name +'</i></p>';
                                                    html2 = html2 + '<p class="clearfix"><span class="deviceName2">温度：</span><i class="ciyao2">'+ wendu.deive_temperature +'</i></p>';
                                                    html2 = html2 + '<p class="clearfix"><span class="deviceName2">湿度：</span><i class="ciyao2">'+ wendu.deive_humidity +'</i></p>';
                                                    html2 = html2 + '</div>';
                                                html2 = html2 + '</div>';
                            html2 = html2 + '</div>';
                            html2 = html2 + '<div class="zc_right_zong2">';
                                html2 = html2 + '<img src="../img/wd.jpg">';
                            html2 = html2 + '</div>';
                        html2 = html2 + '</div>';
                    html2 = html2 + '</div>';
                }
                //获取烟感器
                for (var y=0; y<kongtiao.SmokeSensor.length; y++){
                    var yangan= kongtiao.SmokeSensor[y].data;
                    html2 = html2 + '<div class="seb2">';
                        html2 = html2 + '<h2> '+ kongtiao.SmokeSensor[y].group_name + '</h2>';
                        html2 = html2 + '<div class="clearfix content_div2">';
                            html2 = html2 + '<div class="zc_left_zong">';
                                html2 = html2 + '<div class="content_zc5">';
                                    if(kongtiao.SmokeSensor[y].lastDate ==""){
                                        html2 = html2 + '<h4 class="data_h4" style="color: red;">设备离线</h4>';
                                    }else {
                                        html2 = html2 + '<h4 class="data_h4">'+ kongtiao.SmokeSensor[y].lastDate + '</h4>';
                                    }
                                        html2 = html2 + '<div class="sbzx2">';
                                            html2 = html2 + '<p class="clearfix"><span class="deviceName2">设备名称：</span><i class="ciyao2">'+ yangan.deive_name +'</i></p>';
                                            html2 = html2 + '<p class="clearfix"><span class="deviceName2">温度：</span><i class="ciyao2">'+ yangan.deive_smoke +'</i></p>';
                                        html2 = html2 + '</div>';
                                   html2 = html2 + '</div>';
                              html2 = html2 + '</div>';
                              html2 = html2 + '<div class="zc_right_zong2">';
                                 html2 = html2 + '<img src="../img/yangan.jpg">';
                            html2 = html2 + '</div>';
                         html2 = html2 + '</div>';
                    html2 = html2 + '</div>';
                }
                //获取开关
                // for (var v=0; v<kongtiao.SmokeSensor.length; v++){
                //     alert(kongtiao.SmokeSensor[v].data)
                //     //var chapai= kongtiao.LoRaPlug[z].data;
                //
                // };

                $(".ktiao").html(html2);
                //alert($('.data_h4').html())

                $('.toggle').click(function(e){
                    //开启设备groupId,deviceId": "GEK9320663,1,0
                    e.preventDefault(); // The flicker is a codepen thing
                    //$('.groupId').html()
                    $(this).toggleClass('toggle-on');
                    //var ccc = $(this).hasClass('toggle-on');//判断一个元素有没有class,有返回true，没有返回false

                });
            })
        });

    });



    $('.changj3').hover(function(){

        $(".yh_ty").addClass("yh_span3");
    },function(){
        $(".yh_ty").removeClass("yh_span3");
    });
    $('.changj2').hover(function(){

        $(".cj").addClass("tongjji_span3");
    },function(){
        $(".cj").removeClass("tongjji_span3");
    });
    $('.changj1').hover(function(){

        $(".chji").addClass("changj_span3");
    },function(){
        $(".chji").removeClass("changj_span3");
    });
    $(function(){
        $(".changj3").click(function() {

            $(".yh_span2"). toggleClass("yh_span4");
            $(".changj3_none").toggle(1000);
        });
        $(".changj2").click(function() {

            $(".tongjji_span2"). toggleClass("yh_span4");
            $(".tj").toggle(1000);
        });
        $(".changj1").click(function() {

            $(".changj_span2"). toggleClass("yh_span4");
            $(".kj").toggle(1000);
        });

    });
    $(function(){
        $(".tiao").click(function() {

            $(".content").show();
            $(".xiangqing").hide();

        });
    });

});
