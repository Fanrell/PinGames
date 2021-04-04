function passwordCheckSymbols()
{
    var password = document.getElementById('password').value;
    var repeatedPassword = document.getElementById('repeatedPassword').value;
    if (password != repeatedPassword) {
        document.getElementById('submit').disabled = true;
        document.getElementById("errorMessage").textContent = "Diffrent passwords"
        document.getElementById("errorMessage").style.visibility = "visible"

    }
    else
    {
        document.getElementById('submit').disabled = false;
        document.getElementById("errorMessage").textContent = null;
        document.getElementById("errorMessage").style.visibility = "hidden";
    }
}
