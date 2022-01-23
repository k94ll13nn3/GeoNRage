export function addPreventDefault(e) {
    e.addEventListener('keydown', event => {
        if (['Enter', ' ', ',', 'Tab'].indexOf(event.key) >= 0) {
            event.preventDefault()
        }
    })
}
