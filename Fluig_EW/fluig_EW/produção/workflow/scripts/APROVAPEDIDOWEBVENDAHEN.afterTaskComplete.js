function afterTaskComplete(colleagueId,nextSequenceId,userList){
	var atv      = getValue("WKNumState");
	var nextAtv  = getValue("WKNextState");
	var processo = getValue('WKNumProces');
	var comentario = getValue("WKUserComment");
	
	if (atv == 5) {
		var status = 'Aprovado';
		if (atv == 5 && nextAtv == 11) {
			status = 'Reprovado';
		}
		var idPedido = hAPI.getCardValue('id_web');
		var msg = 'Pedido com a exceção ' + hAPI.getCardValue('tipo_excecao').replace("'","");
			+ ' ' + status + '.';
		var aprovador = hAPI.getCardValue('decisor');
		
		log.info("Atualizando Status do Pedido de Venda Web ID " + idPedido + " Nº Fluig " + processo);
		
		var dataSource = "/jdbc/HLBAPPDSRO";
		var ic = new javax.naming.InitialContext();
		var ds = ic.lookup(dataSource);
		var myQuery = "execute Atualiza_Status_Pedido_Venda_Web " + idPedido + ", '" + status + "', "
			+ "'" + msg + "', '" + aprovador + "', '" + processo + "', '" + comentario + "' ";
		
		log.info("Executando Query... \n" + myQuery);
		
		try {
			var conn = ds.getConnection();
			var stmt = conn.createStatement();
			stmt.setQueryTimeout(600);
			var rs = stmt.executeUpdate(myQuery);
		} catch (e) {
			log.error("ERRO na atualização do Status do Pedido de Venda Web ID " + idPedido + " Nº Fluig " + processo
					+ ": " + e.message);
			// Envia o e-mail
			var descricaoEmail = 'Erro ao integrar ' + processo 
				+' Aprovação de Pedido Web no Fluig: '
				+ e.message;
			
			var destinatarios = new java.util.ArrayList();
			destinatarios.add('hyline');
			
	        var parametros = new java.util.HashMap();
	        parametros.put("RESPONSAVEL", 'Depto. TI');
	        parametros.put("DESCRICAO_TAREFAS_ATRASADAS" , '');
	        parametros.put("DESCRICAO_FECHAMENTO" , descricaoEmail);
	        parametros.put("SERVER_URL", 'http://fluig.hyline.com.br:8080');
	        var link = 'http://fluig.hyline.com.br:8080/portal/p/1/pageworkflowview?'
	        	+'app_ecm_workflowview_detailsProcessInstanceID='
	        	+ processo
	        parametros.put("LINK", link);
	        parametros.put("subject", '[ERRO] ERRO afterTaskComplete APROVA PEDIDO WEB');
	        
	        if (destinatarios.size() > 0) {
	        	log.info('enviou e-mail');
	            notifier.notify("hyline", "TPLPROCESS_FECH_MENSAL_TAR_ATRASADAS", parametros, 
	    	    		destinatarios, "text/html");
	        }
		} finally {
			if (stmt != null) {
				stmt.close();
			}
			if (conn != null) {
				conn.close();
			}
		}
	}
}