# MobileGridGame

**Goals**

The goals of this app are to (1) demonstrate some considerations and implementation relating to a simple, accessible Xamarin game app, and (2) make an enjoyable game available for everyone.

This exploration into accessibility is a continuation of the exploration started with [How a Card Matching game is compliant with international accessibility standards](https://www.linkedin.com/pulse/how-card-matching-game-compliant-international-standards-guy-barker).

&nbsp;

**Playing the game**

The game is based on a square sliding game, where squares in a grid of squares are rearranged by the player to form an ordered sequence in the grid.

When the game is run, a 4x4 grid of squares appears, with 15 of those squares occupied with an movable element, and 1 square being empty. When a square is clicked, if it is adjacent to the empty square, the clicked square moves into the empty square. The space where the clicked square was then becomes the empty square. The aim is to arrange all the squares in a sorted order, leaving the empty square in the bottom right corner of the grid.

If the game setting to have numbers shown on the square is on, then a number is shown in the top left corner of the squares. The sorted order is for the numbers to start at 1, and increase from let to right and top to bottom. 



&nbsp;

**Accessibility Considerations**

**Size of Text**

**Use of Colour**

**TalkBack**

**Use of Timers**

**Keyboard Use**

**Speech input**

**Magnification**

**Support for portrait and landscape orientations**

**Switch Access**


&nbsp;

**Future Udpates**

1. Support a dark theme in the app.
2. Support a swipe gesture to move the squares in the game.
3. Consider improving the contrast of keyboard focus feedback, and supporting a press of F5 to restart the game.
4. Prevent access to the Flyout menu items while a picture is loading.
5. Timeout an attempt to load a picture if it's taking too long.
6. Add an Android version of the Windows Squares game.