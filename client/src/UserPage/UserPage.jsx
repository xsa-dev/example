import React from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { userActions } from '../_actions/index';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEye } from '@fortawesome/free-solid-svg-icons';
import { faEyeSlash } from '@fortawesome/free-solid-svg-icons';


// TODO test
import { userService } from '../_services/user.service';

require ('../additional_scripts/User');


class UserPage extends React.Component {
    constructor(props) {
        super(props);
        const { dispatch } = this.props;
        
        this.state = {
            balance: "Loading balance..",
            oldpassword: true,
            newPassword1: "",
            newPassword2: "",
            validated: {
                password: false
            },
            currentIcon: faEye,
            newusername: "",
            oldemail: "",
            user: "",
            emailSended: false,
            code: "",
            users: ""
        };

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.validateEmail = this.validateEmail.bind(this);
        this.checkCode = this.checkCode.bind(this);
    }

    handleChange(e) {
        const { name, value } = e.target;
        this.setState({ [name]: value });
        console.log('name: ' + name + ' value: ' + value);
    }

    componentDidMount() {
        const { user } = this.props;
        userService.getBalance(user.id).then(
            (value) => 
            this.setState(
                { balance: value }
            )
        );

        this.setState(
            { oldemail: user.email }
        );
    }

    validateEmail() {
        const { user, dispatch } = this.props;
        console.log(user);
        dispatch(userActions.confirm(user));
        this.setState(
            { emailSended: true }
        );
    }

    checkCode() {
        const { user, dispatch } = this.props;
        const { code } = this.state;

        dispatch(userActions.checkConfirmation(code, user.id));
        

    }

    checkOldPassword() {
        // later
    }

    handleSubmit() {
        const { user } = this.props;
        const { newPassword1, newusername } = this.state;      
        const { dispatch } = this.props;
        
        let newPassword = {
            id: user.id,
            Email: user.email,
            Username: user.username,
            Password: newPassword1
        };

        dispatch(userActions.update(newPassword));
    }

    componentWillUpdate() {    
    }
        
    render() {
        const { balance, oldpassword, newPassword1, newPassword2, validated, currentIcon, newusername, emailSended, code} = this.state; 
        const { user } = this.props;
        const { users } = this.state;
        return (            
            <div className="col-md-6 col-md-offset-3">
                <p>Your balance is: <span className="balance"> {balance} </span></p> 
                {!user.isVerified && <h2>Validate email</h2>}
                <div className={'form-group'}>
                    <div className={'form-group'}>
                        <label htmlFor="email">Email</label>
                        <input name="email" type="email" name="email" value={user.email} className="form-control" disabled={true}></input>
                    </div>
                    {
                        !emailSended && !user.isVerified && <button className="btn btn-block" onClick={this.validateEmail}>Verify</button>
                    } 
                    {
                        emailSended
                        && <label htmlFor="code">Copy verification code here:</label>
                        &&
                        <div>
                        <div className="form-group">
                            <input type="text" className={'form-control'} name="code" value={code} onChange={this.handleChange}/>
                        </div>
                            <button className={'btn btn-block'} onClick={this.checkCode} hidden={ users && users.user.isVerified }>Send</button>
                        </div>

                    }
                </div>
                <h2>Change password:</h2>
                <div className={'form-group'}>                    
                    <div className="form-group">
                        <label htmlFor="newPassword1">New password</label>
                        <input name="newPassword1" type="password" name="newPassword1" onChange={this.handleChange} value={newPassword1} className="form-control" disabled={!oldpassword}></input>
                    </div>
                    <div className="form-group">
                        <label htmlFor="oldPassword2">Repeat new password</label>
                        <input name="oldPassword2" type="password" name="newPassword2" onChange={this.handleChange} value={newPassword2} className="form-control" disabled={!oldpassword}></input>                
                    </div>                    
                </div>
                {validated.password = newPassword1 == newPassword2 }
                {validated.password && newPassword1 && newPassword2 && <button className="btn btn-block" onClick={this.handleSubmit}>Update</button>}
                
                <nav className="nav">
                    <li>
                        <ul>
                            <Link to="/">Home</Link>
                        </ul>
                        <ul>
                            <Link to="/transaction">Transactions</Link>
                        </ul>
                        <ul>
                            <Link to="/login">Logout</Link>
                        </ul>
                    </li>
                </nav>
            </div>
        )
    }
}

function mapStateToProps(state) {
    const { authentication, balance } = state;
    const { user } = authentication;
    return {
        balance, 
        user
    };
}

const connectedUserPage = connect(mapStateToProps)(UserPage);
export { connectedUserPage as UserPage };