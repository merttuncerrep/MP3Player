using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MP3PlayerCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize the music library and load data from a JSON file
            var musicLibrary = new MusicLibrary();
            musicLibrary.LoadData("AlbumList.json");

            // Create a player object to manage playback
            var player = new Player(musicLibrary);
            bool running = true;

            // Main application loop
            while (running)
            {
                Console.WriteLine("\nMP3 Player CLI");
                Console.WriteLine("1. Display Albums");
                Console.WriteLine("2. Play a Song");
                Console.WriteLine("3. Playback Controls");
                Console.WriteLine("4. Currently Playing Song");
                Console.WriteLine("5. Search");
                Console.WriteLine("6. Exit");

                Console.Write("Select an option: ");
                var choice = Console.ReadLine();

                // Handle user input for different functionalities
                switch (choice)
                {
                    case "1":
                        musicLibrary.DisplayAlbumsAndSongs();
                        break;
                    case "2":
                        player.SelectAndPlaySong();
                        break;
                    case "3":
                        player.PlaybackControls();
                        break;
                    case "4":
                        player.CurrentlyPlayingSong();
                        break;
                    case "5":
                        musicLibrary.Search();
                        break;
                    case "6":
                        running = false; // Exit the application
                        break;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }
    }

    public class Song
    {
        public string Name { get; set; } // Name of the song
        public string Duration { get; set; } // Duration of the song in MM:SS format
    }

    public class Album
    {
        public string Name { get; set; } // Name of the album
        public string Artist { get; set; } // Artist name
        public string Genre { get; set; } // Genre of the album
        public List<Song> Songs { get; set; } = new List<Song>(); // List of songs in the album
    }

    public class LibraryData
    {
        public List<Album> Albums { get; set; } // Collection of albums

        // Deserialize JSON data into LibraryData object
        public static LibraryData FromJson(string json)
        {
            var albumsData = JsonConvert.DeserializeObject<List<Album>>(json);

            return new LibraryData
            {
                Albums = albumsData
            };
        }
    }

    public class MusicLibrary
    {
        private List<Album> albums = new List<Album>(); // Internal storage of albums

        // Load album data from a JSON file
        public void LoadData(string filePath)
        {
            try
            {
                var jsonData = File.ReadAllText(filePath);
                var libraryData = LibraryData.FromJson(jsonData);

                albums = libraryData.Albums;

                Console.WriteLine("Music library loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading music library: {ex.Message}");
            }
        }

        // Display all albums in the library and allow displaying songs
        public void DisplayAlbumsAndSongs()
        {
            for (int i = 0; i < albums.Count; i++)
            {
                var album = albums[i];
                Console.WriteLine($"{i + 1}. {album.Name} by {album.Artist} ({album.Genre})");
            }

            Console.Write("Enter album number to view songs (or 0 to return): ");
            if (int.TryParse(Console.ReadLine(), out int albumIndex) && albumIndex != 0)
            {
                albumIndex -= 1; // Convert to 0-based index
                if (albumIndex >= 0 && albumIndex < albums.Count)
                {
                    var album = albums[albumIndex];
                    Console.WriteLine($"\nSongs in {album.Name}:");
                    for (int i = 0; i < album.Songs.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {album.Songs[i].Name} ({album.Songs[i].Duration})");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid album selection.");
                }
            }
        }

        // Get a specific album by its index
        public Album GetAlbumByIndex(int index)
        {
            if (index < 0 || index >= albums.Count)
            {
                throw new ArgumentException("Invalid album index.");
            } 
            return albums[index];
        }

        // Retrieve the list of all albums
        public List<Album> GetAlbums()
        {
            return albums;
        }

        // Search for songs, albums, or artists
        public void Search()
        {
            Console.WriteLine("Enter search query (artist, album, or song): ");
            var query = Console.ReadLine()?.ToLower();

            if (string.IsNullOrWhiteSpace(query))
            {
                Console.WriteLine("Search query cannot be empty.");
                return;
            }

            Console.WriteLine("Search Results:");

            var matchingAlbums = albums.Where(a => 
                (a.Name?.ToLower().Contains(query) ?? false) || 
                (a.Artist?.ToLower().Contains(query) ?? false));

            foreach (var album in matchingAlbums)
            {
                Console.WriteLine($"Album: {album.Name} by {album.Artist}");
            }

            // Search for songs matching the query
            foreach (var album in albums)
            {
                var matchingSongs = album.Songs?.Where(s => s.Name?.ToLower().Contains(query) ?? false);
                if (matchingSongs != null)
                {
                    foreach (var song in matchingSongs)
                    {
                        Console.WriteLine($"Song: {song.Name} in Album: {album.Name} by {album.Artist}");
                    }
                }
            }
        }
    }

    public class Player
    {
        private MusicLibrary musicLibrary; // Reference to the music library
        private Song currentSong; // Currently playing song
        private DateTime? startTime; // Start time of the currently playing song
        private TimeSpan pausedDuration; // Duration for which the song was paused
        private bool isPaused; // Indicates if the playback is paused
        private int currentSongIndex; // Index of the currently playing song in the album
        private Album currentAlbum; // Reference to the current album

        public Player(MusicLibrary library)
        {
            musicLibrary = library;
        }

        // Select and play a specific song from an album
        public void SelectAndPlaySong()
        {
            Console.WriteLine("\nSelect an album:");
            musicLibrary.DisplayAlbumsAndSongs();
            Console.Write("Enter album number to play a song: ");
            if (int.TryParse(Console.ReadLine(), out int albumIndex))
            {
                albumIndex -= 1; // Convert to 0-based index
                try
                {
                    var album = musicLibrary.GetAlbumByIndex(albumIndex);
                    currentAlbum = album;
                    Console.WriteLine($"\nSongs in {album.Name}:");
                    for (int i = 0; i < album.Songs.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {album.Songs[i].Name} ({album.Songs[i].Duration})");
                    }
                    Console.Write("Enter song number to play: ");
                    if (int.TryParse(Console.ReadLine(), out int songIndex))
                    {
                        songIndex -= 1; // Convert to 0-based index
                        if (songIndex >= 0 && songIndex < album.Songs.Count)
                        {
                            currentSong = album.Songs[songIndex];
                            currentSongIndex = songIndex;
                            startTime = DateTime.Now;
                            pausedDuration = TimeSpan.Zero;
                            isPaused = false;
                            Console.WriteLine($"Now playing: {currentSong.Name} ({currentSong.Duration})");
                        }
                        else
                        {
                            Console.WriteLine("Invalid song selection.");
                        }
                    }
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }
        }

        // Display information about the currently playing song
        public void CurrentlyPlayingSong()
        {
            if (currentSong == null || startTime == null)
            {
                Console.WriteLine("No song is currently playing.");
            }
            else
            {
                var elapsed = isPaused ? pausedDuration : DateTime.Now - startTime.Value + pausedDuration;
                var songDuration = TimeSpan.Parse("00:" + currentSong.Duration);
                var displayTime = elapsed > songDuration ? songDuration : elapsed;
                Console.WriteLine($"Playing: {currentSong.Name}, Time Elapsed: {displayTime.Hours:D2}:{displayTime.Minutes:D2}:{displayTime.Seconds:D2}");
            }
        }

        // Manage playback controls (play, pause, stop, next, previous)
        public void PlaybackControls()
        {
            if (currentSong == null)
            {
                Console.WriteLine("No song is currently playing.");
                return;
            }

            Console.WriteLine("\nPlayback Controls:");
            Console.WriteLine("1. Play");
            Console.WriteLine("2. Pause");
            Console.WriteLine("3. Stop");
            Console.WriteLine("4. Next");
            Console.WriteLine("5. Previous");
            Console.Write("Select an option: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    if (isPaused)
                    {
                        startTime = DateTime.Now;
                        isPaused = false;
                        Console.WriteLine($"Resumed playing: {currentSong.Name}");
                    }
                    else
                    {
                        Console.WriteLine($"Already playing: {currentSong.Name}");
                    }
                    break;
                case "2":
                    if (!isPaused)
                    {
                        pausedDuration += DateTime.Now - startTime.Value;
                        isPaused = true;
                        Console.WriteLine("Paused playback.");
                    }
                    else
                    {
                        Console.WriteLine("Already paused.");
                    }
                    break;
                case "3":
                    Console.WriteLine("Stopped playback.");
                    currentSong = null;
                    startTime = null;
                    pausedDuration = TimeSpan.Zero;
                    isPaused = false;
                    break;
                case "4":
                    if (currentAlbum != null && currentSongIndex < currentAlbum.Songs.Count - 1)
                    {
                        currentSongIndex++;
                        currentSong = currentAlbum.Songs[currentSongIndex];
                        startTime = DateTime.Now;
                        pausedDuration = TimeSpan.Zero;
                        isPaused = false;
                        Console.WriteLine($"Now playing: {currentSong.Name} ({currentSong.Duration})");
                    }
                    else
                    {
                        Console.WriteLine("No next song available.");
                    }
                    break;
                case "5":
                    if (currentAlbum != null && currentSongIndex > 0)
                    {
                        currentSongIndex--;
                        currentSong = currentAlbum.Songs[currentSongIndex];
                        startTime = DateTime.Now;
                        pausedDuration = TimeSpan.Zero;
                        isPaused = false;
                        Console.WriteLine($"Now playing: {currentSong.Name} ({currentSong.Duration})");
                    }
                    else
                    {
                        Console.WriteLine("No previous song available.");
                    }
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}
