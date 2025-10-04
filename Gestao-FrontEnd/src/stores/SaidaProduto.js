import { defineStore } from 'pinia';
import axios from 'axios';

const API_URL = 'http://localhost:5138/api/Ingrediente/saida';

export const useSaidaStore = defineStore('saida', () => {

    async function addSaida(inputPayload) {
        try {
            // Objeto que representa o DTO esperado pela API
            const saidaRequest = {
                idIngrediente: inputPayload.idIngrediente,
                quantidadeSaida: inputPayload.quantidade,
            };

            console.log('Enviando Payload Corrigido:', saidaRequest);

            // **CORREÇÃO:** Enviamos o objeto 'saidaRequest' diretamente.
            const response = await axios.post(API_URL, saidaRequest);

            console.log('Saída registrada com sucesso!', response.data);
            return response.data;

        } catch (error) {
            let errorMessage = "Erro desconhecido ao registrar saída.";

            if (error.response?.data) {
                console.error("--- RESPOSTA DE ERRO COMPLETA DA API ---", error.response.data);
            }

            if (error.response?.data?.errors) {
                const validationErrors = error.response.data.errors;

                errorMessage = Object.keys(validationErrors)
                    .map(key => {
                        const friendlyKey = key.replace(/([A-Z])/g, ' $1').trim();
                        return `${friendlyKey}: ${validationErrors[key].join(', ')}`;
                    })
                    .join('; ');
                console.error("Erros de Validação da API:", errorMessage);
            } else if (error.message) {
                errorMessage = error.message;
            }

            console.error("Erro ao adicionar saída:", error);
            throw new Error(`Falha na requisição: ${errorMessage}`);
        }
    }

    return {
        addSaida
    };
});