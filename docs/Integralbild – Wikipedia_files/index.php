/*-----------------------------------------------------------------------------------------------*/
 /* ACHTUNG! Common.css enthält die gemeinsamen Skin-Anpassungen der deutschsprachigen Wikipedia */
 /*---------------------------------------------------------------------------------------------*/

 /* Common.css ist in drei grundlegende Abschnitte gegliedert: */

 /* 1. Editierbare Seiteninhalte */
 /* 2. Sonstige Anpassungen (Spezialseiten, u.a.) */
 /* 3. Hauptseitenspezifisches */

 /* Jede Ergänzung/Änderung muss vorher jeweils für alle Skins auf Tauglichkeit getestet werden, */
 /* individuell per User.css! Ist hier erstmal geändert, dauert es dank Cache eine Weile, bis die */
 /* Änderungen bei allen Nutzern sichtbar oder, bei Fehlern, korrigiert sind. */

 /* +++++ 1. ANPASSUNGEN DER ANZEIGE VON EDITIERBAREN SEITENINHALTEN +++++ */

 /* Personennamen in Kapitälchen: class="Person" */

 .Person {
    display: inline;
    font-variant: small-caps;
    font-style: normal;
 }

 /* [[Wikipedia:Zitate|Zitate]] (siehe [[Vorlage:Zitat]]) */

 p.Zitat {
    font-style: normal;
    margin-bottom: 0em;
 }
 p.cite {
    margin-top: 0em;
    padding-left: 1em;
 }

 /* [[Wikipedia:Quellenangaben|Quellenangaben]] nicht kursiv, da sie ggf. verschiedene Bestandteile enthalten */

 cite {
    font-style: normal;
 }

 /* Stylesheet-Ergänzung zu Standard-[[Wikipedia:Navigationsleisten|Navigationsleisten]] */

 div.BoxenVerschmelzen,
 div.NavFrame {
    margin: 0px;
    padding: 2px;
    border: 1px solid #aaaaaa;
    text-align: center;
    border-collapse: collapse;
    font-size: 95%;
    clear:both;
 }
 div.BoxenVerschmelzen div.NavFrame {
    border-style: none;
    border-style: hidden;
 }
 div.NavFrame + div.NavFrame {
    border-top-style: none;
    border-top-style: hidden;
 }
 div.NavPic {
    background-color: #ffffff;
    margin: 0px;
    padding: 2px;
    float: left;
 }
 div.NavFrame div.NavHead {
    font-weight: bold;
    font-size: 100%;
    background-color: #efefef;
 }
 div.NavFrame p {
    font-size: 100%;
 }
 div.NavFrame div.NavContent {
    font-size: 100%;
 }
 div.NavFrame div.NavContent p {
    font-size: 100%;
 }
 div.NavEnd {
    margin: 0px;
    padding: 0px;
    line-height: 1px;
    clear: both;
 }

/* Stylesheet fuer den 'Ausklappen'-Button an Navileisten, der in [[MediaWiki:Common.js]] implementiert wird */
.NavToggle {
    font-size: x-small;
    float:right;
}

/* Workaround für Internet Explorer 6 */
div.NavFrame, div.NavPic, .NavToggle {
    position:relative;
}

 /* Abstand vor Navigationsleisten */

 div.BoxenVerschmelzen,
 div.NavFrame {
   margin-top: 1.5em;
 }
 div.BoxenVerschmelzen div.NavFrame {
   margin-top: 0;
 }
 div.NavFrame + div.NavFrame {
   margin-top: 0;
 }

 /* Stylesheet-Ergänzung zu SideBoxen (ehedem [[Wikipedia:TownBox|TownBox]]) */

 div.sideBox {
    position: relative;
    float: right;
    background: white;
    margin-left: 1em;
    border: 1px solid gray;
    padding: 0.3em;
    width: 200px;
    overflow: hidden;
    clear: right;
 }
 div.sideBox dl {
    padding: 0;
    margin: 0 0 0.3em 0;
    font-size: 96%;
 }
 div.sideBox dl dt {
    background: none;
    margin: 0.4em 0 0 0;
 }
 div.sideBox dl dd {
    margin: 0.1em 0 0 1.1em;
    background-color: #f3f3f3;
 }

 /* Stylesheet-Ergänzung zu [[Wikipedia:Taxoboxen|Taxoboxen]] */

 table.taxobox {
        border-collapse: collapse;
        border: 1px solid gray;
        float: right;
        clear: right;
        margin-left: 0.5em;
        background-color:white;
 }
 table.taxobox th {
        background-color: #9BCD9B;
        border: solid 1px gray;
        text-align: center;
        font-weight: bold;
 }
 table.taxobox td {
        vertical-align:top;
 }
 table.taxobox .Person {
        font-style: normal;
 }
 table.taxobox div.thumb,
 table.taxobox div.thumb * {
        margin: auto;
        padding: 0;
        float: none;
        border: none;
        background-color:#F9F9F9;
 }
 table.taxobox div.magnify {
        display: none;
 }
 table.taxobox tr td div.thumb div div.thumbcaption {
        text-align:center;
 }
 table.taxobox td.Person {
        text-align:center;
        display:block;
 }
 table.taxobox td.taxo-name {
        text-align:center;
 }
 table.taxobox td.taxo-bild {
        text-align:center;
 }

 /* Stylesheet-Ergänzung zu [[Wikipedia:Paläoboxen|Paläoboxen]] */

 table.palaeobox {
       border-collapse: collapse;
       border: 1px solid gray;
       float: right;
       clear: right;
       margin-left: 0.5em;
       background-color:white;
 }
 table.palaeobox th {
       background-color: #e7dcc3;
       border: solid 1px gray;
       text-align: center;
       font-weight: bold;
 }
 table.palaeobox td {
       vertical-align:top;
 }
 table.palaeobox .Person {
       font-style: normal;
 }
 table.palaeobox div.thumb,
 table.palaeobox div.thumb * {
       margin: 0;
       padding: 0;
       float: none;
       border: none;
 }
 table.palaeobox div.magnify {
       display:none;
 }
 table.palaeobox tr td div.thumb div div.thumbcaption {
       text-align:left;
 }
 table.palaeobox td.Person {
       text-align:center;
       display:block;
 }
 table.palaeobox td.taxo-name {
       text-align:center;
 }
 table.palaeobox td.taxo-bild {
       text-align:center;
 }
 table.palaeobox td.taxo-zeit {
       text-align:center;
 }
 table.palaeobox td.taxo-ort {
       text-align:left;
 }

 /* aus Vorlage zur Entlastung, skinabhängigen Darstellung und Kombinierbarkeit hierher ausgelagert */
 .wikitable,
 .prettytable {
  margin: 1em 1em 1em 0;
  background: #f9f9f9;
  border: 1px #AAA solid;
  border-collapse: collapse;
  empty-cells:show;
 }

 .wikitable th, .wikitable td,
 .prettytable th, .prettytable td {
  border: 1px #AAA solid;
  padding: 0.3em;
 }

 .wikitable caption,
 .prettytable caption {
  font-weight: bold;
 }

 .nogrid th, .nogrid td {border: none;}
 div.float-left, table.float-left, .float-left { float: left; clear: left; }
 div.float-right, table.float-right, .float-right { float: right; clear: right; margin: 1em 0 1em 1em; }
 div.centered, table.centered, .centered { margin-left: auto; margin-right: auto; }
 .toptextcells td { vertical-align: top; }

 /* Bitte KEINE weiteren Definitionen dieser Art für Boxen hier, das gehört in entsprechende Vorlagen! */
 /* Hier 20 Mal Trivialitäten wie "text-align:center" zu definieren verlangsamt alles und ist nicht */
 /* Sinn der Sache. (Und wer nicht weiß warum, hat's nicht verstanden.) */

 /* Do not expand [[Wikipedia:WikiProjekt Georeferenzierung|kvaleberg.com-URLs]] for printing */

 #content span.coordinates a.external.text:after, #content span.coordinates a.external.autonumber:after { content: ""; }
 #content div.coordinates a.external.text:after, #content div.coordinates a.external.autonumber:after { content: ""; }

 /* Do not expand URLs for printing */

 #content span.plainlinks-print a.external.text:after, #content span.plainlinks-print a.external.autonumber:after { content: ""; }
 #content div.plainlinks-print a.external.text:after, #content div.plainlinks-print a.external.autonumber:after { content: ""; }

 /* Metadaten (bspw. [[Wikipedia:Personendaten|Personendaten]]) */

 table.metadata {
    border: 1px solid #aaaaaa;
    display: none;
 }
 .metadata-label {
    color: #aaaaaa;
 }
 span.metadata {
    display: none;
 }

 /*
  * Farbdefinitionen für Rahmen und Hintergründe von [[Wikipedia:Textbausteine|Textbausteinen]].
  * Hier: Standardvorgaben für "dunkle Schrift auf hellem Grund".
  * Für Hell-auf-dunkel-Skins ([[MediaWiki:Amethyst.css|Amethyst]]) müssen dort entsprechende Definitionen
  * zusätzlich eingetragen werden! Anpassungen für andere Skins sind optional.
  * (Die Angaben der Rahmenstärke dienen der Bequemlichkeit, so dass man
  * für dünne Rahmen nur noch "border-style" angeben muss.)
  */

 .rahmenfarbe1 { /* Wie Inhaltsverzeichnis */
    border-color: #aaaaaa;
    border-width: 1px;
 }
 .rahmenfarbe2 { /* Unauffällig, geringer Kontrast */
    border-color: #e9e9e9;
    border-width: 1px;
 }
 .rahmenfarbe3 { /* "Rot", auffällig */
    border-color: #c00000;
    border-width: 1px;
 }
 .rahmenfarbe4 { /* Neutrale Farbe, deutlich */
    border-color: #8888aa;
    border-width: 1px;
 }
 .rahmenfarbe5 { /* "Schwarz", hoher Kontrast */
    border-color: #000000;
    border-width: 1px;
 }

 table.wikitable tr.hintergrundfarbe1 th,
 table.wikitable tr th.hintergrundfarbe1,
 table.hintergrundfarbe1,
 .hintergrundfarbe1 { /* Wie Inhaltsverzeichnis */
    background-color: #f9f9f9;
 }
 table.wikitable tr.hintergrundfarbe2 th,
 table.wikitable tr th.hintergrundfarbe2,
 table.hintergrundfarbe2,
 .hintergrundfarbe2 { /* "Weiß", für Nicht-Artikel-Seiten, neutral */
    background-color: #ffffff;
 }
 table.wikitable tr.hintergrundfarbe3 th,
 table.wikitable tr th.hintergrundfarbe3,
 table.hintergrundfarbe3,
 .hintergrundfarbe3 { /* "Gelb", auffällig */
    background-color: #ffff40;
 }
 table.wikitable tr.hintergrundfarbe4 th,
 table.wikitable tr th.hintergrundfarbe4,
 table.hintergrundfarbe4,
 .hintergrundfarbe4 { /* Sehr auffällig */
    background-color: #ffaa00;
 }
 table.wikitable tr.hintergrundfarbe5 th,
 table.wikitable tr th.hintergrundfarbe5,
 table.hintergrundfarbe5,
 .hintergrundfarbe5 { /* Neutral, abgesetzt */
    background-color: #e0e0e0;
 }
 table.wikitable tr.hintergrundfarbe6 th,
 table.wikitable tr th.hintergrundfarbe6,
 table.hintergrundfarbe6,
 .hintergrundfarbe6 { /* Allgemein „bunt“, für Hervorhebungen und Unterscheidungen */
    background-color: #b3b7ff;
 }
 table.wikitable tr.hintergrundfarbe7 th,
 table.wikitable tr th.hintergrundfarbe7,
 table.hintergrundfarbe7,
 .hintergrundfarbe7 { /* Allgemein „bunt“, für Hervorhebungen und Unterscheidungen */
    background-color: #ffcbcb;
 }
 table.wikitable tr.hintergrundfarbe8 th,
 table.wikitable tr th.hintergrundfarbe8,
 table.hintergrundfarbe8,
 .hintergrundfarbe8 { /* Allgemein „bunt“, für Hervorhebungen und Unterscheidungen */
    background-color: #ffebad;
 }
 table.wikitable tr.hintergrundfarbe9 th,
 table.wikitable tr th.hintergrundfarbe9,
 table.hintergrundfarbe9,
 .hintergrundfarbe9 { /* Allgemein „bunt“, für Hervorhebungen und Unterscheidungen */
    background-color: #b9ffc5;
 }

 /* Keine Vergrößerung der Zeilenhöhe durch hochgestellte Zahlen der Fußnoten */

 sup.reference {
     font-weight: 400;
     font-style: normal;
 }
  
 sup, sub { 
     line-height: 1em;
 }


 /* Hervorhebung der angeklickten Fußnoten und der Rückverweise in blau */

 ol.references > li:target {
     background-color: #DEF;
 }

 sup.reference:target {
     background-color: #DEF;
 }

 /* Hochgestellte Buchstaben in der Einzelnachweiseliste kursiv setzen */

 ol.references li a[href|="#cite_ref"] {
     font-style: italic;
 }

 /* Inline-Verwendung der [[:Kategorie:Vorlage:Schwesterprojektverweis|Schwesterprojektverweise]] */

 ol.references li div.sisterproject {
    display:inline !important;
 }

 /* Für <nowiki><hiero>…</hiero></nowiki> */

 .mw-hierotable,
 .mw-hierotable th,
 .mw-hierotable td {
	border: 0px;
	padding: 0px;
 }

 /* Für [[MediaWiki:Anoneditwarning]] und weitere Hinweise, die alle untereinander stehen */

 #mw-anon-edit-warning, #mw-missingsummary, #wp_talkpagetext {
	width: 80%;
	background: #D3E1F2;
	border: 1px solid #1A47FF;
	margin: 1em auto;
	padding: 1em;
}

/* Bei URLs, die auf unser Projekt und verwandte Projekte verweisen, den Pfeil ausblenden
 * Dieser Pfeil dient nur dazu, auf externe Ziele hinzuweisen
 * Auf den Einsatz der Klasse "plainlinks" kann dadurch verzichtet werden
*/
 #content a[href^="http://de.wikipedia.org"],
 #content a[href^="http://toolserver.org"],
 #content a[href^="http://stable.toolserver.org"] {
	background: none !important;
	padding: 0 !important;
}

 /* Falls bei der Seitenbearbeitung vergessen wurde, die Zusammenfassung auszufüllen */
 .mw-summarymissed { border:5px solid red; padding:2px; } 

 /* Copyrightwarnung */

 #editpage-copywarn {
         border: solid 1px #c00000;
         font-size: 90%;
         background-color: #ffffff;
 }
        
 .mw-tos-summary {
         border: solid 1px #c00000;
         background-color: #ffffff;
         padding-left: 1em;
 }

 /* +++++ 2. SONSTIGE ANPASSUNGEN (Spezialseiten u. a.) +++++ */

 /* Markierung von Redirects in [[Special:Allpages]], [[Special:Watchlist]], Kategorien */

 .allpagesredirect, .watchlistredir, .redirect-in-category {
        font-style: italic;
 }

 /* Fettformatierung von Admin-Spezialseiten in [[Special:Specialpages]] abschalten */
 .mw-specialpagerestricted strong {
        font-weight:normal;
 }
 /* Legende auf [[Special:Specialpages]] ebenfalls abschalten */
 div.mw-specialpages-notes {
        display:none;
 }

 /* Einmal gelesene Bestandteile (z.B. in der Sitenotice) ausblenden, sobald css geladen ist */

 #gelesen {
        display:none;
 }

 /* Größerer Abstand zwischen TOC-Nummerierung und TOC-Eintrag */
 span.tocnumber {
	margin-right:0.3em;
 }

 /* Skinabhängige absolute Positionierungen ausblenden */
 /* Bitte [[MediaWiki Diskussion:Common.css#Absolute_Positionierungen]] beachten */

 #coordinates_3_ObenRechts, #issnlink, #editcount, #shortcut, #artikelstadium {
    display: none;
 }

 /* Anpassungen für [[:Template:Link_FA]] */

 /* hide the template */
 #bodyContent span.FA {
     display: none;
 }

 /* change the bullets for links to special articles */
 #p-lang li.FA {
     /* hier immer auch linkFA_bullet in Common.js mit anpassen für die älteren skins! */
     list-style-image: url("http://upload.wikimedia.org/wikipedia/commons/d/d0/Monobook-bullet-star-transparent.png");
 }

 /* change the bullets for links to special articles */
 #p-lang li.GA {
    /* hier immer auch linkGA_bullet in Common.js mit anpassen für die älteren skins! */
    list-style-image: url("http://upload.wikimedia.org/wikipedia/commons/a/a1/Monobook-bullet-star-gray.png");
 }

 /* Verhindere, dass Infoboxen etc. in den Inhalt von Kategorien hineinragen. 
 Das betrifft nur Kategorieseiten. */
 #mw-subcategories { clear:both; }
 #mw-pages { clear:both; }

 /* DIVs in ImageMaps inlinen (Vorschlag von Revolus) */
 .imagemap-inline div {
       display: inline;
 }

 /* [[Special:Hochladen]]: Warnmeldung auf bestehende Dateien, unerwünschte Prefixe usw. deutlicher hervorheben. */

 #wpDestFile-warning ul {
       border:solid red 1px;
       padding: 1.5em;
 }

 /* Warnmeldung für gesperrte Titel */
 .mw-titleprotectedwarning {
       background-color: #eee;
       border: 2px solid red;
       padding: 1em;
 }

 /* Warnmeldung bei der Bearbeitung von Seiten im MediaWiki-Namensraum */
 .mw-editinginterface {
       background-color: #f9f9f9;
       border-color: #c00000;
       border-width: 1px;
       border-style: solid;
       padding: 2px;
 }

 /* Tabellenhintergründe transparent machen */
 /* Vor allem auf Spezialseiten, da diese eine andere Hintergrundfarbe haben. */
 /* Update 21. März 2008: Wurde mit [[rev:32269]] und [[rev:32270]] zentralisiert. */

 table.searchResultImage, table#mw-sitematrix-table, table.mw-specialpages-table,
 table.mw-listgrouprights-table, table.diff, td.diff-otitle, td.diff-ntitle, table.TablePager_nav,
 table#sv-software, table#sv-ext, table.mw-tags-table, table.fr-diff-ratings, table#mw-search-top-table
  {
       background-color: transparent;
  }

 /* ÜBerschrift 5. und 6. Ordnung proportionaler zu Fließtext */
 #content h5 { font-size: 108%; } /* Original: 116% */
 #content h6 { font-size: 100%;  } /* Original: 80% = kleiner als Bodytext, unschön */
 #content h6 .editsection { font-size: 120%; font-weight: normal; }

 /* Anleitung auf Special:Search */
 .mw-searchresult {
       font-size: 84%;
       margin:5px;
  }

 /* Gestaltung der Edittools-Leiste */
 #specialchars {
       margin-top:3px;
       border:solid 1px #aaaaaa;
       padding:1px;
       text-align:left;
       background-color:white;
  }

 /* Legende für Versionsgeschichte */
 .mw-history-legend {
       font-size: 90%;
       margin-top: 2px;
       margin-bottom: 2px;
       border:solid 1px #e9e9e9;
       padding: 0 5px 5px 5px;
       background-color: #f9f9f9;
       clear: both;
  }

 /* +++++ 3. NEUE [[Wikipedia:Hauptseite|HAUPTSEITE]] (ab 2006) +++++ */

 /* Hintergrund weiß */
 body.page-Wikipedia_Hauptseite #content {
       background-color: white;
 }

 /* Kategorie verbergen */
 body.page-Wikipedia_Hauptseite #catlinks {
  display:none;
 }

 /* Überschrift verbergen */
 body.page-Wikipedia_Hauptseite h1.firstHeading {
  display: none;
 }

 /* Redirhinweis verbergen - ACHTUNG: Je nach Skin muss jetzt der Abstand zum oberen Rand korrigiert werden */
body.page-Wikipedia_Hauptseite #contentSub {
  display: none;
 }

 #hauptseite h2 {
       background-color: #d8e8ff;
       border: 1px solid #8898bf;
       font-size: 1em;
       font-weight: bold;
       margin-top: 0;
       margin-bottom: 0;
       padding-top: 0.1em;
       padding-bottom: 0.1em;
 }
 #hauptseite .inhalt {
       background-color: #ffffff;
       border: 1px solid #8898bf;
       border-top: 0px solid white;
       padding: 0.3em 0.8em 0.4em 0.8em;
 }
 #hauptseite .inhalt hr {
       background-color: #8898bf;
       color: #8898bf;
       height: 1px;
       margin:0.5em 0;
       padding: 0;
 }
 #hauptseite .inhalt .mehr {
       clear: both;
       font-size: 95%;
       margin-top: 0.8em;
       text-align: right;
 }

 #hauptseite table {
       background-color: transparent;
 }

 .hauptseite-oben,
 .hauptseite-links,
 .hauptseite-rechts {
       margin-bottom: 1em;
 }
 .hauptseite-links {
       margin-right: 0.5em;
 }
 .hauptseite-rechts {
       margin-left: 0.5em;
 }
 .hauptseite-oben h2,
 .hauptseite-unten h2 {
       text-align: center;
 }
 .hauptseite-oben .inhalt .portale {
       font-weight: bold;
       margin-top: 0.2em;
       margin-bottom: 0.2em;
 }
 .hauptseite-oben .inhalt .intern {
       font-size: 90%;
       text-align: center;
 }
 .hauptseite-links h2,
 .hauptseite-rechts h2 {
       text-indent: 0.8em;
 }
 #hauptseite-schwesterprojekte .inhalt a {
       font-weight: bold;
 }

 /* IPA links nicht unterstreichen */
 .IPA a:link, .IPA a:visited {
    text-decoration: none;
 }

 /* CSS-Klassen für Schriftarten
  * Achtung: IE6 mag keine Zeilenumbrüche zwischen den Fonts.
  */
 .Unicode {
     font-family: 'Code2000','Sun-ExtA','Arial Unicode MS','NSimSun',sans-serif;
 }
 .Unicode1 {
     font-family: 'Code2001','Quivira','MPH 2B Damase',sans-serif;
 }
 .Unicode2 {
     font-family: 'Sun-ExtB','Code2002',sans-serif;
 }
 .IPA {
     font-family: 'Quivira','Code2000','Sun-ExtA','DejaVu Sans','Gentium','Arial Unicode MS','Lucida Sans Unicode',sans-serif;
 }
 .IAST {
     font-family: 'Code2000','SunExtA','Arial Unicode MS',sans-serif;
 }
 .altitalisch {
     font-family: 'Quivira','Code2001','MPH 2B Damase',sans-serif;
 }
 .gotisch {
     font-family: 'Quivira','Code2001','MPH 2B Damase',sans-serif;
 }
 .hebrew {
     font-family: 'Quivira','Sun-ExtA','Arial Unicode MS','SBL Hebrew','Code2000','MPH 2B Damase',  sans-serif;
 }
 .spanAr {
     font-family: 'Arial Unicode MS','Scheherazade','Code2000','DejaVu Sans',sans-serif;
 }
 .music-symbol {
     font-family: 'Musical Symbols','Euterpe','Code2001',sans-serif;
 }

 /* Standardmäßige Ausblendung der Flagged-Revisions-Backlog-Sitenotice */
 #mw-oldreviewed-notice {
  display: none;
 }

 /* Standardmäßige Ausblendung der Flagged-Revisions-Kommentarbox */
 #mw-commentbox {
  display:none;
 }

 /* Abstand zwischen Liste und Inhaltsverzeichnis erhöhen, siehe http://de.wikipedia.org/w/index.php?title=MediaWiki_Diskussion:Common.css&oldid=51639015#Abstand_vor_Inhaltsverzeichnis */
 ul + .toc, ol + .toc {
  margin-top: 0.5em
 }

 /* höheres Uploadfeld */
 #wpUploadDescription {height: 20em;}

 /* Im Projekt WP:GEO benutzt die [[Vorlage:Coordinate]] das «geo-microformat» zur semantischen
 Auszeichnung des Texts. Der Inhalt dieses [[Tag (Informatik)|Tags]] ist nicht für den Leser bestimmt. */
 .geo {display:none;}

 /* Patrollink ausblenden, da die Eweiterung hier nicht aktiviert ist und
    deren Optik zu sehr den gesichteten Versionen ähnelt. */
 .fr-diff-patrollink, .patrollink { display:none; }

 /* Rollback-Knopf auf Beobachtungsliste ausblenden, da es dort nur von sehr beschränktem Nutzen
   ist und zu sehr vielen Reverts aus Versehen führt  */
 .page-Spezial_Beobachtungsliste .mw-rollback-link { display:none; }