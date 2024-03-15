using GitLabApiClient;
using GitLabApiClient.Models.Milestones.Responses;
using GitLabApiClient.Models.Projects.Requests;
using GitLabApiClient.Models.Projects.Responses;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Models;

namespace SEP_Management.Utils
{
    public static class SyncGitLab
    {
        public static async Task SyncLabelsWithProject(Models.Project project)
        {
            GitLabClient gitLabClient = new GitLabClient("https://gitlab.com", project.AccessToken);
            var projectGitlab = await gitLabClient.Projects.GetAsync(GetProjectPath(project.Url));
            int projectId = projectGitlab.Id;
            List<Label> gitlabLabels = (List<Label>)await GetListLabelGitLabAsync(gitLabClient, project);
            int gitlabProjectId = 0;
            using (var context = new SEP_ManagementContext())
            {
                List<IssueSetting> dbLabels = await context.IssueSettings.Where(x => x.ProjectId == project.ProjectId && x.IsActive == 1).ToListAsync();
                foreach (var gitlab in gitlabLabels)
                {
                    var dbLabel = dbLabels.FirstOrDefault(x => x.GitlabId == gitlab.Id);
                    if (dbLabel != null)
                    {
                        await UpdateDatabaseLabel(dbLabel, gitlab, context);
                    }
                    else
                    {
                        await AddLabelToDatabase(gitlab, project.ProjectId, context);
                    }
                }
                foreach (var db in dbLabels)
                {

                    var gitlabLabel = gitlabLabels.FirstOrDefault(x => x.Id == db.GitlabId);
                    if (gitlabLabel != null)
                    {
                        await UpdateGitlabLabel(db, gitlabLabel, projectId, gitLabClient);
                    }
                    else
                    {
                        await CreateGitlabLabel(db, gitlabProjectId, gitLabClient);
                    }
                }
            }
        }

        public static async Task SyncMilestonesWithProject(Models.Project project)
        {
            GitLabClient gitLabClient = new GitLabClient("https://gitlab.com", project.AccessToken);
            List<GitLabApiClient.Models.Milestones.Responses.Milestone> gitlabMilestones = (List<GitLabApiClient.Models.Milestones.Responses.Milestone>)await GetListMilestoneGitLabAsync(gitLabClient, project);
            var projectGitlab = await gitLabClient.Projects.GetAsync(GetProjectPath(project.Url));
            int gitlabProjectId = projectGitlab.Id;
            using (var context = new SEP_ManagementContext())
            {
                List<Models.Milestone> dbMilestones = await context.Milestones.Where(x => x.ProjectId == project.ProjectId).ToListAsync();
                foreach (var gitlab in gitlabMilestones)
                {
                    var dbMilestone = dbMilestones.FirstOrDefault(x => x.GitlabId == gitlab.Id.ToString());
                    if (dbMilestone != null)
                    {
                        if (gitlab.UpdatedAt.CompareTo(dbMilestone.UpdatedAt) > 0)
                        {
                            await UpdateDatabaseMilestone(dbMilestone, gitlab, context);
                        }
                    }
                    else
                    {
                        await AddMilestoneToDatabase(gitlab, project.ProjectId, context);
                    }
                }
                foreach (var db in dbMilestones)
                {

                    var gitlabMilestone = gitlabMilestones.FirstOrDefault(x => x.Id.ToString().Equals(db.GitlabId) || x.Title.Equals(db.MilestoneName));
                    if (gitlabMilestone != null)
                    {
                        if (gitlabMilestone.UpdatedAt.CompareTo(db.UpdatedAt) < 0)
                        {
                            await UpdateGitlabMilestone(db, gitlabMilestone, gitLabClient);
                        }
                    }
                    else
                    {
                        await CreateGitlabMilestone(db, gitlabProjectId, gitLabClient);
                    }
                }
            }
        }

        private static async Task UpdateGitlabLabel(IssueSetting db, Label gitlab, int projectGitlabId, GitLabClient gitLabClient)
        {
            List<Label> gitlabLabels = (List<Label>)await gitLabClient.Projects.GetLabelsAsync(projectGitlabId);
            var gitlabLabel = gitlabLabels.FirstOrDefault(x => x.Id == db.GitlabId);

            if (gitlabLabel != null)
            {
                var updateRequest = UpdateProjectLabelRequest.FromColor(GetNamespaceAndSpecificLabel(db.Title, db.Status), db.Color);
                await gitLabClient.Projects.UpdateLabelAsync(projectGitlabId, updateRequest);
            }
        }

        private static async Task UpdateGitlabMilestone(Models.Milestone db, GitLabApiClient.Models.Milestones.Responses.Milestone gitlab, GitLabClient gitLabClient)
        {
            GitLabApiClient.Models.Milestones.Responses.Milestone gitlabMilestone = await gitLabClient.Projects.GetMilestoneAsync(gitlab.ProjectId, gitlab.Id);
            if (gitlabMilestone != null)
            {
                var updateRequest = new UpdateProjectMilestoneRequest
                {
                    Title = db.MilestoneName,
                    Description = db.Description,
                    DueDate = db.DueDate.ToString(),
                    StartDate = db.StartDate.ToString(),
                    State = ((GitLabApiClient.Models.Milestones.Requests.UpdatedMilestoneState?)(db.IsActive == 1 ? MilestoneState.Active : MilestoneState.Closed))
                };
                await gitLabClient.Projects.UpdateMilestoneAsync(gitlabMilestone.ProjectId, gitlabMilestone.Id, updateRequest);
            }

        }

        private static async Task CreateGitlabLabel(IssueSetting label, int projectId, GitLabClient gitLabClient)
        {
            var updateRequest = UpdateProjectLabelRequest.FromColor(GetNamespaceAndSpecificLabel(label.Title, label.Status), label.Color);
            var newRequest = new CreateProjectLabelRequest(GetNamespaceAndSpecificLabel(label.Title, label.Status));
            await gitLabClient.Projects.CreateLabelAsync(projectId, newRequest);
            await gitLabClient.Projects.UpdateLabelAsync(projectId, updateRequest);
        }

        private static async Task CreateGitlabMilestone(Models.Milestone milestone, int projectId, GitLabClient gitLabClient)
        {
            var updateRequest = new UpdateProjectMilestoneRequest
            {
                Title = milestone.MilestoneName,
                Description = milestone.Description,
                StartDate = milestone.StartDate.ToString(),
                DueDate = milestone.DueDate.ToString(),
                State = ((GitLabApiClient.Models.Milestones.Requests.UpdatedMilestoneState?)(milestone.IsActive == 1 ? MilestoneState.Active : MilestoneState.Closed))
            };
            var newRequest = new CreateProjectMilestoneRequest(milestone.MilestoneName);
            var newMilestone = await gitLabClient.Projects.CreateMilestoneAsync(projectId, newRequest);
            await gitLabClient.Projects.UpdateMilestoneAsync(projectId, newMilestone.Id, updateRequest);
        }

        private static async Task UpdateDatabaseLabel(IssueSetting label, Label gitlab, SEP_ManagementContext context)
        {
            label.Description = gitlab.Description;
            label.GitlabId = gitlab.Id;
            var list = GetLabelTitle(gitlab.Name);
            if (list != null)
            {
                label.Title = list[0];
                label.Status = list[1];
            }
            else
            {
                label.Title = gitlab.Name;
            }
            label.Color = gitlab.Color;
            await context.SaveChangesAsync();
        }

        private static async Task UpdateDatabaseMilestone(Models.Milestone milestone, GitLabApiClient.Models.Milestones.Responses.Milestone gitlab, SEP_ManagementContext context)
        {
            milestone.MilestoneName = gitlab.Title;
            milestone.Description = gitlab.Description;
            milestone.StartDate = gitlab.StartDate == null ? null : DateTime.Parse(gitlab.StartDate);
            milestone.DueDate = gitlab.DueDate == null ? null : DateTime.Parse(gitlab.DueDate);
            milestone.IsActive = (byte?)(gitlab.State.Equals("Active") ? 1 : 0);
            await context.SaveChangesAsync();
        }

        private static async Task AddLabelToDatabase(Label gitlab, int projectId, SEP_ManagementContext context)
        {
            IssueSetting issueSetting = new IssueSetting();
            issueSetting.GitlabId = gitlab.Id;
            issueSetting.ProjectId = projectId;
            issueSetting.Description = gitlab.Description;
            var list = GetLabelTitle(gitlab.Name);
            if (list != null)
            {
                issueSetting.Title = list[0];
                issueSetting.Status = list[1];
            }
            else
            {
                issueSetting.Title = gitlab.Name;
            }
            issueSetting.Color = gitlab.Color;
            issueSetting.IsActive = 1;
            await context.IssueSettings.AddAsync(issueSetting);
            await context.SaveChangesAsync();
        }

        private static async Task AddMilestoneToDatabase(GitLabApiClient.Models.Milestones.Responses.Milestone gitlab, int projectId, SEP_ManagementContext context)
        {
            Models.Milestone milestone = new Models.Milestone();
            milestone.MilestoneName = gitlab.Title;
            milestone.Description = gitlab.Description;
            milestone.StartDate = gitlab.StartDate == null ? null : DateTime.Parse(gitlab.StartDate);
            milestone.DueDate = gitlab.DueDate == null ? null : DateTime.Parse(gitlab.DueDate);
            milestone.ProjectId = projectId;
            milestone.GitlabId = gitlab.Id.ToString();
            milestone.IsActive = (byte?)(gitlab.State.Equals("Active") ? 1 : 0);
            milestone.GitlabId = gitlab.Id.ToString();
            await context.Milestones.AddAsync(milestone);
            await context.SaveChangesAsync();
        }

        private static async Task<IEnumerable<GitLabApiClient.Models.Milestones.Responses.Milestone>> GetListMilestoneGitLabAsync(GitLabClient gitLabClient, Models.Project project)
        {
            var list = await gitLabClient.Projects.GetMilestonesAsync(GetProjectPath(project.Url));
            return (IEnumerable<GitLabApiClient.Models.Milestones.Responses.Milestone>)list;
        }

        private static async Task<IEnumerable<Label>> GetListLabelGitLabAsync(GitLabClient gitLabClient, Models.Project project)
        {
            var list = await gitLabClient.Projects.GetLabelsAsync(GetProjectPath(project.Url));
            return (IEnumerable<Label>)list;
        }

        private static string GetProjectPath(string url)
        {
            return url.Split("https://gitlab.com/")[1];
        }

        public static List<string> GetLabelTitle(string title)
        {
            List<string> list = null;
            if (title.Contains("::"))
            {
                list = new List<string>();
                int index = title.IndexOf("::");
                list.Add(title.Substring(0, index));
                list.Add(title.Substring(index + 2));
            }
            return list;
        }
        public static string GetNamespaceAndSpecificLabel(string namespaceLabel, string specificLabel)
        {
            if (namespaceLabel != null && specificLabel != null)
            {
                if (!string.IsNullOrEmpty(namespaceLabel) && !string.IsNullOrEmpty(specificLabel))
                {
                    return $"{namespaceLabel}::{specificLabel}";
                }
                if (string.IsNullOrEmpty(specificLabel))
                    return namespaceLabel;
            }
            return null;
        }
    }
}
