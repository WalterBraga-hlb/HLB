function validateForm(form) {
	var activity = getValue('WKNumState');
	
	if ((form.getValue("fu_filial") == null || form.getValue("fu_filial") == "")
			&& (getValue('WKNumProces') == null || (getValue('WKNumProces') > 0 && getValue('WKCompletTask') == 'true'))) {
		throw "Empresa / Filial n\u00E3o pode ser vazio.";
	}
	if ((form.getValue("fu_depto") == null || form.getValue("fu_depto") == "")
			&& (getValue('WKNumProces') == null || (getValue('WKNumProces') > 0 && getValue('WKCompletTask') == 'true'))) {
		throw "Departamento n\u00E3o pode ser vazio.";
	}
	if ((form.getValue("fu_nome") == null || form.getValue("fu_nome") == "")
			&& (getValue('WKNumProces') == null || (getValue('WKNumProces') > 0 && getValue('WKCompletTask') == 'true'))) {
		throw "Funcion\u00E1rio n\u00E3o pode ser vazio.";
	}
	if ((form.getValue("fu_curso") == null || form.getValue("fu_curso") == "")
			&& (getValue('WKNumProces') == null || (getValue('WKNumProces') > 0 && getValue('WKCompletTask') == 'true'))) {
		throw "Curso n\u00E3o pode ser vazio.";
	}
	if (((form.getValue("mes_ano") == null || form.getValue("mes_ano") == ""))
			&& (getValue('WKNumProces') == null || (getValue('WKNumProces') > 0 && getValue('WKCompletTask') == 'true'))) {
		throw "Mês / Ano n\u00E3o pode ser vazio. - " + form.getValue("mes_ano");
	}
	
	// Carrega dados de recibo de educação existente caso haja
	var mesAno = form.getValue("mes_ano");
	var matricula = form.getValue("fu_matricula");
	var curso = form.getValue("fu_codCurso");
	var fMesAno = DatasetFactory.createConstraint("mes_ano", mesAno, mesAno, ConstraintType.MUST);
	var fActive = DatasetFactory.createConstraint("metadata#active", "true", "true", ConstraintType.MUST);
	var fMatricula = DatasetFactory.createConstraint("fu_matricula", matricula, matricula, ConstraintType.MUST);
	var fCurso = DatasetFactory.createConstraint("fu_codCurso", curso, curso, ConstraintType.MUST);
	var filtros = new Array(fMesAno, fActive, fMatricula, fCurso);
	var meuDS = DatasetFactory.getDataset("ReembolsodeCursos215", null, filtros, null);
	
	var existeLancado = false;
	for (var i = 0; i < meuDS.values.length; i++) {
		// Localiza o número da solicitação
		var documentid = meuDS.getValue(i, 'documentid');
		var filtraNumDoc = DatasetFactory.createConstraint("documentId", documentid, documentid, ConstraintType.MUST);
		var filtroProcessAttach = new Array(filtraNumDoc);
		var meuDSProcessAttach = DatasetFactory.getDataset("processAttachment", null, filtroProcessAttach, null);
		if (meuDSProcessAttach.values.length > 0) {
			var numSolicitacao = meuDSProcessAttach.getValue(0, "processAttachmentPK.processInstanceId");			
			
			// Verifica se o status da solicitação
			var filtraNumeroProcesso = DatasetFactory.createConstraint(
				"workflowProcessPK.processInstanceId", numSolicitacao, numSolicitacao, ConstraintType.MUST);
			var filtroProcess = new Array(filtraNumeroProcesso);
			var meuDSProcess = DatasetFactory.getDataset("workflowProcess", null, filtroProcess, null);
			
			if (meuDSProcess.values.length > 0) {
				if (meuDSProcess.getValue(0, "status") != "1"){
					existeLancado = true;
					break;
				}
			}
		}
	}
	
	if ((existeLancado)
			&& (getValue('WKNumProces') == null || (getValue('WKNumProces') > 0 && getValue('WKCompletTask') == 'true'))) {
		throw "Já existe recibo para o curso " + form.getValue("fu_curso") + " no mês/ano " + mesAno + "!";
	}
	
	if ((form.getValue("valor_mensalidade") == null || form.getValue("valor_mensalidade") == "")
			&& (getValue('WKNumProces') == null || (getValue('WKNumProces') > 0 && getValue('WKCompletTask') == 'true'))) {
		throw "Valor da Mensalidade n\u00E3o pode ser vazio.";
	}
	
	if ((form.getValue("fu_liderMatric") == null || form.getValue("fu_liderMatric") == "")
			&& (getValue('WKNumProces') == null || (getValue('WKNumProces') > 0 && getValue('WKCompletTask') == 'true'))) {
		throw form.getValue("fu_lider");
	}
}