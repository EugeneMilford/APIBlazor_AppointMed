using AppointMed.API.Controllers;
using AppointMed.API.Data;
using AppointMed.API.Models.Status;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppointMed.Tests.BackEndTests
{
    public class StatusControllerTests
    {
        private readonly Mock<DbSet<Status>> _mockStatusDbSet;
        private readonly Mock<AppointMedDbContext> _mockContext;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<StatusController>> _mockLogger;
        private readonly StatusController _controller;

        public StatusControllerTests()
        {
            _mockStatusDbSet = new Mock<DbSet<Status>>();
            _mockContext = new Mock<AppointMedDbContext>(
                new DbContextOptionsBuilder<AppointMedDbContext>().Options);
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<StatusController>>();

            _mockContext.Setup(c => c.Statuses).Returns(_mockStatusDbSet.Object);

            _controller = new StatusController(
                _mockContext.Object,
                _mockMapper.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task GetStatuses_ReturnsAllStatuses()
        {
            // Arrange
            var statuses = new List<Status>
            {
                new Status { StatusId = 1, StatusName = "Pending", DisplayOrder = 1, IsActive = true },
                new Status { StatusId = 2, StatusName = "Confirmed", DisplayOrder = 2, IsActive = true },
                new Status { StatusId = 3, StatusName = "Cancelled", DisplayOrder = 3, IsActive = false }
            };

            var statusDtos = new List<StatusDto>
            {
                new StatusDto { StatusId = 1, StatusName = "Pending", IsActive = true, DisplayOrder = 1 },
                new StatusDto { StatusId = 2, StatusName = "Confirmed", IsActive = true, DisplayOrder = 2 },
                new StatusDto { StatusId = 3, StatusName = "Cancelled", IsActive = false, DisplayOrder = 3 }
            };

            var mockDbSet = CreateAsyncMockDbSet(statuses);
            _mockContext.Setup(c => c.Statuses).Returns(mockDbSet.Object);
            _mockMapper.Setup(m => m.Map<IEnumerable<StatusDto>>(It.IsAny<List<Status>>()))
                .Returns(statusDtos);

            // Act
            var result = await _controller.GetStatuses();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<StatusDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<StatusDto>>(okResult.Value);
            Assert.Equal(3, returnValue.Count);
        }

        [Fact]
        public async Task GetActiveStatuses_ReturnsOnlyActiveStatuses()
        {
            // Arrange
            var statuses = new List<Status>
            {
                new Status { StatusId = 1, StatusName = "Pending", DisplayOrder = 1, IsActive = true },
                new Status { StatusId = 2, StatusName = "Confirmed", DisplayOrder = 2, IsActive = true }
            };

            var activeStatusDtos = new List<StatusDto>
            {
                new StatusDto { StatusId = 1, StatusName = "Pending", IsActive = true, DisplayOrder = 1 },
                new StatusDto { StatusId = 2, StatusName = "Confirmed", IsActive = true, DisplayOrder = 2 }
            };

            var mockDbSet = CreateAsyncMockDbSet(statuses);
            _mockContext.Setup(c => c.Statuses).Returns(mockDbSet.Object);
            _mockMapper.Setup(m => m.Map<IEnumerable<StatusDto>>(It.IsAny<List<Status>>()))
                .Returns(activeStatusDtos);

            // Act
            var result = await _controller.GetActiveStatuses();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<StatusDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<StatusDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
            Assert.All(returnValue, s => Assert.True(s.IsActive));
        }

        [Fact]
        public async Task GetStatus_WithValidId_ReturnsStatus()
        {
            // Arrange
            var status = new Status
            {
                StatusId = 1,
                StatusName = "Pending",
                DisplayOrder = 1,
                IsActive = true
            };

            var statusDto = new StatusDto
            {
                StatusId = 1,
                StatusName = "Pending",
                DisplayOrder = 1,
                IsActive = true
            };

            _mockStatusDbSet.Setup(d => d.FindAsync(1)).ReturnsAsync(status);
            _mockMapper.Setup(m => m.Map<StatusDto>(status)).Returns(statusDto);

            // Act
            var result = await _controller.GetStatus(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<StatusDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<StatusDto>(okResult.Value);
            Assert.Equal(1, returnValue.StatusId);
            Assert.Equal("Pending", returnValue.StatusName);
        }

        [Fact]
        public async Task GetStatus_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockStatusDbSet.Setup(d => d.FindAsync(999)).ReturnsAsync((Status)null);

            // Act
            var result = await _controller.GetStatus(999);

            // Assert
            var actionResult = Assert.IsType<ActionResult<StatusDto>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task PutStatus_WithMismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var updateStatusDto = new UpdateStatusDto { Id = 1 };

            // Act
            var result = await _controller.PutStatus(2, updateStatusDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Status ID mismatch", badRequestResult.Value);
        }

        [Fact]
        public async Task PutStatus_WithNonExistentStatus_ReturnsNotFound()
        {
            // Arrange
            var updateStatusDto = new UpdateStatusDto { Id = 999 };

            _mockStatusDbSet.Setup(d => d.FindAsync(999)).ReturnsAsync((Status)null);

            // Act
            var result = await _controller.PutStatus(999, updateStatusDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        private Mock<DbSet<T>> CreateAsyncMockDbSet<T>(List<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(queryable.Provider));
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            mockSet.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));

            return mockSet;
        }
    }

    // Helper classes for async query support
    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(System.Linq.Expressions.Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(System.Linq.Expressions.Expression expression)
        {
            return new TestAsyncEnumerable<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken)
        {
            var resultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = typeof(IQueryProvider)
                .GetMethod(
                    name: nameof(IQueryProvider.Execute),
                    genericParameterCount: 1,
                    types: new[] { typeof(System.Linq.Expressions.Expression) })
                .MakeGenericMethod(resultType)
                .Invoke(this, new[] { expression });

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                .MakeGenericMethod(resultType)
                .Invoke(null, new[] { executionResult });
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestAsyncEnumerable(System.Linq.Expressions.Expression expression)
            : base(expression)
        { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return ValueTask.FromResult(_inner.MoveNext());
        }

        public T Current => _inner.Current;
    }
}