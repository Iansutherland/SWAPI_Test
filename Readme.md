# Creating a CSV file of Star Wars People
Using the Star Wars API (SWAPI - The Star Wars API), create an application that generates a csv file that

    1. Lists all characters in all even numbered films (only first six are available)
        * note: films are identified (episode_id) in Chronolocial order (1 = New Hope, 4 = Phantom Menace)
    2. Sorted by Film -> Planet -> Character Age -> Character Name
    3. Where the character name is in surname-given name order.

### CSV
 * You can find a CSV I generated in the project files under csv/
 * You can find the generated csv after running the program in the SwapiCsv\SwapiCsv.Console\bin\Debug\netcoreapp3.1 (to avoid possible permission issues with user space)
### Implementation Details
 * I tried to use OOP principles (save inheritance, it didn't really call for that much)
 * Heavy use of the Task Parallel Library in an attempt to speed up HTTP calls

### Assumptions were made!
#### Sorting
 * Sorted by planet - used character's homeWorld
 * Sorted by film property 'episode_id' which corresponds with film chronolocial order instead of alphabetical with film title
 * Removed CSV Columns where data was an array of strings - these were arrays of URLs for data on things such as vehicles, planets, starships

##### SideNotes
 * I don't use emojis much, and never have in source code (or comments in source), but I did in this project just because of curiosity (see SwPerson.cs). I didn't know it was possible
   but it is :smiling_imp:
   [C# Lexicon Rules](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#identifiers)
   
#### Things I didn't get to
* removal of columns - some are lists of urls. I could remove them, or I could flatten the List<string> to a string
* Refactor - I'm convinced I got carried away a little and this whole thing could be a lot simpler. Or I could have used the existing [helper libs](https://swapi.dev/documentation#python)
