// defined the map of unsupported charachters in PubMed to be replaced in the links related to the the author names
// map was taken from redirect.cgi
var pubmedUnsupportedCharMap = {
'x000b2':'2',
'x000b3':'3',
'x000b9':'1',
'x000c0':'A',
'x000c1':'A',
'x000c2':'A',
'x000c3':'A',
'x000c4':'A',
'x000c5':'A',
'x000c6':'AE',
'x000c7':'C',
'x000c8':'E',
'x000c9':'E',
'x000ca':'E',
'x000cb':'E',
'x000cc':'I',
'x000cd':'I',
'x000ce':'I',
'x000cf':'I',
'x000d1':'N',
'x000d2':'O',
'x000d3':'O',
'x000d4':'O',
'x000d5':'O',
'x000d6':'O',
'x000d7':'x',
'x000d8':'O',
'x000d9':'U',
'x000da':'U',
'x000db':'U',
'x000dc':'U',
'x000dd':'Y',
'x000df':'ss',
'x000e0':'a',
'x000e1':'a',
'x000e2':'a',
'x000e3':'a',
'x000e4':'a',
'x000e5':'a',
'x000e6':'ae',
'x000e7':'c',
'x000e8':'e',
'x000e9':'e',
'x000ea':'e',
'x000eb':'e',
'x000ec':'i',
'x000ed':'i',
'x000ee':'i',
'x000ef':'i',
'x000f1':'n',
'x000f2':'o',
'x000f3':'o',
'x000f4':'o',
'x000f5':'o',
'x000f6':'o',
'x000f8':'o',
'x000f9':'u',
'x000fa':'u',
'x000fb':'u',
'x000fc':'u',
'x000fd':'y',
'x000ff':'y',
'x00100':'A',
'x00101':'a',
'x00102':'A',
'x00103':'a',
'x00104':'A',
'x00105':'a',
'x00106':'C',
'x00107':'c',
'x00108':'C',
'x00109':'c',
'x0010a':'C',
'x0010b':'c',
'x0010c':'C',
'x0010d':'c',
'x0010e':'D',
'x0010f':'d',
'x00110':'D',
'x00111':'d',
'x00112':'E',
'x00113':'e',
'x00116':'E',
'x00117':'e',
'x00118':'E',
'x00119':'e',
'x0011a':'E',
'x0011b':'e',
'x0011c':'G',
'x0011d':'g',
'x0011e':'G',
'x0011f':'g',
'x00120':'G',
'x00121':'g',
'x00122':'G',
'x00124':'H',
'x00125':'h',
'x00126':'H',
'x00127':'h',
'x00128':'I',
'x00129':'ij',
'x0012a':'I',
'x0012b':'ij',
'x0012e':'I',
'x0012f':'ij',
'x00130':'I',
'x00131':'ij',
'x00132':'IJ',
'x00133':'ij',
'x00134':'J',
'x00135':'j',
'x00136':'K',
'x00137':'k',
'x00138':'k',
'x00139':'L',
'x0013a':'l',
'x0013b':'L',
'x0013c':'l',
'x0013d':'L',
'x0013e':'l',
'x0013f':'L',
'x00140':'l',
'x00141':'L',
'x00142':'l',
'x00143':'N',
'x00144':'n',
'x00145':'N',
'x00146':'n',
'x00147':'N',
'x00148':'n',
'x00149':'n',
'x0014c':'O',
'x0014d':'o',
'x00150':'O',
'x00151':'o',
'x00152':'OE',
'x00153':'oe',
'x00154':'R',
'x00155':'r',
'x00156':'R',
'x00157':'r',
'x00158':'R',
'x00159':'r',
'x0015a':'S',
'x0015b':'s',
'x0015c':'S',
'x0015d':'s',
'x0015e':'S',
'x0015f':'s',
'x00160':'S',
'x00161':'s',
'x00162':'T',
'x00163':'t',
'x00164':'T',
'x00165':'t',
'x00166':'T',
'x00167':'t',
'x00168':'U',
'x00169':'u',
'x0016a':'U',
'x0016b':'u',
'x0016c':'U',
'x0016d':'u',
'x0016e':'U',
'x0016f':'u',
'x00170':'U',
'x00171':'u',
'x00172':'U',
'x00173':'u',
'x00174':'W',
'x00175':'w',
'x00176':'Y',
'x00177':'y',
'x00178':'Y',
'x00179':'Z',
'x0017a':'z',
'x0017b':'Z',
'x0017c':'z',
'x0017d':'Z',
'x0017e':'z',
'x001f5':'g',
'x00405':'S',
'x00406':'I',
'x00408':'J',
'x00455':'s',
'x00456':'i',
'x00458':'j',
'x02102':'C',
'x0210b':'H',
'x0210c':'H',
'x02110':'I',
'x02111':'I',
'x02112':'L',
'x02115':'N',
'x02119':'P',
'x0211a':'Q',
'x0211c':'R',
'x0211d':'R',
'x0211e':'Rx',
'x02124':'Z',
'x0212b':'A',
'x0212c':'B',
'x0212f':'e',
'x02130':'E',
'x02131':'F',
'x02133':'M',
'x02134':'o',
'x0e05d':'h',
'x0e05f':'k',
'x0e3b2':'fj',
'x0e500':'A',
'x0e501':'B',
'x0e503':'D',
'x0e504':'E',
'x0e505':'F',
'x0e506':'G',
'x0e507':'H',
'x0e508':'I',
'x0e509':'J',
'x0e50a':'K',
'x0e50b':'L',
'x0e50c':'M',
'x0e50e':'O',
'x0e512':'S',
'x0e513':'T',
'x0e514':'U',
'x0e515':'V',
'x0e516':'W',
'x0e517':'X',
'x0e518':'Y',
'x0e520':'A',
'x0e522':'C',
'x0e523':'D',
'x0e526':'G',
'x0e529':'J',
'x0e52a':'K',
'x0e52d':'N',
'x0e52e':'O',
'x0e52f':'P',
'x0e530':'Q',
'x0e531':'R',
'x0e532':'S',
'x0e533':'T',
'x0e534':'U',
'x0e535':'V',
'x0e536':'W',
'x0e537':'X',
'x0e538':'Y',
'x0e539':'Z',
'x0e540':'a',
'x0e541':'b',
'x0e542':'c',
'x0e543':'d',
'x0e545':'f',
'x0e546':'g',
'x0e547':'h',
'x0e548':'i',
'x0e549':'j',
'x0e54a':'k',
'x0e54b':'l',
'x0e54c':'m',
'x0e54d':'n',
'x0e54f':'p',
'x0e550':'q',
'x0e551':'r',
'x0e552':'s',
'x0e553':'t',
'x0e554':'u',
'x0e555':'v',
'x0e556':'w',
'x0e557':'x',
'x0e558':'y',
'x0e559':'z',
'x0e560':'A',
'x0e561':'B',
'x0e562':'C',
'x0e563':'D',
'x0e564':'E',
'x0e565':'F',
'x0e566':'G',
'x0e569':'J',
'x0e56a':'K',
'x0e56b':'L',
'x0e56c':'M',
'x0e56d':'N',
'x0e56e':'O',
'x0e56f':'P',
'x0e570':'Q',
'x0e572':'S',
'x0e573':'T',
'x0e574':'U',
'x0e575':'V',
'x0e576':'W',
'x0e577':'X',
'x0e578':'Y',
'x0e579':'Z',
'x0e580':'a',
'x0e581':'b',
'x0e582':'c',
'x0e583':'d',
'x0e584':'e',
'x0e585':'f',
'x0e586':'g',
'x0e587':'h',
'x0e588':'i',
'x0e589':'j',
'x0e58a':'k',
'x0e58b':'l',
'x0e58c':'m',
'x0e58d':'n',
'x0e58e':'o',
'x0e58f':'p',
'x0e590':'q',
'x0e591':'r',
'x0e592':'s',
'x0e593':'t',
'x0e594':'u',
'x0e595':'v',
'x0e596':'w',
'x0e597':'x',
'x0e598':'y',
'x0e599':'z',
'x0ea00':'1',
'x0ea01':'1',
'x0ea02':'2',
'x0ea03':'2',
'x0ea04':'3',
'x0ea05':'3',
'x0ea06':'4',
'x0ea07':'a',
'x0ea08':'a',
'x0ea09':'A',
'x0ea0a':'a',
'x0ea0b':'A',
'x0ea0c':'AB',
'x0ea10':'b',
'x0ea11':'B',
'x0ea12':'b',
'x0ea13':'B',
'x0ea14':'B',
'x0ea15':'B',
'x0ea16':'BC',
'x0ea19':'c',
'x0ea1a':'c',
'x0ea1b':'c',
'x0ea1c':'C',
'x0ea1d':'c',
'x0ea1e':'c',
'x0ea1f':'c',
'x0ea20':'c',
'x0ea21':'c',
'x0ea25':'d',
'x0ea26':'D',
'x0ea27':'d',
'x0ea28':'D',
'x0ea29':'d',
'x0ea2a':'D',
'x0ea2b':'D',
'x0ea2f':'dl',
'x0ea30':'e',
'x0ea31':'e',
'x0ea32':'l',
'x0ea33':'l',
'x0ea34':'f',
'x0ea35':'f',
'x0ea36':'F',
'x0ea37':'f',
'x0ea38':'F',
'x0ea39':'f',
'x0ea3a':'F',
'x0ea3d':'g',
'x0ea3e':'g',
'x0ea3f':'G',
'x0ea40':'g',
'x0ea41':'G',
'x0ea46':'H',
'x0ea47':'h',
'x0ea48':'h',
'x0ea49':'i',
'x0ea4a':'I',
'x0ea4b':'j',
'x0ea4c':'J',
'x0ea4d':'J',
'x0ea4e':'J',
'x0ea4f':'k',
'x0ea50':'k',
'x0ea51':'k',
'x0ea52':'K',
'x0ea53':'K',
'x0ea54':'k',
'x0ea55':'l',
'x0ea56':'L',
'x0ea57':'l',
'x0ea5b':'lnV',
'x0ea5c':'m',
'x0ea5d':'M',
'x0ea5e':'m',
'x0ea5f':'m',
'x0ea63':'n',
'x0ea64':'n',
'x0ea65':'N',
'x0ea66':'n',
'x0ea67':'N',
'x0ea68':'n',
'x0ea69':'n',
'x0ea6a':'N',
'x0ea6d':'nv',
'x0ea6e':'O',
'x0ea6f':'O',
'x0ea70':'O',
'x0ea72':'F',
'x0ea73':'p',
'x0ea74':'P',
'x0ea75':'p',
'x0ea76':'P',
'x0ea77':'p',
'x0ea78':'P',
'x0ea79':'p',
'x0ea7a':'P',
'x0ea8d':'q',
'x0ea8e':'Q',
'x0ea8f':'q',
'x0ea90':'q',
'x0ea91':'Q',
'x0ea92':'q',
'x0ea93':'Q',
'x0ea94':'q',
'x0ea95':'q',
'x0ea96':'q',
'x0ea97':'Q',
'x0ea98':'r',
'x0ea99':'R',
'x0ea9a':'R',
'x0ea9b':'r',
'x0ea9c':'R',
'x0ea9d':'R',
'x0ea9e':'r',
'x0ea9f':'R',
'x0eaa0':'r',
'x0eaa1':'ri',
'x0eaa2':'R',
'x0eaa3':'r',
'x0eaa4':'r1',
'x0eaa5':'RE',
'x0eaa9':'ri',
'x0eaaa':'rj',
'x0eaab':'rN',
'x0eaac':'s',
'x0eaad':'S',
'x0eaae':'S',
'x0eaaf':'s',
'x0eab0':'S',
'x0eab1':'s',
'x0eab2':'S',
'x0eab3':'S',
'x0eab4':'B',
'x0eab5':'E',
'x0eab6':'G',
'x0eab7':'P',
'x0eab8':'Q',
'x0eab9':'t',
'x0eaba':'T',
'x0eabb':'T',
'x0eabc':'t',
'x0eabd':'T',
'x0eabe':'t',
'x0eabf':'T',
'x0eac5':'TT',
'x0eac6':'u',
'x0eac7':'u',
'x0eac8':'U',
'x0eac9':'u',
'x0eaca':'u',
'x0eacc':'V',
'x0eacd':'v',
'x0eace':'v',
'x0eacf':'V',
'x0ead0':'v',
'x0ead1':'V',
'x0ead2':'v',
'x0ead3':'v',
'x0ead4':'V',
'x0ead5':'v',
'x0eadc':'w',
'x0eadd':'w',
'x0eade':'w',
'x0eadf':'x',
'x0eae0':'X',
'x0eae1':'x',
'x0eae2':'X',
'x0eae3':'x',
'x0eae4':'x',
'x0eae5':'X',
'x0eae6':'x',
'x0eae8':'y',
'x0eae9':'Y',
'x0eaea':'y',
'x0eaeb':'Y',
'x0eaec':'y',
'x0eaed':'y',
'x0eaee':'z',
'x0eaef':'Z',
'x0eaf0':'z',
'x0eaf1':'z',
'x0eaf2':'Z',
'x0eaf3':'z',
'x0eaf4':'z',
'x0eafc':'B',
'x0eaff':'A',
'x0eb04':'w',
'x0eb05':'m',
'x0eb06':'M',
'x0eb07':'E',
'x0eb08':'W',
'x0eb0a':'h',
'x0eb0b':'n',
'x0eb0d':'H',
'x0eb0f':'M',
'x0eb10':'m',
'x0eb11':'Z',
'x0eb12':'g',
'x0eb14':'C',
'x0eb15':'E',
'x0eb19':'W',
'x0eb1a':'a',
'x0eb1b':'b',
'x0eb1c':'S',
'x0eb1e':'J',
'x0eb1f':'b',
'x0eb20':'Z',
'x0eb21':'L',
'x0eb22':'g',
'x0fb00':'ffi',
'x0fb01':'fi',
'x0fb02':'fl',
'x0fb03':'ffi',
'x0fb04':'ffl'
    }

// Called from many places to handle links.
// Params:
//     link:         URL. 'this' to reuse same window, else ""
//     windowname:   Reference name for window
//     additional:   Attributes for window.open, from:
//        width, height, resizable, scrollbars, toolbar, location, directories,
//        status, menubar, copyhistory.
// Notes: Book version always provides menubar, toolbar, etc.
//
function startTargetBook(link,windowname,width,height,additional)
{
    startTarget(link,windowname,width,height,additional)
}


//===============================================================================
function startTarget(link,windowname,width,height, additional)
{

    if(! window.focus)
        return;

    var sizestring = ",width=" + width + ",height=" + height;
	var opt = "menubar=no,toolbar=no,status=no,scrollbars=yes,resizable=yes,dependent=yes,location=no";
    var allOptions = opt + sizestring;
    if (additional) { allOptions = allOptions + ',' + additional; }

    windowname = String(windowname).replace(/-/g, "_")
    var moveToXDefault = 75
    var moveToYDefault = 50

    var moveToX = moveToXDefault
    var moveToY = moveToYDefault
    var yAdj = 50
    var yAdjDelta = 25

    var barNames = ['menubar', 'toolbar', 'statusbar']
    for (var idx in barNames)
    {
	if (String(allOptions).lastIndexOf(barNames[idx] + '=yes') >
	    String(allOptions).lastIndexOf(barNames[idx] + '=no'))
		yAdj += yAdjDelta
    }

    try {
        if (window.screen.width && width > 0)
            moveToX = Math.ceil((window.screen.width - width)) - 15
        moveToX = (moveToX > 0 ? moveToX : moveToXDefault)

        if (window.screen.height && height > 0)
            moveToY = Math.ceil((window.screen.height - height)) - yAdj
        moveToY = (moveToY > 0 ? moveToY : moveToYDefault)
    }
    catch (e) {}

    var wLeftTopCornerOptions = ',left=' + moveToX + ',top=' + moveToY + ',screenX=' + moveToX + ',screenY=' + moveToY

    allOptions = allOptions + wLeftTopCornerOptions

    w = window.open (link, windowname, allOptions)
    w.focus()

    link.target=windowname

    return false;
}


//===============================================================================
function focuswin(windowname)
{
    windowname = String(windowname).replace(/-/g, "_")
    w = window.open("",windowname,"menubar=yes,scrollbars=yes,toolbar=yes,location=yes,status=yes,directories=yes,resizable=yes");
    w.focus();

    return true;
}


//===============================================================================
function reverseString (inStr)
{
    var outStr = ''

    for (i =0; i <= inStr.length; i++)
    {
	outStr = inStr.charAt(i) + outStr
    }

    return outStr
}


//===============================================================================
function reverseAndReplaceString (inStr, findStr, newStr)
{
    return reverseString(inStr).replace(findStr, newStr)
}

//===============================================================================
function initUnObscureEmail (className, innerHTML)
{

    try{
        if (window.addEventListener)
            window.addEventListener('load', function() {unObscureEmail (className, innerHTML)}, false)
        else if (window.attachEvent)
            window.attachEvent('onload', function() {unObscureEmail (className, innerHTML)})
    }catch(e){
    }
}

//===============================================================================
function unObscureEmail (className, innerHTML)
{
    try {
        if (typeof(unObscuredEmails) == 'undefined')
            unObscuredEmails = new Array()

        if (! unObscuredEmails[className])
        {

            var elmnts = document.getElementsByTagName("span")
            for (var i = 0, len = elmnts.length; i < len; i++)
            {
                if (elmnts.item(i).className.indexOf(className, " ") != -1)
                {
                    elmnts.item(i).innerHTML     = innerHTML;
                    unObscuredEmails[className]  = true

                }
            }
        }
    }
    catch(e){
    }
}

//===============================================================================
function pubMedDbLinkSubmit(control) {
    try{
        if (control.pubmedOption)
            control = control.pubmedOption
        if (control.options)
            location.href = control.options[control.options.selectedIndex].value;
    }
    catch(e){}
    return false;
}

//===============================================================================
// elements "a", "area" with "ref" attribute report to redirect utility before dereferencing link
//
function initRedirectClicks(report_url) {

    if (report_url && typeof jQuery != 'undefined')
        jQuery(document).click(function(e) {

            //
            if (!e) return true;
            //
            if (e.shiftKey || e.altKey || e.ctrlKey) return true;
            //
            var target = jQuery(e.target).closest("a[ref], area[ref]");
            if (!target || target.length == 0) return true;
            //
            var ref  = target.attr("ref");
            var href = target.attr("href")

            // special processing of href element for pubmed's authors
            if (typeof (href) != 'undefined' && href.indexOf('sites/entrez') && href.indexOf('%5Bauth%5D'))
            {
                var newHref = replacePubMedUnsupportedChars(href);
                if (newHref != href)
                    target.attr("href", newHref)
            }

            // append ref with value of href if 'redirect-to-url' is missing in ref.
            if (typeof (ref) != 'undefined' && ref.indexOf('redirect-to-url') == -1)
            {
                ref = ref + '&redirect-to-url=' + encodeURIComponent(href)
                //
                jQuery.ajax({
                   type:    "HEAD",
                   url:     report_url + '?' + ref,
                   timeout: 1000,
                   async:   false
                });
            }

            return true;
        });
};

// function to replace unsupported charachters in author names to be searchable in PubMed database.
function replacePubMedUnsupportedChars (str)
{
    // replace low 128 chars automatically
    str = str.replace(/%26%23(x000[0-7][0-9a-f])%3b/ig,
        function($0, $1){
            return (String.fromCharCode('0' + $1))
        }
    )
    // replace others based on map and if it is not in the map replace with empty string.
    str = str.replace(/%26%23(x[0-9a-f][0-9a-f][0-9a-f][0-9a-f][0-9a-f])%3b/ig,
        function($0, $1){
            return (pubmedUnsupportedCharMap[$1] ? pubmedUnsupportedCharMap[$1] : '')
        }
    )
    return str;
}
//===============================================================================
// Based on script written by Dean Edwards, 2005
// http://dean.edwards.name/

var pmc_script = {
	addEvent: function(element, type, handler) {
		// assign each event handler a unique ID
		if (!handler.$$guid) handler.$$guid = this.addEvent_guid++;
		// create a hash table of event types for the element
		if (!element.events) element.events = {};
		// create a hash table of event handlers for each element/event pair
		var handlers = element.events[type];
		if (!handlers) {
			handlers = element.events[type] = {};
			// store the existing event handler (if there is one)
			if (element["on" + type]) {
				handlers[0] = element["on" + type];
			}
		}
		// store the event handler in the hash table
		handlers[handler.$$guid] = handler;
		// assign a global event handler to do all the work
		element["on" + type] = handleEvent;


		function handleEvent(event) {
			var returnValue = true;
			// grab the event object (IE uses a global event object)
			event = event || fixEvent(window.event);
			// get a reference to the hash table of event handlers
			var handlers = this.events[event.type];
			// execute each event handler
			for (var i in handlers) {
				this.$$handleEvent = handlers[i];
				if (this.$$handleEvent(event) === false) {
					returnValue = false;
				}
			}
			return returnValue;
		};

		function fixEvent(event) {
			// add W3C standard event methods
			event.preventDefault = fixEvent.preventDefault;
			event.stopPropagation = fixEvent.stopPropagation;
			return event;
		};

		fixEvent.preventDefault = function() {
			this.returnValue = false;
		};

		fixEvent.stopPropagation = function() {
			this.cancelBubble = true;
		};
	},

	// a counter used to create unique IDs
	addEvent_guid: 1,

	removeEvent: function(element, type, handler) {
		// delete the event handler from the hash table
		if (element.events && element.events[type]) {
			delete element.events[type][handler.$$guid];
		}
	}
}


// ****************************************************************************
//  implementation of figure's popup using jQuery and jQuery.fn.hoverIntent lib
//  it set appropriate handlers for all "a" elements with "figpopup" class name.
// ****************************************************************************
if (typeof jQuery != 'undefined' && typeof jQuery.fn.hoverIntent != 'undefined') {
    (function($j) {
        // ==========================  hoverHandlers ================================
        var hoverHandlers = function  (opts) {
            this.config = {}
            this.self = this;
            this.__init(opts)
        }

         $j.fn.extend(hoverHandlers.prototype,  {
                //
                __init: function(opts) {
                    $j.fn.extend(this.config, opts)
                },
                //
                over: function (el) {
                    var canvas = $j('.large-thumb-canvas', el)
                    var prnt =  canvas.parent();
                    var win = $j(window)

                    //
                    if (canvas && prnt && win)
                    {

                        // collect necessary coordinates from all intresting objects
                        canvas.coords = this._x_coords(canvas, 'canvas')
                        prnt.coords   = this._x_coords(prnt,   'parent')
                        win.coords    = this._x_coords(win,    'window')

                        // replace @src with value of @hires, because href hold high resolution image and src low one.
                        $j('img', canvas).attr('src',
                                function(){
                                    var hires = $j(this).attr("hires");
                                    return (typeof (hires) != 'undefined' && hires != this.src)
                                            ? hires : this.src
                                }
                        )

                        // make arrangement of canvas in the view port nearby parent element.
                        var cnvsRltvToPrnt = this._x_arrange_canvas(canvas.coords, prnt.coords, win.coords)

                        // visualize the canvas with images.
                        canvas.css('top', cnvsRltvToPrnt.top + 'px')
                        canvas.css('left', cnvsRltvToPrnt.left + 'px')
                        canvas.css('display', 'block')
                    } // canvas
                }, // over
                //
                out: function (el) {
                    var canvas = $j('.large-thumb-canvas', el)
                    canvas.css('display', 'none')
                },
                //
                _x_offset:                     function(el, elname) {
                    var c = {}
                    if (el[0] == window )
                        c = this._x_scroll(el, elname)
                    else
                        c = el.offset()
                    //console.info('%s.offset {top,left} = {%s, %s}', elname, c.top, c.left)
                    return c
                },
                _x_scroll:                     function(el, elname) {
                    var c = {top: el.scrollTop(), left: el.scrollLeft()}
                    //console.info('%s.scroll {top,left} = {%s, %s}', elname, c.top, c.left)
                    return c
                },
                _x_width:                     function(el, elname) {
                    var w = el.outerWidth() || el.width()
                    //console.info('%s.width      = %s; outerWidth = %s; width = %s', elname, w, el.outerWidth(), el.width())
                    //IE: alert(elname + '.width      = ' + w + '; outerWidth = ' + el.outerWidth() + 'width = ' + el.width())
                    return w
                },
                _x_height:                     function(el, elname) {
                    var h = el.outerHeight() || el.height()
                    // console.info('%s.height = %s', elname, h)
                    return h
                },
                _x_coords:                    function(el, elname) {
                    var size = { width: this._x_width (el, elname), height:  this._x_height(el, elname)}
                    var coords = {
                            scroll:  this._x_scroll(el, elname),
                            width:   size.width,
                            height:  size.height,
                            offset:  this._x_offset(el, elname),
                            size:    size
                        }
                    return coords
                },
                _x_get_rltv_coord:            function(trgtObj, baseObj, label) {
                    var c = {top: trgtObj.top - baseObj.top, left: trgtObj.left - baseObj.left}
                    //console.info("%s {top: %s, left: %s}", label, c.top, c.left)
                    return c
                },
                //
                _x_get_empty_space:            function(objCoords, objSize, viewPortCoords, viewPortSize, label) {
                    var objRelToViewPort = this._x_get_rltv_coord(objCoords, viewPortCoords, '_x_get_empty_space: for ' + label);
                    var c = {above: objRelToViewPort.top,
                             below: viewPortSize.height - (objRelToViewPort.top + objSize.height),
                             left: objRelToViewPort.left,
                             right: viewPortSize.width - (objRelToViewPort.left + objSize.width)}
                    $j.each(c, function(k, v){c[k] = (v < 0 ? 0 : v)})
                    //console.info("%s {above: %s, below: %s, left: %s, right: %s}", label, c.above, c.below, c.left, c.right)
                    return c
                },
                //
                _x_get_bigest_space_area_name: function(spaces, label) {
                    var sArr = [];
                    for (k in spaces){ sArr[sArr.length] = {k: k, v: spaces[k]} }
                    // sort in reverse order
                    sArr.sort (function(a,b){return ( a.v < b.v ? 1 : ( a.v > b.v ?  - 1 : 0 ) )})
                    // console.info('%s: %s', label, sArr[0].k)
                    return sArr[0].k
                },
                //
                _x_get_space_ratios:             function(spaces, viewPortSize, label) {
                    var ratios = {above: 0, left: 0}
                    if (viewPortSize.height > 0)
                        ratios.above = spaces.above / viewPortSize.height
                    if (viewPortSize.width > 0)
                        ratios.left = spaces.left / viewPortSize.width
                    // console.info('ratio: %s {above: %s, left: %s}', label, ratios.above, ratios.left)
                    return ratios
                },
                _x_arrange_canvas:                function(c_c, prnt_c, viewp_c) {
                // then calculate empty space areas around parent element of the canvas, which is ususally an anchor ("a" element)
                    var emptySpacesArndPrntInViewPort = this._x_get_empty_space (prnt_c.offset, prnt_c.size, viewp_c.offset, viewp_c.size, 'emptySpacesArndPrntInWinPort')

                    // get the area name, where the most linear space avaiable
                    var areaName = this._x_get_bigest_space_area_name(emptySpacesArndPrntInViewPort, viewp_c.size, 'spacios areaName')

                    // calculate the proportions of area to be taken by canvas in empty space area
                    // relativly to the parent element.
                    var ratios = this._x_get_space_ratios(emptySpacesArndPrntInViewPort, viewp_c.size, 'ratios')

                    // calculate parent's coordinates relative to view port
                    var prntRltvToViewPort = this._x_get_rltv_coord(prnt_c.offset, viewp_c.offset, 'prntRltvToViewPort')

                    // declare canvas relative coordinates
                    var cnvsRltvToPrnt = {top: 0, left: 0}

                    // choose appropriate location for placing canvas.
                    // and choose the 1st element of an array as the one with the biggest area around parent of canvas.
                    // console.info ('place it at the %s', bigestEmptySpaceAreaName)
                    // calculations are relative to the left corner of the parent element of canvas. (it has to have the css property position:relative)
                    switch (areaName)
                    {
                        case 'above':
                            cnvsRltvToPrnt.top  = - c_c.height
                            cnvsRltvToPrnt.left = prnt_c.width * 0.5 - c_c.width * ratios.left
                        break;
                        case 'below':
                            cnvsRltvToPrnt.top  = prnt_c.height
                            cnvsRltvToPrnt.left = prnt_c.width * 0.5 - c_c.width * ratios.left
                        break;
                        case 'left':
                            cnvsRltvToPrnt.left = - c_c.width
                            cnvsRltvToPrnt.top  = prnt_c.height * 0.5 - c_c.height * ratios.above
                        break;
                        case 'right':
                            cnvsRltvToPrnt.left = prnt_c.width
                            cnvsRltvToPrnt.top  = prnt_c.height * 0.5 - c_c.height * ratios.above
                        break;
                    }

                    // recalculate canvas' coordinates relative to parent into coordinates relative to view port.
                    var cnvsRltvToViewPort = {
                        top:  prntRltvToViewPort.top + cnvsRltvToPrnt.top,
                        left: prntRltvToViewPort.left + cnvsRltvToPrnt.left
                    }
                    //console.info("cnvsRltvToPrnt {top:%s, left: %s}", cnvsRltvToPrnt.top, cnvsRltvToPrnt.left)
                    //console.info("cnvsRltvToViewPort {top:%s, left: %s}", cnvsRltvToViewPort.top, cnvsRltvToViewPort.left)

                    // adjust relative position of canvas to the window if calculated values
                    // are forcing canvas go out of view port
                    // next order should be preserved to keep left top corner visible always

                    // adjust right side and move a bit to the left
                    if (cnvsRltvToViewPort.left + c_c.width > viewp_c.width)
                    {
                         var horAdj = c_c.width - (viewp_c.width - cnvsRltvToViewPort.left)
                         cnvsRltvToPrnt.left     -= horAdj
                         cnvsRltvToViewPort.left -= horAdj
                         //console.log('adjust right side and move to the left %spx', horizontalAdjustment)
                    }

                    // adjust below side and move to the top
                    if (cnvsRltvToViewPort.top + c_c.height > viewp_c.height)
                    {
                        var vertAdj = c_c.height - (viewp_c.height - cnvsRltvToViewPort.top)
                        cnvsRltvToPrnt.top     -= vertAdj
                        cnvsRltvToViewPort.top -= vertAdj
                        // console.log('adjust below side and move to the top %spx', verticalAdjustment)
                    }

                    // adjust left side and move to the right
                    if (cnvsRltvToViewPort.left < 0)
                    {
                        // console.log('adjust left side and move to the right %spx', -(cnvsLeftRelativeToWindowPort))
                        cnvsRltvToPrnt.left    -= cnvsRltvToViewPort.left
                        cnvsRltvToViewPort.left = 0
                    }
                    // adjust above side and move to the bottom
                    if (cnvsRltvToViewPort.top < 0)
                    {
                        // console.log('adjust above side and move to the bottom %spx', -(cnvsTopRelativeToWindowPort))
                        cnvsRltvToPrnt.top          -= cnvsRltvToViewPort.top
                        cnvsRltvToViewPort.top = 0
                    }
                    // console.info("cnvsRltvToPrnt adjusted {top: %s, left: %s}", cnvsRltvToPrnt.top, cnvsRltvToPrnt.left)
                    // console.info("cnvsRltvToViewPort adjusted {top: %s, left: %s}", cnvsRltvToViewPort.top, cnvsRltvToViewPort.left)
                    return cnvsRltvToPrnt
                }
        })
         // ==========================  end of hoverHandlers ========================


         // ==========================  $j.fn.figPopup ==============================
        function  hoverOverHandler  () {
            var h = new hoverHandlers();
            h.over(this)
        }

        function hoverOutHandler () {
            var h = new hoverHandlers();
            h.out(this)
        }

        $j.fn.figPopup = function(opts){
            var config = {
                interval:      500,
                timeout:       0,
                over:          hoverOverHandler, // handler onMouseOver
                out:           hoverOutHandler    // handler  onMouseOut
            }
            $j.extend(config, opts)
            return  config
        }

        // ==========================  end of $j.fn.figPopup ========================

    }) (jQuery) // end of extention of jQuery with figPopup



    jQuery(document).ready( function () {
        var fp_cfg = new jQuery.fn.figPopup()
        jQuery('a.figpopup').hoverIntent(fp_cfg)
    });
}

// end of hoverIntent routines for figures' popups
// ****************************************************************************
//  implementation of ....
// ****************************************************************************
