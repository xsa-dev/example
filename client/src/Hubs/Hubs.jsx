import React from 'react';

import * as signalR from '@aspnet/signalr/dist/browser/signalr';
import { config } from "../_helpers";


class Hubs extends React.Component {
    // todo incomponent using
    constructor(props) {
        super(props);

        this.state = {
            user: '',
            message: '',
        };
    }

    componentDidMount() {
        let connection = new signalR.HubConnectionBuilder()
            .withUrl(config.apiUrl + "/chatHub")
            .build();

        connection.on("ReceiveMessage", function (user, message) {
            console.log("Balance:" + "@" + user + ":" + message);
        });

        connection.on("send", data => {
            console.log(data);
        });

        connection.start()
            .then(() => connection.invoke("SendMessage", "Server", "Hello, user! This is on connection message"));
    }
}
export default Hubs;


