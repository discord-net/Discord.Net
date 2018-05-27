const baseUrl = document.getElementById("docfx-style:rel").content;

function onThemeSelect(event) {
    const theme = event.target.value;
    window.localStorage.setItem("theme", theme);
    window.themeElement.href = getUrl(theme);
}

function getUrl(slug) {
    return baseUrl + "styles/" + slug + ".css";
}

const themeElement = document.createElement("link");
themeElement.rel = "stylesheet";

const theme = window.localStorage.getItem("theme") || "light";
themeElement.href = getUrl(theme);

document.head.appendChild(themeElement);
window.themeElement = themeElement;

document.addEventListener("DOMContentLoaded", function() {
    const themeSwitcher = document.getElementById("theme-switcher");
    themeSwitcher.onchange = onThemeSelect;
    themeSwitcher.value = theme;
}, false);
