# tpastar

Triangulated Polygon A-star is a pathfinder, which is able to determine the shortest path between one start and multiple goal points in a triangulated polygon that contains polygon-holes.

![The result of an exploration between one start and multiple goals][cover_image]

The image shows the result of a pathfinding between one start and three goal points. The shade of the triangles indicates how many times the triangle was traversed: white-zero, grey-once.

This repository contains the implementation of the algorithm in C#. Using the demo application it can be tested under various arrangements. Licensed under Apache 2.0.

Acknowledgements:
- István Engedy, my thesis advisor at BME
- Douglas Demyen, whose master's thesis drove me this direction
- B. Chazelle, D. T. Lee and F. P. Preparata for the Funnel algorithm, which is the basis of this solution
- Richard Potter, whose Vector3 library was used almost entirely during development
- Ákos Pfeff for his remarks during the release

[cover_image]: ./exploration-one-start-multiple-goals-cropped.png
