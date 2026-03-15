window.setupIntersectionObserver = function (elementId, dotnetHelper) {
    const element = document.getElementById(elementId);

    if (!element) return;

    const observer = new IntersectionObserver(entries => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                dotnetHelper.invokeMethodAsync('OnElementVisible');
            }
        });
    });

    observer.observe(element);
};