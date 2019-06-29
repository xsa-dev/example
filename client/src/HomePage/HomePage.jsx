import React from 'react';
import {Link} from 'react-router-dom';
import {connect} from 'react-redux';

import {Typeahead} from 'react-bootstrap-typeahead'; 


import { userActions } from '../_actions/user.actions';
import { transactionActions } from "../_actions/transaction.actions";
import { Actions } from '../_actions';
import { alertConstants } from "../_constants";

import { userService } from '../_services/user.service';
import { transactionService } from '../_services';


class HomePage extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            from: '',
            to: '',
            amount: '',
            balance_checked: false,
            submitted: false,
            options: 
                [{ id: 0, label: 'server is loading users..' }],
        };

        this.handleChange = this.handleChange.bind(this);
        this.handleCreateTransaction = this.handleCreateTransaction.bind(this);
        this.handleTransactionCheckBalance = this.handleTransactionCheckBalance.bind(this);
    }

    componentWillMount() {
        const { user } = this.props;
        userService.getAll(user.id)
            .then(
                (users_objects) =>
                    this.setState(
                        { options: users_objects }
                    )
            );
    }

    handleTransactionCheckBalance() {
        const { user } = this.props;
        const { amount } = this.state;
        if (user.id == 'undefined') {
            return;
        }
        transactionService.checkTransactionBalance(user.id, amount)
            .then(
                (check) => {
                    this.setState(
                        { balance_checked: check }
                    );
                 }
        ).catch((e) => console.log(e));           
    }

    handleChangeChangeTo(selected) {
        try {
            selected = selected[0].id;
            this.setState(
                { to: selected }
            );
        } catch (E) {
            return;
        }            
    }

    handleChange(e) {
        const {name, value} = e.target;
        this.setState({ [name]: value });
        console.log('name: ' + name + ' value: ' + value);
    }

    handleCreateTransaction() {
        const { user } = this.props;
        const { to, amount } = this.state;

        this.props.dispatch(transactionActions.createTransaction(user.id, to, amount));
    }

    handleDeleteUser(id) {
        return (e) => this.props.dispatch(userActions.delete(id));
    }

    render() {   
        const { user, users } = this.props;
        const { from, to, amount, options } = this.state;
        const { balance_checked } = this.state;
        return (
                <div className="col-md-6 col-md-offset-3">
                {/* <h1>Hi, {user.username}!</h1> */}
                <p>You're logged in with React and ASP.NET</p>
                {!user.isVerified && <p style={{ color: 'red' }}>The email is not check, please send confirmation <a href='/user'>here</a> </p>}

                {users.error && <span className="text-danger">ERROR: {users.error}</span>}
                <h3>Create transaction:</h3>
                <div className="form-group">
                    <label>To</label>     
                    <Typeahead
                        options={options}
                        placeholder="Choose a user..."
                        value={to}
                        labelKey="label"
                        key="id"
                        id="id"
                        filterBy={["label"]}                        
                        onChange={
                            (selected) => {
                            this.handleChangeChangeTo(selected)
                            }}
                        disabled = {!user.isVerified}
                    />                    
                    <label>Amount</label>
                    <input type="number" className="form-control" min="0" step="1" name="amount" value={amount} onChange={this.handleChange} onKeyUp={this.handleTransactionCheckBalance} disabled={!to}/>
                    <div className="p-3">                    
                        {balance_checked && to && <button className="btn btn-block" onClick={this.handleCreateTransaction}>Send</button>}    
                        {!balance_checked && to && <p>Type valid amount to send PW</p>}
                    </div>
                </div>

                <nav className="nav">
                    <li>
                        <ul>
                            <Link to="/transaction">Transactions</Link>
                        </ul>
                        <ul>
                            <Link to="/login">Logout</Link>
                        </ul>
                    </li>
                </nav>
            </div>
        );
    }
}

function mapStateToProps(state) {
    const {users, authentication} = state;
    const { user } = authentication;
    return {
        user,
        users,
    };
}

const connectedHomePage = connect(mapStateToProps)(HomePage);
export { connectedHomePage as HomePage };