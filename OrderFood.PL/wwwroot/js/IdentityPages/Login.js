const togglePassword = document.getElementById('togglePassword');
const passwordInput = document.getElementById('passwordInput');
const icon = document.getElementById('togglePasswordIcon');

togglePassword.addEventListener('click', () => {
    const isPassword = passwordInput.type === 'password';
    passwordInput.type = isPassword ? 'text' : 'password';
    icon.classList.toggle('fa-eye');
    icon.classList.toggle('fa-eye-slash');
});