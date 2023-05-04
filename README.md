# Tree Edit Difference
 C# Implementation of the Zhang Shasha tree edit difference algorithm
 Special thanks to Tim Henderson for his Python implementation  
 https://github.com/timtadh/zhang-shasha

 After a great deal of searching, head scratching and failed tests, this code - and notes pointing to other papers providing further analysis of Zhang-Shasha - was what allowed me to fix the bugs I'm aware of.

### Known Issues (Work In Progress)
* The edit operations are reporting a change at the root which shouldn't be there
* The XmlEditOperation printer is outputting the entire tree rather than just the node - that should be an easy one!
