using AutoMapper;
using CSharpClicker.Web.Domain;
using CSharpClicker.Web.Infrastructure.Abstractions;
using CSharpClicker.Web.UseCases.GetCurrentUser;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CSharpClicker.Web.Tests.UseCases.Tests;

public class GetCurrentUserQueryHandlerTests
{
    private Mock<ICurrentUserAccessor> currentUserAccessorMock;
    private Mock<IAppDbContext> appDbContextMock;
    private Mock<IMapper> mapperMock;
    private GetCurrentUserQueryHandler handler;

    [SetUp]
    public void Setup()
    {
        currentUserAccessorMock = new Mock<ICurrentUserAccessor>();
        appDbContextMock = new Mock<IAppDbContext>();
        mapperMock = new Mock<IMapper>();
        handler = new GetCurrentUserQueryHandler(currentUserAccessorMock.Object,
            appDbContextMock.Object,
            mapperMock.Object);
    }

    [Test]
    public void Handle_ShouldThrowException_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();

        var fakeUsers = new List<ApplicationUser>().AsQueryable();

        var dbSetMock = new Mock<DbSet<ApplicationUser>>();
        dbSetMock.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(fakeUsers.Provider);
        dbSetMock.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(fakeUsers.Expression);
        dbSetMock.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(fakeUsers.ElementType);
        using var enumerator = fakeUsers.GetEnumerator();
        dbSetMock.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(enumerator);

        currentUserAccessorMock.Setup(m => m.GetCurrentUserId()).Returns(userId);
        appDbContextMock.Setup(m => m.ApplicationUsers).Returns(dbSetMock.Object);

        var act = async () => await handler.Handle(new GetCurrentUserQuery(), CancellationToken.None);

        act.Should().ThrowAsync<InvalidOperationException>();
    }
}