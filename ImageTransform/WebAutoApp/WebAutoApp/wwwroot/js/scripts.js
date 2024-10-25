
window.ChangeMode = function () {
    const body = document.body;
    const modeText = document.querySelector(".mode-text");

    const isDarkMode = body.classList.toggle("dark");

    return isDarkMode;
}
