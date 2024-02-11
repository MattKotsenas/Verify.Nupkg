using FluentAssertions;

namespace Verify.Nupkg.Tests
{
    public class NuspecScrubberTests
    {
        [Theory]
        [InlineData("    <repository type=\"git\" url=\"https://github.com/MattKotsenas/Verify.Nupkg.git\" commit=\"3143135c18bb9bc04b35550ea77189e806738d46\" />", "    <repository type=\"git\" url=\"https://github.com/MattKotsenas/Verify.Nupkg.git\" commit=\"3143135c18bb9bc04b35550ea77189e806738d46\" />")]
        [InlineData("    <repository type=\"git\" url=\"https://github.com/MattKotsenas/Verify.Nupkg\" commit=\"3143135c18bb9bc04b35550ea77189e806738d46\" />", "    <repository type=\"git\" url=\"https://github.com/MattKotsenas/Verify.Nupkg.git\" commit=\"3143135c18bb9bc04b35550ea77189e806738d46\" />")]
        [InlineData("    <repository type=\"git\" url=\"file://github.com/MattKotsenas/Verify.Nupkg\" commit=\"3143135c18bb9bc04b35550ea77189e806738d46\" />", "    <repository type=\"git\" url=\"file://github.com/MattKotsenas/Verify.Nupkg\" commit=\"3143135c18bb9bc04b35550ea77189e806738d46\" />")]
        [InlineData("    <repository type=\"git\" url=\"https://bitbucket.org/the_best/awesome_repo\" commit=\"3143135c18bb9bc04b35550ea77189e806738d46\" />", "    <repository type=\"git\" url=\"https://bitbucket.org/the_best/awesome_repo\" commit=\"3143135c18bb9bc04b35550ea77189e806738d46\" />")]
        public void ScrubRepositoryUrlNormalizesDotGitForGitHubUrls(string line, string expected)
        {
            var scrubber = new NuspecScrubber();

            string actual = scrubber.ScrubRepositoryUrl(line);

            actual.Should().Be(expected);
        }
    }
}
