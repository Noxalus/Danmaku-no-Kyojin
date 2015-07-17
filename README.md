# Danmaku no Kyojin

![Danmaku no Kyojin](http://danmakunokyojin.free.fr/images/screen.jpg)

Danmaku no Kyojin (弾幕の巨人) is a 2D danmaku developed in C# with XNA.

## Project ideas

* An arena shooter with danmaku like bullet patterns 
* Boss wave only (no "Campaign", just a survival mode exclusively composed of boss increasingly large)
* 4 difficulty modes (Easy, Normal, Hard, Impossible)
* Control a ship with keyboard (moving + slow mode) and mouse (shoot + bullet time) or with a gamepad
* Boss creation using procedural generation
* Local and online multiplayer (cooperation with scoring race)
* Bullet time mode (time is slowed except for player(s))
* Simple sprites (geometric) but heavy use of shaders (kikoo effects)
* Bounded area but 2D camera (game area is not limited to the screen size)
* Scriptable bullet patterns (BulletML)
* Steam publication forecast

## Inspiration

### Touhou Project

For its danmaku/bullet hell aspect (complex bullet patterns) and its incredible difficulty.

<p align="center">
  <img src="https://dl.dropboxusercontent.com/u/63123790/screenshots/Danmaku-no-Kyojin/touhou.jpg" alt="Touhou Project" />
</p>

### Warning Forever

For its idea of figthing a single enemy that become stronger each time we beat it.

<p align="center">
  <img src="https://dl.dropboxusercontent.com/u/63123790/screenshots/Danmaku-no-Kyojin/warning_forever.jpg" alt="Warning Forever" />
</p>

### Geometry Wars

For its bloomy visual and particle effects.

<p align="center">
  <img src="https://dl.dropboxusercontent.com/u/63123790/screenshots/Danmaku-no-Kyojin/geometry_wars.jpg" alt="Geometry Wars" />
</p>

## Current state

* The player controls a ship and can shoot multiple bullets
* Boss procedural generation works and its size can grow at any time
* Bosses can be splitted in 2 parts and its total life depends on the number of its remaining portion
* Simple particle engine
* Bloom effect

## TODO List

* Port the game from XNA 4.0 to MonoGame 3.4
* Add random set of turrets to each boss
    * Basic turret: fires a simple bullet to player's position
    * Shootgun turret: fires multiple bullets to player's position
    * Laser turret: fires a laser to player's position
    * Homing turret: fires homing missiles 
* When the boss splits in 2 parts, the part that comes off must act independently
* Add an outline effect around boss shape
* Bosses are static, add some random movments
* The camera's motion is not optimal to avoid bullets
    * Tweak it to have something similar to Geometry Wars one
    * Add a focus button that slow down the player's speed and zoom on player's target
* Add Steam API support
* Online multiplayer (coop)

## Videos 

* https://www.youtube.com/watch?v=mgHTQp9HcFc&list=PLnlMaeuxrONy0xSr0SApVxfRYJriJkex0 (complete playlist)
* https://www.youtube.com/watch?v=mgHTQp9HcFc (old trailer)
* https://www.youtube.com/watch?v=O0Nfy-iFGIw (bullet patterns test with colors)
* https://www.youtube.com/watch?v=iaeV2USSr8c (boss triangulation experimentation #1)
* https://www.youtube.com/watch?v=39kUZooj2sE (boss triangulation experimentation #2)
* https://www.youtube.com/watch?v=I6HTUhUgdI8 (boss triangulation experimentation #3)
* https://www.youtube.com/watch?v=vcpW3cF-qfQ (boss triangulation experimentation #4)
* https://www.youtube.com/watch?v=bN9937blvjw (concave polygon collision test with convex polygon subdivision)
* https://www.youtube.com/watch?v=2Z664BJ_cxQ (boss split + bloom effect + particles)
* https://www.youtube.com/watch?v=lWXfODTq7Yc (boss split into independent parts)
* https://www.youtube.com/watch?v=oRGans5XNNk (boss part explosion random inertia + bounding boxes generation)

## Additional information
* [Web site](http://danmakunokyojin.free.fr/)
* [Dev blog](http://danmakunokyojin.blogspot.fr/)
* [Dev journal](http://www.gamedev.net/blog/2077-danmaku-no-kyojin/)
* [Trello](https://trello.com/b/8FIBQSBT/danmaku-no-kyojin)
* [Youtube](https://www.youtube.com/channel/UC-75NW0MoGzjQcR0gajx5Eg)
* [Twitter](https://twitter.com/Noxalus)
* [Live coding](https://www.livecoding.tv/noxalus/)
* [OpenHub](https://www.openhub.net/p/Danmaku-no-Kyojin)
