//order page
function quantity(option) {
    let qty = $('#qty').val();
    let price = parseInt($('#price').val());
    let totalAmout = 0;
    if (option === 'inc') {
        qty = parseInt(qty) + 1;        
    } else {
        qty = qty == 1 ? qty : qty - 1;
    }
    totalAmout = price * qty;
    $('#qty').val(qty);
    $('#totalAmout').val(totalAmout);
}