function inputFields(form){
	var atividadeAtual = getValue("WKNumState");
	var proxAtividade = getValue("WKNextState");
	var userFluig = getValue("WKUser");
	var nomeUsuario = getUsuario(userFluig,'colleagueName');
	
	var dataSolicitacao = form.getValue("data_solicitacao");
	if (dataSolicitacao != "") {
		var split = dataSolicitacao.split('/');
	    form.setValue("data_solicitacao",split[2].substring(0,4) + '-' + split[1] + '-' + split[0]+ ' '
	    		+dataSolicitacao.substring(11,13)+':'+dataSolicitacao.substring(14,16));
	}
	var dataDecisao = form.getValue("data_decisao");
	if (dataDecisao != "") {
		var split = dataDecisao.split('/');
	    form.setValue("data_decisao",split[2].substring(0,4) + '-' + split[1] + '-' + split[0]+ ' '
	    		+dataDecisao.substring(11,13)+':'+dataDecisao.substring(14,16));
	}
	var indexes = form.getChildrenIndexes("itens_pedido_venda");
	for (var i = 0; i < indexes.length; i++) {
    	var dataInicialEntrega = form.getValue('data_inicial_entrega_ipv___' + indexes[i]);
    	if (dataInicialEntrega != '') {
    		var split = dataInicialEntrega.split('/');
    		form.setValue("data_inicial_entrega_ipv___" + indexes[i], 
    				split[2] + '-' + split[1] + '-' + split[0]);
    	}
    	var dataFinalEntrega = form.getValue('data_final_entrega_ipv___' + indexes[i]);
    	if (dataFinalEntrega != '') {
    		var split = dataFinalEntrega.split('/');
    		form.setValue("data_final_entrega_ipv___" + indexes[i], 
    				split[2] + '-' + split[1] + '-' + split[0]);
    	}
	}
	
//	if (atividadeAtual == 5){
//		form.setValue('data_decisao',getNow().substring(0,16));
//		form.setValue('decisor',nomeUsuario);
//		form.setValue('usuario_decisor',userFluig);
//		if (proxAtividade == 9) {
//			form.setValue('status','Aprovada');
//		} else {
//			form.setValue('status','Reprovada');
//		}
//	}
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