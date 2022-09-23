// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function onMangaHover(itemId) {
    var manga = document.getElementById(itemId);
    var mangaInfoStyle = document.getElementById(itemId + "-mangaInfo").style;
    mangaInfoStyle.display = "block";
    var x = manga.offsetLeft;
    var y = manga.offsetTop;

    if (x < 1300) {
        mangaInfoStyle.left = (manga.offsetLeft + 275) + "px";
    }
    else {
        mangaInfoStyle.left = (manga.offsetLeft - 430) + "px";
    }

    mangaInfoStyle.top = (manga.offsetTop - 16) + "px";
}

function onMangaOut(itemId) {
    document.getElementById(itemId + "-mangaInfo").style.display = "none";

}
function genersLinkClick() {
    var selector = document.getElementById("genersSelector");
    selector.style.display = selector.style.display == "none" ? "block" : "none";
    var arrow = document.getElementById("arrow");
    if (arrow.classList.contains("up")) {
        arrow.classList.replace("up", "down");
    }
    else {
        arrow.classList.replace("down", "up");
    }
}