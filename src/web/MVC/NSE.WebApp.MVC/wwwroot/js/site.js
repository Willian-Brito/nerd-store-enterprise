function CreditCard() {
    
    if ($("#card_number").length > 0) {
        $("#card_number").validateCreditCard(function (e) {

            $(this).removeClass();
            $(this).addClass("form-control");

            if (e.card_type !== null)
                $(this).addClass(e.card_type.name);

            if (e.valid)
                $(this).addClass("valid");
            else
                $(this).removeClass("valid");

            if (e.length_valid && !e.valid) {
                $(this).addClass("invalid");
            } else {
                $(this).removeClass("invalid");
            }
        });
    }
}

function BuscaCep() {
    $(document).ready(function () {

        function limpa_formulario_cep() {
            // Limpa valores do formulário de cep.
            $("#Address_StreetAddress").val("");
            $("#Address_Neighborhood").val("");
            $("#Address_City").val("");
            $("#Address_State").val("");
        }

        //Quando o campo cep perde o foco.
        $("#Address_ZipCode").blur(function () {

            //Nova variável "cep" somente com dígitos.
            var cep = $(this).val().replace(/\D/g, '');

            //Verifica se campo cep possui valor informado.
            if (cep != "") {

                //Expressão regular para validar o CEP.
                var validacep = /^[0-9]{8}$/;

                //Valida o formato do CEP.
                if (validacep.test(cep)) {

                    //Preenche os campos com "..." enquanto consulta webservice.
                    $("#Address_StreetAddress").val("...");
                    $("#Address_Neighborhood").val("...");
                    $("#Address_City").val("...");
                    $("#Address_State").val("...");

                    //Consulta o webservice viacep.com.br/
                    $.getJSON("https://viacep.com.br/ws/" + cep + "/json/?callback=?",
                        function (dados) {

                            if (!("erro" in dados)) {
                                //Atualiza os campos com os valores da consulta.
                                $("#Address_StreetAddress").val(dados.logradouro);
                                $("#Address_Neighborhood").val(dados.bairro);
                                $("#Address_City").val(dados.localidade);
                                $("#Address_State").val(dados.uf);
                            } //end if.
                            else {
                                //CEP pesquisado não foi encontrado.
                                limpa_formulario_cep();
                                alert("CEP não encontrado.");
                            }
                        });
                } //end if.
                else {
                    //cep é inválido.
                    limpa_formulario_cep();
                    alert("Formato de CEP inválido.");
                }
            } //end if.
            else {
                //cep sem valor, limpa formulário.
                limpa_formulario_cep();
            }
        });
    });
}