/*jquery.cascade.js */
/*
* jQuery UI cascade
* version: 1.1.1 (6/16/2008)
* @requires: jQuery v1.2 or later
* adapted from Yehuda Katz, Rein Henrichs autocomplete plugin
* Dual licensed under the MIT and GPL licenses:
*   http://www.opensource.org/licenses/mit-license.php
*   http://www.gnu.org/licenses/gpl.html
*
* Copyright 2008 Mike Nichols
   	
*/

/*jquery.cascade.ui.ext.js */
/*
* jQuery UI cascade
* version: 1.1 (5/20/2008)
* @requires: jQuery v1.2 or later
* adapted from Yehuda Katz, Rein Henrichs autocomplete plugin
* Dual licensed under the MIT and GPL licenses:
*   http://www.opensource.org/licenses/mit-license.php
*   http://www.gnu.org/licenses/gpl.html
*  depends on templating  plugin if using with templateText
* Copyright 2008 Mike Nichols
*/

; (function($) {
    $.ui = $.ui || {};
    $.ui.cascade = $.ui.cascade || {};
    $.ui.cascade.ext = $.ui.cascade.ext || {};
    $.ui.cascade.event = $.ui.cascade.event || {};

    $.ui.cascade.ext.ajax = function(opt) {
        var ajax = opt.ajax; //ajax options hash...not just the url
        return { getList: function(parent) {
            var _ajax = {};
            var $this = $(this); //child element
            var defaultAjaxOptions = {
                type: "GET",
                dataType: "json",
                success: function(json) { $this.trigger("updateList", [json]); },
                data: $.extend(_ajax.data, ajax.data, { val: opt.getParentValue(parent) })
            };
            //overwrite opt.ajax with required props (json,successcallback,data)		
            //this lets us still pass in handling the other ajax callbacks and options
            $.extend(_ajax, ajax, defaultAjaxOptions);

            $.ajax(_ajax);
        } 
        };
    };

    $.ui.cascade.ext.templateText = function(opt) {
        var template = $.makeTemplate(opt.templateText, "<%", "%>");
        return { template: function(obj) { return template(obj); } };
    };

    /*these events are bound on every instance...so the indicator appears  on each target */
    /* 
    *	CSS: .cascade-loading: { background: transparent url("${staticDir}/Content/images/indicator.gif") no-repeat center; }
    */
    $.ui.cascade.event.loading = function(e, source) {
        $(this).empty();
        var position = {
            'z-index': '6000',
            'position': 'absolute',
            'width': '16px'
        };
        $.extend(position, $(this).offset());
        position.top = position.top + 3;
        position.left = position.left + 3;
        $("<div class='cascade-loading'>&nbsp;</div>").appendTo("body").css(position);
        $(this)[0].disabled = true;
    };
    $.ui.cascade.event.loaded = function(e, source) {
        $(this)[0].disabled = false;
        $(".cascade-loading").remove();
    };

})(jQuery);

; (function($) {

    $.ui = $.ui || {}; $.ui.cascade = $.ui.cascade || {};

    $.fn.cascade = function(parent, opt) {
        if (opt.event) {
            //namespace our event 
            opt.event = opt.event.replace('.cascade', '') + '.cascade';
        }

        opt = $.extend({}, {
            list: [], //static list to use as datasource 
            timeout: 10, //delay before firing getList operation
            getList: function(select) { $(this).trigger("updateList", [opt.list]); }, //function to fetch datasource
            template: function(str) { return "<option value='" + str + "'>" + str + "</option>"; }, //applied to each item in datasource      
            match: function(selectedValue) { return true; }, //'this' is the js object, or the current list item from 'getList',
            event: "change.cascade", //event to listen on parent which fires the cascade
            getParentValue: function(parent) { return $(parent).val(); } //delegate for retrieving the parent element's value
        }, opt);

        if ($.ui.cascade.ext) {
            for (var ext in $.ui.cascade.ext) {
                if (opt[ext]) {
                    opt = $.extend(opt, $.ui.cascade.ext[ext](opt));
                    delete opt[ext];
                }
            }
        }

        return this.each(function() {
            var source = $(parent);
            var self = $(this);

            //bind any events in extensions to each instance
            if ($.ui.cascade.event) {
                for (var e in $.ui.cascade.event) {
                    self.bind(e + ".cascade", [source], $.ui.cascade.event[e]);
                }
            }

            $(source).bind(opt.event, function() {
                self.trigger("loading.cascade", [source[0]]);

                var selectTimeout = $.data(self, "selectTimeout");
                if (selectTimeout) { window.clearInterval(selectTimeout); }
                $.data(self, "selectTimeout", window.setTimeout(function() {
                    self.trigger("cascade");
                }, opt.timeout));

            });

            self.bind("cascade", function() {
                self.one("updateList", function(e, list) {
                    list = $(list)
              .filter(function() { return opt.match.call(this, opt.getParentValue(parent)); })
              .map(function() {
                  var node = $(opt.template(this))[0];
                  return node;
              });

                    self.empty(); //clear the source/select

                    if (list.length) {
                        self.html(list);
                    }

                    self.trigger("loaded.cascade", [source[0]]); //be sure to fire even if there is no data

                    if (self.is(":input")) {
                        self.trigger("change.cascade");
                    }
                });

                opt.getList.call(self[0], source); //call with child element as this

            });
        });
    };

})(jQuery);