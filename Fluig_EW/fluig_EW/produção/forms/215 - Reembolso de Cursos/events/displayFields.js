function displayFields(form,customHTML){ 
	var activity = getValue('WKNumState');
	var estado = form.getFormMode();
	var userCode = getValue("WKUser");
	var company = getValue("WKCompany");
	var numProcesso = getValue('WKNumProces');
	
	customHTML.append("<script>"
			+ "var atividade = '" + activity + "'; "
			+ "var userCode = '" + userCode + "'; "
			+ "var estado = '" + estado + "'; "
			+ "var numProcesso = " + numProcesso + "; "
			+"</script>")
	
	if (activity != 13) {
		customHTML.append('<script>');
		customHTML
				.append('$(\'*[name="diretor"]\').css(\'display\', \'none\');var closers = $(\'*[name="diretor"]\').closest(\'.form-field\').find(\'input, textarea, select\');var hideDiv = true;$.each(closers, function(i, close) {if (close.style.display != \'none\' && close.type != \'hidden\') {hideDiv = false;}});if (hideDiv == true) {$(\'*[name="diretor"]\').closest(\'.form-field\').css(\'display\', \'none\');}');
		customHTML.append('</script>');
		customHTML.append('<script>');
		customHTML.append('$(\'*[name="diretor"]\').closest("li").hide()');
		customHTML.append('</script>');
		
		customHTML.append('<script>');
		customHTML
				.append('$(\'*[name="data_aprov_diretor"]\').css(\'display\', \'none\');var closers = $(\'*[name="data_aprov_diretor"]\').closest(\'.form-field\').find(\'input, textarea, select\');var hideDiv = true;$.each(closers, function(i, close) {if (close.style.display != \'none\' && close.type != \'hidden\') {hideDiv = false;}});if (hideDiv == true) {$(\'*[name="data_aprov_diretor"]\').closest(\'.form-field\').css(\'display\', \'none\');}');
		customHTML.append('</script>');
		customHTML.append('<script>');
		customHTML.append('$(\'*[name="data_aprov_diretor"]\').closest("li").hide()');
		customHTML.append('</script>');
		
		customHTML.append('<script>');
		customHTML
				.append('$(\'*[name="data_pagamento"]\').css(\'display\', \'none\');var closers = $(\'*[name="data_pagamento"]\').closest(\'.form-field\').find(\'input, textarea, select\');var hideDiv = true;$.each(closers, function(i, close) {if (close.style.display != \'none\' && close.type != \'hidden\') {hideDiv = false;}});if (hideDiv == true) {$(\'*[name="data_pagamento"]\').closest(\'.form-field\').css(\'display\', \'none\');}');
		customHTML.append('</script>');
		customHTML.append('<script>');
		customHTML.append('$(\'*[name="data_pagamento"]\').closest("li").hide()');
		customHTML.append('</script>');
	} else {
		customHTML.append('<script>');
		customHTML
				.append('$(\'*[name="valor_mensalidade"]\').css(\'display\', \'none\');var closers = $(\'*[name="valor_mensalidade"]\').closest(\'.form-field\').find(\'input, textarea, select\');var hideDiv = true;$.each(closers, function(i, close) {if (close.style.display != \'none\' && close.type != \'hidden\') {hideDiv = false;}});if (hideDiv == true) {$(\'*[name="valor_mensalidade"]\').closest(\'.form-field\').css(\'display\', \'none\');}');
		customHTML.append('</script>');
		customHTML.append('<script>');
		customHTML.append('$(\'*[name="valor_mensalidade"]\').closest("li").hide()');
		customHTML.append('</script>');
	}
	
	if (estado == 'ADD') {
		customHTML.append('<script>');
		customHTML
				.append('$(\'*[name="mes_ano"]\').css(\'display\', \'none\');var closers = $(\'*[name="mes_ano"]\').closest(\'.form-field\').find(\'input, textarea, select\');var hideDiv = true;$.each(closers, function(i, close) {if (close.style.display != \'none\' && close.type != \'hidden\') {hideDiv = false;}});if (hideDiv == true) {$(\'*[name="mes_ano"]\').closest(\'.form-field\').css(\'display\', \'none\');}');
		customHTML.append('</script>');
		customHTML.append('<script>');
		customHTML.append('$(\'*[name="mes_ano"]\').closest("li").hide()');
		customHTML.append('</script>');
	} else {
		customHTML.append('<script>');
		customHTML
				.append('$(\'*[name="mes_ano_2"]\').css(\'display\', \'none\');var closers = $(\'*[name="mes_ano_2"]\').closest(\'.form-field\').find(\'input, textarea, select\');var hideDiv = true;$.each(closers, function(i, close) {if (close.style.display != \'none\' && close.type != \'hidden\') {hideDiv = false;}});if (hideDiv == true) {$(\'*[name="mes_ano_2"]\').closest(\'.form-field\').css(\'display\', \'none\');}');
		customHTML.append('</script>');
		customHTML.append('<script>');
		customHTML.append('$(\'*[name="mes_ano_2"]\').closest("li").hide()');
		customHTML.append('</script>');
	}
	
	if (estado == 'NONE' || estado == 'ADD') {
		var filtraUserCode = DatasetFactory.createConstraint("USER_CODE", userCode, userCode, ConstraintType.MUST);
		var filtraDataKey = DatasetFactory.createConstraint("DATA_KEY", 'MatriculaMIX', 'MatriculaMIX', ConstraintType.MUST);
	  	var filtros = new Array(filtraUserCode, filtraDataKey);
		var dsUserData = DatasetFactory.getDataset("ds_Custom_User_Data", null, filtros, null);
		
		log.info('RHRECEDU - entrou 1');
		
		if (dsUserData.values.length > 0) {
			log.info('RHRECEDU - entrou 2');
			
			var matriculaMIX = dsUserData.getValue(0, "DATA_VALUE");
			var existeUsuarioGrupo = verificaGrupoUsuario(company, userCode, 'SOLUSUMESMAFILIAL');
			
			var filtraMatricula = DatasetFactory.createConstraint("FU_MATRICULA", matriculaMIX, matriculaMIX, 
					ConstraintType.MUST);
			var filtrosFuncionarios = new Array(filtraMatricula);
			var dsColleagueDataMIX = DatasetFactory.getDataset("ds_FuncionariosChefiaMIX", null, filtrosFuncionarios, null);
			
			log.info('RHRECEDU - entrou 3');

			var filtraDataKeyFiliais = DatasetFactory.createConstraint("DATA_KEY", 'AcessoFilialRH%', 'AcessoFilialRH%', 
					ConstraintType.MUST);
		  	var filtrosUDFiliais = new Array(filtraUserCode, filtraDataKeyFiliais);
			var dsUserDataFiliais = DatasetFactory.getDataset("ds_Custom_User_Data", null, filtrosUDFiliais, null);
			
			if (dsUserDataFiliais.values.length == 0) {
				form.setValue('fu_filial', dsColleagueDataMIX.getValue(0, 'FU_ES_COD_FILIAL'));
				form.setValue('fu_CodigoFilial', dsColleagueDataMIX.getValue(0, 'FU_ES_COD_FILIAL'));				
			}

			if (!existeUsuarioGrupo) {
				
				log.info('RHRECEDU - entrou 4');
				
				form.setValue('fu_depto', dsColleagueDataMIX.getValue(0, 'ES_NOME'));
				form.setValue('fu_nome', dsColleagueDataMIX.getValue(0, 'PE_NOME'));
				form.setValue('fu_matricula', dsColleagueDataMIX.getValue(0, 'FU_MATRICULA'));
				form.setValue('fu_cargoConf', dsColleagueDataMIX.getValue(0, 'CARGO_CONFIANCA'));
				form.setValue('fu_cargo', dsColleagueDataMIX.getValue(0, 'CAR_NOME'));
				form.setValue('fu_lider', dsColleagueDataMIX.getValue(0, 'PE_EMAIL'));
				form.setValue('fu_cpf', dsColleagueDataMIX.getValue(0, 'PE_CPF'));
				var matriculaLider = getIDUsuariobyMail(dsColleagueDataMIX.getValue(0, 'PE_EMAIL'));
				if (matriculaLider == "") {
					form.setValue('fu_lider', "LÍDER " + dsColleagueDataMIX.getValue(0, 'PE_EMAIL') 
							+ " NÃO CADASTRADO NO FLUIG!");
				} else {
					form.setValue('fu_liderMatric', matriculaLider);
				}
			}
		}
	}
}

function verificaGrupoUsuario(company, userCode, grupo) {
	var existe = false;
	var ds;
	
	try { 
		var c1 = DatasetFactory.createConstraint("colleagueGroupPK.companyId", company, company, ConstraintType.MUST); 
		var c2 = DatasetFactory.createConstraint("colleagueGroupPK.colleagueId", userCode, userCode, ConstraintType.MUST); 
		var c3 = DatasetFactory.createConstraint("colleagueGroupPK.groupId", grupo, grupo, ConstraintType.MUST);
		var constraints = new Array(c1, c2, c3);
		
		ds = DatasetFactory.getDataset("colleagueGroup", null, constraints, null); 
	} catch (e) {
		log.error("Erro ao tentar recuperar grupo do usuário: " + e.message); 
	}
	
	if (ds != null && ds.rowsCount > 0) {
		existe = true;
	}
	
	return existe;
}

function getIDUsuariobyMail (userFunc){
	var filtro = DatasetFactory.createConstraint(
			"mail", userFunc, userFunc, ConstraintType.MUST);

	var filtros = new Array(filtro);
	var meuDS = DatasetFactory.getDataset("colleague",
			null, filtros, null);
    
	if (meuDS.values.length > 0){
		return meuDS.getValue(0, "colleaguePK.colleagueId");
	} else {
		return "";
	}
}