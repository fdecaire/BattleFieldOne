BattleFieldOne
==============

Battle Field One Army Simulation Game

This is a game that I am using to demonstrte various game algorithms in my blog: frankdecaire.blogspot.com.  This 
game is based on the generic hex-grid board game.  You may download the entire source code and make it into your own game
if you want.  I will be adding to this code in future versions as I write more articles on my blog.

Game Design

The basic game is an MVC web application using SVG for graphical output.  The game is turn-based and it consists of the 
old-fashion movement then attack phase for each player.  The current game only has two players, the Allied units are the
human (as in you) player and the German or Axis player is the computer.  When the game is started most of the map is 
obscured and you cannot see the terrain until you send a unit out to scout.  After the terrain has been revealed, then the 
terrain is always shown.  There is a secondary mask that is somewhat transparent that shows what your units cannot currently 
see.  If any enemy units are under this area, they will not be shown.  You can only see enemy units that are within visual
range of your units.  This techinique is similar to the game Civilization (although their masking is more sophisticated).
Cities are at fixed locations on the map and there is no way to add or remove a city.  I have added a mountain cell to
give the map an unpassible terrain type.  You can modify the game initialization to include mountains to block units from
going straight to a target.

Game Goals (to win the game)

* Destroy all enemey units.
* Capture all cities.
