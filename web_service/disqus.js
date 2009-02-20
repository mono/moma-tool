Type.registerNamespace('MoMATool');

var disqus_developer = 0;
var disqus_url = '';
var disqus_title = '';
var disqus_message = '';
var disqus_container_id = "disqus_thread";
var disqus_identifier = '';
                
MoMATool.DisqusControl = function(element) {
    MoMATool.DisqusControl.initializeBase(this, [element]);
    
    this._disqusForum = null;
}

MoMATool.DisqusControl.prototype = {
    initialize: function() {
        MoMATool.DisqusControl.callBaseMethod(this, 'initialize');
        
        $addHandlers(this.get_element(),
                    {},
                    this);
    },
    
    dispose: function() {
        $clearHandlers(this.get_element());
        
        MoMATool.DisqusControl.callBaseMethod(this, 'dispose');
    },
    
    get_disqusForum: function() {
        return this._disqusForum;
    },
    
    set_disqusForum: function(value) {
        if (this._disqusForum !== value) {
            this._disqusForum = value;
            this.raisePropertyChanged('disqusForum');
        }
    },
    
    get_disqusDeveloper: function() {
        return disqus_developer;
    },
    
    set_disqusDeveloper: function(value) {
        if (disqus_developer !== value) {
            disqus_developer = value;
            this.raisePropertyChanged('disqusDeveloper');
        }
    },
    
    get_disqusContainerId: function() {
        return disqus_container_id;
    },
    
    set_disqusContainerId: function(value) {
        if (value != null && disqus_container_id !== value) {
            disqus_container_id = value;
            this.raisePropertyChanged('disqusContainerId');
        }
    },
    
    get_disqusURL: function() {
        return disqus_url;
    },
    
    set_disqusURL: function(value) {
        if (disqus_url !== value) {
            disqus_url = value;
            this.raisePropertyChanged('disqusURL');
        }
    },
    
    get_disqusTitle: function() {
        return disqus_title;
    },
    
    set_disqusTitle: function(value) {
        if (disqus_title !== value) {
            disqus_title = value;
            this.raisePropertyChanged('disqusTitle');
        }
    },
    
    get_disqusMessage: function() {
        return disqus_message;
    },
    
    set_disqusMessage: function(value) {
        if (disqus_message !== value) {
            disqus_message = value;
            this.raisePropertyChanged('disqusMessage');
        }
    },
    
    get_disqusIdentifier: function() {
        return disqus_identifier;
    },
    
    set_disqusIdentifier: function(value) {
        if (disqus_identifier !== value) {
            disqus_identifier = value;
            this.raisePropertyChanged('disqusIdentifier');
        }
    },
    
    get_update: function() {},
    
    set_update: function(value) {        
        if (disqus_identifier !== null && disqus_message != null && disqus_title != null) {
            // call the thread creation proxy service to make sure the correct disqus thread gets
            // created on demand, and load the iframe when the proxy returns
            
            var forum = this._disqusForum; // makes it easier to reference in the EnsureThreadCreated closure
            
            DisqusProxy.EnsureThreadCreated(disqus_title, disqus_message, disqus_identifier, function(success) {
                var disqus_control_iframe = document.getElementById(value);
                var doc = disqus_control_iframe.contentDocument;
                if (doc == undefined || doc == null) {
                    doc = disqus_control_iframe.contentWindow.document;
                }
            
                doc.open();
            
                // The doctype is needed for IE to load the stylesheet contained inside embed.js.  Except something
                // seems to have broken it again. &#@%ing IE.  Workaround by ripping the CSS out of embed.js and
                // referencing it directly.  The 'disqus_no_style' setting is needed for firefox when the CSS is
                // directly referenced.
            
                var iframe_html = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">\n' +
                    '<html xmlns="http://www.w3.org/1999/xhtml">\n' +
                    '<head>\n' +
                    '<title>Disqus Comments</title>\n' +
                    '<link href="disqus.css" type="text/css" rel="stylesheet" media="screen"></link>\n' +
                    '</head><body>\n' +
                    '<' + 'script charset="utf-8" type="text/javascript">\n' +
                    '<!--//--><![CDATA[//><!--\n';
                    
                if (disqus_developer) {
                    iframe_html += 'var disqus_developer = 1;\n';
                }
                
                iframe_html += 'var disqus_identifier = "' + disqus_identifier + '";\n' +
                    'window.disqus_no_style = true;\n' +
                    '//--><!]]>\n' +
                    '<' + '/script>\n' +
                    '<div id="disqus_thread"></div>\n' +
                    '<' + 'script type="text/javascript" src="http://disqus.com/forums/' + forum + '/embed.js"><' + '/script>\n' +
                    '<a href="http://disqus.com" class="dsq-brlink">Comments powered by <span class="logo-disqus">Disqus</span></a>\n' +
                    '</body></html>';
                    
                doc.write(iframe_html);
                doc.close();
            
                // Resize the iframe whenever Disqus loads its content.  The fudge factor is needed to ensure
                // scrollbars don't appear when the 'Options' button is toggled.
                //
                // This doesn't work on IE (surprise!) so I just set the height to 2000px there
                    
                if (doc.addEventListener) {
                    doc.addEventListener("DOMNodeInserted", function() {
                        disqus_control_iframe.height = disqus_control_iframe.contentDocument.body.offsetHeight + 150;
                    }, false);
                    doc.addEventListener("DOMNodeRemoved", function() {
                        disqus_control_iframe.height = disqus_control_iframe.contentDocument.body.offsetHeight + 150;
                    }, false);
                    
                    // This one keeps Google Chrome happier (otherwise it doesn't resize properly on first load)
                    doc.addEventListener("DOMSubtreeModified", function() {
                        disqus_control_iframe.height = disqus_control_iframe.contentDocument.body.offsetHeight + 150;
                    }, false);
                } else {
                    disqus_control_iframe.height = '2000px';
                }
            
                // I want to set display=none/display=block to hide and show the comments, but it breaks
                // in *%$&ing IE.  So I have to fart around setting the height to 0px to hide it
                //disqus_control_iframe.style.display = 'block';
            }, function() {
                //alert("DisqusProxy fail");
            });
        } else {
            //alert("Not updating");
        }
    }
}

MoMATool.DisqusControl.registerClass('MoMATool.DisqusControl', Sys.UI.Control);
if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
