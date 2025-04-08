using AnimeDessert.Data;
using AnimeDessert.Interfaces;
using AnimeDessert.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MimeTypes;
using System.Text.RegularExpressions;

namespace AnimeDessert.Services
{
    public class DessertService : IDessertService
    {
        private readonly ApplicationDbContext _context;
        // dependency injection of database context
        public DessertService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DessertDto>> ListDesserts()
        {
            // all desserts and their first image if any
            List<Dessert> Desserts = await _context.Desserts
                .Select(d => new Dessert
                {
                    DessertId = d.DessertId,
                    DessertName = d.DessertName,
                    DessertDescription = d.DessertDescription,
                    SpecificTag = d.SpecificTag,
                    Images = d.Images!.OrderBy(i => i.ImageId).Take(1).ToList()

                })
                .ToListAsync();

            // empty list of data transfer object DessertDto
            List<DessertDto> DessertDtos = new List<DessertDto>();

            // foreach Dessert record in database
            foreach (Dessert Dessert in Desserts)
            {
                // create new instance of DessertDto, add to list
                DessertDtos.Add(new DessertDto()
                {
                    DessertId = Dessert.DessertId,
                    DessertName = Dessert.DessertName,
                    DessertDescription = Dessert.DessertDescription,
                    SpecificTag = Dessert.SpecificTag,
                    ImageDtos = Dessert.Images?.Select(ImageService.ToImageDto).ToList()
                });
            }

            // return DessertDtos
            return DessertDtos;

        }

        public async Task<DessertDto?> FindDessert(int id)
        {
            // fisrt or default async will get the first (d)essert matching the {id}
            var Dessert = await _context.Desserts
                .FirstOrDefaultAsync(d => d.DessertId == id);

            // no dessert found
            if (Dessert == null)
            {
                return null;
            }

            // create an instance of DessertDto
            DessertDto DessertDto = new DessertDto()
            {
                DessertId = Dessert.DessertId,
                DessertName = Dessert.DessertName,
                DessertDescription = Dessert.DessertDescription,
                SpecificTag = Dessert.SpecificTag,
                CharacterId = Dessert.CharacterId
            };

            return DessertDto;
        }

        public async Task<ServiceResponse> UpdateDessert(DessertDto DessertDto)
        {
            ServiceResponse serviceResponse = new();

            // Create instance of Dessert
            Dessert Dessert = new Dessert()
            {
                DessertId = DessertDto.DessertId,
                DessertName = DessertDto.DessertName,
                DessertDescription = DessertDto.DessertDescription,
                SpecificTag = DessertDto.SpecificTag,
                CharacterId = DessertDto.CharacterId
            };

            // flags that the object has changed
            _context.Entry(Dessert).State = EntityState.Modified;

            try
            {
                //SQL Equivalent: Update Desserts set ... where DessertId={id}
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.Status = ServiceStatus.Error;
                serviceResponse.Messages.Add("An error occured updating the record");
                return serviceResponse;
            }
            serviceResponse.Status = ServiceStatus.Updated;
            return serviceResponse;
        }

        public async Task<ServiceResponse> AddDessert(DessertDto DessertDto)
        {
            ServiceResponse serviceResponse = new();


            // Create instance of Dessert
            Dessert Dessert = new Dessert()
            {
                DessertName = DessertDto.DessertName,
                DessertDescription = DessertDto.DessertDescription,
                SpecificTag = DessertDto.SpecificTag
            };
            // SQL Equivalent: Insert into Desserts (..) values (..)

            try
            {
                _context.Desserts.Add(Dessert);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the Dessert.");
                serviceResponse.Messages.Add(ex.Message);

            }

            serviceResponse.Status = ServiceStatus.Created;
            serviceResponse.CreatedId = Dessert.DessertId;
            return serviceResponse;
        }


        public async Task<ServiceResponse> DeleteDessert(int id)
        {
            ServiceResponse response = new();

            // Dessert must exist in the first place
            var Dessert = await _context.Desserts.FindAsync(id);

            if (Dessert == null)
            {
                response.Status = ServiceStatus.NotFound;
                response.Messages.Add("Dessert cannot be deleted because it does not exist.");
                return response;
            }

            try
            {
                _context.Desserts.Remove(Dessert);
                await _context.SaveChangesAsync();

                // Delete all uploaded images
                DirectoryInfo imageDir = new("wwwroot/assets/images/");

                foreach (FileInfo file in imageDir.EnumerateFiles($"Dessert_{Dessert.DessertId}_*"))
                {
                    file.Delete();
                }
            }
            catch (Exception)
            {
                response.Status = ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the dessert");
                return response;
            }

            response.Status = ServiceStatus.Deleted;
            return response;

        }

        public async Task<IEnumerable<DessertDto>> ListDessertsForIngredient(int id)
        {
            // join DessertIngredient on desserts.dessertid = DessertIngredient.dessertid WHERE DessertIngredient.ingredientid = {id}
            List<Dessert> Desserts = await _context.Desserts
                .Where(d => d.Ingredients!.Any(i => i.IngredientId == id))
                .ToListAsync();

            // empty list of data transfer object DessertDto
            List<DessertDto> DessertDtos = new List<DessertDto>();
            // foreach Dessert record in database
            foreach (Dessert Dessert in Desserts)
            {
                // create new instance of DessertDto, add to list
                DessertDtos.Add(new DessertDto()
                {
                    DessertId = Dessert.DessertId,
                    DessertName = Dessert.DessertName,
                    DessertDescription = Dessert.DessertDescription,
                    SpecificTag = Dessert.SpecificTag
                });
            }
            // return DessertDtos
            return DessertDtos;

        }
        public async Task<ServiceResponse> LinkDessertToIngredient(int dessertId, int ingredientId)
        {
            ServiceResponse serviceResponse = new();

            Dessert? dessert = await _context.Desserts
                .Include(d => d.Ingredients)
            .Where(d => d.DessertId == dessertId)
                .FirstOrDefaultAsync();
            Ingredient? ingredient = await _context.Ingredients.FindAsync(ingredientId);

            // Data must link to a valid entity
            if (ingredient == null || dessert == null)
            {
                serviceResponse.Status = ServiceStatus.NotFound;
                if (ingredient == null)
                {
                    serviceResponse.Messages.Add("Ingredient was not found. ");
                }
                if (dessert == null)
                {
                    serviceResponse.Messages.Add("Dessert was not found.");
                }
                return serviceResponse;
            }
            try
            {
                dessert.Ingredients!.Add(ingredient);
                _context.SaveChanges();
            }
            catch (Exception Ex)
            {
                serviceResponse.Messages.Add("There was an issue linking the ingredient to the dessert");
                serviceResponse.Messages.Add(Ex.Message);
            }


            serviceResponse.Status = ServiceStatus.Created;
            return serviceResponse;
        }

        public async Task<ServiceResponse> UnlinkDessertFromIngredient(int dessertId, int ingredientId)
        {
            ServiceResponse serviceResponse = new();

            Dessert? dessert = await _context.Desserts
                .Include(d => d.Ingredients)
            .Where(d => d.DessertId == dessertId)
                .FirstOrDefaultAsync();
            Ingredient? ingredient = await _context.Ingredients.FindAsync(ingredientId);

            // Data must link to a valid entity
            if (ingredient == null || dessert == null)
            {
                serviceResponse.Status = ServiceStatus.NotFound;
                if (ingredient == null)
                {
                    serviceResponse.Messages.Add("Ingredient was not found. ");
                }
                if (dessert == null)
                {
                    serviceResponse.Messages.Add("Dessert was not found.");
                }
                return serviceResponse;
            }
            try
            {
                dessert.Ingredients!.Remove(ingredient);
                _context.SaveChanges();
            }
            catch (Exception Ex)
            {
                serviceResponse.Messages.Add("There was an issue unlinking the ingredient to the dessert");
                serviceResponse.Messages.Add(Ex.Message);
            }


            serviceResponse.Status = ServiceStatus.Deleted;
            return serviceResponse;
        }

        public async Task<IEnumerable<ImageDto>> ListImagesForDessert(int id)
        {
            // Find Images for Dessert {id}
            List<Image> images = await _context.Images
                .Where(i => i.DessertId == id)
                .ToListAsync();

            // Convert to Dtos
            IEnumerable<ImageDto> imageDtos = images.Select(ImageService.ToImageDto);
            return imageDtos;
        }

        public async Task<ServiceResponse> AddImagesToDessert(int id, AddImagesToDessertRequest request)
        {
            ServiceResponse response = new();
            Boolean invalidData = false;

            // Validate requested ImageFiles length
            if (request.ImageFiles.Count == 0)
            {
                invalidData = true;
                response.Status = ServiceStatus.BadRequest;
                response.Messages.Add("Must include at least one ImageFile when adding to Dessert.");
            }

            // Validate reqeusted ImageFiles' format
            foreach (IFormFile ImageFile in request.ImageFiles)
            {
                if (!ImageService.ValidContentTypes.Contains(ImageFile.ContentType))
                {
                    invalidData = true;
                    response.Status = ServiceStatus.BadRequest;
                    response.Messages.Add($"Invalid format for file {ImageFile.FileName}, please use jpeg / png / gif / webp.");
                }
            }

            if (invalidData)
            {
                return response;
            }

            // Dessert must exist in the first place
            Dessert? dessert = await _context.Desserts
                .Include(d => d.Images)
                .SingleOrDefaultAsync(d => d.DessertId == id);

            if (dessert == null)
            {
                response.Status = ServiceStatus.NotFound;
                response.Messages.Add("Cannot add Images to Dessert because it does not exist.");
                return response;
            }

            List<UploadImageFile> uploadImageFiles = [];

            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (IFormFile ImageFile in request.ImageFiles)
                    {
                        string uid = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");
                        string imageFileExtension = MimeTypeMap.GetExtension(ImageFile.ContentType);

                        Image image = new()
                        {
                            ImageFilename = $"Dessert_{dessert.DessertId}_{uid}{imageFileExtension}",
                            DessertId = dessert.DessertId
                        };

                        dessert.Images?.Add(image);

                        uploadImageFiles.Add(new()
                        {
                            ImageFile = ImageFile,
                            ImagePath = Path.Combine("wwwroot/assets/images/", image.ImageFilename)
                        });
                    }

                    // Save changes data in context
                    await _context.SaveChangesAsync();

                    foreach (UploadImageFile uploadImageFile in uploadImageFiles)
                    {
                        using (FileStream imageStream = File.Create(uploadImageFile.ImagePath))
                        {
                            await uploadImageFile.ImageFile.CopyToAsync(imageStream);
                        }

                        if (!File.Exists(uploadImageFile.ImagePath))
                        {
                            throw new UploadException("There was an error uploading the Image.");
                        }
                    }

                    // Commit changes
                    await transaction.CommitAsync();

                    response.Status = ServiceStatus.Created;
                    response.Messages.Add($"{uploadImageFiles.Count} records are added.");
                    return response;
                }
                catch (Exception ex)
                {
                    // Rollback all changes
                    await transaction.RollbackAsync();

                    // Delete all images uploaded by this request
                    foreach (UploadImageFile uploadImageFile in uploadImageFiles)
                    {
                        File.Delete(uploadImageFile.ImagePath);
                    }

                    response.Status = ServiceStatus.Error;
                    response.Messages.Add("There was an error adding Images to Dessert.");
                    response.Messages.Add(ex.Message);
                    return response;
                }
            }
        }

        public async Task<ServiceResponse> RemoveImagesFromDessert(int id, RemoveImagesFromDessertRequest request)
        {
            ServiceResponse response = new();

            // Validate requested ImageIds length
            if (request.ImageIds.Count == 0)
            {
                response.Status = ServiceStatus.BadRequest;
                response.Messages.Add("Must include at least one ImageId when deleting from Dessert.");
                return response;
            }

            // Dessert must exist in the first place
            Dessert? dessert = await _context.Desserts
                .Include(d => d.Images!.Where(i => request.ImageIds.Contains(i.ImageId)))
                .SingleOrDefaultAsync(d => d.DessertId == id);

            if (dessert == null)
            {
                response.Status = ServiceStatus.NotFound;
                response.Messages.Add("Cannot delete Images from Dessert because it does not exist.");
                return response;
            }

            int affectedRecordsNumber = dessert.Images?.Count ?? 0;

            foreach (Image image in dessert.Images ?? [])
            {
                File.Delete(Path.Combine("wwwroot/assets/images/", image.ImageFilename));
                _context.Images.Remove(image);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception Ex)
            {
                response.Status = ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting Images from Dessert.");
                response.Messages.Add(Ex.Message);
                return response;
            }

            response.Status = ServiceStatus.Deleted;
            response.Messages.Add($"{affectedRecordsNumber} records are affected.");
            return response;
        }

        
    }
}
