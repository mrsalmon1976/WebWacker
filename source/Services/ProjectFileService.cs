using Microsoft.Extensions.Logging;
using System.Text.Json;
using WebWacker.IO;
using WebWacker.Logging;
using WebWacker.Models;

namespace WebWacker.Services
{
    public interface IProjectFileService
    {
        IEnumerable<Project> LoadAllProjects();
    }
    public class ProjectFileService : IProjectFileService
    {
        private readonly ILogger<ProjectFileService> _logger;
        private readonly IFileSystemWrapper _fileSystemWrapper;

        public ProjectFileService(ILogger<ProjectFileService> logger, IFileSystemWrapper fileSystemWrapper)
        {
            _logger = logger;
            _fileSystemWrapper = fileSystemWrapper;
        }

        public IEnumerable<Project> LoadAllProjects()
        {
            string dir = AppContext.BaseDirectory;

            _logger.LogInformation($"Loading all projects from '{dir}'.");
            string[] jsonFiles = _fileSystemWrapper.GetFiles(dir, "*.json");

            List<Project> projects = new List<Project>();

            foreach (string filePath in jsonFiles)
            {
                if (filePath.ToLower().StartsWith("appsettings"))
                {
                    _logger.LogInformation($"Ignoring app settings file '{filePath}'.");
                    continue;
                }

                Project? project = LoadFromFile(filePath);
                if (project != null)
                {
                    _logger.LogInformation($"Loaded project from file '{filePath}'.");
                    projects.Add(project);
                }
            }

            return projects;
        }

        private Project? LoadFromFile(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            try
            {
                string fileContents = _fileSystemWrapper.ReadAllText(filePath);
                Project? project = JsonSerializer.Deserialize<Project>(fileContents);
                if (project == null || !project.IsValid())
                {
                    _logger.LogWarning($"File '{fileName}' is not a valid Project file.");
                    return null;
                }
                return project;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to load project from file '{fileName}': {ex.Message}");
                return null;
            }
        }
    }
}
