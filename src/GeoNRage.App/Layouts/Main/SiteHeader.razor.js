export function enableStyleSheet(e) {
    document.body.removeAttribute('data-theme');
    if (e) {
        document.body.setAttribute('data-theme', e)
    }
}
