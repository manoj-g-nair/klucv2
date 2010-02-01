/* CSS placed here will affect the print output */

 /* Navigationsleisten nicht mit ausdrucken */
 
 .NavFrame {
   display: none;
 }
 .BoxenVerschmelzen {
   display: none;
 }

 /* (+/-) in Sichtungsbox nicht mit ausdrucken */
 #mw-revisiontoggle {
   display:none;
 }

 /* ↑ a b c usw. in der Einzelnachweiseliste nicht mit ausdrucken,
    sinnlos, da man auf dem Papiert nicht klicken kann und die Buchstaben
    im Text auch nicht vorkommen
  */
 ol.references li a[href|="#cite_ref"] {display:none;}