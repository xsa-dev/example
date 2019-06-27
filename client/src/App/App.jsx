import React from 'react';
import { Router, Route, Link } from 'react-router-dom';
import { connect } from 'react-redux';

import { history } from '../_helpers';
import { alertActions } from '../_actions';
import { PrivateRoute } from '../_components';
import { HomePage } from '../HomePage';
import { LoginPage } from '../LoginPage';
import { RegisterPage } from '../RegisterPage';
import { ForgotPage } from '../ForgotPage';
import { TransactionPage } from '../TransactionPage';
import { UserPage } from '../UserPage';
import { userService } from '../_services/user.service';


class App extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            balance: "Loading..."            
        }

        const { dispatch } = this.props;
        history.listen((location, action) => {
            dispatch(alertActions.clear());
        });        
    }

    componentWillUpdate() {
        const { user } = this.props;
        if (typeof(user) != 'undefined') {
            userService.getBalance(user.id).then(
                (value) =>
                    this.setState(
                        { balance: value }
                    )
            );
        }
    }

    render() {
        const { alert } = this.props;
        const { user } = this.props;
        const { balance } = this.state;
        return (

            <div className="jumbotron vertical-center">
                <div className="container">
                    <div className="col-sm-8 col-sm-offset-2">
                        {alert.message &&
                            <div className={`alert ${alert.type}`}>{alert.message}</div>
                        }
                        <Router history={history}>
                                <div>
                                    {user &&
                                        <nav className="nav p-3">
                                            <Link to="/user" className="p-1" id="nav_user">{user.username}</Link>
                                            <Link to="/transaction" className="p-1" id="nav_balance">Balance: { balance }</Link>                                        
                                        </nav>
                                    }                             

                                <PrivateRoute exact path="/" component={HomePage} />
                                <Route path="/login" component={LoginPage} />
                                <Route path="/register" component={RegisterPage} />
                                <Route path="/forgot" component={ForgotPage} />
                                <Route path="/transaction" component={TransactionPage} />
                                <Route path="/user" component={UserPage} />
                                <Route path="/verify" component={UserPage}/>
                            </div>
                        </Router>
                    </div>
                </div>
            </div>
        );
    }
}

function mapStateToProps(state) {
    const { alert, authentication, balance } = state;
    const { user } = authentication;
    return {
        alert,
        user,
        balance
    };
}

const connectedApp = connect(mapStateToProps)(App);
export { connectedApp as App }; 