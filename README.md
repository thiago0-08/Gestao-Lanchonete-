# 🍔 Gestão Lanchonete

Um sistema completo e robusto para gestão de lanchonetes, hamburguerias e restaurantes de pequeno porte. Este projeto _Full Stack_ oferece controle total sobre pedidos, estoque e receitas

 ## 🚀 Funcionalidades

### 📊 Dashboard Interativo
- Visão geral em tempo real do faturamento diário.
- Indicadores de "Ticket Médio" e "Total de Pedidos diario".
- Alertas visuais de estoque baixo ou crítico.
- Gráficos de desempenho (Faturamento Mensal e Produtos Mais Vendidos).

### 🛒 Gestão de Pedidos
- Criação de novos pedidos com cálculo automático de valores.
- Acompanhamento de status (Pendente, Em Preparação, Entregue, Cancelado).
- Visualização detalhada dos itens de cada pedido.
- Filtros por status e data.

### 📦 Controle de Estoque Inteligente
- Registo de ingredientes com unidades de medida personalizadas.
- Definição de estoque mínimo para alertas automáticos.
- Registo de entradas (compras) e saídas (perdas/ajustes).
- Histórico de movimentações.

### 🍕 Gestão de Produtos e Receitas
- Cadastro de produtos com imagem, descrição e preço.
- Receita: Associação de ingredientes a um produto.
-  Ao finalizar um pedido, o sistema abate automaticamente os ingredientes do estoque com base na receita.

### 📈 Relatórios
- Análise de itens em falta.
- Histórico de vendas.

---

## 🛠️ Tecnologias Utilizadas

### Frontend 
- Vue.js 3 


### Backend 
- C#
- PostgreSQL
- Autenticação JWT


---

## 🔧 Como Rodar o Projeto



### 1. Configuração do Banco de Dados
1. Crie um banco de dados no PostgreSQL chamado `ControleGestao` (ou ajuste a ConnectionString).
2. No arquivo `appsettings.json` do Backend, verifique a string de conexão:
  
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=ControleGestao;Username=seu_usuario;Password=sua_senha"
   }
