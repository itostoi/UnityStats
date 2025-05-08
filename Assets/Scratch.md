# Wishlist
1) A guy who has health and can receive statuses
2) He has additional stats, such as attack, and defense
3) He can be affected by statuses
4) For example, 
- heal 10% hp per second
- take 20% more damage
- Decaying buff.
- etc. etc.
- Can we make this generic? 
- Eg. effects & duration can be separated, how much separation can we do...
- Like, can the user specify, on death, split into two, vs. on death, explode.
5) What can statuses do? Examples:
- On death, explode
- Stacking (eg. poison/bleed)
- Etc.
6) What can things to do statuses?
- Check if status exists (run code accordingly)
- Cleanse
7) Come up with some fun puzzles/interactions with it.
- Avoid circularity?

For example, damage calc:
Incoming damage is first passed to statuses, 
Then affected by resistance stat.
Then hp is changed.
    This can then proc additonal statuses lol.
    May need bypasses or some system.