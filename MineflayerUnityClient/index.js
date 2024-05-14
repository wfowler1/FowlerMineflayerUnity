const mineflayer = require('mineflayer');
const webSocket = require('ws');



let bot;
let websocket;
let ready = false;

const wss = new webSocket.Server( { port: 3000 }, () => {
    console.log('server started');
    SetUpServerEvents();
});

function InstantiateBot()
{
    bot = mineflayer.createBot({
        host: 'localhost',
        port: 37269,
        username: 'UnityBot'
    });

    SetUpBotEvents();
}

function DestroyBot()
{
    bot.quit();
}

function SetUpServerEvents()
{
    wss.on('listening', () => {
        console.log('server listening on port 3000');
    })
    
    wss.once('connection', (ws) => {
        websocket = ws;
        console.log('client connected on port 3000');
        InstantiateBot();
    
        websocket.on('message', OnMessage);
    
        websocket.on('disconnect', () => {
            console.log('client disconnected');
            DestroyBot();
        });
    })
}

function SetUpBotEvents()
{
    bot.on('spawn', () => { ready = true; });
    bot.on('chat', OnChat);
    
    bot.on('kicked', console.log);
    bot.on('error', console.log);
    
    bot.on('move', OnBotMoved);
    bot.on('physicsTick', OnTimeOfDayChanged);
    bot.on('physicsTick', SendEntities);
}

function OnChat(username, text)
{
    console.log('<' + username + '> ' + text);
    let obj = {
        'botNumber': -1,
        'type': 'chat',
        'message': '<' + username + '> ' + text
    };
    websocket.send(JSON.stringify(obj));
}

function OnBotMoved()
{
    let obj = {
        'botNumber': 0,
        'type': 'entityupdate',
        'message': Entity2JSON(bot.entity)
    };
    websocket.send(JSON.stringify(obj));
}

function OnTimeOfDayChanged()
{
    let obj = {
        'botNumber': -1,
        'type': 'time',
        'message': bot.time.timeOfDay
    };
    websocket.send(JSON.stringify(obj));
}

function SendEntities()
{
    const playerFilter = (entity) => entity.type === 'player';
    const nearestPlayer = bot.nearestEntity(playerFilter);

    if(!nearestPlayer)
    {
        return;
    }
    
    let obj = {
        'botNumber': -1,
        'type': 'entityupdate',
        'message': Entity2JSON(nearestPlayer)
    };
    websocket.send(JSON.stringify(obj));
}

function OnMessage(data)
{
    //console.log('Client sent message: ' + data);

    if (!ready)
    {
        return;
    }

    var message = JSON.parse(data);

    if (bot)
    {
        if (message.type === 'chat')
        {
            bot.chat(message.message)
        }
        else if (message.type === 'control')
        {
            const strings = message.message.split(':');
            bot.setControlState(strings[0], strings[1].charAt(0) == 'T');
        }
        else if (message.type === 'look')
        {
            if (bot.entity.yaw !== undefined)
            {
                const strings = message.message.split(',');
                bot.look(parseFloat(strings[1]), parseFloat(strings[0]), true);
            }
        }
    }
}

function Entity2JSON(entity)
{
    let name = '';
    if (entity.type === 'player')
    {
        name = entity.username;
    }
    else
    {
        name = entity.kind;
    }
    return '{\"id\":' + entity.id + ',\"name\":\"' + name + '\",\"type\":\"' + entity.type + '\",\"position\":{\"x\":' + entity.position.x + ',\"y\":' + entity.position.y + ',\"z\":' + entity.position.z + '},\"pitch\":' + entity.pitch + ',\"yaw\":' + entity.yaw + '}';
}

