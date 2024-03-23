export function enableStyleSheet(e) {
    document.documentElement.removeAttribute('data-theme');
    if (e) {
        document.documentElement.setAttribute('data-theme', e)
    }
}
