import React from 'react';
import {Link } from 'react-router-dom';
import { connect } from 'react-redux';

import { transactionActions} from "../_actions/transaction.actions";
import { alertActions } from '../_actions';

class TransactionPage extends React.Component {
    constructor(props) {
        super(props);

        this.state = {

        };

        // add transaction
    }

    componentDidMount() {
        const { user } = this.props;
        this.props.dispatch(transactionActions.getAllUserTransactions(user.id));
    }

    copyTransaction(transaction) {
        let message = "Do you want create a copy of this transaction? \n" +
            'Id: ' + transaction.id + "\n" + 
            'Amount: ' + transaction.amount + "\n" +
            'To: ' + transaction.correspondent + "\n";
        
        let create = confirm(message);
        
        if (create) {
            this.props.dispatch(transactionActions.createTransaction(transaction.fromId, transaction.toId, transaction.amount));
            this.props.dispatch(transactionActions.getAllUserTransactions(transaction.fromId));
            this.forceUpdate();                
        }        
    }

    handleSubmit(e) {
        e.preventDefault();
        this.setState({submitted: true});
    }

    render() {
        const { transactions, user } = this.props;

        return <div className="col-md-6 col-md-offset-3">
            <h1>Transactions</h1>
            {transactions.items &&
                <table className='table'>
                <thead>
                    <tr>
                        <th scope="col">Date/Time</th>
                        <th scope="col">Correspondent</th>
                        <th scope="col">Amount</th>
                        <th scope="col">Resulting</th>
                    </tr>
                </thead>
                    <tbody>
                    {transactions.items.map((transaction, index) =>
                        <tr scope="row" key={index}>
                            <td>{ transaction.formated_date}</td>
                            <td>{transaction.correspondent}</td>
                            <td>{transaction.direction == 1 && '-' || transaction.direction == 2 && '+'}
                                {transaction.amount}
                            </td>
                            <td> 
                                {transaction.subtotal}
                            </td>
                            <td>
                                {transaction.correspondent !== "PW" && transaction.direction !== 2 && <button className="btn" onClick={this.copyTransaction.bind(this, transaction)}>Copy</button> }
                            </td>
                        </tr>
                        )}     
                    </tbody>    
                </table>
            }
            {/* {transactions.items &&
                <table className='table'>
                    <thead>
                        <th scope="col">Date/Time</th>
                        <th scope="col">From</th>
                        <th scope="col">To</th>
                        <th scope="col">Amount (Debit/Credit)</th>
                        <th scope="col">Resulting</th>
                    </thead>
                    <tbody>
                        {
                        }
                        {transactions.items.map((transaction, index) =>
                            <tr scope="row">
                                <td>{transaction.date}</td>
                                <td>{transaction.from.username}</td>
                                <td>{transaction.to.username}</td>
                                <td>{transaction.direction == 1 && '+' || transaction.direction == 0 && '-'}
                                    {transaction.amount}
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            } */}

            <nav className="nav">
                <li>
                    <ul>
                        <Link to="/">Home</Link>
                    </ul>
                    <ul>
                        <Link to="/login">Logout</Link>
                    </ul>
                </li>
            </nav>
        </div>

    }
}

function mapStateToProps(state) {
    const {transactions, authentication} = state;
    const { user } = authentication;
    return {
        transactions,
        user,
    }
}

const connectedTransactionPage = connect(mapStateToProps)(TransactionPage);
export { connectedTransactionPage as TransactionPage };