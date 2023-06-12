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
} else {
	// Try to get the user's browser language
	let userLanguage = navigator.language || navigator.userLanguage;
	
	if (userLanguage) {
		footerLanguageSelector.childNodes.forEach((option) => {
			if (option.value == null) return;
			
			if (option.value.startsWith(userLanguage)) {
				footerLanguageSelector.value = option.value;
			}
		});
	}
	
	// Set the cookie
	document.cookie = `_lang=c=${footerLanguageSelector.value}|uic=${footerLanguageSelector.value};path=/;max-age=31536000;samesite=strict`;
}

if (footerLanguageSelector) {
	footerLanguageSelector.addEventListener("change", onUserLanguageChange);
}