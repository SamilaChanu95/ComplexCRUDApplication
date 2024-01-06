using ComplexCRUDApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComplexCRUDApplication.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IWebHostEnvironment webHostEnvironment, ILogger<ProductController> logger) 
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        [HttpPut]
        [Route("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile formFile, string productCode) 
        {
            ApiResponse response = new ApiResponse();
            try 
            {
                string filePath = _webHostEnvironment.WebRootPath + "\\upload\\product\\" + productCode;
                if (!System.IO.Directory.Exists(filePath)) 
                {
                    System.IO.Directory.CreateDirectory(filePath); 
                }

                // string imagePath = filePath + "\\" + productCode + ".png";

                string imagePath = filePath + "\\" + formFile.FileName;
                
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                using (FileStream fileStream = System.IO.File.Create(imagePath)) 
                {
                    await formFile.CopyToAsync(fileStream);
                    response.ResponseCode = 200;
                    response.Result = "Image uploaded successfully";
                }
            }
            catch (Exception ex) 
            {
                response.ErrorMessage = ex.Message;
                _logger.LogError(ex.Message);
            }
            return Ok(response);
        }

        [HttpPut]
        [Route("upload-multiple-images")]
        public async Task<IActionResult> UploadMultipleImages(IFormFileCollection fileCollection, string productCode)
        {
            ApiResponse response = new ApiResponse();
            int successCount = 0;
            int errorCount = 0;
            try
            {
                string filePath = GetFilepath(productCode);
                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }

                foreach (var file in fileCollection)
                {

                    string imagePath = filePath + "\\" + file.FileName;
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    try
                    {
                        using (FileStream fileStream = System.IO.File.Create(imagePath))
                        {
                            await file.CopyToAsync(fileStream);
                            successCount++;
                            response.ResponseCode = 200;
                            response.Result = "Image uploaded successfully.";
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        response.ErrorMessage = ex.Message;
                        _logger.LogError(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                _logger.LogError(ex.Message);
            }
            finally 
            {
                _logger.LogInformation($"Successfull Count: {successCount}, UnSuccessfull Count: {errorCount}.");
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("get-image")]
        public async Task<IActionResult> GetImage(string productCode) 
        {
            try 
            {
                string hostingUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
                if (IsDirectoryExists(productCode))
                {
                    string folderPath = GetFilepath(productCode);
                    //string[] imageURLs = new string [20];
                    List<string> imageURLs = new List<string>();
                    var imageFiles = Directory.EnumerateFiles(folderPath, "*");
                    if (imageFiles.Any())
                    {
                        foreach (var imageFile in imageFiles)
                        {
                            string[] nameArray = imageFile.Split("\\");
                            string imageName = nameArray[nameArray.Length - 1];
                            string ImageUrl = hostingUrl + "/upload/product/" + productCode + "/" + imageName;
                            imageURLs.Add(imageFile.ToString());
                            // imageURLs.Add(ImageUrl.ToString());
                        }
                        _logger.LogInformation("Every Image's URLs successfully returned...!");
                        DownloadImage(imageURLs);
                        return Ok(imageURLs);
                    }
                    else 
                    {
                        _logger.LogError("Image file not found...!");
                        return NotFound();
                    }
                }
                else 
                {
                    _logger.LogError("Requested productCode not found...!");
                    return NotFound();
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [NonAction]
        public string GetFilepath(string productCode) 
        {
            return _webHostEnvironment.WebRootPath + "\\upload\\product\\" + productCode;
        }

        [NonAction]
        public bool IsDirectoryExists(string productCode) 
        {
            string path = GetFilepath(productCode);
            if (System.IO.Directory.Exists(path))
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        [NonAction]
        public async Task<IActionResult> DownloadImage(List<string> hostUrls) 
        {
            try 
            {
                foreach(var hostUrl in hostUrls)
                {
                    string[] imageNames = hostUrl.Split("\\");
                    string imageName = imageNames[imageNames.Length - 1];
                    MemoryStream stream = new MemoryStream();
                    using (FileStream fileStream = new FileStream(hostUrl, FileMode.Open)) 
                    {
                        await fileStream.CopyToAsync(stream);
                    }
                    stream.Position = 0;
                    return File(stream, "image/png", imageName);
                }
                return Ok();
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
                return BadRequest();  
            }
        }

    }
}
