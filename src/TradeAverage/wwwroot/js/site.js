const uri = 'Trade';
function getAverageData() {
    fetch(uri)
        .then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => console.error('Unable to get items.', error));
}

function updateAverageData(data) {
   
    fetch(uri, {
        method: "POST",
        mode: "cors", 
        headers: {
            "accept": "text/plain",
            "Content-Type": "application/json",
        },
        body: JSON.stringify(data)
    })
        .then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => console.error('Unable to get items.', error));
}

function _displayItems(data) {
    const tBody = document.getElementById('Trade');
    tBody.innerHTML = '';  
        
        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        let textNode1 = document.createTextNode(data.averageNumberPerMinute);
        td1.appendChild(textNode1);


        let td2 = tr.insertCell(1);
        let textNode2 = document.createTextNode(data.averageVolumePerMinute);
        td2.appendChild(textNode2); 
}

const websocket = new WebSocket("wss://websockets.independentreserve.com/?subscribe=ticker-xbt");

websocket.onopen = function (event) {
    console.log("Connected!");
    websocket.send("Hello, WebSocket server!");
};

websocket.onmessage = function (event) {
    var data = JSON.parse(event.data);
    if (data.Event == "Trade")
    {
        console.log("new trade was added");       
        updateAverageData(data);
    }
};

websocket.onerror = function (event) {
    console.error("Error:", event);
};

websocket.onclose = function (event) {
    console.log("Connection closed");
};