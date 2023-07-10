function displayFields(form,customHTML){ 
	// Formata Datas
	var dataSolicitacao = form.getValue("data_solicitacao");
	if (dataSolicitacao != "") {
	    form.setValue("data_solicitacao",dataSolicitacao.substring(8,10)+'/'+dataSolicitacao.substring(5,7)+'/'
	    		+dataSolicitacao.substring(0,4)+ ' '
	    		+dataSolicitacao.substring(11,13)+':'+dataSolicitacao.substring(14,16));
	}
	var dataDecisao = form.getValue("data_decisao");
	if (dataDecisao != "") {
	    form.setValue("data_decisao",dataDecisao.substring(8,10)+'/'+dataDecisao.substring(5,7)+'/'
	    		+dataDecisao.substring(0,4)+ ' '
	    		+dataDecisao.substring(11,13)+':'+dataDecisao.substring(14,16));
	}
	
	var indexes = form.getChildrenIndexes("itens_pedido_venda");
	for (var i = 0; i < indexes.length; i++) {
    	var dataInicialEntrega = form.getValue('data_inicial_entrega_ipv___' + indexes[i]);
    	if (dataInicialEntrega != '') {
    		var split = dataInicialEntrega.split('-');
    		form.setValue("data_inicial_entrega_ipv___" + indexes[i], 
    				split[2] + '/' + split[1] + '/' + split[0]);
    	}
    	var dataFinalEntrega = form.getValue('data_final_entrega_ipv___' + indexes[i]);
    	if (dataFinalEntrega != '') {
    		var split = dataFinalEntrega.split('-');
    		form.setValue("data_final_entrega_ipv___" + indexes[i], 
    				split[2] + '/' + split[1] + '/' + split[0]);
    	}
	}
}