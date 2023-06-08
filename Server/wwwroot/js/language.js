/**
 * @param {Event} e
 * @returns {void}
 */
function onUserLanguageChange(e) {
	document.cookie = `_lang=c=${e.target.value}|uic=${e.target.value};path=/;max-age=31536000;samesite=strict`;
	location.reload();
}

let footerLanguageSelector = document.getElementById("footer-language-selector");

// Get the `_lang` cookie value
let cookie = document.cookie.split(";").find((c) => c.trim().startsWith("_lang="));

if (cookie) {
	let cookieValue = cookie.substring("_lang=".length, cookie.length);
	footerLanguageSelector.value = cookieValue.split("|")[0].split("=")[1];
}

if (footerLanguageSelector) {
	footerLanguageSelector.addEventListener("change", onUserLanguageChange);
}