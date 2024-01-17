// recipe.js
document.addEventListener("DOMContentLoaded", function () {
    function saveIconState(recipeId, isIconSolid) {
        localStorage.setItem('recipe_' + recipeId, isIconSolid);
    }

    function getIconState(recipeId) {
        return localStorage.getItem('recipe_' + recipeId) === 'true';
    }

    function updateIconState(button, isIconSolid) {
        var iconElement = button.querySelector('i');
        iconElement.classList.toggle('fa-solid', isIconSolid);
        iconElement.classList.toggle('fa-regular', !isIconSolid);
        button.disabled = isIconSolid;
        button.classList.toggle('clicked', isIconSolid);
    }

    var cartButtons = document.querySelectorAll(".addToCartIcon");

    cartButtons.forEach(function (button) {
        var recipeId = button.dataset.recipeId;
        var isIconSolid = getIconState(recipeId);
        updateIconState(button, isIconSolid);

        button.addEventListener("click", function () {
            var recipeId = this.dataset.recipeId;
            var isIconSolid = !getIconState(recipeId);

            // Save the icon state to localStorage
            saveIconState(recipeId, isIconSolid);

            // Update the icon state on the button
            updateIconState(this, isIconSolid);

            // Disable the button to prevent multiple clicks
            this.disabled = true;

            // Trigger a custom event for synchronization
            var event = new Event('recipeIconClicked');
            this.dispatchEvent(event);

            // Send recipeId to the server to save to the database
            saveRecipeToCart(recipeId);
        });
    });

    // Fetch data from the server to synchronize icon states with the database
    function synchronizeIconStates() {
        var allButtons = document.querySelectorAll('.addToCartIcon');

        allButtons.forEach(function (button) {
            var recipeId = button.dataset.recipeId;

            fetch('/Cart/GetAddedCarts')
                .then(response => response.json())
                .then(data => {
                    var isIconSolid = data.includes(recipeId);
                    updateIconState(button, isIconSolid);
                })
                .catch(error => console.error('Error fetching data:', error));
        });
    }

    // Initial synchronization
    synchronizeIconStates();

    // Event listener for custom event
    document.addEventListener('recipeIconClicked', synchronizeIconStates);

    // Function to send recipeId to the server to save to the database
    function saveRecipeToCart(recipeId) {
        fetch('/Cart/SaveCart', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: 'recipeId=' + encodeURIComponent(recipeId),
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
            })
            .catch(error => console.error('Error saving recipe to cart:', error));
    }
});
