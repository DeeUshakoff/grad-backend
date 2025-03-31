using AuthService.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Octokit;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GitHubController : ControllerBase
    {
        private readonly HttpClient httpClient;
        private readonly EncryptionService encryptionService;
        private readonly AuthDbContext dbContext;
        private readonly GitHubClient gitHubClient = new(new ProductHeaderValue("GradBackend"));

        private const string baseUrl = "https://api.github.com";
        public GitHubController(
            HttpClient httpClient,
            EncryptionService encryptionService,
            AuthDbContext dbContext
            )
        {
            this.httpClient = httpClient;
            this.httpClient.DefaultRequestHeaders.Clear();
            this.httpClient.DefaultRequestHeaders.Add("User-Agent", "GradBackend");
            this.httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            this.httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");

            this.encryptionService = encryptionService;
            this.dbContext = dbContext;
        }

        [HttpGet("tasks/{id}")]
        public async Task<IActionResult> GetTaskRepo(int id)
        {
            var task = await dbContext.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            var result = await dbContext.TaskGitRelations
                .Where(x => x.Task == task)
                .FirstOrDefaultAsync();
            if (result == null)
            {
                return Ok(result);
            }

            var git = await dbContext.GitRepositories.FindAsync(result.GitRepositoryId);

            var project = await dbContext.Projects.FindAsync(git.ProjectId);
            if(project == null)
            {
                return NotFound();
            }

            var org = await dbContext.Organizations.FindAsync(project.OrganizationId);

            var orgGitRelation = await dbContext.OrganizationGitRelations
                .Where(x => x.Organization == org)
                .FirstOrDefaultAsync();

            if (orgGitRelation == null)
            {
                return NotFound("Org git relation not found");
            }
            
            var tokenAuth = new Credentials(encryptionService.Decrypt(orgGitRelation.GitToken));
            gitHubClient.Credentials = tokenAuth;

            var repos = await gitHubClient.Repository.GetAllForOrg(org.Name);
            
            return Ok(repos.Where(x => x.Id == Convert.ToInt64(git.GitRepoId)).FirstOrDefault());
        }

        [HttpGet("organizations/{id}")]
        public async Task<IActionResult> GetOrg(int id)
        {
            var orgGitRelation = await dbContext.OrganizationGitRelations.Where(x => x.OrganizationId == id).FirstOrDefaultAsync();
            if (orgGitRelation == null)
            {
                return NotFound("Org git relation not found");
            }

            var url = $"{baseUrl}/orgs/{orgGitRelation.GitName}";

            SetAuth(encryptionService.Decrypt(orgGitRelation.GitToken));
            var response = await httpClient.GetAsync(url);
            SetAuth(null);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }

        [HttpGet("organizations/{id}/projects")]
        public async Task<IActionResult> GetOrgProjects(int id)
        {
            var orgGitRelation = await dbContext.OrganizationGitRelations.Where(x => x.OrganizationId == id).FirstOrDefaultAsync();
            if (orgGitRelation == null)
            {
                return NotFound("Org git relation not found");
            }

            var url = $"{baseUrl}/orgs/{orgGitRelation.GitName}/projects";

            SetAuth(encryptionService.Decrypt(orgGitRelation.GitToken));
            var response = await httpClient.GetAsync(url);
            SetAuth(null);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }

        [HttpGet("organizations/{id}/repos")]
        public async Task<IActionResult> GetOrgRepos(int id)
        {
            var orgGitRelation = await dbContext.OrganizationGitRelations.Where(x => x.OrganizationId == id).FirstOrDefaultAsync();
            if (orgGitRelation == null)
            {
                return NotFound("Org git relation not found");
            }

            var url = $"{baseUrl}/orgs/{orgGitRelation.GitName}/repos";

            SetAuth(encryptionService.Decrypt(orgGitRelation.GitToken));
            var response = await httpClient.GetAsync(url);
            SetAuth(null);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }


        [HttpGet("projects/{id}/repos/{repoId}/branches")]
        public async Task<IActionResult> GetRepoBranches(int id, int repoId)
        {

            var project = await dbContext.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound("Проект не найден.");
            }

            var org = await dbContext.Organizations.FindAsync(project.OrganizationId);
            if (org == null)
            {
                return NotFound();
            }
            var orgGitRelation = await dbContext.OrganizationGitRelations
                .Where(x => x.Organization == org)
                .FirstOrDefaultAsync();

            if (orgGitRelation == null)
            {
                return NotFound("Связь с GitHub организацией не найдена.");
            }

            var localRepo = await dbContext.GitRepositories
                .FindAsync(repoId);

            if(localRepo == null)
            {
                return NotFound("Local repo not found");
            }

            var tokenAuth = new Credentials(encryptionService.Decrypt(orgGitRelation.GitToken));
            gitHubClient.Credentials = tokenAuth;
            var branches = await gitHubClient.Repository.Branch.GetAll(Convert.ToInt64(localRepo.GitRepoId));
            return Ok(branches);
        }

        [HttpGet("projects/{id}/repos")]
        public async Task<IActionResult> GetProjectRepos(int id)
        {
            var project = await dbContext.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound("Проект не найден.");
            }

            var org = await dbContext.Organizations.FindAsync(project.OrganizationId);
            if(org == null)
            {
                return NotFound();
            }
            var orgGitRelation = await dbContext.OrganizationGitRelations
                .Where(x => x.Organization == org)
                .FirstOrDefaultAsync();

            if (orgGitRelation == null)
            {
                return NotFound("Связь с GitHub организацией не найдена.");
            }

            var localRepos = dbContext.GitRepositories
                .Where(x => x.Project == project)
                .ToList();
            var url = $"{baseUrl}/orgs/{orgGitRelation.GitName}/repos";

            SetAuth(encryptionService.Decrypt(orgGitRelation.GitToken));

            var response = await httpClient.GetAsync(url);
            SetAuth(null);

            if (!response.IsSuccessStatusCode)
            {
                return Ok(localRepos);
            }

            var content = await response.Content.ReadAsStringAsync();
            var gitHubRepos = JsonConvert.DeserializeObject<List<GitHubRepo>>(content);

            var matchedRepos = gitHubRepos
                .GroupJoin(
                    localRepos,
                    githubRepo => githubRepo.Id.ToString(),
                    localRepo => localRepo.GitRepoId,
                    (githubRepo, localReposGroup) => new
                    {
                        GitHubRepo = githubRepo,
                        LocalRepos = localReposGroup
                    }
                )
                .SelectMany(
                    x => x.LocalRepos.DefaultIfEmpty(),
                    (x, localRepo) => new
                    {
                        Id = x.GitHubRepo.Id,
                        Name = x.GitHubRepo.Name,
                        FullName = x.GitHubRepo.FullName,
                        Description = x.GitHubRepo.Description,
                        IsLocal = localRepo != null,
                        LocalData = localRepo
                    }
                )
                .ToList();

            return Ok(matchedRepos);
        }

        public class GitHubRepo
        {
            public int Id { get; set; }
            public string Name { get; set; }

            [JsonProperty("full_name")]
            public string FullName { get; set; }
            public bool Private { get; set; }
            public string HtmlUrl { get; set; }
            public string Description { get; set; }
            // Добавьте другие свойства, если они нужны
        }

        private void SetAuth(string? token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}
