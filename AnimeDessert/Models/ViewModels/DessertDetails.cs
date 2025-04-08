namespace AnimeDessert.Models.ViewModels
{
    public class DessertDetails
    {
        //A dessert page must have a dessert
        public required DessertDto Dessert { get; set; }

        //A dessert page can have many ingredients
        public IEnumerable<IngredientDto>? DessertIngredients { get; set; }

        // All ingredients
        // ListIngredients()
        public IEnumerable<IngredientDto>? AllIngredients { get; set; }

        //A dessert page can have many reviews
        public IEnumerable<ReviewDto>? DessertReviews { get; set; }

        // All instructions for this dessert
        public IEnumerable<InstructionDto>? DessertInstructions { get; set; }

        // All images for this dessert
        public IEnumerable<ImageDto>? DessertImages { get; set; }

        // All characters
        // ListCharacters()
        public IEnumerable<CharacterDto>? AllCharacters { get; set; }

        // The character associated with this dessert
        public CharacterDto? DessertCharacter { get; set; }

        public IEnumerable<CharacterVersionDto>? CharacterVersionDtos { get; set; }
        public ImageDto? FirstCharacterImage { get; set; }
    }
}
