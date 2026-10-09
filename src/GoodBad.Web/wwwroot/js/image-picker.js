// Shared image picker: "Tag billede" opens the camera directly on mobile,
// "Vælg billede" opens the normal picker (camera roll, Google Photos, files),
// and an external image URL can be typed instead.
(function () {
    if (window.goodbadImagePicker) {
        return;
    }
    window.goodbadImagePicker = true;

    document.addEventListener('click', function (event) {
        var button = event.target.closest('[data-image-camera], [data-image-gallery]');
        if (!button) {
            return;
        }

        var field = button.getAttribute('data-image-camera') || button.getAttribute('data-image-gallery');
        var input = document.getElementById(field + '-file');
        if (!input) {
            return;
        }

        if (button.hasAttribute('data-image-camera')) {
            // capture=environment asks the browser for the rear camera.
            input.setAttribute('capture', 'environment');
        } else {
            input.removeAttribute('capture');
        }

        input.click();
    });

    document.addEventListener('change', function (event) {
        var input = event.target.closest('input[type=file][data-image-input]');
        if (!input) {
            return;
        }

        var selector = input.getAttribute('data-image-preview');
        var preview = selector ? document.querySelector(selector) : null;
        if (!preview || !input.files || input.files.length === 0) {
            return;
        }

        preview.src = URL.createObjectURL(input.files[0]);
        preview.style.display = '';
    });
})();
