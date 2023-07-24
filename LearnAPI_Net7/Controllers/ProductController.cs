using LearnAPI_Net7.Helpers;
using LearnAPI_Net7.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace LearnAPI_Net7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IWebHostEnvironment _webHostEnvironment;

        public ProductController(IWebHostEnvironment webHostEnvironment)
        {
            this._webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("UploadImage")]
        public async Task<IActionResult>UploadImage(IFormFile formFile, string productCode)
        {
            APIResponse response = new APIResponse();

            try
            {
                string FilePath = GetFilePath(productCode);
                if(!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }

                string imagePath = FilePath + "\\" + productCode + ".png";
                if(System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                using(FileStream fileStream = System.IO.File.Create(imagePath))
                {
                    await formFile.CopyToAsync(fileStream);
                    response.ResponseCode = 200;
                    response.Result = "Pass";

                }

            }
            catch (Exception e)
            {
                response.ResponseCode = 500;
                response.ErrorMessage = e.Message;
            }

            return Ok(response);
        }


        [HttpPost("MultipleFileUpload")]
        public async Task<IActionResult> MultipleFileUpload(IFormFileCollection fromFileCollection, string productCode)
        {
            APIResponse response = new APIResponse();
            int passCount = 0; int errorCount = 0;
            try
            {
                string FilePath = GetFilePath(productCode);
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }

                foreach (var file in fromFileCollection)
                {
                    string imagePath = FilePath + "\\" + file.FileName;
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    using (FileStream fileStream = System.IO.File.Create(imagePath))
                    {
                        await file.CopyToAsync(fileStream);
                        //response.ResponseCode = 200;
                        //response.Result = "Pass";

                        passCount++;

                    }

                }

            }
            catch (Exception e)
            {
                errorCount++;
                //response.ResponseCode = 500;
                response.ErrorMessage = e.Message;
            }

            response.ResponseCode = 200;
            response.Result = $"{passCount} Files Uploaded & {errorCount} Files Failed";

            return Ok(response);
        }


        [HttpGet("GetSingleFile")]
        public async 
            Task<IActionResult> GetSingleFile(string productCode)
        {
            string imageUrl = string.Empty;
            string hostURL = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/";
            try
            {
                string FilePath = GetFilePath(productCode);

                string imagePath = FilePath + "\\" + productCode + ".png";
                //string[] splitString= imagePath.Split('\\');
                if(System.IO.File.Exists(imagePath))
                {
                    imageUrl = hostURL + "Upload/Products/" + productCode + "/" + productCode + ".png";
                }
            }
            catch (Exception)
            {

            }
            return Ok(imageUrl);
        }



        [HttpGet("GetMultiImagesOrFiles")]
        public async Task<IActionResult> GetMultiImagesOrFiles(string productCode)
        {
            List<string> imageUrl = new List<string>();
            string hostURL = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/";
            try
            {
                string FilePath = GetFilePath(productCode);

                if(Directory.Exists(FilePath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(FilePath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();

                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        string fileName= fileInfo.Name;
                        string imagePath = FilePath + "\\" + fileName;
                        if (System.IO.File.Exists(imagePath))
                        {
                            var data = hostURL + "Upload/Products/" + productCode + "/" + fileName;
                            imageUrl.Add(data);
                        }
                    }
                }

              
            }
            catch (Exception)
            {

            }
            return Ok(imageUrl);
        }


        [HttpGet("DownloadSingleFile")]
        public async
            Task<IActionResult> DownloadSingleFile(string productCode)
        {
            //string imageUrl = string.Empty;
            //string hostURL = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/";
            try
            {
                string FilePath = GetFilePath(productCode);

                string imagePath = FilePath + "\\" + productCode + ".png";
                //string[] splitString= imagePath.Split('\\');
                if (System.IO.File.Exists(imagePath))
                {
                    
                    var mimeType = "application/....";
                    MemoryStream stream = new MemoryStream();
                    using(FileStream fileStream =new FileStream(imagePath,FileMode.Open))
                    {
                        await fileStream.CopyToAsync(stream);
                    }

                    stream.Position = 0;
                    //imageUrl = hostURL + "Upload/Products/" + productCode + "/" + productCode + ".png";
                    //return File(stream, "image/png", productCode + ".png");
                    return File(stream, mimeType, productCode + ".png");

                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        [HttpDelete("RemoveFile")]
        public async Task<IActionResult> RemoveFile(string productCode)
        {
            try
            {
                string FilePath = GetFilePath(productCode);
               string imagePath = FilePath + "\\" + productCode + ".png";
                if(System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                    return Ok("Pass");
                }else
                    return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        [HttpDelete("RemoveMultipleFiles")]
        public async Task<IActionResult> RemoveMultipleFiles(string productCode)
        {
            APIResponse response=new APIResponse();
            List<string> imageUrl = new List<string>();
            string hostURL = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/";
            try
            {
                string FilePath = GetFilePath(productCode);

                if (Directory.Exists(FilePath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(FilePath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();

                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        fileInfo.Delete();
                    }
                    response.ResponseCode= 200;
                    response.Result = "Directory Deleted";
                }
                else
                {
                    response.ResponseCode = 400;
                    response.Result = "Directory does not exist";
                }
               
            }
            catch (Exception)
            {
                response.ResponseCode = 500;
                response.ErrorMessage = "Something went wrong";
            }

            return Ok(response);    
        }




        [HttpPost("MultipleFileUploadtoDb")]
        public async Task<IActionResult> MultipleFileUploadtoDb(IFormFileCollection fromFileCollection, string productCode)
        {
            APIResponse response = new APIResponse();
            int passCount = 0; int errorCount = 0;
            try
            {
                foreach (var file in fromFileCollection)
                {
                
                    using (MemoryStream fileStream = new MemoryStream())
                    {
                        await file.CopyToAsync(fileStream);
                        
                        passCount++;
                        ProductImage data = new ProductImage()
                        {
                            ImageData = fileStream.ToArray(),
                            ProductCode = productCode
                        };

                    }

                }

            }
            catch (Exception e)
            {
                errorCount++;
                //response.ResponseCode = 500;
                response.ErrorMessage = e.Message;
            }

            response.ResponseCode = 200;
            response.Result = $"{passCount} Files Uploaded & {errorCount} Files Failed";

            return Ok(response);
        }


        [HttpPost("GetSingplefileFromProjectFolder_UploadDB")]
        public async Task<IActionResult> GetSingplefileFromProjectFolder_UploadDB(IFormFileCollection fromFileCollection, string productCode)
        {
            APIResponse response = new APIResponse();
            int passCount = 0; int errorCount = 0;
            try
            {

                string filePath = GetFilePath(productCode);
                string imagePath = filePath + "\\" + productCode + ".png";
                //string[] splitString= imagePath.Split('\\');
                if (System.IO.File.Exists(imagePath))
                {

                    var mimeType = "application/....";
                    MemoryStream stream = new MemoryStream();
                    using (FileStream fileStream = new FileStream(imagePath, FileMode.Open))
                    {
                        await fileStream.CopyToAsync(stream);

                    }
                    ProductImage data = new ProductImage()
                    {
                        ImageData = stream.ToArray(),
                        ProductCode = productCode
                    };
                    stream.Position = 0;
                    //imageUrl = hostURL + "Upload/Products/" + productCode + "/" + productCode + ".png";
                    //return File(stream, "image/png", productCode + ".png");
                    return File(stream, mimeType, productCode + ".png");

                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception e)
            {
                errorCount++;
                //response.ResponseCode = 500;
                response.ErrorMessage = e.Message;
            }

            response.ResponseCode = 200;
            response.Result = $"{passCount} Files Uploaded & {errorCount} Files Failed";

            return Ok(response);
        }


        [HttpGet("DownloadImageFromDB")]
        public async 
           Task<IActionResult> DownloadImageFromDB(string productCode)
        {
            ProductImage data = new ProductImage();

            return File(data.ImageData, "image/png", productCode + ".png");
        }

        [NonAction]
        private string GetFilePath(string productCode)
        {
            return this._webHostEnvironment.WebRootPath + "\\Upload\\Products\\" + productCode;
        }
    }
}
