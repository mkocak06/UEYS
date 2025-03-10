"use strict";

// Class Definition
var KTLogin = function () {
    var _login;

    



    // Handle forgot button
    $('#kt_login_forgot').on('click', function (e) {
        e.preventDefault();
        _showForm('forgot');
    });

    // Handle signup
    $('#kt_login_signup').on('click', function (e) {
        e.preventDefault();
        _showForm('signup');
    });


    // Handle cancel button
    $('#kt_login_signup_cancel').on('click', function (e) {
        e.preventDefault();

        _showForm('signin');
    });

var _handleForgotForm = function (e) {
    var validation;



    // Handle cancel button
    $('#kt_login_forgot_cancel').on('click', function (e) {
        e.preventDefault();

        _showForm('signin');
    });
}

// Public Functions
return {
    // public functions
    init: function () {
        _login = $('#kt_login');

        //_handleSignUpForm();
        _handleForgotForm();
    }
};
}();

var _showForm = function (form) {

    var cls = 'login-' + form + '-on';
    var form = 'kt_login_' + form + '_form';
    var _login = $('#kt_login');
    _login.removeClass('login-forgot-on');
    _login.removeClass('login-signin-on');
    _login.removeClass('login-signup-on');

    _login.addClass(cls);

    KTUtil.animateClass(KTUtil.getById(form), 'animate__animated animate__backInUp');
}

// Class Initialization
jQuery(document).ready(function () {
    KTLogin.init();
});

