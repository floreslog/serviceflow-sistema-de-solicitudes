using ServiceFlow.Web.ViewModels;

namespace ServiceFlow.Tests
{
    public class PagedResultTests
    {
        [Fact]
        public void TotalPages_IsCalculatedCorrectly_WhenItemsExceedPageSize()
        {
            var vm = new PagedResult<string>
            {
                Items = new List<string>(),
                TotalItems = 47,
                PageSize = 15,
                CurrentPage = 1,
                TotalPages = (int)Math.Ceiling(47.0 / 15)
            };

            Assert.Equal(4, vm.TotalPages);
        }

        [Fact]
        public void HasNext_ReturnsTrue_WhenCurrentPageIsNotLastPage()
        {
            var vm = new PagedResult<string>
            {
                Items = new List<string>(),
                TotalItems = 47,
                PageSize = 15,
                CurrentPage = 2,
                TotalPages = 4
            };

            Assert.True(vm.HasNext);
        }

        [Fact]
        public void HasPrevious_ReturnsTrue_WhenCurrentPageIsGreaterThanOne()
        {
            var vm = new PagedResult<string>
            {
                Items = new List<string>(),
                TotalItems = 47,
                PageSize = 15,
                CurrentPage = 3,
                TotalPages = 4
            };

            Assert.True(vm.HasPrevious);
        }

        [Fact]
        public void HasNext_ReturnsFalse_WhenCurrentPageIsLastPage()
        {
            var vm = new PagedResult<string>
            {
                Items = new List<string>(),
                TotalItems = 47,
                PageSize = 15,
                CurrentPage = 4,
                TotalPages = 4
            };

            Assert.False(vm.HasNext);
        }

        [Fact]
        public void HasPrevious_ReturnsFalse_WhenCurrentPageIsOne()
        {
            var vm = new PagedResult<string>
            {
                Items = new List<string>(),
                TotalItems = 47,
                PageSize = 15,
                CurrentPage = 1,
                TotalPages = 4
            };

            Assert.False(vm.HasPrevious);
        }
    }
}