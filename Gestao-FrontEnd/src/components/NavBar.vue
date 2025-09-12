<template>
    <aside class="sidebar" role="navigation" aria-label="Menu Lateral">
        <div class="sidebar-header">
            <h1 class="logo">
                <i class="fas fa-utensils"></i>
                <span>Gestão Lanchonete</span>
            </h1>
        </div>

        <nav class="sidebar-nav">
            <ul>
                <RouterLink class="nav-link" to="/dashboard" active-class="active">
                    <li>
                        <i class="fas fa-tachometer-alt"></i>
                        <span>Dashboard</span>
                    </li>
                </RouterLink>

                <RouterLink class="nav-link" to="/pedidos" active-class="active">
                    <li>
                        <i class="fas fa-shopping-basket"></i>
                        <span>Pedidos</span>
                    </li>
                </RouterLink>

                <RouterLink class="nav-link" to="/receitas" active-class="active">
                    <li>
                        <i class="fas fa-pizza-slice"></i>
                        <span>Receitas</span>
                    </li>
                </RouterLink>

                <RouterLink class="nav-link" to="/estoque" active-class="active">
                    <li>
                        <i class="fas fa-boxes"></i>
                        <span>Estoque</span>
                    </li>
                </RouterLink>

                <RouterLink class="nav-link" to="/relatorio" active-class="active">
                    <li>
                        <i class="fas fa-chart-bar"></i>
                        <span>Relatório</span>
                    </li>
                </RouterLink>
                 <RouterLink class="nav-link" to="/" active-class="active">
                    <li>
                        <i class="fa-solid fa-right-from-bracket"></i>
                        <span>Sair</span>
                    </li>
                </RouterLink>
            </ul>
        </nav>
        
        <div class="sidebar-user">
            <div class="user-info">
                <div class="user-avatar">
                    <img src="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRhl_zjFSX6iLpLMk8U65vEA_jY6gzFhC9FbA&s"
                        alt="Avatar do Usuário" />
                </div>
                <span class="user-name">Usuário</span>
            </div>
        </div>
    </aside>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Barlow:wght@400;500;600;700;800;900&display=swap');


/* Estilos globais para reset e fonte */
* {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
    font-family: 'Barlow', sans-serif;
}


/* Estilo do menu lateral */
.sidebar {
    position: fixed;
    top: 0;
    left: 0;
    width: 250px;
    height: 100vh;
    background-color: #fff;
    color: var(--text-color);
    display: flex;
    flex-direction: column;
    padding: 1.5rem 1rem;
    border-right: 1px solid #000000;
    box-shadow: 2px 0 5px ;
    z-index: 1000;
    transition: width 0.3s ease;
}

/* Estilo do cabeçalho (logo) */
.sidebar-header {
    padding-bottom: 1.5rem;
    border-bottom: 1px solid #000000;
    margin-bottom: 1.5rem;
    display: flex;
    align-items: center;
    justify-content: center;
}

.logo {
    display: flex;
    align-items: center;
    font-size: 1.5rem;
    font-weight: 700;
    color: #ff7200;
}

.logo i {
    margin-right: 0.5rem;
    font-size: 1.8rem;
}

/* Estilo dos links de navegação */
.sidebar-nav ul {
    list-style: none;
    padding: 0;
}

.nav-link {
    text-decoration: none;
    color: var(--text-color);
}

.nav-link li {
    display: flex;
    align-items: center;
    padding: 0.75rem 1rem;
    margin: 0.5rem 0;
    border-radius: 8px;
    cursor: pointer;
    transition: background-color 0.2s ease, transform 0.2s ease;
}

.nav-link li i {
    font-size: 1.25rem;
    width: 30px;
    /* Garante que os ícones fiquem alinhados */
    text-align: center;
}

.nav-link li span {
    font-size: 1rem;
    font-weight: 500;
    margin-left: 1rem;
}

.nav-link li:hover {
    background-color: rgb(255, 114, 0);
}

.nav-link.active li {
    background-color: var(--active-bg-color);
    color: #000000;
    transform: translateX(5px);
    box-shadow: 0 4px 6px  rgb(255, 114, 0);
}

.nav-link.active li i,
.nav-link.active li span {
    color: #000000;
}

/* Estilo da área do usuário */
.sidebar-user {
    margin-top: auto;
    padding-top: 1rem;
    border-top: 1px solid #000000;
}

.user-info {
    display: flex;
    align-items: center;
    padding: 0.75rem 1rem;
    cursor: pointer;
}

.user-avatar img {
    width: 40px;
    height: 40px;
    border-radius: 50%;
    object-fit: cover;
    border: 2px solid #fff;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    transition: transform 0.2s ease;
}

.user-info:hover .user-avatar img {
    transform: scale(1.05);
}

.user-name {
    font-weight: 600;
    font-size: 1rem;
    margin-left: 1rem;
}
</style>