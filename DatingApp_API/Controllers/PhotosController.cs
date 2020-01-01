using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using DatingApp_API.Data;
using Microsoft.Extensions.Options;
using DatingApp_API.Helpers;
using CloudinaryDotNet;
using System.Threading.Tasks;
using DatingApp_API.Dtos;
using System.Security.Claims;
using CloudinaryDotNet.Actions;
using DatingApp.API.Models;
using System.Linq;
using System;

namespace DatingApp_API.Controllers
{
  [Authorize]
  [Route("api/users/{userId}/photos")]
  [ApiController]
  public class PhotosController : ControllerBase
  {
    // Bring in IDatingRepository, IMapper, 
    // And bring in Cloudinary, in order to access this because we provided Cloudinary as a service inside our startup of class,
    // We need to make use of IOptions, and give type "CloudinarySettings"
    private readonly IDatingRepository _repo;
    private readonly IMapper _mapper;
    private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
    private Cloudinary _cloudinary;

    public PhotosController(IDatingRepository repo, IMapper mapper,
                IOptions<CloudinarySettings> cloudinaryConfig)
    {
      _cloudinaryConfig = cloudinaryConfig;
      _mapper = mapper;
      _repo = repo;

      // Set up a new account and this is going to be our cloudinary accounts
      Account acc = new Account(
          _cloudinaryConfig.Value.CloudName,
          _cloudinaryConfig.Value.ApiKey,
          _cloudinaryConfig.Value.ApiSecret
      );

      // pass acc into Cloudinary in the Internet
      _cloudinary = new Cloudinary(acc);
    }

    // HttpGet("{id}") to get route of photo in cloudinary
    // because we need to pass a "route name" for CreatedAtRoute(), so add Name="GetPhoto"
    // This mehtod would be used by CreatedAtRoute() in below.
    [HttpGet("{id}", Name = "GetPhoto")]
    public async Task<IActionResult> GetPhoto(int id)
    {
      // photoFromRepo including user details because that's a navigation property on our photo.
      var photoFromRepo = await _repo.GetPhoto(id);

      // Because that's a navigation property on our photo. Then use autoMapper 
      // transfer photoFromRepo into PhotoFroReturnDto to get info we want
      //Console.WriteLine("photoFromRepo: ", photoFromRepo);

      var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

      return Ok(photo);
    }


    [HttpPost]  // Have to add [FromForm] to tell this api to get data from "form-data"
    // The KEY in Postman must be wrote, otherwise it show 500 internet server error
    public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoForCreationDto)
    {
      // First, compare userId from here and userId from route paremeter
      // User form controllerBase.User, that store a lots variables in ClaimTypes
      // And user ClaimTypes can get Id (NameIdentifier) from token
      if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        return Unauthorized(); // If the id is not the same id from server

      var userFromRepo = await _repo.GetUser(userId);

      // Save photo file
      var file = photoForCreationDto.File;

      // The uploadResult store info from Cloudinary, use cloudinary method
      var uploadResult = new ImageUploadResult();

      // Check there is a file or not
      if (file.Length > 0)
      {
        // It would be a fileStream, use "using" so that we can dispose of what the file "inside memory" once we've completed this method.
        // read our uploaded file into memeory
        using (var stream = file.OpenReadStream())
        {
          var uploadParams = new ImageUploadParams() // want to upload file into Cloudinary
          {
            // Pass file name and steam 
            File = new FileDescription(file.Name, stream),

            // What inside uploadParams is to transform the images so that if we upload an incredibly long photo for, And just crop a square for part of image 
            // Crop for spcific width and height
            Transformation = new Transformation()
                  .Width(500).Height(500).Crop("fill").Gravity("face")
          };

          // Upload file into cloudinary and get result
          uploadResult = _cloudinary.Upload(uploadParams);
        }
      }

      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~: ");

      // Populating some of the other fields inside our photo for creation.
      photoForCreationDto.Url = uploadResult.Uri.ToString();
      photoForCreationDto.PublicId = uploadResult.PublicId;

      //And then what we want to do is map of "PhotoForCreationDto" into our "Photo" itself based on the properties that we now have.
      // Transfer photoForCreationDto model into Photo model
      var photo = _mapper.Map<Photo>(photoForCreationDto);

      // If this the first photo they're uploading we're going to want to set this photo to be their main photo.
      // If Any(u => u.IsMain) return false This means the user doesn't have a main photo.
      if (!userFromRepo.Photos.Any(u => u.IsMain))
        photo.IsMain = true;

      // Save data as photo Model
      userFromRepo.Photos.Add(photo);

      // finally we can save it back to our repo
      if (await _repo.SaveAll())  // if save all successfully
      {
        var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);  // we won't get "Id of photo" until "_repo.SaveAll()" to save photo

        //  Finally, return a "location" header with the location of the created route like http://localhost:5000/api/users/1/photos/12
        // couple of overload, provide the "location of the resource (route)" we've just created
        // The first para is string of route name, second para for different ovarload, 
        // in here use routeObject value, that is Id of photo
        // Finally, return data into "photoToReturn"
        return CreatedAtRoute("GetPhoto", new { userId = userId, id = photo.Id },
          photoToReturn); // CreatedAtRoute() in ControllerBase

        // But now just simple Ok
        //return Ok();
      }

      return BadRequest("Cloud not add the photo");
    }

    // Add api to set a photo is Main photo or not 
    // Need to pass Id
    [HttpPost("{id}/setMain")]
    public async Task<IActionResult> SetMainPhoto(int userId, int id)  // id => photo Id 
    {
      // Check user id
      if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        return Unauthorized();

      // Want to make sure the user is attempting to update one of their own photos.
      // check that the id matches the id of one of the user's photos in the Repos.
      var user = await _repo.GetUser(userId);
      // Make user the photo exist in the user's photo collection.

      if (!user.Photos.Any(p => p.Id == id))  // if photo Id and it do not match
        return Unauthorized();

      var photoFromRepo = await _repo.GetPhoto(id);

      if (photoFromRepo.IsMain)
        return BadRequest("This is already the main photo");

      var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);

      // Set a new photo as Main, so need to set old main photo as not a main photo
      currentMainPhoto.IsMain = false;

      photoFromRepo.IsMain = true;

      if (await _repo.SaveAll()) // save sucessfully
        return NoContent();

      return BadRequest("Could not set photo to main"); // If cannot save successfully
    }

    [HttpDelete("{id}")] // id of photo
    public async Task<IActionResult> DeletePhoto(int userId, int id) // userId from route, and id from photo
    {
      // Check user id
      if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        return Unauthorized();

      // Want to make sure the user is attempting to update one of their own photos.
      // check that the id matches the id of one of the user's photos in the Repos.
      var user = await _repo.GetUser(userId);
      // Make user the photo exist in the user's photo collection.

      if (!user.Photos.Any(p => p.Id == id))  // if photo Id and it do not match
        return Unauthorized();

      var photoFromRepo = await _repo.GetPhoto(id);

      if (photoFromRepo.IsMain)  // Cannot delete the main photo
        return BadRequest("You cannot delete your main photo");

      // First of all, need to check there is a publicId 
      if (photoFromRepo.PublicId != null)
      {
        // Delete need photo PublicId 
        var deleteParams = new DeletionParams(photoFromRepo.PublicId);

        // going to return to us is a deletion results and what we want to check inside here is 
        // the response to the results comes back as okay and the okay as a string of text.
        // Delete data in Cloudinary
        var result = _cloudinary.Destroy(deleteParams);

        if (result.Result == "ok")
        { // succressfully delete on Cloudinary
          // Delete a photo in DB
          _repo.Delete(photoFromRepo);
        }
      }

      if (photoFromRepo.PublicId == null)
      {
        // Just delete data in BD, not in Cloudinary
        _repo.Delete(photoFromRepo);
      }

      // End of checking publicId

      if (await _repo.SaveAll())
      {
        return Ok();
      }

      // If cannnot delete the photo
      return BadRequest("Failed to delete the photo");

    }

  }
}