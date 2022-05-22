// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const textToBeCopied = document.querySelector('.textToBeCopied');
const copyButton = document.querySelector('.copyButton');

textToBeCopied.addEventListener('blur', function () {
    copyButton.classList.remove('active');
    copyButton.innerHTML = "Copy";
})

copyButton.addEventListener('click', function () {
    copyButton.classList.add('active');
    textToBeCopied.focus();
    textToBeCopied.select();
    document.execCommand('copy');
    if (this.innerHTML = "Copy") {
        this.innerHTML = "Copied!";
    }
})
