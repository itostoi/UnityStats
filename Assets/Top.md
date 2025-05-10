# Top
1. Rename statuses to effects - Done
2. Implement Items - Done
- An item is basically a set of permanent statuses.
3. Implement Item slots/ownership. 
- Item Owner: gameobject with the stats script - Done
- Item Slot: gameobject with slot script - Done
- Slot script: Basically just holds a reference the slot's owner. - Done

- Test item 1: Increase hp by 10 - Done
- Test item 2: Increase crit rate by 50% - TODO

4. Implement weapons/attacks
- We might need to communicate between weapon/stat manager
- Can/should we break up things like attack, hp, other stats?