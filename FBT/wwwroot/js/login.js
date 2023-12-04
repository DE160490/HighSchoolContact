const loginButton = document.querySelector('.button_login');

loginButton.addEventListener('click', (event) => {
    event.preventDefault();
    const accountIdInput = document.getElementById('AccountId');
    const passwordInput = document.getElementById('Password');

    if (accountIdInput.value.trim() === '') {
        alert('Vui lòng nhập Account ID.');
        accountIdInput.focus();
        return;
    }

    if (passwordInput.value.trim() === '') {
        alert('Vui lòng nhập Password.');
        passwordInput.focus();
        return;
    }

    // Submit the form if both inputs are not empty
    loginButton.form.submit();
});
