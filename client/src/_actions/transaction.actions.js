import { transactionConstants} from "../_constants";
import { transactionService} from "../_services";
import { alertActions} from "./";
import { history } from "../_helpers";

export const transactionActions = {
    createTransaction,
    getAllTransactions,
    getByIdTransaction,
    getAllUserTransactions
}

function createTransaction(from, to, amount) {
    console.log(from);
    console.log(to);
    console.log(amount);

    return dispatch => {
        dispatch(request({from,to,amount}));

        transactionService.createTransaction(from, to, amount)
            .then(
                transaction => {
                    // todo double check
                    dispatch(success(transaction));
                    //history.push('/');
                    dispatch(alertActions.success('Transaction: ' + transaction.id + ' created.'));
                },
                error => {
                    dispatch(failure(error));
                    dispatch(alertActions.error(error));
                }
            );
    };

    function request(transaction) { return { type: transactionConstants.TRANSACTION_REQUEST, transaction }}
    function success(transaction) { return { type: transactionConstants.TRANSACTION_SUCCESS, transaction }}
    function failure(error) {return { type: transactionConstants.TRANSACTION_FAILURE, error }}
}

function getAllTransactions() {
    return dispatch => {
        dispatch(request());

        transactionService.getAllTransactions()
            .then(
                transactions => dispatch(success(transactions)),
                error => dispatch(failure(error))
            );
    };

    function request() {return {type: transactionConstants.GETALL_REQUEST}}
    function success(transactions) { return { type: transactionConstants.GETALL_SUCCESS, transactions}}
    function failure(error) { return { type: transactionConstants.GETALL_FAILURE, error}}
}

function getAllUserTransactions(userId) {
    return dispatch => {
        dispatch(request());

        transactionService.getAllUserTransactions(userId)
            .then(
                transactions => dispatch(success(transactions)),
                error => dispatch(failure(error))
            );
    };

    function request() { return { type: transactionConstants.GETUSERTRANSACTIONS_REQUEST }}
    function success(transactions) { return { type: transactionConstants.GETUSERTRANSACTIONS_SUCCESS, transactions }}
    function failure(error) { return { type: transactionConstants.GETUSERTRANSACTIONS_FAILURE, error}}
}

function getByIdTransaction(id) {
    return dispatch => {

        dispatch(request(id))

        transactionService.getById(id)
            .then(
                () => {
                    dispatch(success(id));
                },
                error => {
                    dispatch(failure(id, error));
                }
            );
    };

    function request(id) { return { type: transactionConstants.GETBYID_REQUEST, id}}
    function success(id) { return { type: transactionConstants.GETBYID_SUCCESS, id}}
    function failure(id, error) {return { type: transactionConstants.GETBYID_FAILURE, id, error}}
}
