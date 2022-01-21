# MobileGridGame

**Note for devs**

This app was based on an Visual Studio template for MVVM Xamarin apps. The app is now however definitely not an MVVM app, and does not show best practices at all for building Xamarin apps. It exists at the moment only to explore a variety if interaction models when playing this specific game. Depending on whether the game generates any interests, the code might be updated to have a better MVVM design, but until then, devs should only consider looking at this code if they're interested in specific accessibility-related topics.

**Goals**

The goals of this app are to (1) demonstrate some considerations and implementation relating to a simple, accessible Xamarin game app, and (2) make an enjoyable game available for everyone.

This exploration into accessibility is a continuation of the exploration started with [How a Card Matching game is compliant with international accessibility standards](https://www.linkedin.com/pulse/how-card-matching-game-compliant-international-standards-guy-barker).

&nbsp;

The game is based on a square sliding game, where squares in a grid of squares are rearranged by the player to form an ordered sequence in the grid.

When the game is run, a 4x4 grid of squares appears, with 15 of those squares occupied with an movable element, and 1 square being empty. When a square is tapped, if it is adjacent to the empty square, the tapped square moves into the empty square. The space where the tapped square was then becomes the empty square. The aim is to arrange all the squares in a sorted order, leaving the empty square in the bottom right corner of the grid.

If the game setting to have numbers shown on the square is on, then a number is shown in the top left corner of the squares. The sorted order is for the numbers to start at 1, and increases from left to right and top to bottom. 

If the game setting to have a picture shown on the squares is on, then a single picture is shown across all the squares. The sections of the picture will appear jumbled until the squares are in their sorted order.

&nbsp;

**Ways to play the game**

***Tapping with a finger***: Tap the square to be moved.

***Voice Access***: Speak the number shown on a square to move the square.

***Switch Access***: Scan to the square to be moved, then press the switch to move the square.

***TalkBack***: Move your finger around the screen to have the number of the square beneath your finger announced. If TalkBack moves between rows or columns in the grid of squares, it will announce the new row or column. Double tap to move a square. TalkBack will make various announcements as actions are taking in the game.

***Text size***: The game respects the system "Font size" and "Display size" settings, and also has its own text size setting which only applies to text in shown on the squares in the game.

***Magnification***: The standard use of magnification can be used when playing this game.

&nbsp;

**Technical Considerations**

***Grid Control***: An early consideration in build this app was which Xamarin control would be the best match for the grid of Squares. While the WinForms version of this game uses a DataGridView, the best match for this Xamarin app seemed to be the CollectionView control. It seemed relatively straightforward to present a 4x4 grid of items in the CollectionView, and resize the items to fill the screen regardless of screen size or orientation. If also seemed practical to resize the contents of the items to fill whatever portion of the item is required.

***Reacting to input***: When development on the app began, it was assumed that the first version of the app would support keyboard use. The simplest way to support keyboard use seemed to be through the SelectionChanged event on the CollectionView. So this approach was taken to support both keyboard use and touch use, despite the suggestion at []() that a tap gesture might be more straightforward way to support touch input. However, testing indicated that tap gesture support would be required to support Voice Access, and as such, the SelectionChanged approach was replaced with the tap gesture approach. However, testing then indicated the teh SelectionChanged support would be required to support Swith Access. The end result is that the app reacts to both SelectionChanged and tap gestures.

***Showing portions of the picture on each square***: TBD

***Size of Text***: As part of this exploration into accessibility, there is no explicit sizing of text or containers anywhere in the code. The height of text shown in the grid squares is some fraction of the height of the grid row containing the square. The height of text shown in the Settings window is set by the system, and the window uses a ScrollView to ensure all content can be reached when text is shown as its maximum size. As a result, all text is viewable, even when the game's in-app text size settings, and the system's Font size and Display size settings are all at their maximum values.

***Use of Colour***: The app uses the same colours that came with the template used to create the app. At some point work may be done to support a Dark theme in the app.

***Use of Timers***: No timers are used in the app. If timers were used, the timeout periods much be in full control of the player.

***TalkBack***: The Xamarin AutomationProperties class is used to set explicit accessible names to the squares. If the app ever ships in a non-English region, the accessible names must be localized. The app can easily support having accessible descriptions set on the squares too, and so this will be done if feedback suggest that would be helpful to players. 

Android platform-specific code is used to raise events for TalkBack to react to an make the following announcements:

"Please wait a moment while the pictures are loaded into the squares."
A countdown as the pictures is loaded into the squares.
"Game is ready to play."
"Moved ***Square number*** ***Direction of move***."
"A move is not possible from here."

***Keyboard Use***: Today the game does not support keyboard use. If keyboard use is supported in the future, it will involve the CollectionView reacting to SelectionChanged events as the Space key is pressed when an item has keyboard focus. It will also require keyboard focus visual feedback to be much stronger on the squares. 


&nbsp;

**Future Udpates**

Future updates to the app may include the following:

1. Support a dark theme in the app.

2. Support a swipe gesture to move the squares in the game. It's expected that this will require changes to prevent the CollectionView managing swipe gestures made over the view.

3. Support keyboard use and stronger keyboard focus feedback.

4. Timeout an attempt to load a picture if it's taking too long.

5. Add an Android version of the Windows Squares game.
