$(document).ready(function () {
    let x = 0;
    let s = "";

    console.log("Hello World");

    let theForm = $("#theForm");
    theForm.hide();

    let button = $("#buyButton");
    button.on("click", function () {
        console.log("Buying Item");
    });

    let productInfo = $(".product-props li");
    productInfo.on("click", function () {
        console.log("You clicked on " + $(this).text());
    });

    let $loginToggle = $("#loginToggle");
    let $popupForm = $(".popup-form");

    $loginToggle.on("click", function () {
        $popupForm.slideToggle(200);
    });
});


