// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function displayBusyIndicator() {
    document.getElementById("loading").style.display = "block";
    document.getElementById("loadingtext").style.display = "block";
}
function hideBusyIndicator() {
    document.getElementById("loading").style.display = "none";
    document.getElementById("loadingtext").style.display = "none";
}
