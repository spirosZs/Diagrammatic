"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/gameHub")
    .configureLogging(signalR.LogLevel.Information)
    .withAutomaticReconnect()
    .build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start()
    .then(function () {
        document.getElementById("sendButton").disabled = false;
        console.log("connected");

    }).catch(function (err) {
        return console.error(err.toString());
    });

connection.onreconnecting(function (error) {
    console.assert(connection.state === signalR.HubConnectionState.Reconnecting);

    document.getElementById("messageInput").disabled = true;

    var li = document.createElement("li");
    li.textContent = "Connection lost due to error " + error + ". Reconnecting.";
    document.getElementById("messagesList").appendChild(li);
});

connection.onreconnected(function (connectionId) {
    console.assert(connection.state === signalR.HubConnectionState.Connected);

    document.getElementById("messageInput").disabled = false;

    var li = document.createElement("li");
    li.textContent = "Connection reestablished. Connected with connectionId " + connectionId + ".";
    document.getElementById("messagesList").appendChild(li);
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});