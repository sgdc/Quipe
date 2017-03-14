# Quipe
A card swipe mini-game that also records attendance.

## Usage
Quipe is a Unity game - booted with its normal .exe file. On boot, the game will search your desktop /Quipe folder for a file in the format "mm-dd-yyyy Quipe Log.txt"
If one exists, it will read all swipes in and populate the list. (This is done in case the program closes before your event is over)

On startup, define the name of your database. I recommend starting them with an underscore -- ie "_sgdc" Use this name when loading each time. (This lets you send the database from PC to PC, if desired. It is stored locally, not online.)

## Exporting leaderboard
Press the left control key at any time to export a leaderboard. If names haven't yet been changed, students' IDs will be displayed, which is not preferred.

## Changing names
There are two ways to change a user's name. First is to press Right CTRL, then have them swipe their ID. They can then type their name and press enter, which will overwrite their display name for log in and leaderboards.

Alternatively, pressing Numpad + searches for a file called names.txt in your /Quipe/ folder on your Desktop. This file is in the format: 
FirstName	LastName	ID
FirstName	LastName	ID
FirstName	LastName	ID

ie:
Adam Gincel 11122333
Alex Massenzio 99988777

Pressing Numpad + will iterate through that file, searching for users with the IDs on the right, and changing their name to the First and Last names given. Use Ducksync to export participation and format the file accordingly.

## Exporting Swipe information.
Pressing Caps Lock will generate a file in the format: "mm-dd-yyyy Quipe.ahk" This is an AutoHotKey script, which can be installed here: https://autohotkey.com/

Double click the generated ahk file (make sure no other scripts are running), navigate to the DuckSync page you'd normally log participation for, go to Participation, and press the "Turn On Card Swipe" button (despite not using a card swipe, don't worry)
Now press the "Home" key. You should see the first person who swiped marked as attended. Press the caps lock key to log the next person. Wait until you see one person logged before pressing capslock again. Repeat this until the swipes are all logged.

## Changing Level
Pressing Numpad Enter will change to the next level. Each level asks you to load from the database, so change to the appropriate level before entering the database name.

## Fun
Type 'b' and press Enter to drop a fake ball into the machine. These do not collide with other balls and have no effect on anybody's score. They're just fun.

