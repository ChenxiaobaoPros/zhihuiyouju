function getZoom(imgW,imgH,divW,divH){
	var res;
	if(divW/imgW>=divH/imgH){
	    res = divH/imgH;
	}else{
		res = divW/imgW;
	}
	//alert(Math.floor(res*100)/100);
	return Math.floor(res*100)/100;
}

function getDivLeft(imgW,divW,zoom){
	var res = (divW-imgW*zoom)/2;
	return res;
}

function Map(id,width, height, src, backWidth, backHeight,backSrc,zoom)
{
    var m = this;
    m.viewingBox = $("#" + id);
	m.viewingBox.empty();
	m.dtZoom = 1;
	//alert($("#"+id).css("width"));
	m.zoomqp = getZoom(width,height,$("#"+id).css("width").replace('px', ''),$("#"+id).css("height").replace('px', ''));
	if(!zoom){
		zoom = m.zoomqp;
	}
	m.moveImage = true;
    m.mouseDown = false;
    m.lastPosition = new Point();
	
    m.front = true;
    m.innerMap = $('<div></div>');
    m.innerMap.css("position", "absolute").css("width", width).css("height", height).css("background-image", "url(" + src + ")").css("zoom",zoom).css("left",getDivLeft(width,$("#"+id).css("width").replace('px', ''),zoom)+"px");
    m.viewingBox.append(m.innerMap);

    if (backSrc)
    {
		m.zoomqp = getZoom(backWidth,backHeight,$("#"+id).css("width").replace('px', ''),$("#"+id).css("height").replace('px', ''));
        if(!zoom){
			zoom = m.zoomqp;
		}
		m.innerBackMap = $('<div></div>');
        m.innerBackMap.css("position", "absolute").css("width", backWidth).css("height", backHeight).css("background-image", "url(" + backSrc + ")").css("zoom",zoom).css("left",getDivLeft(backWidth,$("#"+id).css("width").replace('px', ''),zoom)+"px");
        m.viewingBox.append(m.innerBackMap);
        m.innerBackMap.hide();

        m.toggle = function ()
        {
            if (m.front)
            {
                m.front = false;
                m.innerMap.hide();
                m.innerBackMap.show();
            }
            else
            {
                m.front = true;
                m.innerMap.show();
                m.innerBackMap.hide();
            }

            if (m.afterToggle)
            {
                m.afterToggle();
            }
        };
    }

	m.alingCenter = function ()
	{
		if (backSrc){
			if (m.front)
			{
				m.innerMap.css("left",getDivLeft(width,$("#"+id).css("width").replace('px', ''),m.dtZoom)+"px").css("top","0px");
			}
			else
			{
				m.innerBackMap.css("left",getDivLeft(width,$("#"+id).css("width").replace('px', ''),m.dtZoom)+"px").css("top","0px");
			}
		}else{
			m.innerMap.css("left",getDivLeft(width,$("#"+id).css("width").replace('px', ''),m.dtZoom)+"px").css("top","0px");
		}
		
	};
	
	m.shuofang = function (flag)
	{
		var num = 10;
		
		$("#"+id+" div").each(function (index, item)
		{
			var zoomt = 1;
			if(this.style.zoom) zoomt = this.style.zoom;
			
			if(this.style.display!='none'){
				
				if('sx'==flag){
					if(this.style.zoom<=0.1){
						//this.style.zoom = 0.1;
					}else{
						if(parseFloat(zoomt)>num/100){
							this.style.zoom = parseFloat(zoomt)-num/100;
						}else{
							//this.style.zoom = 0.1;
						}
					}
				}else if('fd'==flag){
					if(parseFloat(zoomt)>=1){
						this.style.zoom = 1;
					}else{
						this.style.zoom = parseFloat(zoomt) + num/100;
					}
					
				}else if('qp'==flag){//È«ÆÁ
					this.style.zoom = m.zoomqp;
					
				}else if('yt'==flag){//Ô­Í¼
					this.style.zoom = 1;
				}
				m.dtZoom = this.style.zoom;
				m.alingCenter();
			}
			//alert(m.dtZoom);
		});
		
	}
	
	
    m.viewingBox.mousedown(function (event)
    {
        m.mouseDown = true;
		//alert()
        m.lastPosition.x = event.pageX;
        m.lastPosition.y = event.pageY;
    });

    m.viewingBox.mouseup(function (event)
    {
        m.mouseDown = false;
    });

    m.viewingBox.mouseleave(function (event)
    {
        m.mouseDown = false;
    });

	
    m.viewingBox.mousemove(function (event)
    {
        if (m.mouseDown&&m.moveImage)
        {
            var dx = event.pageX - m.lastPosition.x;
            var dy = event.pageY - m.lastPosition.y;

            var pos = m.front ? m.innerMap.position() : m.innerBackMap.position();
            var newPos = pos;
            pos.left += dx;
            pos.top += dy;

            //pos.left = (pos.left > 0) ? 0 : pos.left;
            //pos.top = (pos.top > 0) ? 0 : pos.top;

            var ox = m.viewingBox.width() - width;
            var oy = m.viewingBox.height() - height;

            pos.left = (pos.left < ox) ? ox : pos.left;
            pos.top = (pos.top < oy) ? oy : pos.top;

            if (m.front)
            {
                m.innerMap.css({
                    "left": (pos.left) + "px",
                    "top": (pos.top) + "px"
                });
            }
            else
            {
                m.innerBackMap.css({
                    "left": (pos.left) + "px",
                    "top": (pos.top) + "px"
                });
            }

            m.lastPosition.x = event.pageX;
            m.lastPosition.y = event.pageY;
			
			

            event.preventDefault();
        }
    });
	
}

function Point()
{
    this.x = 0;
    this.y = 0;
}

