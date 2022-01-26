export function disableStyleSheet() {
    var elements = document.querySelectorAll('link[rel=stylesheet][title=temp]');
    for (var i = 0; i < elements.length; i++) {
        elements[i].parentNode.removeChild(elements[i]);
    }
}
