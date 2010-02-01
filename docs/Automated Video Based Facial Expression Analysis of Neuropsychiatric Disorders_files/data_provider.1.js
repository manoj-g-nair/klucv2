<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"> 

<html>

<head>
<title>NCBI - WWW Error 404 Diagnostic</title>
<meta http-equiv="Content-Type" content="application/xhtml+xml; charset=utf-8" />
<meta http-equiv="pragma" content="no-cache" />
<meta http-equiv="cache-control" content="no-cache" />
<meta name="robots" content="noarchive,none" />

<style type="text/css">
body {
min-width: 950px;
_width: 950px;
}
h1.error {color: red; font-size: 40pt}

#mainContent {
padding: 1em 2em;
}

dl#diags {
padding: 0.5em 0.5em 1.5em 0.5em;
padding-left: 2em;
border: solid 1px #888;
border-left: none;
border-right: none;
margin-bottom:0;
background-color:#eeeeee;
color:#666;
font-size: 80%;
_font-size: 70%;
font-family: Verdana, sans-serif;
}

dl#diags dt {
float: left;
font-weight: bold;
width: auto;
}

dl#diags dd {
margin-left:1em;
float: left;
margin-right: 2em;
}

#footer span {
float: left;
color: #888;
font-size: 80%;
}

#footer {
text-align: right;
padding: 0 0.5em;
clear: left;
}

#footer img {
border: none;
}

.ncbi {
margin: 0;
padding:0;
font-size:240%;
font-weight: bold;
font-family: Times, serif;
color: #336699;
float: left;
display: inline;
}

.ncbi a {
text-decoration: none;
color: #336699;
}

.ncbi a:visited {
text-decoration: none;
color: #336699;
}

.ncbi a:hover {
text-decoration: underline;
}

.message {
font-family: Verdana, sans-serif;
background-color: #336699;
color: white;
padding: .35em;
margin-left: 7em;
margin-top: .67em;
_margin-top: 0.5em;
font-weight: bold;
font-size: 100%;
margin-bottom: 0;
}

h1 {
clear: left;
font-size: 110%;
font-family: Verdana, sans-serif;
}


body.denied {
background-color: black;
color: white;
}

body.denied h1 {
color: red;
}

body.denied a {
color: green;
}

body.denied #footer, body.denied #diags {
color: black;
}

#searchme:focus {
background-color: #ffa;
}

.errurl {
letter-spacing: 0;
margin: 0 1em;
padding: 0.25em;
background-color: #fff0f0;
color: #c00;
font-family: "Courier New", Courier, monospace;
font-size: 90%;
_font-size: 80%;
}

body.denied .errurl {
background-color: black;
color: yellow;
}

span.x {
display: none;
}

</style>

<!--[if IE]>
<script type="text/javascript">
/* <![CDATA[ */
_isIE=1;
/* ]]> */
</script>
<![endif]-->

<script type="text/javascript">
/* <![CDATA[ */
addEvent = function(obj, type, fn, b) {
    if (obj.attachEvent) {
        var name = "" + type + fn; 
        name = name.substring(0, name.indexOf("\n"));
        obj["e" + name] = fn;
        obj[name] = function(){ obj["e" + name](window.event);}
        obj.attachEvent("on" + type, obj[name]);
    } else {
        obj.addEventListener(type, fn, b);
        return true;
    }
}

fixlink = function() {
   var ie = typeof(_isIE) != 'undefined';
   var v = document.getElementById("mlink");

   if (v) {
      var ieHACK = v.innerHTML;
      var newText;

      newText = "%3E%20REQUEST_URI%3D" + document.REQUEST_URI + "%0D" +
                "%3E%20HTTP_HOST%3D" + document.HTTP_HOST + "%0D" +
                "%3E%20REMOTE_ADDR%3D" + document.REMOTE_ADDR + "%0D" +
                "%3E%20REQUEST_METHOD%3D" + document.REQUEST_METHOD + "%0D";

      var href = v.getAttribute("href");
      newText = newText.replace(/'/g, "%27");
      newText = newText.replace(/\?/g, "%3F");

      // IE bugfixes
      if (ie) {
          var m = /(.*)\?(.*)/.exec(href);
          if (m) {
             href = m[1] + "?" + escape(m[2]);
          }
          href = href.replace(/(.*)(%0D-----%0D.*)/m, "$1" + newText + "$2");
          href = href.replace(/subject%3D/, "subject=");
          href = href.replace(/%26body%3D/, "&body=");
          href=href.replace(/%0D/mg, "%0A");
      } else {
          href = href.replace(/(.*)(%0D-----%0D.*)/m, "$1" + newText + "$2");
      }
      v.setAttribute("href", href);
      v.innerHTML = ieHACK;  // I can't believe this...
   }  
   v = document.getElementById("searchme");
   if (v) { v.focus(); }
}


addEvent(window, "load", fixlink, false);
/* ]]> */
</script>


</head>

<body>

<div id='header'>
<a href="#mainContent" title="Skip to main content" />
<p class="ncbi"><a href="http://www.ncbi.nlm.nih.gov">NCBI</a></p>
<p class="message">Error</p>
</div>

<div id='mainContent'>
<h1>Can't find the requested web page.</h1>

<p class='errurl'>
<script type="text/javascript">
/* <![CDATA[ */
  document.REQUEST_URI = "/corehtml/jsutils/data_provider.1.js";
  document.HTTP_HOST = "www.ncbi.nlm.nih.gov";
  document.REMOTE_ADDR = "91.1.224.107";
  document.REQUEST_METHOD = "GET";

  var p = "http://" + document.HTTP_HOST + document.REQUEST_URI;
  p = p.replace(/(.)/g, "<span class=\"x\"> </span>$1");
  p = p.replace(/'/g, "&apos;");
  document.write(p);
//  document.write(p.replace(/[^A-Za-z0-9\#\;\:\.\/\?&\=\_\- ]*/g, "<span class='x'> </span>"));
/* ]]> */
</script>
</p>

<p>The page you requested could not be found on our web server.
This is usually caused by a error in the web request;
however, it could also be caused by a problem on our server.
</p>
<p>If you know what the request should be, please check it and correct it
if necessary.  If not, you may be able to find the desired page by browsing
our site at <a href="http://www.ncbi.nlm.nih.gov">http://www.ncbi.nlm.nih.gov</a
> or using
the search below.</p>
<p> If you need additional assistance, send e-mail to
<a id='mlink'
href="mailto:info@ncbi.nlm.nih.gov?subject=NCBI%20Web%20site%20error%20404&amp;body=%3E%20Error%3D404%0D%3E%20Server%3D130.14.29.110%0D%3E%20Client%3D91.1.224.107%0D%3E%20Time%3DThursday, 07-Jan-2010 10:35:33 EST%0D%0D-----%0DPlease%20enter%20comments%20below:%0D%0D">info@ncbi.nlm.nih.gov</a>. 
</p>


<form method="post" action="http://www.ncbi.nlm.nih.gov/gquery/gquery.fcgi"
     enctype="application/x-www-form-urlencoded">
<p>
<strong>Search NCBI</strong>
<input name="term" size="40" style="margin-left: 1em; border: solid 1px #336699;" id="searchme"/>
<input type="submit" value="Go" />
</p>
</form>


</div>

<dl id="diags">
   <dt>Error</dt><dd>404</dd>
   <dt>Server</dt><dd>130.14.29.110</dd>
   <dt>Client</dt><dd>91.1.224.107</dd>
   <dt>Time</dt><dd>Thursday, 07-Jan-2010 10:35:33 EST</dd>
</dl>

<p id='footer'>

<span id='rev'>Rev. 19 Sep 2006 14:13:18 EDT</span>
<a href="http://validator.w3.org/check?uri=referer"><img src="http://www.w3.org/Icons/valid-xhtml11" alt="Valid XHTML 1.1!" /></a>
</p>

</body>
</html>

