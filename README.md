HockeyStatsExtractor
====================
Stats Extractor For Hockey.

Current Build: 0.4.0
--------------------
Beta build 0.4.x (Second public release version)

-> Added OBS overlay workings.

-> Fixed bug where the game would not end after a goal was scored in OT.

--------------------
Beta build 0.3.x (Somewhat of a release candidate)

Update March 29 2014 (0.3.1):

Quickfix on problem with goals and assists printing correctly.

Update March 28 2014 (0.3.0):

The refactor and the PlayerHolder/Player disambiguation have been successfully completed. The code is mostly bug free. The week that follows should be a testing week that will be necessary to guarantee that the code is accurate and reliable for the league to depend upon it.

If everything functions according to plan, it should be released in time for use in Week 8.

--------------------
Alpha Build 0.2

Update March 12 2014 (0.2): A high number of bugs are caused by the fact that the hockey.exe application overwrites player data when a player leaves the server and another one joins. This leads to crashes when the part that handles the +/- tries to access a player whose name has been changed.

A partial rewrite is in order to have more maintaiable code and tone down the level of spaghetti-code and fatal bugs.

Estimated time remaining: 2 weeks

