<template>
    <div class="login-page">
        <div class="login-card">
            <h1 class="login-title">Gestão Lanchonete</h1>
            <p class="login-subtitle">Acesse sua conta</p>
            <div class="form-group">
                <label for="email" class="form-label">E-mail</label>
                <input type="text" id="email" class="form-control" placeholder="nome">
            </div>
            <div class="form-group">
                <label for="password" class="form-label">Senha</label>
                <input type="password" id="password" class="form-control" placeholder="********">
            </div>
            <p v-if="error" class="error-message">E-mail ou senha incorretos.</p>
            <RouterLink class="nav-link" to="/dashboard" active-class="active">
                <button class="btn-login">Entrar</button>
            </RouterLink>
        </div>
    </div>
</template>

<script>
export default {
    data() {
        return {
            error: false,
        };
    },
    methods: {
        // Você pode adicionar a sua lógica de autenticação aqui.
        // Por exemplo:
        // login() {
        //     // Lógica para chamar a API de login
        //     // Se o login falhar:
        //     // this.error = true;
        // }
    },
};
</script>

<style scoped>
.login-page {
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 100vh;
    background-color: #f7f7f7;
    padding: 20px;
}

.login-card {
    background: #ffffff;
    border: 1px solid #e0e0e0;
    border-radius: 8px;
    padding: 40px;
    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
    width: 100%;
    max-width: 400px;
    text-align: center;
}

.login-title {
    font-size: 2.2em;
    font-weight: bold;
    color: #ff7200;
    margin-bottom: 5px;
}

.login-subtitle {
    color: #555;
    margin-bottom: 25px;
}

.form-group {
    margin-bottom: 20px;
    text-align: left;
}

.form-label {
    font-weight: bold;
    color: #555;
    display: block;
    margin-bottom: 5px;
}

.form-control {
    width: 100%;
    padding: 10px;
    border: 1px solid #ccc;
    border-radius: 4px;
    font-size: 1em;
}

.form-control:focus {
    border-color: #ff7200;
    box-shadow: 0 0 5px rgba(255, 114, 0, 0.5);
    outline: none;
}

.btn-login {
    width: 100%;
    padding: 12px;
    background-color: #ff7200;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 1.1em;
    font-weight: bold;
    transition: background-color 0.3s ease;
}

.btn-login:hover {
    background-color: #e65c00;
}

.error-message {
    color: #dc3545;
    font-size: 0.9em;
    margin-bottom: 15px;
}
</style>
