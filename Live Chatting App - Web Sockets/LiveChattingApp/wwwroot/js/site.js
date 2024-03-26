// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();

connection.on('ReceiveMessage', displayMessage);
connection.on('PrintOlderMessage', displayPastMsgs);
connection.start();
var userName;

//form submit event handler
document.forms.userForm.addEventListener('submit', function (e) {
    e.preventDefault();

    var userMsg = document.getElementById('message');
    var text = userMsg.value;
    userMsg.value = '';

    var userField = document.getElementById('username');
    userName = userField.value;
    userField.disable;
    sendMessage(userName, text);
});

//sending values to method 'SendAllMsgsAsync'
function sendMessage(userName, message) {
    if (message.length != null) {
        console.log(userName + " - " + message);
        connection.invoke("SendAllMsgsAsync", userName, message); //sendAllMsgs
    }
}

//Metho displays message
function displayMessage(name, time, message) {
    //used of setting css class for design
    userType = "";
    switch (name) {
        case "Chat Hub":
            userType = "system";
            break;
        case userName:
            userType = "user";
            break;
        case "ERROR":
            userType = "error";
            break;
        default:
            userType = "members";
    }

    //checking time format using moment
    var outputTime = moment(time).format('H:mm:ss');

    var userLi = document.createElement('li');
    userLi.className = 'userLi list-group-item ' + userType;
    userLi.textContent = outputTime + ": " + name + " says:";

    var messageLi = document.createElement('li');
    messageLi.className = 'msgLi list-group-item';
    messageLi.textContent = message;

    var msgsArea = document.getElementById('msgsArea');
    msgsArea.appendChild(userLi);
    msgsArea.appendChild(messageLi);

    $('#msgsArea').animate({ scrollTop: $('#msgsArea').prop('scrollHeight') }, 50);
}

//function displays all older msgs
function displayPastMsgs(recordList)
{
    //going through whole list and priting activities
    for (let i = 0; i < recordList.length; i++)
    {
        var outputTime = moment(recordList[i].time).format('H:mm:ss');

        var userLi = document.createElement('li');
        userLi.className = 'userLi list-group-item oldRecords';
        userLi.textContent = outputTime + ": " + recordList[i].userName + " says:";

        var messageLi = document.createElement('li');
        messageLi.className = 'msgLi list-group-item';
        messageLi.textContent = recordList[i].associatedMessage;

        var msgsArea = document.getElementById('msgsArea');
        msgsArea.appendChild(userLi);
        msgsArea.appendChild(messageLi);

        $('#msgsArea').animate({ scrollTop: $('#msgsArea').prop('scrollHeight') }, 50);
    }
}