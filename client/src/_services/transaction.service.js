import { authHeader, config, authHeaderEncodedForm } from '../_helpers';

export const transactionService = {
    createTransaction,
    getAllTransactions,
    getByIdTransaction,
    getAllUserTransactions,
    checkTransactionBalance
}

function createTransaction(from, to, sum) {
    console.log('from: ' + from + ", to: "  + to + ", amount:" + sum);
    
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: "fromId=" + from + "&toId=" + to + "&amount=" + sum + "&state=true&account=0/"
    };

    return fetch(config.apiUrl + '/transactions/create', requestOptions)
        .then(handleResponse, handleError);
}

function getAllTransactions() {
    const requestOptions = {
        method: 'GET',
        headers: authHeader()
    };

    return fetch(config.apiUrl + '/transactions/get', requestOptions).then(handleResponse,handleError);
}

function checkTransactionBalance(id, amount) {
    const requestOptions = {
        method: 'POST',
        headers: authHeaderEncodedForm(),
        body: "userId=" + id + "&amount=" + amount
    };

    return fetch(config.apiUrl + '/transactions/CheckTransaction', requestOptions)
        .then(handleResponse, handleError);
}

function getAllUserTransactions(id) {
    const requestOptions = {
        method: 'GET',
        headers: authHeader(),
    };

    return fetch(config.apiUrl + '/transactions/GetTransactionsByUser/' + id, requestOptions).then(handleResponse, handleError);
}

function getByIdTransaction(id) {
    const requestOptions = {
        method: 'GET',
        headers: authHeader()
    };

    return fetch(config.apiUrl + '/transactions/' + _id, requestOptions).then(handleResponse, handleError);
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