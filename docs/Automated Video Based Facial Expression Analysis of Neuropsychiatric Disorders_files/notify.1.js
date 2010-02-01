// /web/private/htdocs/staff/sponomar/TEST/corehtml/jsutils/notify.1.js.orig

function Notifier(){this.oQuee=new Array;this.bTraceOn=false;}
Notifier.prototype.setListener=function(oListener,sMessage,fFunction,oNotifier){var oThis=this;if(utils.isArray(oListener)){for(var i in oListener){x_setListener(oListener[i],sMessage,fFunction,oNotifier);}}else{return x_setListener(oListener,sMessage,fFunction,oNotifier);}
function x_setListener(oListener,sMessage,fFunction,oNotifier){for(var i in oThis.oQuee[sMessage]){if(oThis.oQuee[sMessage][i].obj===oListener){if(oListener.NAME)console.info("Reset setListener [",i,"] ",oListener.NAME,sMessage);oThis.oQuee[sMessage][i]=({obj:oListener,fun:fFunction,ntf:oNotifier});return true;}}
if(!oThis.oQuee[sMessage])oThis.oQuee[sMessage]=new Array();var i=oThis.oQuee[sMessage].length;if(oThis.bTraceOn)console.info("setListener:",sMessage,"["+i+"]: ",oListener.NAME);oThis.oQuee[sMessage][i]=({obj:oListener,fun:fFunction,ntf:oNotifier});return true;}}
Notifier.prototype.Notify=function(oNotifier,sMessage,oComment,oListener){var oThis=this;var sAnyMessage="*";for(var i in this.oQuee[sAnyMessage]){if(null==oListener||this.oQuee[sAnyMessage][i].obj==oListener){if(this.oQuee[sAnyMessage][i].ntf==null||this.oQuee[sAnyMessage][i].ntf===oNotifier){if("function"==typeof this.oQuee[sAnyMessage][i].fun)
this.oQuee[sAnyMessage][i].fun(this.oQuee[sAnyMessage][i].obj,oComment,sMessage,oNotifier);}}}
for(var i in this.oQuee[sMessage]){if(null==oListener||this.oQuee[sMessage][i].obj==oListener){if(this.oQuee[sMessage][i].ntf==null||this.oQuee[sMessage][i].ntf===oNotifier){if("function"==typeof this.oQuee[sMessage][i].fun){if(oThis.bTraceOn)console.info("Notify:",sMessage,"["+i+"]: ",this.oQuee[sMessage][i].obj.NAME);if(this.oQuee[sMessage][i].fun(this.oQuee[sMessage][i].obj,oComment,sMessage,oNotifier)){return;}}}}}}
Notifier.prototype.Clear=function(sMessage){if("*"==sMessage){this.oQuee=[];}else{this.oQuee[sMessage]=[];}}
Notifier.getInstance=function(){if(!Notifier.instance){Notifier.instance=new Notifier();}
return Notifier.instance;}