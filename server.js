var express = require('express');
var app = express();

app.use('/js', express.static(__dirname + '/js'));
app.use('/templates', express.static(__dirname + '/templates'));
app.use('/css', express.static(__dirname + '/css'));

app.get('/', function(req, res){
	res.sendFile(__dirname + '/index.html');
});

var port = 3000;
app.listen(port, function(){
	console.log('Server listening at %s', port);
});