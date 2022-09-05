using idb.Backend.Attributes;
using idb.Backend.Requests.v1;
using idb.Backend.Storage;
using Microsoft.AspNetCore.Mvc;

namespace idb.Backend.Controllers.v1
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IdbController : ControllerBase
    {
        [HttpPost("image_upload")]
        public IActionResult PostImage([FromServices] IImageStorage azureStorage, [FromBody] ImageUpload imageInformation)
        {
            (var uploadUrl, var imageUrl) = azureStorage.GetImageUrls(imageInformation.filename);

            return new OkObjectResult(new ImageUploadResponse(uploadUrl, imageUrl));
        }
    }
}