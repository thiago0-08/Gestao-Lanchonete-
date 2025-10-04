import { defineStore } from 'pinia';
import axios from 'axios';


const API_URL = 'http://localhost:5138/api/Ingrediente/entrada';

export const useEntradaStore = defineStore('entrada', () => {

    const safeParseInt = (value) => {
        const parsed = parseInt(value);
        return isNaN(parsed) ? null : parsed;
    };
    
    const safeParseFloat = (value) => {
        const parsed = parseFloat(value);
        return isNaN(parsed) ? null : parsed; 
    };

    /**
     * Registra uma nova entrada de ingrediente.
     * @param {object} payload - Dados da entrada (IdIngrediente, quantidade, custo, etc.)
     */
    async function addEntrada(inputPayload) {
        try {
            const finalPayload = {
                IdIngrediente: safeParseInt(inputPayload.IdIngrediente), 
                QuantidadeEntrada: safeParseFloat(inputPayload.quantidade), 
                CustoUnitario: safeParseFloat(inputPayload.custoUnitario),
                DataEntrada: new Date().toISOString(), 
                DataValidade: inputPayload.dataValidade ? new Date(inputPayload.dataValidade).toISOString() : null,
            };

            
            console.log('Enviando Payload Entrada:', finalPayload);
            const response = await axios.post(API_URL, finalPayload);
            console.log('Entrada registrada com sucesso!', response.data);
            return response.data;

        } catch (error) {
            let errorMessage = "Erro desconhecido ao registrar entrada.";
            
            if (error.response && error.response.data) {
                console.error("--- RESPOSTA DE ERRO COMPLETA DA API ---", error.response.data);
            }
            
            if (error.response && error.response.data && error.response.data.errors) {
                const validationErrors = error.response.data.errors;
                
                errorMessage = Object.keys(validationErrors)
                    .map(key => {
                        return `${key}: ${validationErrors[key].join(', ')}`;
                    })
                    .join('; ');
                console.error("Erros de Validação da API:", errorMessage);
            } else if (error.message) {
                errorMessage = error.message;
            }
            
            console.error("Erro ao adicionar entrada:", error);
            throw new Error(`Falha na requisição: ${errorMessage}`);
        }
    }

    return {
        addEntrada
    };
});
