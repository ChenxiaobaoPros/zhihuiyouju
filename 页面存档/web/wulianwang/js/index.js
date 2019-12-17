$(document).ready(function(){

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


});
