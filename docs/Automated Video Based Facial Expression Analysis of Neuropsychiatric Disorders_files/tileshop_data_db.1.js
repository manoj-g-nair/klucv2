

utils.jsLoader.load(["data_provider.1.js","remote_data_provider.1.js"]);function TileDataDb(oImg,oNotifier){this.NAME="TileDataDb";var oThis=this;this.sProjectId="";this.sPictureId="";this.sSatId="";this.oMetadata="";this.Init(oImg,oNotifier);var oDataProvider=new RemoteDataProvider();oDataProvider.sUrl=this.sUrl+"?manifest=1&p="+this.sProjectId
+"&id="+this.sPictureId+"&w="+this.oViewport.w+"&h="+this.oViewport.h;oDataProvider.onSuccess=function(obj){eval("oThis.oMetadata="+obj.responseText);oNotifier.Notify(oThis,"metadata",oThis.oMetadata);};oDataProvider.onError=function(obj){alert("Error occured: can not get metadata. Check Project name and/or Image name");};function x_Update(oMetadata,i){oThis.fScale=oMetadata.aView[i].W/oMetadata.aView[0].W;oThis.bIsStaticImage=oMetadata.aView[i].W==oMetadata.aView[i].w&&oMetadata.aView[i].H==oMetadata.aView[i].h;oThis.oPicture.w=oMetadata.aView[i].W;oThis.oPicture.h=oMetadata.aView[i].H;oThis.sSat=oMetadata.Sat;oThis.sTileDbId=oMetadata.aView[i].sId;oThis.sPrefix="id_"+oThis.sSat+"_"+oThis.sTileDbId;oThis.oTile.w=oMetadata.aView[i].w;oThis.oTile.h=oMetadata.aView[i].h;oThis.Calculate();}
oNotifier.setListener(this,"metadata",function(oListener,oMetadata){oThis.oMetadata=oMetadata;if(oMetadata.aView.length<1){return true;}
oThis.iScaleIndex=0;if(oThis.fScale==-1){oThis.iScaleIndex=oMetadata.aView.length-2;if(oThis.iScaleIndex<0)oThis.iScaleIndex=0;}else if(oThis.fScale==0){oThis.iScaleIndex=oMetadata.aView.length-1;}else if(oThis.fScale<=1&&oThis.fScale>0){var W=oMetadata.aView[0].W*oThis.fScale;for(oThis.iScaleIndex=0;oThis.iScaleIndex<oMetadata.aView.length-1;oThis.iScaleIndex++){var s=oMetadata.aView[oThis.iScaleIndex].W/W;if(s<=0.5||s<1.36)break;}}else
oThis.iScaleIndex=0;var W=oMetadata.aView[0].W;for(var i=0;i<oMetadata.aView.length;i++){var v=oMetadata.aView[i].W/W;oThis.oScales[i]={n:oMetadata.aView[i].sName,v:v,w:oMetadata.aView[i].W,h:oMetadata.aView[i].H,enable:true};if(oMetadata.aView[i].W==oMetadata.aView[i].w&&oMetadata.aView[i].H==oMetadata.aView[i].h){oThis.oScales[i]={n:oMetadata.aView[i].sName,v:v};break;}}
x_Update(oMetadata,oThis.iScaleIndex);},null);oNotifier.setListener(this,"scale",function(oListener,oComment){oThis.iScaleIndexPrevious=oThis.iScaleIndex;switch(oComment){case"up":if(oThis.iScaleIndex<1)return;oThis.iScaleIndex--;while(!oThis.oScales[oThis.iScaleIndex]){oThis.iScaleIndex--;if(oThis.iScaleIndex<1)break;}
break;case"down":if(oThis.oScales.length-1<=oThis.iScaleIndex)return;oThis.iScaleIndex++;while(oThis.oScales[oThis.iScaleIndex]==null){oThis.iScaleIndex++;if(oThis.oScales.length-1<=oThis.iScaleIndex)break;}
break;default:oThis.iScaleIndex=oComment;break;}
x_Update(oThis.oMetadata,oThis.iScaleIndex);});setTimeout(function(){oDataProvider.Request();},5);}
TileDataDb.prototype=new TileData();TileDataDb.prototype.Parse=function(arr){var tmp;if(arr.indexOf("p=")==0){tmp=arr.split("=");this.sProjectId=tmp[1];}else if(arr.indexOf("id=")==0){tmp=arr.split("=");this.sPictureId=tmp[1];}}
TileDataDb.prototype.GetTileUrl=function(row,col){return this.sUrl+"?p="+this.sProjectId
+"&id="+this.sTileDbId+"&s="+this.sSat
+"&r="+(row+1)+"&c="+(col+1);}
TileDataDb.prototype.GetFitUrl=function(){return this.sUrl+"?p="+this.sProjectId
+"&id="+this.oMetadata.aView[this.oMetadata.aView.length-1].sId
+"&s="+this.sSat+"&r=1&c=1";}