using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using AccountingBlueBook.Controllers;
using Abp.Application.Services.Dto;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AccountingBlueBook.Web.Host.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttachmentController : AccountingBlueBookControllerBase
    {
        private IWebHostEnvironment _webHostEnvironment;

        public AttachmentController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("wwwroot", "Files", "CustomerAttachments");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                // Create the folder if it doesn't exist
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var filePath = Path.Combine(pathToSave, fileName);
                if (file.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Cannot Upload File");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CustomerAttachmentsGet(EntityDto input)
        {
            try
            {
                var folderName = Path.Combine("wwwroot", "Files", "CustomerAttachments");
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (Directory.Exists(folderPath))
                {
                    var fileNames = Directory.GetFiles(folderPath)
                        .Select(Path.GetFileName)
                        .Where(fileName => fileName.Split('_')[0] == input.Id.ToString())
                        .ToList();

                    return Ok(fileNames);
                }
                else
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error retrieving files");
            }
        }

        [HttpGet("{fileName}")]
        public IActionResult GetFile(string fileName)
        {
            try
            {
                var folderName = Path.Combine("wwwroot", "Files", "CustomerAttachments");
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName, fileName);

                if (System.IO.File.Exists(filePath))
                {
                    var fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return File(fileBytes, "application/octet-stream", fileName);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error retrieving file");
            }
        }


        [HttpDelete("{fileName}")]
        public IActionResult DeleteFile(string fileName)
        {
            try
            {
                var folderName = Path.Combine("wwwroot", "Files", "CustomerAttachments");
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName, fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return Ok("File deleted successfully");
                }
                else
                {
                    return NotFound("File not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error deleting file");
            }
        }


    }
}
