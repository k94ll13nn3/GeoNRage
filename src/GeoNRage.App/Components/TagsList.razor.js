export function getTextContent(e) {
    return e.textContent;
}

export function resetElement(e) {
    e.innerHTML = '';
}

export function addPreventDefault(e) {
    e.addEventListener('keydown', event => {
        if (['Enter', ' ', ',', 'Tab'].indexOf(event.key) >= 0) {
            event.preventDefault()
        }
    })
}
