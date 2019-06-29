import { authHeader, config } from '../_helpers';

export const userService = {
    login,
    logout,
    register,
    getAll,
    getById,
    update,
    delete: _delete,
    getBalance,
    reset,
    sendVerificationCode,
    verifyCodeForUser
};

function login(email, password) {
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password })
    };

    return fetch(config.apiUrl + '/users/authenticate', requestOptions)
        .then(handleResponse, handleError)
        .then(user => {
            if (user && user.token) {
                localStorage.setItem('user', JSON.stringify(user));
            }

            return user;
        });
}

function logout() {
    localStorage.removeItem('user');
}

function getAll(user) {
    const requestOptions = {
        method: 'GET',
        headers: authHeader()
    };

    return fetch(config.apiUrl + '/users/allFrom/' + user, requestOptions).then(handleResponse, handleError);
}

function getById(id) {
    const requestOptions = {
        method: 'GET',
        headers: authHeader()
    };

    return fetch(config.apiUrl + '/users/' + _id, requestOptions).then(handleResponse, handleError);
}

function register(user) {
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(user)
    };

    return fetch(config.apiUrl + '/users/register', requestOptions).then(handleResponse, handleError);
}

function update(user) {
    const requestOptions = {
        method: 'PUT',
        headers: { ...authHeader(), 'Content-Type': 'application/json' },
        body: JSON.stringify(user)
    };

    return fetch(config.apiUrl + '/users/' + user.id, requestOptions).then(handleResponse, handleError);
}

// prefixed function name with underscore because delete is a reserved word in javascript
function _delete(id) {
    const requestOptions = {
        method: 'DELETE',
        headers: authHeader()
    };

    return fetch(config.apiUrl + '/users/' + id, requestOptions).then(handleResponse, handleError);
}

function reset(email) {
    const requestOptions = {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' },
    };

    return fetch(config.apiUrl + '/users/resetPassword?=' + email.email, requestOptions).then(handleResponse, handleError);
}

function getBalance(userId) {
    const requestOptions = {
        method: 'POST',
        headers: { ...authHeader(), 'Content-Type': 'application/json'},
        body: JSON.stringify(userId)
    };
    
    return fetch(config.apiUrl + '/users/getBalance/' + userId, requestOptions).then(handleResponse, handleError);
}

function sendVerificationCode(email) {
    const requestOptions = {
        method: 'POST',
        headers: { ...authHeader() }
    };

    return fetch(config.apiUrl + '/Users/verifyEmail?email=' + email, requestOptions).then(handleResponse, handleError);
}

function verifyCodeForUser(code, userId) {
    const requestOptions = {
        method: 'GET',
        headers: { ...authHeader() }
    };

    return fetch(config.apiUrl + '/Users/verifyEmailCode?code=' + code + '&userid=' + userId, requestOptions)
        .then(handleResponse, handleError)
        .then(user => {

            if (user && user.isVerified) {         
                let old_user = JSON.parse(localStorage.getItem('user'));
                old_user.isVerified = user.isVerified;                
                localStorage.setItem('user', JSON.stringify(old_user));
            }

            return user;
        });
}

function handleResponse(response) {
    return new Promise((resolve, reject) => {
        if (response.ok) {
            var contentType = response.headers.get("content-type");
            if (contentType && contentType.includes("application/json")) {
                response.json().then(json => resolve(json));
            } else {
                resolve();
            }
        } else {
            response.text().then(text => reject(text));
        }
    });
}

function handleError(error) {
    return Promise.reject(error && error.message);
}