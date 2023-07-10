function onNotify(subject, receivers, template, params){
log.info('inicio - onNotify - APROVAPEDIDOWEBVENDAHLB - ' + template);
	
	if (template == "TPLNEW_TASK" 
		|| template == "TPLNEW_TASK_POOL_GROUP"
		|| template == "TPLNEW_TASK_POOL_ROLE"
		|| template == "TPLPROCESS_COMPLETED_TO_REQUESTER")
    {
		var atividadeAtual = getValue("WKNumState");
    	var proximaAtividade = getValue("WKNextState");
    	var numeroDaSolicitacao = getValue('WKNumProces');
    	var comentario = getValue("WKUserComment"); 
    	
    	log.info('atividadeAtual: ' + atividadeAtual);
    	log.info('proximaAtividade: ' + proximaAtividade);
    	
    	var nomeResponsavelEmail = '';
        var subject = '';
        var descricaoEmail = '';
        var listaDocumentos = '';
        var destinatarios = new java.util.ArrayList();
        
        // Carrega os dados do formulário
        var solicitante = hAPI.getCardValue('solicitante');
    	var excecao = hAPI.getCardValue('mensagem');
    	var decisor = hAPI.getCardValue('decisor');
    	
    	// Carrega os dados do pedido
        var idPedido = hAPI.getCardValue('id_web');
    	var empresa = hAPI.getCardValue('empresa');
    	var codigoCliente = hAPI.getCardValue('codigo_cliente');
    	var nomeCliente = hAPI.getCardValue('nome_cliente');
    	var cidade = hAPI.getCardValue('cidade');
    	var uf = hAPI.getCardValue('uf');
    	var codigoRepresentante = hAPI.getCardValue('codigo_representante');
    	var nomeRepresentante = hAPI.getCardValue('nome_representante');
    	var emailRepresentante = hAPI.getCardValue('email_representante');
    	
    	var descricaoPedido = 'Cliente: ' + codigoCliente + ' - ' 
    		+ nomeCliente + ' - ' + cidade + '-' + uf +'<br />'
    		+ 'Representante: ' + codigoRepresentante + ' - ' + nomeRepresentante + '<br /><br />';
    	
    	// Carrega e-mails de cópia
    	var emailGerencia = 'jcarchangelo@hyline.com.br';
    	var emailProgramacao = 'programacao@hyline.com.br';
    	var emailFinanceiro = 'financeiro@hyline.com.br';
    	if (empresa == 'LB') {
    		emailGerencia = 'lklassmann@ltz.com.br';
        	emailProgramacao = 'programacao@ltz.com.br';
        	emailFinanceiro = 'financeiro@ltz.com.br';
    	} else if (empresa == 'HN') {
    		emailGerencia = 'cfracaro@hnavicultura.com.br';
        	emailProgramacao = 'programacao@hnavicultura.com.br';
        	emailFinanceiro = 'financeiro@hnavicultura.com.br';
    	} else if (empresa == 'PL') {
    		emailGerencia = 'ccervelatti@planaltopostura.com.br';
        	emailProgramacao = 'programacao@planaltopostura.com.br';
        	emailFinanceiro = 'financeiro@planaltopostura.com.br';
    	} else if (empresa == 'NG') {
    		emailGerencia = 'garaujo@novogen.com.br';
        	emailProgramacao = 'programacao@novogen.com.br';
        	emailFinanceiro = 'financeiro@novogen.com.br';
    	}
    	
    	// Carrega os dados dos itens do pedido
    	var qtdeTotal = 0;
    	var valorTotal = 0;
    	var camposItens = hAPI.getCardData(numeroDaSolicitacao);
    	var contadorItens = camposItens.keySet().iterator();
    	var listaItens = 
			'<table class="DescrMsgForum">'
				+ '<thead>'
    				+ '<tr class="tableHeadRow">'
	    				+ '<th class="tableColumn">Seq.</th>'
	    				+ '<th class="tableColumn">De</th>'
	    				+ '<th class="tableColumn">A</th>'
	    				+ '<th class="tableColumn">Linha</th>'
	    				+ '<th class="tableColumn">Qt. Líq.</th>'
	    				+ '<th class="tableColumn">% Bon.</th>'
	    				+ '<th class="tableColumn">Qt. Total</th>'
	    				+ '<th class="tableColumn">Pr. Tot.</th>'
	    				+ '<th class="tableColumn">Val. Tot.</th>'
    				+ '</tr>'
				+ '</thead>'
				+ '<tbody>';
    	
    	while (contadorItens.hasNext()) {
        	var id = contadorItens.next();
        	if (id.match(/sequencia_ipv___/)) { // qualquer campo do Filho
        		var seq = id.split("___");
        		var sequenciaIpv = camposItens.get(id);
        		var dataInicialIpv = camposItens.get("data_inicial_entrega_ipv___" + seq[1]);
        		dataInicialIpv = dataInicialIpv.substring(8,10)+'/'+dataInicialIpv.substring(5,7)+'/'
		    		+dataInicialIpv.substring(0,4);
        		var dataFinalIpv = camposItens.get("data_final_entrega_ipv___" + seq[1]);
        		dataFinalIpv = dataFinalIpv.substring(8,10)+'/'+dataFinalIpv.substring(5,7)+'/'
	    			+dataFinalIpv.substring(0,4);
        		var linhaIpv = camposItens.get("linha_ipv___" + seq[1]);
        		var qtdeLiquidaIpv = camposItens.get("qtde_liquida_ipv___" + seq[1]);
        		var percBonificadaIpv = camposItens.get("perc_bonificada_ipv___" + seq[1]);
        		var qtdeTotalIpv = camposItens.get("qtde_total_ipv___" + seq[1]);
        		var precoTotalIpv = camposItens.get("preco_total_ipv___" + seq[1]);
        		var valorTotalIpv = camposItens.get("valor_total_ipv___" + seq[1]);
        		
        		listaItens = listaItens
        				+'<tr>'
        					+ '<td>' + sequenciaIpv +'</td>'
        					+ '<td>' + dataInicialIpv +'</td>'
        					+ '<td>' + dataFinalIpv +'</td>'
        					+ '<td>' + linhaIpv +'</td>'
        					+ '<td>' + qtdeLiquidaIpv +'</td>'
        					+ '<td>' + percBonificadaIpv +'</td>'
        					+ '<td>' + qtdeTotalIpv +'</td>'
        					+ '<td>' + precoTotalIpv +'</td>'
        					+ '<td>' + valorTotalIpv +'</td>'
        				+'</tr>';
        		
        		qtdeTotal = qtdeTotal 
        			+ parseInt(qtdeLiquidaIpv.replace('.','').replace(',','.'))
        			+ parseInt((parseFloat(percBonificadaIpv.replace('.','').replace(',','.')) / 100.0) 
        					* parseFloat(qtdeLiquidaIpv.replace('.','').replace(',','.')));
        		valorTotal = valorTotal + parseFloat(valorTotalIpv.replace('.','').replace(',','.'));
        	}
        }
    	
    	listaItens = listaItens
    			+ '</tbody>'
    		+ '</table>'
    		+ '<br />'
    		+ '<b>Qtde. Total do Pedido: ' + qtdeTotal + '</b>'
    		+ '<br />'
    		+ '<b>Valor. Total do Pedido: R$ ' + valorTotal + '</b>';
    	
    	if (atividadeAtual == 4) {
    		for (var i = 0; i < receivers.size(); i++) {
	    		destinatarios.add(getFieldColleague(receivers.get(i), 'mail', 1));
	    	}
    		//destinatarios.add(emailGerencia);
    		destinatarios.add(emailProgramacao);
    		destinatarios.add(emailFinanceiro);
	    	//destinatarios.add('hyline');
	    	
	    	//Limpa destinatários do e-mail com template padrão
            receivers.clear();
            
            nomeResponsavelEmail = 'Aprovador';
            subject = excecao;
            descricaoEmail = 'O usuário ' + decisor + ' solicitou a aprovação da seguinte exceção: <b>'
            	+ excecao + '</b>.'
            	+ ' Seguem abaixo os dados do pedido:';
    	} else if (atividadeAtual == 5 && proximaAtividade == 11) {
    		for (var i = 0; i < receivers.size(); i++) {
	    		destinatarios.add(getFieldColleague(receivers.get(i), 'mail', 1));
	    	}
    		//destinatarios.add(emailGerencia);
    		destinatarios.add(emailProgramacao);
    		destinatarios.add(emailFinanceiro);
    		destinatarios.add(emailRepresentante);
	    	//destinatarios.add('hyline');
	    	
	    	//Limpa destinatários do e-mail com template padrão
            receivers.clear();
            
            nomeResponsavelEmail = solicitante;
            subject = excecao + ' Reprovada';
            descricaoEmail = 'O usuário ' + decisor + ' reprovou a exceção a seguir: <b>'
            	+ excecao + '</b>.'
            	+ 'Motivo: ' + comentario + '</b>.'
	        	+ ' Seguem abaixo os dados do pedido:';
    	} else if (atividadeAtual == 5 && proximaAtividade == 9) {
    		for (var i = 0; i < receivers.size(); i++) {
	    		destinatarios.add(getFieldColleague(receivers.get(i), 'mail', 1));
	    	}
	    	//destinatarios.add('hyline');
    		//destinatarios.add(emailGerencia);
    		destinatarios.add(emailProgramacao);
    		destinatarios.add(emailFinanceiro);
    		destinatarios.add(emailRepresentante);
	    	
	    	//Limpa destinatários do e-mail com template padrão
            receivers.clear();
            
            nomeResponsavelEmail = solicitante;
            subject = excecao + ' Aprovada';
            descricaoEmail = 'O usuário ' + decisor + ' aprovou a exceção a seguir: <b>'
	            + excecao + '</b>.'
	        	+ ' Seguem abaixo os dados do pedido:';
    	}
        
        // Envia o e-mail
        var parametros = new java.util.HashMap();
        parametros.put("RESPONSAVEL", nomeResponsavelEmail);
        parametros.put("DESCRICAO_TAREFAS_ATRASADAS" , descricaoPedido + listaItens);
        parametros.put("DESCRICAO_FECHAMENTO" , descricaoEmail);
        parametros.put("SERVER_URL", 'http://fluig.hyline.com.br:8080');
        var link = hAPI.getUserTaskLink(proximaAtividade);
//        var link = 'http://fluigteste.hyline.com.br/portal/p/1/pageworkflowview?'
//        	+'app_ecm_workflowview_detailsProcessInstanceID='
//        	+ numeroDaSolicitacao
        parametros.put("LINK", link);
        parametros.put("subject", subject);
        
        if (destinatarios.size() > 0) {
        	log.info('enviou e-mail');
            notifier.notify("hyline", "TPLPROCESS_FECH_MENSAL_TAR_ATRASADAS", parametros, 
    	    		destinatarios, "text/html");
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

function getFieldColleague(userFunc, fieldSearch, indexfieldReturn){
	var filtraLogin = DatasetFactory.createConstraint(fieldSearch, userFunc, userFunc, 
			ConstraintType.MUST);
	var filtros = new Array(filtraLogin);
	var colaborador = DatasetFactory.getDataset("colleague", null, filtros, null);
    return colaborador.values[0][indexfieldReturn];
}