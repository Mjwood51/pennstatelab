/*
* jQuery modal plugin
* version 3.0
* Created by Steven Ye (steven_ye@foxmail.com) on 2014-10-22.
* MIT license
*
*/
;(function($, window, document,undefined) {
	//定义Modal的构造函数
    var Modal = function(ele, opt) {
        this.$element = ele,
        this.defaults = {
            version: '3.0',
			width:400,
			height:300,
			showSpeed: 500,
			closeSpeed: 500,
			position:{},
			autoClose:0,
			buttons:{},
			message:'',
			overlay:true,
			shadow:true,
			autoOpen: false,
			title:true,
			draggable:false,
			action:'fade',
			skin:''
        },
        this.options = $.extend({}, this.defaults, $.fn.modal.defaults, opt),
		this.modal = $('<div class="modal_window"><h2><p></p><span></span></h2><div></div><p></p>'),
		this.overlay = $('<div>')
    }
    //定义Modal的方法
    Modal.prototype = {
        init: function() {
			var that = this,settings = this.options,
			    h = this.$element.attr('href'),
				modal = this.modal,overlay = this.overlay,
			    modal_head = $(modal).children('h2'),
			    modal_body = $(modal).children('div').empty(),
			    modal_foot = $(modal).children('p').empty(),
			    modal_title = $(modal_head).children('p').empty(),
			    modal_span = $(modal_head).children('span').html("&times;").click(function(){that.hide();return false});
				
				if(h.charAt(0)=='#'){$(h).hide();}
				
				if(settings.title){
					modal_title.text(that.$element.attr('title'));
					modal_head.show();
				}else{
					modal_head.hide();
				}
				modal_body.css({
			 	"max-height":settings.height+"px",
			 	"overflow":"auto",
			 	"display":"block"
				});
				
				modal.css({"width":settings.width+"px"});
				if($.isEmptyObject(settings.position)){
					modal.css({
			 	        "top":($(window).height()-settings.height)/2+"px",
			 	        "left":($(window).width()-settings.width)/2+"px"
		    	    });
				}else{
				    modal.css(settings.position);
				}
			
				//customized buttons
					if($.isEmptyObject(settings.buttons)){
						modal_foot.hide();
					}else{
						modal_foot.show();
						$.each(settings.buttons,function(name,callback){
							var button = $('<a>',{text: name}).appendTo(modal_foot);
							if(typeof callback == 'function'){
								button.click(function(){callback();return false});
							}else if(typeof callback == 'string'){
								button.click(function(){that[callback]();return false});
							}
							
						});
					}
			
			if(settings.shadow)this.modal.addClass('shadow');
			if(settings.skin)this.modal.addClass(settings.skin);
			
			if(settings.overlay){this.overlay.addClass('overlay');}
			else{this.overlay.removeClass('overlay');}
			this.overlay.click(function(){that.hide();return false});
			
			this.$element.click(function(e){
				e.preventDefault();
				
				that.show();
			});
			if(settings.autoOpen)this.show();
        },
		show: function() {
			var that = this, settings = this.options,
			    h = this.$element.attr('href'),
				m = this.$element.attr('message'),
				modal_body = this.modal.children('div');
									
				if(h.charAt(0)!='#'){
					modal_body.html('<div class="modal_loader"></div>');  //show loading.gif
				    
			    	modal_body.load(h,function(r,e,q){
				        if(e=='error'){
				    	    modal_body.html("Failed to load the file: "+ h);
				        }
			    	});
			    }else if($(h).length>0){
			    	modal_body.html($(h).html());
			    }else if(h.length==1){
					modal_body.html(m?m:settings.message);
				}else{
			        modal_body.html("This element doesn't exist: "+h);
			    }
			
			this.overlay.appendTo('body').fadeIn(settings.openSpeed);
			
			if(settings.draggable){
				this.modal.children('h2,p').mousedown(function(e){
					var isMove = true,
					    abs_x = e.pageX - that.modal.offset().left;
					    abs_y = e.pageY - that.modal.offset().top;
					$(document).mousemove(function (e) {
					    if (isMove) {
					       that.modal.css({'left':e.pageX - abs_x, 'top':e.pageY - abs_y});
					    }
					}).mouseup(function () {
					    isMove = false;
					});
				});
			}
			
			this.modal.appendTo('body');
			if(settings.autoClose>0){
				this.modal.fadeIn(settings.openSpeed,function(){
					setTimeout(function(){that.hide()},settings.autoClose)
				});
			}else{
				this.modal.fadeIn(settings.openSpeed);
			}
		},
		hide: function() {
			var settings = this.options;
			this.overlay.fadeOut(settings.closeSpeed, function(){$(this).detach()});
			this.modal.fadeOut(settings.closeSpeed, function(){$(this).detach()});
		},
		version : function(){
			alert('jQuery.modal v3.0 by Steven Ye (steven_ye@foxmail.com');
		}
    }
    //在插件中使用Modal对象
    $.fn.modal = function(options) {
		return this.each(function(){
			var $this = $(this),
                data = $this.data('modal');
			if(!data){
			    //创建Modal的实体
			    $this.data('modal',(data = new Modal($(this), options)));
			    //调用其方法
			    data.init();
			    if(typeof options == 'string') data[options];
			}
		});
    }
	
	$.fn.modal.defaults = {}
	
})(jQuery, window, document);