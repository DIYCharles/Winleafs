using System.Linq;
using System.Threading.Tasks;
using Octokit;
using Winleafs.External.interfaces;

namespace Winleafs.External
{
    public class ReleaseClient : IReleaseClient
    {
        private const string USERNAME = "StijnOostdam";
        private const string REPOSITORY_NAME = "Winleafs";

        /// <inheritdoc />
        public async Task<string> GetLatestVersion(bool usePreRelease = false)
        {
            var githubClient = new GitHubClient(new ProductHeaderValue("winleafs"));
            if (usePreRelease)
            {
                var releases = await githubClient.Repository.Release.GetAll(USERNAME, REPOSITORY_NAME);
                var release = releases.OrderByDescending(x => x.CreatedAt).First();
                return release.TagName;
            }

            return (await githubClient.Repository.Release.GetLatest(USERNAME, REPOSITORY_NAME).ConfigureAwait(false)).TagName;
        }
    }
}