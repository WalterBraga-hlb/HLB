function afterTaskComplete(colleagueId,nextSequenceId,userList){
	var atv      = getValue("WKNumState");
	var nextAtv  = getValue("WKNextState");
	
	if (atv == 10 && nextAtv == 13) {
		// Integra com o Apolo
		var matricula = hAPI.getCardValue("fu_matricula");
		var codCurso = hAPI.getCardValue("fu_codCurso");
		var processo = getValue('WKNumProces');
		var mesAno = hAPI.getCardValue("mes_ano");
		var valorReembolso = hAPI.getCardValue("valor_reembolso");
		var diretor = hAPI.getCardValue("diretor");
		var dataAprovDiretor = hAPI.getCardValue("data_aprov_diretor");
		log.info("Inserindo Recibo de Educação no Apolo Nº " + processo);
		
		var dataSource = "/jdbc/Apolo";
		var ic = new javax.naming.InitialContext();
		var ds = ic.lookup(dataSource);
		var myQuery = "execute USER_Insere_Recibo_Educacao " + matricula + ", " + codCurso + ", "
			+ processo + ", '" + mesAno + "', " + valorReembolso + ", '" + diretor + "', '"
			+ dataAprovDiretor + "'";
		
		log.info("Executando USER_Insere_Recibo_Educacao \n" + myQuery);
		
		try {
			var conn = ds.getConnection();
			var stmt = conn.createStatement();
			stmt.setQueryTimeout(600);
			var rs = stmt.executeUpdate(myQuery);
		} catch (e) {
			log.error("ERRO PROCESSO " + processo + "==============> " + e.message);
			throw "ERRO AO INTEGRAR PROCESSO " + processo + " NO APOLO: " + e.message;
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