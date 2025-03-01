document.addEventListener("DOMContentLoaded", function () {
    const inputs = document.querySelectorAll(".otp-input");
    const form = document.querySelector(".otp-Form");

    inputs.forEach((input, index) => {
        input.addEventListener("input", (e) => {
            let value = e.target.value;

            // Ensure only one digit is entered
            if (value.length > 1) {
                e.target.value = value.charAt(value.length - 1);
            }

            // Move to the next input automatically
            if (value.length === 1 && index < inputs.length - 1) {
                inputs[index + 1].focus();
            }
        });

        input.addEventListener("keydown", (e) => {
            if (e.key === "Backspace" && !e.target.value && index > 0) {
                inputs[index - 1].focus(); // Move back
            }
        });
    });

    // Combine OTP before form submission
    form.addEventListener("submit", function (e) {
        e.preventDefault(); // Prevent default form submission

        let otpValue = "";
        inputs.forEach(input => {
            otpValue += input.value;
        });

        // Create a hidden input field to send the combined OTP
        let hiddenOtpInput = document.createElement("input");
        hiddenOtpInput.type = "hidden";
        hiddenOtpInput.name = "otp";
        hiddenOtpInput.value = otpValue;
        form.appendChild(hiddenOtpInput);

        // Submit the form
        form.submit();
    });

    // Allow pasting a 6-digit OTP
    document.addEventListener("paste", (event) => {
        let pastedData = (event.clipboardData || window.clipboardData).getData("text");
        if (pastedData.length === inputs.length && /^\d+$/.test(pastedData)) { // Ensure only numbers are pasted
            event.preventDefault();
            pastedData.split("").forEach((char, i) => {
                if (inputs[i]) {
                    inputs[i].value = char;
                }
            });
            inputs[inputs.length - 1].focus(); // Focus last input
        }
    });
});


