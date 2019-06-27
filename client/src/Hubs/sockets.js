// TODO REWRITE THIS ON REACT COMPONENT

import * as signalR from '@aspnet/signalr/dist/browser/signalr';
import {config} from "../_helpers";

let connection = new signalR.HubConnectionBuilder()
    .withUrl(config.apiUrl + "/chatHub")
    .build();

connection.on("ReceiveMessage", function (user, message) {
    // console.log("Message:" + "@" + user + ":" + message);
    if (document.getElementById('nav_user') != null) {
        if (user == document.getElementById('nav_user').innerHTML) {
            document.getElementById('nav_balance').innerHTML = message;
        } else {
            let user = JSON.parse(localStorage.getItem('user'));
            var userId = JSON.parse(localStorage.getItem('user')).id;

            const requestOptions = {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + user.token
                },
                body: JSON.stringify(userId)
            };

            fetch(config.apiUrl + '/users/getBalance/' + userId, requestOptions)
                .then(function (response) {
                    response.json().then(function (data) {
                        //console.log(data);
                        if (document.getElementById('nav_balance')) {
                            document.getElementById('nav_balance').innerHTML = data;
                    
                            var elements = document.getElementsByClassName('balance');
                            for (let index = 0; index < elements.length; index++) {
                                elements[index].innerHTML = data;
                            }
                        }
                    });
                });
        }
    };
});

connection.on("send", data => {
    // console.log(data);
});

connection.start()
    .then(() => connection.invoke("SendMessage", "send", "Hello"));

    // TODO add methods for notificate transactions 
