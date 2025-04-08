using AnimeDessert.Interfaces;
using AnimeDessert.Models;
using AnimeDessert.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnimeDessert.Controllers
{
    public class DessertPageController : Controller
    {
        private readonly IDessertService _dessertService;
        private readonly IIngredientService _ingredientService;
        private readonly IReviewService _reviewService;
        private readonly IInstructionService _instructionService;
        private readonly ICharacterService _characterService;

        // dependency injection of service interfaces
        public DessertPageController(IDessertService DessertService, IIngredientService IngredientService, IReviewService ReviewService, IInstructionService InstructionService, ICharacterService CharacterService)
        {
            _dessertService = DessertService;
            _ingredientService = IngredientService;
            _reviewService = ReviewService;
            _instructionService = InstructionService;
            _characterService = CharacterService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: DessertPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<DessertDto> DessertDtos = await _dessertService.ListDesserts();
            return View(DessertDtos);
        }

        // GET: DessertPage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            DessertDto? DessertDto = await _dessertService.FindDessert(id);
            IEnumerable<IngredientDto> AssociatedIngredients = await _ingredientService.ListIngredientsForDessert(id);
            IEnumerable<IngredientDto> Ingredients = await _ingredientService.ListIngredients();
            IEnumerable<ReviewDto> AssociatedReviews = await _reviewService.ListReviewsForDessert(id);

            //need the instructions for this dessert
            IEnumerable<InstructionDto> Instructions = await _instructionService.ListInstructionsForDessert(id);

            //need the images for this dessert
            IEnumerable<ImageDto> Images = await _dessertService.ListImagesForDessert(id);

            if (DessertDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find dessert"] });
            }
            CharacterDto? characterDto = null;
            IEnumerable<CharacterVersionDto>? characterVersions = null;
            ImageDto? firstCharacterImage = null;

            if (DessertDto.CharacterId != null)
            {
                characterDto = await _characterService.FindCharacter((int)DessertDto.CharacterId);
                characterVersions = characterDto?.CharacterVersionDtos;
                firstCharacterImage = characterDto?.CharacterVersionDtos?
                .FirstOrDefault()?
                .ImageDtos?
                .FirstOrDefault();
            }
       
                // information which drives a dessert page
                DessertDetails DessertInfo = new DessertDetails()
                {
                    Dessert = DessertDto,
                    DessertIngredients = AssociatedIngredients,
                    AllIngredients = Ingredients,
                    DessertReviews = AssociatedReviews,
                    DessertInstructions = Instructions,
                    DessertImages = Images,
                    DessertCharacter = DessertDto.CharacterId == null ? null : await _characterService.FindCharacter((int)DessertDto.CharacterId),
                    CharacterVersionDtos = characterVersions,
                    FirstCharacterImage = firstCharacterImage
                };
                return View(DessertInfo);
            }
        
        // GET DessertPage/New
        [Authorize]
        public ActionResult New()
        {
            return View();
        }

        // POST DessertPage/Add
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(DessertDto DessertDto)
        {
            ServiceResponse response = await _dessertService.AddDessert(DessertDto);

            if (response.Status == ServiceStatus.Created)
            {
                return RedirectToAction("Details", "DessertPage", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //GET DessertPage/Edit/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            DessertDto? DessertDto = await _dessertService.FindDessert(id);

            if (DessertDto == null)
            {
                return View("Error");
            }
            else
            {
                var allCharacters = await _characterService.ListCharacters(); // Ensure this method exists
                // information which drives a dessert page
                var characterDto = DessertDto.CharacterId.HasValue
            ? await _characterService.FindCharacter(DessertDto.CharacterId.Value)
            : null;

                IEnumerable<CharacterVersionDto>? characterVersions = null;
                ImageDto? firstCharacterImage = null;

                if (characterDto != null)
                {
                    // Get the character versions
                    characterVersions = characterDto.CharacterVersionDtos;
                    // Get the first image from the first version
                    firstCharacterImage = characterDto.CharacterVersionDtos?.FirstOrDefault()?
                        .ImageDtos?.FirstOrDefault();
                }

                DessertDetails DessertInfo = new DessertDetails()
                {
                    Dessert = DessertDto,
                    DessertCharacter = characterDto, // Still set this if you use it elsewhere
                    AllCharacters = allCharacters,
                    CharacterVersionDtos = characterVersions,
                    FirstCharacterImage = firstCharacterImage
                };


                return View(DessertInfo);
            }
        }

        //POST DessertPage/Update/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(int id, DessertDto DessertDto)
        {
            ServiceResponse response = await _dessertService.UpdateDessert(DessertDto);

            if (response.Status == ServiceStatus.Updated)
            {
                return RedirectToAction("Details", "DessertPage", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //GET DessertPage/ConfirmDelete/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            DessertDto? DessertDto = await _dessertService.FindDessert(id);
            if (DessertDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(DessertDto);
            }
        }

        //POST DessertPage/Delete/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _dessertService.DeleteDessert(id);
            if (response.Status == ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "DessertPage");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        //POST DessertPage/LinkToIngredient
        //DATA: ingredientId={ingredientId}&dessertId={dessertId}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LinkToIngredient([FromForm] int dessertId, [FromForm] int ingredientId)
        {
            await _ingredientService.LinkIngredientToDessert(ingredientId, dessertId);

            return RedirectToAction("Details", new { id = dessertId });
        }

        //POST DessertPage/UnlinkFromIngredient
        //DATA: ingredientId={ingredientId}&dessertId={dessertId}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UnlinkFromIngredient([FromForm] int dessertId, [FromForm] int ingredientId)
        {
            await _ingredientService.UnlinkIngredientFromDessert(ingredientId, dessertId);

            return RedirectToAction("Details", new { id = dessertId });
        }

        // GET: DessertPage/NewImages/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> NewImages(int id)
        {
            DessertDto? dessertDto = await _dessertService.FindDessert(id);

            return dessertDto != null
                ? View(dessertDto)
                : View("Error", new ErrorViewModel() { Errors = ["Dessert not found."] });
        }

        // POST: DessertPage/AddImages/{id}
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> AddImages(int id, [FromForm] AddImagesToDessertRequest request)
        {
            ServiceResponse response = await _dessertService.AddImagesToDessert(id, request);

            return response.Status == ServiceStatus.Created
                ? RedirectToAction("Details", "DessertPage", new { id = id })
                : PartialView("Error", new ErrorViewModel() { Errors = response.Messages });
        }

        // POST: DessertPage/RemoveImages/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveImages(int id, [FromForm] RemoveImagesFromDessertRequest request)
        {
            ServiceResponse response = await _dessertService.RemoveImagesFromDessert(id, request);

            return response.Status == ServiceStatus.Deleted
                ? RedirectToAction("Details", "DessertPage", new { id = id })
                : View("Error", new ErrorViewModel() { Errors = response.Messages });
        }

        //POST DessertPage/LinkToCharacter
        //DATA: characterId={characterId}&dessertId={dessertId}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LinkToCharacter([FromForm] int dessertId, [FromForm] int characterId)
        {
            await _characterService.LinkCharacterToDessert(characterId, dessertId);

            return RedirectToAction("Edit", new { id = dessertId });
        }

        //POST DessertPage/UnlinkFromCharacter
        //DATA: characterId={characterId}&dessertId={dessertId}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UnlinkFromCharacter([FromForm] int dessertId, [FromForm] int characterId)
        {
            await _characterService.UnlinkCharacterFromDessert(characterId, dessertId);

            return RedirectToAction("Edit", new { id = dessertId });
        }

        //// POST DessertPage/LinkToCharacter
        //[HttpPost]
        //[Authorize]
        //public async Task<IActionResult> LinkToCharacter([FromForm] int dessertId, [FromForm] int characterId)
        //{
        //    await _characterService.LinkCharacterToDessert(characterId, dessertId);
        //    return Json(new { success = true, characterId = characterId });
        //}

        //// POST DessertPage/UnlinkFromCharacter
        //[HttpPost]
        //[Authorize]
        //public async Task<IActionResult> UnlinkFromCharacter([FromForm] int dessertId, [FromForm] int characterId)
        //{
        //    await _characterService.UnlinkCharacterFromDessert(characterId, dessertId);
        //    return Json(new { success = true });
        //}


    }
}
