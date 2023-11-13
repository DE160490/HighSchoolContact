const loginButton = document.querySelector('.button_login');

loginButton.addEventListener('click', (event) => {
    event.preventDefault();
    const accountIdInput = document.getElementById('AccountId');
    const passwordInput = document.getElementById('Password');

    if (accountIdInput.value === '') {
        alert('Please enter your account ID.');
        accountIdInput.focus();
        return;
    }

    if (passwordInput.value === '') {
        alert('Please enter your password.');
        passwordInput.focus();
        return;
    }

    // Submit the form if both inputs are not empty
    loginButton.form.submit();
});
