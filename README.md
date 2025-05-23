# AnimeDessert

---

AnimeDessert is a web app built by ASP .NET Core 8 MVC with Entity Framework Core as ORM, Bootstrap for styling, and JQuery for scripting.

---

## Task Allocation

- Yik Kan Sze
    - ListDessertsForCharacter, UpdateDessert, ListImagesForDessert, AddImagesToDessert, and RemoveImagesFromDessert APIs
    - Display Desserts of Character in Character Detail Page
    - Display Images of Dessert in Dessert Detail Page with delete buttons
    - Add Images to Dessert function in Views
    - Add role based access

- Peiyu Han
    - Display Character of Dessert with first version first image in Dessert Detail Page
    - Allow updating Character of Dessert in Dessert Edit Page, Link and Unlink Character with dessert with image showing.

## Features

- CRUD for Anime Database
    - Create Anime, attach Images, Musics, and Characters to Anime
    - Create Character, attach Versions for the Character
    - Create Character Version, attach Images and Voice Actors to the Version
    - Create Staff, of which can be added to Music as Singer or Character as Voice Actor
    - Read Animes, single Anime, single Music, and Music Singer
    - Read Characters, single Character
    - Read Character Versions inside Character, single Character Version, and Voice Actor
    - Read Staffs, single Staff
    - Update Anime Name, Character Name, Version Name, and Staff Name
    - Delete Anime, Character, Character Version, Image, Music, Staff
    - Delete link of Anime to Character, Anime to Image, Anime to Music, and Music to Singer
    - Delete link of Character to Version, Version to Image, Version to Voice Actor
    
- Anime Quiz
    - Generate questions for quiz
    - Able to randomly generate questions of different types (e.g. Image or Music question)
    - The answer types differ from question to question too, let say for Music question it can ask about the Name or the Singer
    - The order of questions and choices are randomly shuffled
    - There is a 50/50 tool of limited chances which can help you to eliminate 2 incorrect answers
    
- Dropzone for file uploading
    - Multiple files can be uploaded by drag & drop or click
    - There is a preview for each Image you uploaded
    - There is a audio player for each Music you uploaded
    - File can be removed from the dropzone
    - There is a clear all button for removing all inputs

- CRUD for Dessert Database
    - Create Dessert, attach Images, link and unlink with ingredient, link and unlink with Character
    - Create Ingredient
    - Create Review
    - Create Instruction
    - Read Dessert
    - Read Ingredient
    - Read Review
    - Read Instruction
    - Update Dessert and also change character linking
    - Update Ingredient, Review, Instruction
    - Delete Dessert, Ingredient, Review, Instruction

## Roles and Permissions

- **Admin**: Full access to manage the system
- **AnimeAdmin**: Create, Update, Delete Anime related data
- **DessertAdmin**: Create, Update, Delete Dessert related data
- **User**: All Read features

## Tech Stack

- ASP .NET Core 8 MVC app
- Entity Framework Core
- Bootstrap for styling
- JQuery for scripting

## Project Structures

- API Controllers: For handling API request
- MVC Controllers: For handling Page request
- Interfaces: For abstracting Services' methods
- Services: For underline backend logics
- Models: For typing
- Data: For data schema & migration
- Views: For rendering content

## APIs

### Anime
    - GET: /api/Anime ; Listing Animes
    - POST: /api/Anime ; Create an Anime
    - GET: /api/Anime/{id} ; Retrieve an Anime
    - PUT: /api/Anime/{id} ; Update an Anime
    - DELETE: /api/Anime/{id} ; Delete an Anime
    - POST: /api/Anime/{id}/CharacterVersion ; Link Characters to Anime
    - DELETE: /api/Anime/{id}/CharacterVersion ; Unlink Characters from Anime
    - POST: /api/Anime/{id}/Image ; Upload Images to Anime
    - DELETE: /api/Anime/{id}/Image ; Delete Images from Anime
    - POST: /api/Anime/{id}/Music ; Upload Musics to Anime
    - DELETE: /api/Anime/{id}/Music ; Delete Musics from Anime

### AnimeQuiz
    - GET: /api/AnimeQuiz?numOfQuestions=8 ; Generate an AnimeQuiz

### Character
    - GET: /api/Character ; Listing Characters
    - POST: /api/Character ; Create a Character
    - GET: /api/Character/{id} ; Retrieve a Character
    - PUT: /api/Character/{id} ; Update a Character
    - DELETE: /api/Character/{id} ; Delete a Character
    - POST: /api/Character/{id}/Version ; Add a Version to Character
    - DELETE: /api/Character/{id}/Version ; Delete Versions from Character

### CharacterVersion
    - GET: /api/CharacterVersion/{id} ; Retrieve a CharacterVersion
    - PUT: /api/CharacterVersion/{id} ; Update a CharacterVersion
    - POST: /api/CharacterVersion/{id}/Image ; Upload Images to CharacterVersion
    - DELETE: /api/CharacterVersion/{id}/Image ; Delete Images from CharacterVersion
    - POST: /api/CharacterVersion/{id}/VoiceActor ; Link VoiceActors to CharacterVersion
    - DELETE: /api/CharacterVersion/{id}/VoiceActor ; Unlink VoiceActors from CharacterVersion
 
### Music
    - POST: /api/Music/{id}/Singer ; Link Singers to Music
    - DELETE: /api/Music/{id}/Singer ; Unlink Singers from Music

### Staff
    - GET: /api/Staff ; Listing Staffs
    - POST: /api/Staff ; Create a Staff
    - GET: /api/Staff/{id} ; Retrieve a Staff
    - PUT: /api/Staff/{id} ; Update a Staff
    - DELETE: /api/Staff/{id} ; Delete a Staff

### Dessert
    - GET：/api/Dessert/List
    - POST：/api/Dessert/Add
    - GET： /api/Dessert/Find/{id}
    - PUT： /api/Dessert/Update/{id}
    - DELETE： /api/Dessert/Delete/{id}
    - GET：/api/Dessert/ListForIngredient/{id}
    - POST： /api/Dessert/Link
    - DELETE： /api/Dessert/Unlink
    - GET: /api/Dessert/ListImages/{id}
    - POST: /api/Dessert/AddImages/{id}
    - DELETE: /api/Dessert/RemoveImages/{id}

### Ingredient
    - GET: /api/Ingredient/List
    - POST: /api/Ingredient/Add
    - GET: /api/Ingredient/Find/{id}
    - PUT: /api/Ingredient/Update/{id}
    - DELETE: /api/Ingredient/Delete/{id}
    - GET: /api/Ingredient/ListForDessert/{id}
    - POST: /api/Ingredient/Link
    - DELETE: /api/Ingredient/Unlink

### Review
    - GET: /api/Review/List
    - POST: /api/Review/Add
    - GET: /api/Review/Find/{id}
    - PUT: /api/Review/Update/{id}
    - DELETE: /api/Review/Delete/{id}
    - GET: /api/Review/ListForDessert/{id}

### Instruction
    - GET: /api/Instruction/List
    - POST: /api/Instruction/Add
    - GET: /api/Instruction/Find/{id}
    - PUT: /api/Instruction/Update/{id}
    - DELETE: /api/Instrucion/Delete/{id}


