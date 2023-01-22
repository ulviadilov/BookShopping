let addToBastekBtns = document.querySelectorAll(".add-to-basket")

addToBastekBtns.forEach(btn => btn.addEventListener("click", function (e) {
    e.preventDefault();
    let url = btn.getAttribute("href");

    fetch(url)
        .then(response => {
            if (response.status == 200) {
                alert("Added to basket");
            }
            else {
                alert("Error");
                window.location.reload(true);
            }
        })
}))


let deleteFromBasket = document.querySelectorAll(".remove-from-basket")

deleteFromBasket.forEach(btn => btn.addEventListener("click", function (e) {
    e.preventDefault();
    btn.parentElement.remove();
}))