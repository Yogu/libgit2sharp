using System;
using LibGit2Sharp.Tests.TestHelpers;
using Xunit;
using Xunit.Extensions;

namespace LibGit2Sharp.Tests
{
    public class RefSpecFixture : BaseFixture
    {
        [Fact]
        public void CanCountRefSpecs()
        {
            var path = CloneStandardTestRepo();
            using (var repo = new Repository(path))
            {
                var remote = repo.Network.Remotes["origin"];
                Assert.Equal(1, remote.RefSpecs.Count);
            }
        }

        [Fact]
        public void CanGetRefSpecByIndex()
        {
            var path = CloneStandardTestRepo();
            using (var repo = new Repository(path))
            {
                var remote = repo.Network.Remotes["origin"];
                RefSpec refSpec = remote.RefSpecs[0];
                Assert.NotNull(refSpec);
                Assert.NotNull(refSpec.Specification);
                Assert.NotEmpty(refSpec.Specification);
            }
        }

        [Fact]
        public void CanIterateOverRefSpecs()
        {
            var path = CloneStandardTestRepo();
            using (var repo = new Repository(path))
            {
                var remote = repo.Network.Remotes["origin"];
                int count = 0;
                foreach (RefSpec refSpec in remote.RefSpecs)
                {
                    Assert.NotNull(refSpec);
                    count++;
                }
                Assert.Equal(remote.RefSpecs.Count, count);
            }
        }

        [Fact]
        public void CanAddFetchRefSpec()
        {
            var path = CloneStandardTestRepo();
            using (var repo = new Repository(path))
            {
                string specification = "+refs/theirs/*:refs/my/*";

                var remote = repo.Network.Remotes["origin"];
                remote.RefSpecs.AddFetch(specification);
                Assert.Equal(2, remote.RefSpecs.Count);

                RefSpec refSpec = remote.RefSpecs[1];
                Assert.NotNull(refSpec);
                Assert.Equal(specification, refSpec.Specification);
                Assert.Equal(RefSpecDirection.Fetch, refSpec.Direction);
            }
        }

        [Fact]
        public void CanAddPushRefSpec()
        {
            var path = CloneStandardTestRepo();
            using (var repo = new Repository(path))
            {
                string specification = "+refs/my/*:refs/theirs/*";

                var remote = repo.Network.Remotes["origin"];
                remote.RefSpecs.AddPush(specification);
                Assert.Equal(2, remote.RefSpecs.Count);

                RefSpec refSpec = remote.RefSpecs[1];
                Assert.NotNull(refSpec);
                Assert.Equal(specification, refSpec.Specification);
                Assert.Equal(RefSpecDirection.Push, refSpec.Direction);
            }
        }

        [Theory]
        [InlineData(true, "refs/theirs/*", "refs/my/*")]
        [InlineData(false, "refs/abc/def", "refs/something/else")]
        public void CanReadRefSpecDetails(bool forceUpdate, string source, string destination)
        {
            var path = CloneStandardTestRepo();
            using (var repo = new Repository(path))
            {
                string specification = string.Format("{0}{1}:{2}", forceUpdate ? "+" : "", source, destination);

                var remote = repo.Network.Remotes["origin"];
                remote.RefSpecs.AddFetch(specification);

                RefSpec refSpec = remote.RefSpecs[1];
                Assert.NotNull(refSpec);

                Assert.Equal(source, refSpec.Source);
                Assert.Equal(destination, refSpec.Destination);
                Assert.Equal(forceUpdate, refSpec.ForceUpdate);
            }
        }

        [Fact]
        public void CanClearRefSpecs()
        {
            var path = CloneStandardTestRepo();
            using (var repo = new Repository(path))
            {
                var remote = repo.Network.Remotes["origin"];
                remote.RefSpecs.Clear();
                Assert.Equal(0, remote.RefSpecs.Count);
            }
        }

        [Fact]
        public void CanRemoveRefSpec()
        {
            var path = CloneStandardTestRepo();
            using (var repo = new Repository(path))
            {
                var remote = repo.Network.Remotes["origin"];
                remote.RefSpecs.Remove(0);
                Assert.Equal(0, remote.RefSpecs.Count);
            }
        }

        [Fact]
        public void GettingInvalidRefSpecThrows()
        {
            var path = CloneStandardTestRepo();
            using (var repo = new Repository(path))
            {
                var remote = repo.Network.Remotes["origin"];
                Assert.Throws<ArgumentOutOfRangeException>(() => remote.RefSpecs[-1]);
                Assert.Throws<ArgumentOutOfRangeException>(() => remote.RefSpecs[10]);
            }
        }

        [Fact]
        public void RemovingInvalidRefSpecThrows()
        {
            var path = CloneStandardTestRepo();
            using (var repo = new Repository(path))
            {
                var remote = repo.Network.Remotes["origin"];
                Assert.Throws<ArgumentOutOfRangeException>(() => remote.RefSpecs.Remove(-1));
                Assert.Throws<ArgumentOutOfRangeException>(() => remote.RefSpecs.Remove(10));
            }
        }
    }
}
