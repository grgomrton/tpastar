# tpastar

_Triangulated Polygon A-star_ is a pathfinder, which is able to determine the shortest path between one start and multiple goal points in a triangulated polygon with polygon holes.

<p align="center"><img src="./Documentation/exploration-one-start-multiple-goals-cropped.png" alt="The result of an exploration between one start and multiple goals" /></p>  

The image shows the result of a pathfinding between one start and three goal points. The shade of the triangles indicate how many times the given triangle was expanded. In this case white means zero, grey means once.

## Licensing

This repository contains the implementation of the algorithm in C#. It ships with a demo application, which can be used to test it in various arrangements. The entire codebase is licensed under [![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

## Acknowledgements
- _István Engedy_, my thesis advisor, without whose support I couldn't have pulled this through
- _Douglas Demyen_, whose master's thesis guided me this direction
- The _creators_ of the _Funnel algorithm_, which is the basis of this solution
- _Richard Potter_, whose _Vector3_ library was used almost entirely during development
- _Ákos Pfeff_ for his remarks during refactoring

---
Copyright 2017 Márton Gergó
