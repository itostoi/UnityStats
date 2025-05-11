# Top
1. Implement weapons/attacks
- We might need to communicate between weapon/stat manager
- Can/should we break up things like attack, hp, other stats?

2. "Trace" trigger calls - Anything that's circular 
will cause a crash.

Idea: Every trigger needs to keep a set of callers,
and we need to detect cycles.