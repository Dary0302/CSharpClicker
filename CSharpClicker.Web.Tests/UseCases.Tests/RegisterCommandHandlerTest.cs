using System.ComponentModel.DataAnnotations;
using CSharpClicker.Web.Domain;
using CSharpClicker.Web.UseCases.Register;
using FakeItEasy;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CSharpClicker.Web.Tests.UseCases.Tests;

public class RegisterCommandHandlerTests
{
    #pragma warning disable NUnit1032
    private UserManager<ApplicationUser> userManager;
    #pragma warning restore NUnit1032
    private RegisterCommandHandler handler;

    [SetUp]
    public void Setup()
    {
        userManager = A.Fake<UserManager<ApplicationUser>>();
        handler = new RegisterCommandHandler(userManager);
    }

    [Test]
    public async Task Handle_ShouldThrowValidationException_WhenUserAlreadyExists()
    {
        var command = new RegisterCommand("existinguser", "password");
        A.CallTo(() => userManager.Users)
            .Returns(new List<ApplicationUser> { new ApplicationUser { UserName = "existinguser" } }.AsQueryable());

        var act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>().WithMessage("Such user already exists.");
    }

    [Test]
    public async Task Handle_ShouldThrowValidationException_WhenUserCreationFails()
    {
        var command = new RegisterCommand("newuser", "password");
        var identityResult = IdentityResult.Failed(new IdentityError { Description = "Error creating user" });
        A.CallTo(() => userManager.Users).Returns(new List<ApplicationUser>().AsQueryable());
        A.CallTo(() => userManager.CreateAsync(A<ApplicationUser>.Ignored, command.Password))
            .Returns(Task.FromResult(identityResult));

        var act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task Handle_ShouldReturnUnit_WhenUserIsCreatedSuccessfully()
    {
        var command = new RegisterCommand("newuser", "password");
        var identityResult = IdentityResult.Success;
        A.CallTo(() => userManager.Users).Returns(new List<ApplicationUser>().AsQueryable());
        A.CallTo(() => userManager.CreateAsync(A<ApplicationUser>.Ignored, command.Password))
            .Returns(Task.FromResult(identityResult));

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
    }
}