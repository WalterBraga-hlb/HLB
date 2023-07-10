function enableFields(form){
	var estado = form.getFormMode();
	var activity = getValue('WKNumState');
	
	if (form.getValue('fu_filial') != '') {
		form.setEnabled("fu_filial",false);
	}
	if (form.getValue('fu_depto') != '') {
		form.setEnabled("fu_depto",false);
	}
	if (form.getValue('fu_nome') != '') {
		form.setEnabled("fu_nome",false);
	}
	
	//if (estado != 'NONE' && estado != 'ADD') {
	if (activity > 4) {
		form.setEnabled("fu_filial",false);
		form.setEnabled("fu_depto",false);
		form.setEnabled("fu_nome",false);
		form.setEnabled("fu_matricula",false);
		form.setEnabled("fu_cpf",false);
		form.setEnabled("fu_cargo",false);
		form.setEnabled("fu_lider",false);
		form.setEnabled("fu_curso",false);
		form.setEnabled("mes_ano",false);
		form.setEnabled("valor_mensalidade",false);
		//form.setEnabled("valor_reembolso",false);
	}
//	if (activity != 13) {
//		form.setVisible("diretor",false);
//		form.setVisible("data_aprov_diretor",false);
//	} else {
//		form.setVisible("diretor",true);
//		form.setVisible("data_aprov_diretor",true);
//	}
}