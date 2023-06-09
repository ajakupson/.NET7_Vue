using backend.IServices;
using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZipController : ControllerBase
    {
        private readonly IZipService _zipService;

        public ZipController(IZipService zipService)
        {
            _zipService = zipService;
        }

        [HttpGet]
        [Route("health")]
        public IActionResult HealthCheck()
        {
            return Ok("API is working!");
        }

        [HttpGet]
        [Route("get")]
        public IActionResult GetZipArchives()
        {
            try
            {
                List<string> zipNames = _zipService.GetZipsList();
                return Ok(new { zips = zipNames });
            }
            catch (DirectoryNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        [Route("get/{zipName}")]
        public IActionResult GetZipContents(string zipName)
        {
            try
            {
                List<Node> tree = _zipService.GetZipContent(zipName);
                return Ok(new { content = tree });
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Route("validate")]
        public IActionResult ValidateZip([FromForm] IFormFile zip)
        {
            List<Node> tree = _zipService.GetZipContentIFormFile(zip);
            List<string> errors = _zipService.ValidateZip(tree);

            return Ok(new { content = tree, errors = errors });
        }

        [HttpPost]
        [Route("delete/{zipName}")]
        public IActionResult DeleteZip(string zipName)
        {
            try
            {
                _zipService.DeleteZip(zipName);
                return Ok();
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadZip([FromForm] IFormFile zip)
        {
            var result = await _zipService.UploadFile(zip);

            if (result["status"] == "success")
            {
                return Ok(new { result = result, zip = zip.FileName });
            } 
            else
            {
                return BadRequest(result);
            }
        }
    }
}
