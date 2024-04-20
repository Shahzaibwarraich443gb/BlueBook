using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using AccountingBlueBook.Controllers;
using Abp.Application.Services.Dto;
using System.Linq;
using AccountingBlueBook.Entities.MainEntities;
using Abp.Domain.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AccountingBlueBook.Web.Host.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VendorAttachmentController : AccountingBlueBookControllerBase
    {
        private IWebHostEnvironment _webHostEnvironment;
        private readonly IRepository<VendorAttachment> _vendorAttachmentRepository;

        public VendorAttachmentController(IWebHostEnvironment webHostEnvironment, IRepository<VendorAttachment> vendorAttachmentRepository)
        {
            _webHostEnvironment = webHostEnvironment;
            _vendorAttachmentRepository = vendorAttachmentRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("wwwroot", "Files", "VendorAttachments");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                // save in vendorattachment table
                // VendorAttachment attachment = new VendorAttachment();
                // {
                // attachment.VendorId = file.;
                //   attachment.FileName = file.Name;
                // }
                // await _vendorAttachmentRepository.InsertAsync(attachment);

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
        public async Task<IActionResult> VendorAttachmentsGet(EntityDto input)
        {
            try
            {
                var folderName = Path.Combine("wwwroot", "Files", "VendorAttachments");
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
                var folderName = Path.Combine("wwwroot", "Files", "VendorAttachments");
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
                var folderName = Path.Combine("wwwroot", "Files", "VendorAttachments");
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
