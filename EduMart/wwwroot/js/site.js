// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Top bar: the Menu button opens and closes the nav panel under 60rem.
(function () {
    var bar = document.querySelector('[data-bar]');
    var toggle = document.querySelector('[data-bar-toggle]');
    if (bar && toggle) {
        toggle.addEventListener('click', function () {
            var open = toggle.getAttribute('aria-expanded') === 'true';
            toggle.setAttribute('aria-expanded', String(!open));
            bar.toggleAttribute('data-open', !open);
        });
    }

    // Disclosure menus (Management, profile): close on outside click and Escape.
    var menus = Array.prototype.slice.call(document.querySelectorAll('details[data-menu]'));
    document.addEventListener('click', function (e) {
        menus.forEach(function (m) { if (m.open && !m.contains(e.target)) m.open = false; });
    });
    document.addEventListener('keydown', function (e) {
        if (e.key !== 'Escape') return;
        menus.forEach(function (m) {
            if (m.open) { m.open = false; m.querySelector('summary').focus(); }
        });
    });
})();
