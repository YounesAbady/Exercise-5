$('textarea').on('input', function () {
    $(this)
        .width(600)
        .height(100)
        .width(this.scrollWidth)
        .height(this.scrollHeight);
});