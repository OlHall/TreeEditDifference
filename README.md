# Tree Edit Difference
 C# Implementation of the Zhang Shasha tree edit difference algorithm

## But... it's broken

 I've tried to follow the algorithm reasonably faithfully, allowing for C# language features that might make the code easier to read and so on.
 
 The trees can take any node type, with my real-world goal being to implement an XML document TED tool, and the trees are processed into post-order before the TED computation.  The unit tests implenent a simple single character node label, input parser, node comparison and unit cost calculation, so this all works with the sample data floating around the Internet.
 
 However - Although the algorithm passes for the trees given in the Zhang-Shasha paper (and generates all of the correct intermediate forest distance tables), it fails on other trees.
 
 Specifically this fairly simple tree:
 
 ```
 T1:     T2:
   e       e
  / \     / \
 f   g   f   g
     |       |
     i       h
 ```
 
 The edit distance here is clearly 1, yet my implementation produces 2.
 
