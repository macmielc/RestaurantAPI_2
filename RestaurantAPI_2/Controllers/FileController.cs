using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace RestaurantAPI_2.Controllers
{
    [ApiController]
    [Route("file")] // bez api aby oddzielić logike obsługi plików od logiki obsługi restauracji
    [Authorize]
    public class FileController : ControllerBase
    {
        public IActionResult GetFile([FromQuery] string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "PrivateFiles", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

           
            var fileBytes = System.IO.File.ReadAllBytes(filePath); // fileContent

            var contentTypeProvider = new FileExtensionContentTypeProvider();
            contentTypeProvider.TryGetContentType(fileName, out string contentType); // próba określenia typu MIME na podstawie rozszerzenia pliku. Jeśli nie uda się określić typu, contentType będzie null.

            if (contentType == null)
            {
                contentType = "application/octet-stream"; // Można dostosować typ MIME w zależności od rodzaju pliku
            }

            return File(fileBytes, contentType, fileName);  // Stosujemy File zamiast Ok() aby zwrócić plik do pobrania. W ten sposób przeglądarka będzie wiedziała, że ma do czynienia z plikiem i umożliwi użytkownikowi jego pobranie.
        }

        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
        {
            // Weyfikacja czy plik został przesłany i czy nie jest pusty. Jeśli plik jest null lub jego długość wynosi 0
            if (file != null && file.Length > 0)
            {

                // Tworzenie sciezki do katalogu "PrivateFiles" w bieżącym katalogu roboczym. Jeśli katalog nie istnieje, zostanie utworzony. Następnie plik jest zapisywany w tym katalogu.
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "PrivateFiles");
                var filePath = Path.Combine(uploadPath, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return Ok("File uploaded successfully.");
            }
            else
            {
                return BadRequest("No file uploaded.");
            }
        }
    }
}
