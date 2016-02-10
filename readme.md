

# Planet Wars AI Competition

![Planet Wars](https://cloud.githubusercontent.com/assets/563819/12822156/4062f4c0-cb2c-11e5-9672-7d94c6338970.gif)

### Game Overview

In this game you will build and AI to compete to colonize all of the available planets or destroy all enemy ships. On each turn, a player may send a fleets of ships from planets they control to other planets. 

Some planets are closer, others are further away. Some planets produce more ships, some produce less ships. Some planets are more costly to colonize, some are less. It is up to you to decide which planets to colonize first.

#### Winning the game

The game is over when:

* All of the planets have been colonized by a player
* All of you opponents ships have been destroyed
* The max number of turns is reached player with the most ships wins

The game is a draw when:

* Both players run out of ships, or planets at the same time

#### Colonizing, Planets, and Fleets

Planets are laid out on a 2D grid. Each planet has an X and Y coordinate, an ID number, growth rate, and amount of ships required to colonize from the neutral player.

The number of turns it takes a fleet to travel from a source planet to a destination planet is given by the ceiling of euclidean distance between 2 planets:
```javascript
 Math.ceil(Math.sqrt(
        Math.pow(destination.x - source.x, 2) + 
        Math.pow(destination.y - source.y, 2) ))
```

To colonize a planet, ships must be sent in fleets in order to outnumber the current player's ships on the planet (neutral or hostile). However, once a fleet of ships is sent to another planet it cannot be stopped until it reaches its destination. Fleets sent will battle enemy ships and cancel out one for one. The player with ships left after battling has successfully colonized the planet.

Once you a planet is colonized it will begin producing ships every turn based on the growth rate of the planet, which can they be used to colonize more planets.

#### Simulation Phases In Order

1. Departure - Agent commands are carried out. New fleets are created and the appropriate number of ships are removed from each planet. The fleet will arrive in X number of turns according to the formula above.

2. Advancement - Fleets advance through space and `NumberOfTurnsToDestination` decrements by 1. Planets owned by players will increase their `NumberOfShips` by the growth rate of the planet.

3. Arrival - Fleets that have a zero `NumberOfTurnsToDestination` arrive at destination planets and combat is resolved if necessary. The largest force will win with the number of ships equal to the largest force minus the second largest force. For example if player 1 has 5 ships arriving, player 2 has 4 ships arriving, and neutral occupies with 3 ships; player 1 will win and have a remaining force of 1 ship.
 

### Building an Agent

The planet wars api is a restul JSON api, as long as the agent can speak http can participate. Perl, PHP, nodejs, Ruby, Python, C#, C/C++, Erlang, etc. are welcome.

#### Restful Server API

An agent can be built using the following api methods, any program that can speak http will work.

POST api/logon - This endpoint logs the agent into the server and provides the authentication token, game id, and start time of the game.

Request
```json
{
  "AgentName": "MyCoolAgent"
}
```

Response
```json
{
  "AuthToken": "sometoken",
  "Id": 1,
  "GameId": 45,
  "GameStart": "someDateTime"
}
```

POST api/move - This endpoint allows agents to move fleets to colonize other planets

Request
```json
{
  "AuthToken": "mytoken",
  "GameId": 45,
  "DestinationPlanetId": 1,
  "SourcePlanetId": 1,
  "NumberOfShips": 30
}
```

Response
```json
{
  "Fleet": {
   "Id": 22,
   "OwnerId": 1,
   "NumberOfShips": 30,
   "DestinationPlanetId": 1,
   "SourcePlanetId": 1
  }
}
```

POST api/status - This returns the current status of the game, planets, fleets, etc. This powers the visualization.

Request
```json
{
  "GameId": 45
}
```

Response
```json
{
  "IsGameOver": false,
  "Status": "if over winner string",
  "PlayerA": 1,
  "PlayerB": 2,
  "PlayerAScore": 1,
  "PlayerBScore": 2,
  "PlayerAScoreOverTime": [],
  "PlayerBScoreOverTime": [],
  "CurrentTurn": 30,
  "EndOfCurrentTurn": "somedatetime",
  "NextTurnStart": "somedatetime",
  "PlayerTurnLength": 600, /* in ms */
  "ServerTurnLength": 200, /* in ms */
  "Planets" : [],
  "Fleets" : []
}
```

### Submitting an Agent

Please include a zip file with your agent, source code, and a readme on how to compile/run your agent.

All agents must provide a command line argument to change the endpoint they are pointing at.

For example:
```bash
>runmyagent.exe -endpoint http://my-planet-wars-game.com:1337
```

### Getting Started Locally Running the Server
 
First clone the repository locally using git ([GitHub for Windows](http://windows.github.com) is good)

```bash
git clone https://github.com/eonarheim/planet-wars-competition.git`
```

1. Open the solution in VS 2015 ([Community Edition](https://www.visualstudio.com/en-us/products/vs-2015-product-editions.aspx) is free)
2. Ensure you have [.NET Framework 4.6](https://www.microsoft.com/en-us/download/details.aspx?id=48130) installed
3. Ensure you have at least [TypeScript 1.6.4 installed](http://typescriptlang.org)

You may run the server locally by starting the "PlanetWars" web project in Visual Studio. Once you have a local server, you can point your agent at the local endpoint.

There is a prebuilt agent you can build off of :)

## FAQ

1. What happens on a planet you already own when there are both friendly and enemy units incoming? Do the planet's units and the incoming friendly units get combined when resolving the conflict? Or is all combat on a 1-1 basis?

   - Ships cancel each other out. i.e. two players both have 5 ships arriving same turn, 0 ships land. Units you control on a planet are taken into account when enemy units try to land but if you have a fleet that arrives the same turn, combat resolves between the fleets.

2. If I send ships to a planet I own and assuming there are no enemy ship incoming do they get added to the ship total on that planet? 

   - Yes, ships that arrive at a planet add to the total **if you control it** otherwise it will **decrement it** if it's under another player's control.

3. If multiple fleets from a player (me or enemy) arrive on the same turn are the ship totals from the fleets treated as a combined total or are they treated as separate entities when resolving combat?

   - It will take the sum total of all arriving fleets *per player* that turn and resolve combat.
