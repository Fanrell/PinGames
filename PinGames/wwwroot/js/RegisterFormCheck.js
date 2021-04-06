function FromValidation()
{
    var password = new String(document.getElementById('password').value);
    var repeatedPassword = document.getElementById('repeatedPassword').value;
    var login = new String(document.getElementById('login').value);

    if (password == repeatedPassword && password.length >= 8 && login.length > 0) {
        document.getElementById('submit').disabled = false;
        document.getElementById("errorMessage").textContent = null;
        document.getElementById("errorMessage").style.visibility = "hidden";
    }
    else
    {
        document.getElementById('submit').disabled = true;
        document.getElementById("errorMessage").textContent = "Uncorect form filled"
        document.getElementById("errorMessage").style.visibility = "visible"

    }
}