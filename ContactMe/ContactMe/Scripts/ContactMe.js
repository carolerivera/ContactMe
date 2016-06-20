// Convert a string time such as 9:00 AM to a date object
function timeToDate(time) {
    // Use a default date since we only care about the time component.
    var date = Date.parse("1/1/2000 " + time);

    // When validating the time to call, we need to add 12 hours worth of seconds if it's PM.
    // 12 * 60 * 60 = 43,200 seconds
    if (time.toString().toLowerCase().indexOf("pm")) {
        date += 43200;
    }

    return date;
}

// Remove any server side error messages
function clearErrorMessages() {
    var container = $(".error-messages");
    container.find("h4, ul").empty();
}

// Display any server side error messages
function showErrorMessages(messages) {
    // Empty the container, then add the new message(s)
    clearErrorMessages();

    // If there are multiple messages, it's server side model validation
    if ($.isArray(messages) && messages.length > 1) {
        $(".error-messages h4").text("Please fix the following issues, then try again.");
        $.each(messages, function (index, value) {
            $(".error-messages ul").append("<li>" + value + "</li>");
        });
    } else {
        $(".error-messages h4").text(messages || "An error occurred, please try again.");
    }
}

// Reset the form (user-entered values, validation, etc.)
function resetForm() {
    // Clear all values from the form
    var form = $("#contact_form");
    form.find("input[type=radio]").prop('checked', false);

    // If the Captcha has already been validated, don't reset it.
    // This is because the Captcha can only be validated once per page.
    if (form.find(".captcha-field-wrapper.validated").length) {
        form.find("input[type=text]:not([name=CaptchaCode])").val("");
    } else {
        form.find("input[type=text]").val("");
    }

    // Reset the jQuery validation
    var validator = form.validate();
    validator.resetForm();

    // Remove any server side messages
    clearErrorMessages();

    // Focus on the first field
    $("#FirstName").focus();
}

// Toggle a loading state on the form while the AJAX call completes
function toggleLoading() {
    var form = $("#contact_form");
    var isLoading = form.hasClass("loading");

    // Enable/disable the form
    if (isLoading) {
        form.find("input").prop("disabled", false);
    } else {
        form.find("input").prop("disabled", true);
    }

    form.toggleClass("loading");
}

// Add all custom jQuery validation methods
function addValidationMethods() {
    // Validate hidden fields such as hidden radio buttons.
    $.validator.setDefaults({
        ignore: []
    });

    // Show an error if the time has been entered in the wrong format,
    // or if AM/PM has been selected and there is no time entered.
    $.validator.addMethod("time12", function (value, element) {
        var amOrPm = $("input[name=AMPM]:checked").val();
        var timeIsValid = /^((0?[0-9])|(1[0-2])):[0-5][0-9]$/.test(value);

        if (amOrPm) {
            return timeIsValid;
        }

        return this.optional(element) || timeIsValid;
    }, "Enter a 12-hour time such as \"12:30\".");

    // Show an error message if a time has been entered, but AM or PM has not been selected
    $.validator.addMethod("hasAmOrPm", function (value, element) {
        var amOrPm = $("input[name=AMPM]:checked").val();
        var timeHasBeenEntered = value.length;

        // If the time has not been entered, AM or PM is not required and is valid
        if (timeHasBeenEntered) {
            return amOrPm;
        }

        return true;
    }, "Select AM or PM.");

    // Show an error message if the time is outside the specified range
    $.validator.addMethod("timeRange", function (value, element, params) {

        // If no time has not been entered, the field is valid
        if (!value.length) {
            return true;
        }

        var valueWithAMPM = value;
        var amPmValue = $(".am-pm-wrapper input:checked").val();
        if (amPmValue) {
            valueWithAMPM += " " + amPmValue;
        }

        var minTime = timeToDate(params[0]);
        var maxTime = timeToDate(params[1]);
        var enteredTime = timeToDate(valueWithAMPM);

        return enteredTime >= minTime && enteredTime <= maxTime;
    }, function (params, element) {
        return "Enter a time between " + params[0] + " and " + params[1] + ".";
    });

    // Show an error message if a time has an invalid increment
    $.validator.addMethod("minuteIncrement", function (value, element, params) {
        var increment = Number(params);
        var minutes = Number(value.substr(value.indexOf(":") + 1));
        return !isNaN(minutes) && minutes % increment === 0;
    }, function (params, element) {
        return "Enter a time with a " + params + " minute increment.";
    });

    $.validator.addMethod("phone", function (value, element) {
        // Very light phone number validation
        return this.optional(element) || /^[0-9 \-+()x.]*$/.test(value);
    }, "Enter a valid phone number such as \"555-555-5555\".");
}

// When the page is ready, run the handler
$(function () {

    // Configure the validator
    addValidationMethods();

    // The .get is required by BotDetect
    var captcha = $("#CaptchaCode").get(0);

    // Form validation, including captcha
    $("#contact_form").validate({
        onkeyup: false,
        errorPlacement: function errorPlacement(error, element) {
            if (element.attr("name") == "TimeToCall") {
                error.insertAfter(".am-pm-wrapper");
            } else {
                error.insertAfter(element);
            }
        },
        rules: {
            FirstName: {
                required: true,
                maxlength: 40
            },
            LastName: {
                required: true,
                maxlength: 40
            },
            EmailAddress: {
                required: true,
                maxlength: 80,
                email: true
            },
            Telephone: {
                maxlength: 16,
                phone: true
            },
            TimeToCall: {
                time12: true,
                hasAmOrPm: true,
                timeRange: [$("#TimeToCall").data("mintime"), $("#TimeToCall").data("maxtime")],
                minuteIncrement: $("#TimeToCall").data("minuteinc")
            },
            CaptchaCode: {
                required: true,
                remote: {
                    url: "/Contact/ValidateCaptcha",
                    type: "POST",
                    data: {
                        captchaId: captcha.Captcha.Id,
                        instanceId: captcha.Captcha.InstanceId,
                        userInput: function userInput() {
                            return captcha.value;
                        }
                    },
                    complete: function complete(result) {
                        if (result.responseJSON) {
                            // Once the user has successfully validated, disable the Captcha.
                            // Captcha can only be validated once per page.
                            var wrapper = $(".captcha-field-wrapper");
                            wrapper.addClass("validated");
                            wrapper.find("input").removeClass("error").prop("disabled", true);
                        } else {
                            // Always change Captcha code if validation fails.
                            captcha.Captcha.ReloadImage();
                        }
                    }
                }
            }
        },
        messages: {
            FirstName: {
                required: "Enter your first name.",
                maxlength: "Do not exceed 40 characters."
            },
            LastName: {
                required: "Enter your last name.",
                maxlength: "Do not exceed 40 characters."
            },
            Telephone: {
                maxlength: "Do not exceed 16 characters."
            },
            EmailAddress: {
                required: "Enter your email address.",
                email: "Enter a valid email address such as \"name@domain.com\".",
                maxlength: "Do not exceed 80 characters."
            },
            CaptchaCode: {
                required: "Enter the code you see above.",
                remote: "Incorrect code."
            }
        },
        submitHandler: function submitHandler(form, event) {
            // Prevent the postback since we want an AJAX call instead
            event.preventDefault();

            // Set the value of BestTimeToCall since the UI is broken into multiple fields
            var time = $("#TimeToCall").val();
            if (time && time.length) {
                var amOrPm = $("input[name=AMPM]:checked").val();
                if (amOrPm && amOrPm.length) {
                    $("#BestTimeToCall").val(time + " " + amOrPm);
                } else {
                    $("#BestTimeToCall").val(time);
                }
            }

            // Set the hidden Captcha fields and enable the CaptchaCode
            $("#CaptchaId").val(captcha.Captcha.Id);
            $("#CaptchaInstanceId").val(captcha.Captcha.InstanceId);
            $("#CaptchaCode").prop("disabled", false);

            // Serialize the form data
            var data = $(form).serialize();

            // Show the loading icon and disable the form
            toggleLoading();

            // Submit the form
            $.ajax({
                type: "POST",
                url: form.action,
                data: data,
                dataType: "json",
                success: function success(result) {
                    // Process the response from the server and display messages to the user.
                    toggleLoading();
                    if (result.Success) {
                        $("#contact_form").fadeOut().remove();
                        $("#success").fadeIn();
                    } else {
                        showErrorMessages(result.Errors);
                        // Always change Captcha code if something fails.
                        captcha.Captcha.ReloadImage();
                    }
                },
                error: function error() {
                    toggleLoading();
                    showErrorMessages("An error occurred, please try again.");
                    // Always change Captcha code if something fails.
                    captcha.Captcha.ReloadImage();
                }
            });
        }
    });

    // Validate the time to call when the AM or PM inputs change
    $(".am-pm-wrapper input").on("change", function () {
        $("#TimeToCall").valid();
    });
});


