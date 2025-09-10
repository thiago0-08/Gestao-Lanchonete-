<template>
    <div class="container">
        <h1>Controle de Estoque</h1>
        <div class="box dashboard-card">
            
            <button class="btn-entrada" @click="showcriaNovaEntrada = true"> Registrar Entrada </button>
            <button class="btn-saida" @click="showcriaNovaSaida = true"> Registrar Saída</button>
        </div>

        <!-- Modais de controle de estoque -->
        <div v-if="showcriaNovaEntrada">
            <CriaEntrada @close="showcriaNovaEntrada = false" />
        </div>

        <div v-if="showcriaNovaSaida">
            <CriaSaida @close="showcriaNovaSaida = false" />
        </div>

        

        <div class="box2 dashboard-card">
            <table>
                <thead>
                    <tr>
                        <th>Ingrediente</th>
                        <th>Unidade</th>
                        <th>Estoque Atual</th>
                        <th>Estoque Mínimo</th>
                        <th>Status</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>Pão</td>
                        <td>Unidades</td>
                        <td>5</td>
                        <td>20</td>
                        <td><span class="status-badge critico">Crítico</span></td>
                        <td class="acoes">
                            <button class="btn-editar"><i class="fas fa-pencil-alt"></i></button>
                            <button class="btn-excluir"><i class="fas fa-trash-alt"></i></button>
                        </td>
                    </tr>
                    <tr>
                        <td>Queijo</td>
                        <td>kg</td>
                        <td>12</td>
                        <td>5</td>
                        <td><span class="status-badge normal">Normal</span></td>
                        <td class="acoes">
                            <button class="btn-editar"><i class="fas fa-pencil-alt"></i></button>
                            <button class="btn-excluir"><i class="fas fa-trash-alt"></i></button>
                        </td>
                    </tr>


                    <!---- <tr>  v-for="item in estoque" :key="item.id"> 
                        <td> Pão </td> {{ item.ingrediente }}</td> 
                        <td> Unidades </td>
                        <td> 5 </td> {{ item.quantidade }}</td> 
                        <td> 20 </td> {{ item.unidade }}</td> 
                        <td> Critico </td>
                        <td>
                            <button class="btn-editar"><i class="fas fa-pencil-alt"></i></button>
                            <button class="btn-excluir"><i class="fas fa-trash-alt"></i></button>
                            <button @click="editarItem(item.id)"> Editar </button>
                        <button @click="deletarItem(item.id)"> Deletar </button> 
                        </td>
                    </tr> -->



                </tbody>
            </table>
        </div>
    </div>
</template>


<script>


import CriaSaida from '@/components/EstoqueComponents/CriaSaida.vue';
import CriaEntrada from '@/components/EstoqueComponents/CriaEntrada.vue';

export default {
    components: {
        CriaEntrada,
        CriaSaida,
    },

    data() {
        return {
            showcriaNovaSaida: false,
            showcriaNovaEntrada: false,
        };
    },

};

</script>


<style scoped>
.container {
    margin-left: 250px;
    padding: 20px;
    font-family: Arial, sans-serif;
}

h1 {
    color: #333;
    margin-bottom: 20px;
}

/* Base para Cards */
.dashboard-card {
    background: #ffffff;
    border: 1px solid #e0e0e0;
    border-radius: 8px;
    padding: 20px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    margin-top: 20px;
}

.box {
    display: flex;
    gap: 10px;
    margin-bottom: 20px;
}

/* --- Tabela --- */
.box2 {
    padding: 0;
}

table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 15px;
}

th,
td {
    border: 1px solid #e0e0e0;
    padding: 12px;
    text-align: left;
}

th {
    background-color: #f2f2f2;
    font-weight: bold;
    color: #555;
}

tbody tr:nth-child(even) {
    background-color: #f9f9f9;
}

tbody tr:hover {
    background-color: #f1f1f1;
}

/* --- Estilo dos Botões --- */
button {
    padding: 8px 15px;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    transition: all 0.3s ease;
    font-weight: 500;
}



.btn-entrada {
    background-color: #28a745;
}

.btn-entrada:hover {
    background-color: #218838;
}

.btn-saida {
    background-color: #dc3545;
}

.btn-saida:hover {
    background-color: #c82333;
}

/* --- Botões de Ação na Tabela --- */
.acoes {
    text-align: center;
    white-space: nowrap;
}

.acoes button {
    padding: 6px 10px;
    margin: 0 4px;
    border-radius: 50%;
    font-size: 1rem;
}

.btn-editar {
    background-color: #6c757d;
}

.btn-editar:hover {
    background-color: #5a6268;
}

.btn-excluir {
    background-color: #dc3545;
}

.btn-excluir:hover {
    background-color: #c82333;
}

/* --- Status Badges --- */
.status-badge {
    padding: 4px 8px;
    border-radius: 12px;
    color: #fff;
    font-size: 0.85em;
    font-weight: bold;
}

.status-badge.critico {
    background-color: #dc3545;
}

.status-badge.normal {
    background-color: #28a745;
}
</style>
