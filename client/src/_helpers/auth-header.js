export function authHeader() {
    // return authorization header with jwt token
    let user = JSON.parse(localStorage.getItem('user'));

    if (user && user.token) {
        return { 'Authorization': 'Bearer ' + user.token };
    } else {
        return {};
    }
}

export function authHeaderEncodedForm() {
    // return authorization header with jwt token
    let user = JSON.parse(localStorage.getItem('user'));

    if (user && user.token) {
        return {
            'Content-Type': 'application/x-www-form-urlencoded',
            'Authorization': 'Bearer ' + user.token
        };
    } else {
        return {};
    }
}