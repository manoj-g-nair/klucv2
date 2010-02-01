/*-----------------------------------------------------------------------------*/
/* Änderungen am Monobook-Skin für die deutschsprachige Wikipedia              */
/* --> Skinübergreifende Anpassungen befinden sich in [[MediaWiki:Common.css]] */
/*-----------------------------------------------------------------------------*/

/* Änderungen in Fett markieren */
span.diffchange {
        font-weight: bold;
}

/* Links nicht unterstreichen */
a {
       text-decoration: none;
}

/* Fetter Bearbeiten-Link */
#ca-edit a {
	font-weight: bold;
}

/* Vermeide hässliche weiße Ränder bei Float-Objekten auf nicht-weißen Hintergründen. */
div.thumb {
	border: none;
}
div.tright {
	border: none;
	margin: 0.5em 0 0.8em 1.4em;
}
div.tleft {
	border: none;
	margin: 0.5em 1.4em 0.8em 0;
}

/* fromWikipedia */
#fromWikipedia {
	font-size: x-small;
}

/* Make all non-namespace pages have a light blue content area. This is done by
	setting the background color for all #content areas to light blue and then
	overriding it for any #content enclosed in a .ns-0 (main namespace). I then
	do the same for the "tab" background colors. --Lupo */
#content,
#mytabs li.selected a,
#p-cactions li.selected a {
	background-color: #F3F3FF; /* ein leichtes blau */
}
.ns-0 #content,
.ns-0 #mytabs li.selected a,
.ns-0 #p-cactions li.selected a {
	background-color: white;
}

/* Kategorien etwas schöner darstellen */
#catlinks {
	border: 0px;
	border-top: 2px dotted #aaaaaa;
}

/*  Kategorienseite mit transparentem Hintergrund */
#mw-subcategories table {
       background-color: transparent;
}

#mw-pages table {
       background-color: transparent;
}

/* Tabellenüberschriften (mit |+) größer und fett */
caption {
	font-size: 105%;
	font-weight: bold;
}

/* Einzelnachweise */
ol.references {
	font-size:90%;
}

/* Abschnitt-Bearbeiten-Links */
.editsection {
	font-size:90%;
}

/* [[Wikipedia:Externhinweis|Externhinweis]] */
#externHinweis {
	font-size: x-small;
	text-align: center;
	padding: 5px;
}

/*-----------------------------------------------------------------------------*/

/* Absolute Positionierungen für Monobook.css */
/* Bitte [[MediaWiki Diskussion:Common.css#Absolute Positionierungen]] beachten */

/* Koordinaten-Vorlagen */
#coordinates_3_ObenRechts {
      display: inline;
}

#coordinates {
   display:inline;
   position:absolute; z-index:1; border:none; background:none; right:12px; top:0.2em; float:right; margin:0 18px 0 0;
   padding: 0 0.1em 0 0; line-height:11px; text-align:right; text-indent:0; font-size:10px; text-transform:none; white-space:nowrap;
}

/* Unterdrückung des Weblink-Pfeiles für im Artikel eingebettete Koordinaten */
#bodyContent span.coordinates a.external[href ^="http://"] {
     background-image:none;
     padding-right: 0;
}

/* Sitenotice linksbündig, sollte dort keine Probleme machen, dafür die Koordinaten freihalten */
/* (für die Dauer der Spendenaktion deaktivert, weil der Rest mittig steht. Ggfs. neu anpassen */
/* #siteNotice p {text-align:left;} */
/* #siteNotice {padding-left:0;}    */

/* Sitenotice Hintergrund transparent, vor allem für die nicht-Artikelseiten. */
#siteNotice table {
     background-color: transparent;
     font-size: 100%;
}

/* verhindert, dass der Verbergen-Link die Icons rechts oben verdeckt */
#mw-dismissable-notice td {
    vertical-align: bottom;
}

/* Formatierung für [[Wikipedia:Editcount]]-Vorlagen und [[Vorlage:ISSN-Link]] (Schriftgröße 85%) */
#editcount,
#issnlink {
     display: inline;
     position:absolute; z-index:1; border:none; background:none; right:12px; top:0.3em; float:right; margin:0.0em;
     padding:0.0em; line-height:1.5em; text-align:right; text-indent:0; font-size:85%; text-transform:none; white-space:normal;
}

/* Formatierung für Abkürzungen (Schriftgröße 90%) */
#shortcut {
     display: inline;
     position:absolute; z-index:1; border:none; background:none; right:12px; top:0.3em; float:right; margin:0.0em;
     padding:0.0em; line-height:1.5em; text-align:right; text-indent:0; font-size:90%; text-transform:none; white-space:normal;
}

/* Formatierung für Kennzeichnung von Artikelstadien (Exzellent, Lesenswert etc.) */
#artikelstadium {
     display: block;
     position:absolute; z-index:1; border:none; background:none; right:12px; top:0.3em; float:right; margin:0.0em;
     padding:0.0em; line-height:1.5em; text-align:right; text-indent:0; font-size:90%; text-transform:none; white-space:normal;
}

/*-----------------------------------------------------------------------------*/
/* Anpassungen für die Stabilen Versionen */
/*-----------------------------------------------------------------------------*/

/* Die Prüfbox verschieben */
.flaggedrevs_short { float:right; position:relative; z-index:1; top:-33px; right:20px; border:0; padding:1px; margin-left:25px; margin-bottom:-28px; }

/* Tabelle mit redundantem Sichtungsbalken in der Prüfbox ausblenden */
table #mw-revisionratings-box { display:none; }

/* Korrektur des oberen Rands auf der Hauptseite nach Entfernung des Redirhinweises */
.page-Wikipedia_Hauptseite #content {
 padding: 1em;
}

/* Korrektur Reiter auf der Hauptseite */
.page-Wikipedia_Hauptseite #mytabs li.selected a,
.page-Wikipedia_Hauptseite #p-cactions li.selected a {
       background-color: white;
}

/* Abstand der Box zu den positionierten Elementen oben rechts (WP:ABK, Geokoordinaten, Bapperl) */
div.siteNoticeBig,
div#centralNotice.collapsed {margin-top:22px;}