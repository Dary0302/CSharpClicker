using CSharpClicker.Web.Domain;
using CSharpClicker.Web.UseCases.Logout;
using FakeItEasy;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CSharpClicker.Web.Tests.UseCases.Tests;

public class LogoutCommandHandlerTests
{
    private SignInManager<ApplicationUser> signInManager;
    private LogoutCommandHandler handler;

    [SetUp]
    public void Setup()
    {
        signInManager = A.Fake<SignInManager<ApplicationUser>>();
        handler = new LogoutCommandHandler(signInManager);
    }

    [Test]
    public async Task Handle_ShouldSignOutUser()
    {
        var command = new LogoutCommand();
        var cancellationToken = new CancellationToken();

        var result = await handler.Handle(command, cancellationToken);

        A.CallTo(() => signInManager.SignOutAsync()).MustHaveHappenedOnceExactly();
        result.Should().Be(Unit.Value);
    }
}