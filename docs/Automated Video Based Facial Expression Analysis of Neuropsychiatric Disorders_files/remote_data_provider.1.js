// /web/private/htdocs/staff/sponomar/TEST/corehtml/jsutils/remote_data_provider.1.js.orig

function RemoteDataProvider(sUrl){this.sUrl=sUrl;this.bAsync=true;this.iTimeout=0;var oHttpObj=null;}
RemoteDataProvider.iCount=0;RemoteDataProvider.prototype.x_GetHttpObj=function(){oHttpObj=null;try{oHttpObj=new ActiveXObject("Msxml2.XMLHTTP");}catch(e){try{oHttpObj=new ActiveXObject("Microsoft.XMLHTTP")}catch(oc){oHttpObj=null;}}
if(!oHttpObj&&typeof XMLHttpRequest!="undefined"){oHttpObj=new XMLHttpRequest();}
return oHttpObj;}
RemoteDataProvider.prototype.x_onChange=function(oHttpObj,oTimer){if(oTimer.bTimeout)return;var iStatus;try{iStatus=oHttpObj.status;}catch(e){return;}
if(oHttpObj.readyState==4&&iStatus==200){RemoteDataProvider.iCount--;if(oTimer.oTimer)clearTimeout(oTimer.oTimer);this.onSuccess(oHttpObj);this.onStop();}else if(oHttpObj.readyState==4&&iStatus!=200){RemoteDataProvider.iCount--;if(oTimer.oTimer)clearTimeout(oTimer.oTimer);this.onError(oHttpObj);this.onStop();}}
RemoteDataProvider.prototype.x_Init=function(oTimer){var oHttpObj=this.x_GetHttpObj();if(null==oHttpObj)return null;if(oHttpObj.readyState!=0)oHttpObj.abort();var oThis=this;if(this.bAsync){oHttpObj.onreadystatechange=function(){oThis.x_onChange(oHttpObj,oTimer);};}
RemoteDataProvider.iCount++;this.onStart();if(this.iTimeout>0){oTimer.bTimeout=false;oTimer.oTimer=setTimeout(function(){RemoteDataProvider.iCount--;oTimer.bTimeout=true;oHttpObj.abort();oThis.onTimeout(oThis.iTimeout);},this.iTimeout);}
return oHttpObj;}
RemoteDataProvider.prototype.Abort=function(){if(RemoteDataProvider.iCount>0)RemoteDataProvider.iCount--;oHttpObj.abort();}
RemoteDataProvider.prototype.Get=function(sRequest){var sUrl=this.sUrl+(sRequest?sRequest:"");var oTimer={};var oHttpObj=this.x_Init(oTimer);if(oHttpObj){oHttpObj.open("get",sUrl,this.bAsync);oHttpObj.send(null);if(!this.bAsync){this.x_onChange(oHttpObj,oTimer);}}}
RemoteDataProvider.prototype.Post=function(sRequest){this.Request(null,sRequest);}
RemoteDataProvider.prototype.Request=function(sGetRequest,sPostRequest){var sUrl=this.sUrl+(sGetRequest?sGetRequest:"");var oTimer={};var oHttpObj=this.x_Init(oTimer);if(oHttpObj){if(typeof sPostRequest!="string"||sPostRequest==""){sPostRequest="";}
if(sPostRequest>""){oHttpObj.open("post",sUrl,this.bAsync);oHttpObj.setRequestHeader("Content-type","application/x-www-form-urlencoded");oHttpObj.setRequestHeader("Content-length",sPostRequest.length);oHttpObj.send(sPostRequest);}else{oHttpObj.open("get",sUrl,this.bAsync);oHttpObj.send(null);}
if(!this.bAsync){this.x_onChange(oHttpObj,oTimer);}}}
RemoteDataProvider.prototype.onSuccess=function(obj){alert(["succes:",obj.responseText]);}
RemoteDataProvider.prototype.onStart=function(){}
RemoteDataProvider.prototype.onStop=function(){}
RemoteDataProvider.prototype.onError=function(obj){alert(["error:",obj.status]);}
RemoteDataProvider.prototype.onTimeout=function(iTime){alert(["timeout:",iTime+" ms"]);}