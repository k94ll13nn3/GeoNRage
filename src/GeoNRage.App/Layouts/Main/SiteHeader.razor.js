export function enableStyleSheet(e) {
    var elements = document.querySelectorAll('link[rel=stylesheet][title=main]');
    for (var i = 0; i < elements.length; i++) {
        elements[i].href = e;
    }
}
