export function disableStyleSheet() {
    var elements = document.querySelectorAll('link[rel=stylesheet][title=temp]');
    for (var i = 0; i < elements.length; i++) {
        elements[i].parentNode.removeChild(elements[i]);
    }
}

    
export function enableStyleSheet() {
    var head = document.getElementsByTagName('head')[0];
    var link = document.createElement('link');
    link.rel = 'stylesheet';
    link.title = 'temp'
    link.href = 'styles/main.css';

    head.appendChild(link);
}
