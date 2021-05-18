var elements = document.getElementsByClassName("column");
var i;

function one()
{
    for (i = 0; i < elements.length; i++)
    {
        elements[i].style.msFlex = "100%";
        elements[i].style.Flex = "100%";
    }
}

function two() {
    for (i = 0; i < elements.length; i++) {
        elements[i].style.msFlex = "50%";
        elements[i].style.Flex = "50%";
    }
}

function four() {
    for (i = 0; i < elements.length; i++) {
        elements[i].style.msFlex = "25%";
        elements[i].style.Flex = "25%";
    }
}

var header = document.getElementById("header");
var btns = header.getElementsByClassName("btn");
for (var i = 0; btns.length; i++)
{
    btns[i].addEventListener("click", function () {
        var current = document.getElementById("btn-dark");
        current[0].className = current[0].className.replace(" btn-dark", "");
        this.className += " btn-dark";
    });
}