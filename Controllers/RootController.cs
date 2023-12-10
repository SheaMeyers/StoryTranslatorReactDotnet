using Microsoft.AspNetCore.Mvc;

namespace ReactDotnetCsvUploader.Controllers;

[Route("")]
public class RootController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return File("~/index.html", "text/html"); 
    }
}
