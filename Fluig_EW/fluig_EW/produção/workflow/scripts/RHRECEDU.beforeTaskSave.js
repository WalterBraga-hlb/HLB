function beforeTaskSave(colleagueId,nextSequenceId,userList){
	var atv      = getValue("WKNumState");
    var nextAtv  = getValue("WKNextState");

    if (atv == 4) {

        var anexos   = hAPI.listAttachments();
        var temAnexo = false;

        if (anexos.size() > 0) {
            temAnexo = true;
        }

        if (!temAnexo) {
            throw "É preciso anexar o documento de solicitação assinada para continuar o processo!";
        }

    }
    
    if (atv == 10 && nextAtv == 13) {
		// Código para trazer o nome do usuário logado.
		var userFluig =  getValue("WKUser");
		var user = getUsuarioCorrente(userFluig);
		hAPI.setCardValue('diretor', user)
		hAPI.setCardValue('cod_diretor', userFluig)
		
		// Dados data atual
		var today = new Date();
	  	var year = today.getFullYear();
	  	//var month = today.getMonth() + 1;
	  	//var day = today.getDate();
	  	var month = today.getMonth() + 1 < 10 ? '0' + (today.getMonth() + 1) : (today.getMonth() + 1);
  	  	var day = today.getDate() < 10 ? '0' + today.getDate() : today.getDate();
	  	//var hour = today.getHours();
	  	//var minutes = today.getMinutes();
	  	var hour = today.getHours() < 10 ? '0' + (today.getHours()) : (today.getHours());
  	  	var minutes = today.getMinutes() < 10 ? '0' + today.getMinutes() : today.getMinutes();
	  	//var seconds = today.getSeconds();
	  	var seconds = today.getSeconds() < 10 ? '0' + today.getSeconds() : today.getSeconds();
	  	var todayStr = day+'/'+month+'/'+year+' '+hour+':'+minutes+':'+seconds;
		
		hAPI.setCardValue('data_aprov_diretor', todayStr)
		
		
		// Dados data atual
		var today = new Date().addDays(5);
	  	var year = today.getFullYear();
	  	//var month = today.getMonth() + 1;
	  	//var day = today.getDate();
	  	var month = today.getMonth() + 1 < 10 ? '0' + (today.getMonth() + 1) : (today.getMonth() + 1);
  	  	var day = today.getDate() < 10 ? '0' + today.getDate() : today.getDate();
	  	//var hour = today.getHours();
	  	//var minutes = today.getMinutes();
	  	var hour = today.getHours() < 10 ? '0' + (today.getHours()) : (today.getHours());
  	  	var minutes = today.getMinutes() < 10 ? '0' + today.getMinutes() : today.getMinutes();
	  	//var seconds = today.getSeconds();
	  	var seconds = today.getSeconds() < 10 ? '0' + today.getSeconds() : today.getSeconds();
	  	var todayPag = day+'/'+month+'/'+year;
		hAPI.setCardValue('data_pagamento', todayPag);
	}
}

function getUsuarioCorrente (userFunc){
    filter = new java.util.HashMap();
    filter.put('colleaguePK.colleagueId',userFunc);
    colaborador = getDatasetValues('colleague',filter);
    return colaborador.get(0).get('colleagueName');
}

Date.prototype.addDays = function(days) {
    var date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date;
}