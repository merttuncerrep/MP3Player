# MP3 Player
# Assumptions about the Application
## Data Storage and Access:
The application assumes that data is stored in a JSON file named AlbumList.json located in the root directory of the application.
The JSON file contains structured data for albums, including artist names, genres, and songs with their names and durations.
Data is loaded into memory at runtime and managed using in-memory lists (e.g., List<Album>).
No database or persistent storage beyond the JSON file is utilized.
## Data Integrity:
It is assumed that the JSON file is properly formatted and adheres to the expected schema.
Missing or malformed data in the JSON file may cause errors during loading.
## Playback Flow:
Only one song can be played at a time.
Playback state is tracked in-memory and is reset when the application restarts.
## Platform Assumptions:
The application is designed as a Command-Line Interface (CLI) to ensure simplicity and ease of use.
It is not dependent on a graphical user interface (GUI) or web technologies.
The CLI is cross-platform and can run on any system with the .NET runtime (e.g., macOS, Windows, Linux).
## Search Functionality:
The search feature looks for matches in album names, artist names, and song names.
Search results are case-insensitive and partial matches are supported.
## Shuffle and Repeat:
If shuffle is enabled, the next song is selected randomly from the current album.
If repeat is enabled, the current song restarts when it finishes.
# Platform Specification
## Rationale:
Lightweight, portable, and ideal for quick prototyping.
Suitable for users who prefer a straightforward textual interface.
Easier to debug and extend compared to a web application for this use case.

# Classes:
Program: The main entry point of the application that manages the user interface and overall program flow.
Song: Represents individual songs with attributes like name and duration.
Album: Represents an album containing metadata such as name, artist, genre, and a list of songs.
LibraryData: Handles deserialization of albums from a JSON data source.
MusicLibrary: Manages the collection of albums and provides search, display, and data retrieval functionalities.
Player: Handles playback operations, including play, pause, stop, next, previous, shuffle, and repeat functionalities.
# Methods:
# Program Class:
Main: Initializes the MusicLibrary and Player classes, runs the main menu loop, and handles user interactions.

## LibraryData Class:
FromJson: Converts a JSON string into a LibraryData object containing a list of albums.

## MusicLibrary Class:
LoadData: Reads album data from a JSON file and loads it into memory.
DisplayAlbums: Lists all available albums with their respective details.
GetAlbumByIndex: Retrieves a specific album from the library by its index.
GetAlbums: Returns the entire list of albums.
Search: Searches for albums, artists, or songs based on a user-provided query.

## Player Class:
Player: Initializes the player with a reference to the MusicLibrary.
SelectAndPlaySong: Allows the user to select an album and play a specific song.
CurrentlyPlayingSong: Displays details about the currently playing song, including elapsed time.
PlaybackControls: Provides options to play, pause, stop, skip to the next song, or return to the previous song.


# Explanation of the Design and Rationale
## Design Overview:
The application is built as a Command-Line Interface (CLI) to provide a lightweight, efficient, and easily testable user experience for managing and playing music.
The core design uses object-oriented principles, encapsulating data and functionality into cohesive classes (Program, MusicLibrary, Player, Album, and Song).
## Why CLI?
CLI applications are platform-independent, simple to implement, and well-suited for quick prototyping.
They allow for straightforward debugging and are easily extensible for future web or GUI implementations.
## Responsibilities and Modular Structure:
Separation of Concerns:
Program: Manages the application's main menu and flow.
MusicLibrary: Handles data storage, retrieval, and search functionality.
Player: Manages song playback controls, such as play, pause, stop, next, and previous.
Album and Song: Represent domain objects for structured data management.
This modular structure makes the code easy to maintain and extend. For example, new features like playlists or cloud sync can be added without disrupting existing functionalities.
## Why JSON for Data Storage?
JSON is lightweight, human-readable, and easy to serialize/deserialize.
It fits the use case for a static dataset of songs and albums and avoids the complexity of a database for this prototype.
## Playback and Controls:
The Player class handles playback states (playing, paused, stopped) and keeps track of the currently playing song, making it intuitive for the user.
Features like next and previous navigation in albums add to the user's control.
## Scalability Considerations:
While designed as a CLI prototype, the modular architecture ensures that components like MusicLibrary and Player can easily be adapted for a GUI or web interface.
Features like shuffle and repeat can be added seamlessly within the existing design.
This design prioritizes simplicity, ease of use, and modularity, making it an excellent starting point for an MP3 player application. 


