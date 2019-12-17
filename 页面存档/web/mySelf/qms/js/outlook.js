var $centerFrame,$wnav;

$(function()
{
    $wnav = $("#wnav");
	$centerFrame = $("#centerFrame");
});

function Clearnav()
{
    var pp = $wnav.accordion('panels');

    $.each(pp, function(i, n)
    {
        if (n)
        {
            var t = n.panel('options').title;
            $wnav.accordion('remove', t);
        }
    });

    pp = $wnav.accordion('getSelected');
    if (pp)
    {
        var title = pp.panel('options').title;
        $wnav.accordion('remove', title);
    }
}

function initCenterFrame(url){
	$centerFrame.attr("src",url);
}