

# Planet Wars AI Competition

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

The number of turns it takes a fleet to travel from a source planet to a destionation planet is given by the ceiling of euclidean distance between 2 planets:
```javascript
 Math.ceil(Math.sqrt(
        Math.pow(destination.x - source.x, 2) + 
        Math.pow(destination.y - source.y, 2) ))
```

To colonize a planet, ships must be sent in fleets in order to outnumber the current player's ships on the planet (neutral or hostile). However, once a fleet of ships is sent to another planet it cannot be stopped until it reaches its destination. Fleets sent will battle enemy ships and cancel out one for one. The player with ships left after battling has successfully colonized the planet.

Once you a planet is colonized it will begin producing ships every turn based on the growth rate of the planet, which can they be used to colonize more planets.

### Building an Agent

The planet wars api is a restul JSON api, as long as the agent can speak http can participate. Perl, PHP, nodejs, Ruby, Python, C#, C/C++, Erlang, etc. are welcome.

### Submitting an Agent

Please include a zip file with your agent, source code, and a readme on how to compile/run your agent.

All agents must provide a command line argument to change the endpoint they are pointing at.

For example:
```bash
>runmyagent.exe -endpoint http://planet-wars-game.com 
```

### Running the Server
 
You may run the server locally by starting the "PlanetWars" project in visual studio. Once you have a local server, you can point your agent at the local endpoint.

### API


