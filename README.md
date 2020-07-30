![Develop Branch Workflow](https://github.com/altunsercan/Kegstand/workflows/Develop%20Branch%20Workflow/badge.svg?branch=develop) [![Maintainability](https://api.codeclimate.com/v1/badges/7db3ee33d8eca1babb18/maintainability)](https://codeclimate.com/github/altunsercan/Kegstand/maintainability) [![codecov](https://codecov.io/gh/altunsercan/Kegstand/branch/develop/graph/badge.svg)](https://codecov.io/gh/altunsercan/Kegstand)

# Kegstand 
Discrete Event Simulator for Games

This repositiory is work in progress and is not recommended for use.

# What is this about?
You remember the math problems about filling/emptying pools you likely solved in highschool. Now imagine these pools are Kegs. And they have taps that flow in/out juices. Now imagine there buoys that trigger alerts when juice levels pass certain tresholds. Now imagine little oompa loompa's tasked to open/close taps on other kegs based on events. Boom! Now you have time based logic system that can simulate discrete-time events.

Discrete-Event Simulation is a method used to quickly simulate time depedent processes with interconnected entities. In industries they are used to model real life stuff like a factory floor and run simulations. But it is also useful for some management games where you can fast forward time but keep accurate simulation. Or have an online game where when you login, the game simulates what happened when you are away, like an incremental game.

This library is created to provide a game library in Unity Package Manager (UPM) format to use Keg / Tap metaphor to build Discrete Event Simulators.
 
