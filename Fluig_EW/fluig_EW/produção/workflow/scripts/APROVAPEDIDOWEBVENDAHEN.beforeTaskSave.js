function beforeTaskSave(colleagueId,nextSequenceId,userList){
	var atividadeAtual = getValue('WKNumState');
	var proxAtividade = getValue("WKNextState");
	var userFluig = getValue("WKUser");
	var nomeUsuario = getUsuario(userFluig,'colleagueName');
	var comentario = getValue("WKUserComment"); 
	
	if (atividadeAtual == 5 && proxAtividade == 11){
		if (comentario == "") {
            throw "Obrigatório informar um motivo ao reprovar! (Aba 'Complementos' no Fluig)"
        }
	}
	
	if (atividadeAtual == 5){
		hAPI.setCardValue("data_decisao", getNow().substring(0,16));
		hAPI.setCardValue("decisor", nomeUsuario);
		hAPI.setCardValue("usuario_decisor", userFluig);
		if (proxAtividade == 9) {
			hAPI.setCardValue("status", 'Aprovada');
		} else {
			hAPI.setCardValue("status", 'Reprovada');
		}
	}
}

function getNow() {
	var today = new Date();
	var dd = today.getDate();
	var mm = today.getMonth()+1; //January is 0!
	var yyyy = today.getFullYear();
	var hour = today.getHours() < 10 ? '0' + (today.getHours()) : (today.getHours());
	var minutes = today.getMinutes() < 10 ? '0' + today.getMinutes() : today.getMinutes();
  	var seconds = today.getSeconds() < 10 ? '0' + today.getSeconds() : today.getSeconds();
	 if(dd<10){
	        dd='0'+dd
	    } 
	    if(mm<10){
	        mm='0'+mm
	    } 

	return today = yyyy+'-'+mm+'-'+dd+' '+hour+':'+minutes+':'+seconds;
}

function getUsuario(userFunc, field){
    filter = new java.util.HashMap();
    filter.put('colleaguePK.colleagueId',userFunc);
    colaborador = getDatasetValues('colleague',filter);
    return colaborador.get(0).get(field);
}