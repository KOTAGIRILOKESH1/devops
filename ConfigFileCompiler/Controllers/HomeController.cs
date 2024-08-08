using Microsoft.AspNetCore.Mvc;
using ConfigFileComparer.Models;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using System;

namespace ConfigFileComparer.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new ConfigFileModel());
        }

        [HttpPost]
        public async Task<IActionResult> Compare(ConfigFileModel model)
        {
            // Validate file paths
            if (string.IsNullOrEmpty(model.OldFilePath) || string.IsNullOrEmpty(model.NewFilePath))
            {
                ModelState.AddModelError("", "Please provide both file paths.");
                return View("Index", model);
            }

            // Validate file types
            if (!IsConfigFile(model.OldFilePath) || !IsConfigFile(model.NewFilePath))
            {
                ModelState.AddModelError("", "Both files must be .config files.");
                return View("Index", model);
            }

            // Read file contents
            model.OldFileContent = await System.IO.File.ReadAllTextAsync(model.OldFilePath);
            model.NewFileContent = await System.IO.File.ReadAllTextAsync(model.NewFilePath);

            // Compare files and format differences
            model.ComparisonResult = FormatComparisonResult(model.OldFileContent, model.NewFileContent);

            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Refresh(ConfigFileModel model)
        {
            // Validate file paths
            if (string.IsNullOrEmpty(model.OldFilePath) || string.IsNullOrEmpty(model.NewFilePath))
            {
                ModelState.AddModelError("", "Please provide both file paths.");
                return View("Index", model);
            }

            // Validate file types
            if (!IsConfigFile(model.OldFilePath) || !IsConfigFile(model.NewFilePath))
            {
                ModelState.AddModelError("", "Both files must be .config files.");
                return View("Index", model);
            }

            // Read file contents
            model.OldFileContent = await System.IO.File.ReadAllTextAsync(model.OldFilePath);
            model.NewFileContent = await System.IO.File.ReadAllTextAsync(model.NewFilePath);

            // Compare files and format differences
            model.ComparisonResult = FormatComparisonResult(model.OldFileContent, model.NewFileContent);

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult Clear()
        {
            // Clear model for a fresh start
            return RedirectToAction("Index");
        }

        private bool IsConfigFile(string filePath)
        {
            // Check if file path ends with .config
            return filePath.EndsWith(".config");
        }

        private string FormatComparisonResult(string oldFileContent, string newFileContent)
        {
            var diffBuilder = new InlineDiffBuilder(new Differ());
            var diffResult = diffBuilder.BuildDiffModel(oldFileContent, newFileContent);

            var result = new StringBuilder();
            result.AppendLine("<div><h3>Comparison Result:</h3><pre>");

            // Iterate over the diff lines
            foreach (var line in diffResult.Lines)
            {
                switch (line.Type)
                {
                    case ChangeType.Inserted:
                        result.AppendLine($"<span style='background-color: yellow;'>{line.Position}: {line.Text}</span><br>");
                        break;
                    case ChangeType.Deleted:
                        result.AppendLine($"<span>{line.Position}: {line.Text}</span><br>");
                        break;
                    case ChangeType.Unchanged:
                        result.AppendLine($"<span>{line.Position}: {line.Text}</span><br>");
                        break;
                }
            }

            result.AppendLine("</pre></div>");
            return result.ToString();
        }
    }
}