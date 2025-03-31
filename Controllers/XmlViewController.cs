using System;
using System.IO;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using XML_editor.Models;

namespace XML_editor.Controllers
{
    public class XmlViewController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public XmlViewController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
//hello
        public IActionResult Index()
        {
            try
            {
                var dataPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Data");
                var defaultXmlPath = Path.Combine(dataPath, "yourfile.xml");
                
                if (System.IO.File.Exists(defaultXmlPath))
                {
                    var xmlContent = System.IO.File.ReadAllText(defaultXmlPath);
                    var xDoc = XDocument.Parse(xmlContent);
                    var rootNode = XmlNode.FromXElement(xDoc.Root);
                    ViewBag.InitialXml = xmlContent;
                    ViewBag.InitialData = rootNode;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading default XML: {ex.Message}";
            }
            
            return View();
        }

        [HttpPost]
        public IActionResult LoadXml([FromBody] string xmlContent)
        {
            try
            {
                var xDoc = XDocument.Parse(xmlContent);
                var rootNode = XmlNode.FromXElement(xDoc.Root);
                return Json(rootNode);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error parsing XML: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult SaveXml([FromBody] SaveXmlRequest request)
        {
            try
            {
                var xDoc = new XDocument(request.Node.ToXElement());
                
                // Create Data directory if it doesn't exist
                var dataPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Data");
                if (!Directory.Exists(dataPath))
                {
                    Directory.CreateDirectory(dataPath);
                }

                // Generate filename using the provided name or a timestamp
                string fileName = string.IsNullOrEmpty(request.FileName) 
                    ? $"xml_{DateTime.Now:yyyyMMdd_HHmmss}.xml"
                    : $"{request.FileName}.xml";

                // Ensure filename is valid and unique
                fileName = Path.GetFileNameWithoutExtension(fileName).Replace(" ", "_");
                fileName = $"{fileName}.xml";
                string fullPath = Path.Combine(dataPath, fileName);

                // Ensure filename is unique
                int counter = 1;
                while (System.IO.File.Exists(fullPath))
                {
                    fileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{counter}.xml";
                    fullPath = Path.Combine(dataPath, fileName);
                    counter++;
                }

                // Save the file
                xDoc.Save(fullPath);

                return Json(new { 
                    xml = xDoc.ToString(),
                    fileName = fileName,
                    fullPath = fullPath
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error saving XML: {ex.Message}");
            }
        }
    }

    public class SaveXmlRequest
    {
        public XmlNode Node { get; set; }
        public string FileName { get; set; }
    }
} 