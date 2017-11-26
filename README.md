# tpastar

_Triangulated Polygon A-star_ is a pathfinder, which is able to determine the shortest path between one start and multiple goal points in a triangulated polygon with polygon-holes.

<p align="center"><img src="./Documentation/exploration-one-start-multiple-goals-cropped.png" alt="The result of an exploration between one start and multiple goals" /></p>  

The image shows the result of a pathfinding between a start and three goal points. The shade of the triangles indicates how many times the triangle was traversed. In this case white means zero, grey means once.

## Licensing

This repository contains the implementation of the algorithm in _C#_. It ships with a demo application that can be used to test it under various arrangements. The entire codebase is licensed under [![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0).

## Acknowledgements
- _István Engedy_, who was my thesis advisor at BME
- _Douglas Demyen_, whose master's thesis drove me this direction
- _B. Chazelle, D. T. Lee and F. P. Preparata_ for the _Funnel algorithm_, which is the basis of this solution
- _Richard Potter_, whose _Vector3_ library was used almost entirely during development
- _Ákos Pfeff_ for his remarks during the release

---
Copyright 2017 Márton Gergó
