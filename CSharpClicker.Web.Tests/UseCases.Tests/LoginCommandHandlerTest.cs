using System.ComponentModel.DataAnnotations;
using CSharpClicker.Web.Domain;
using CSharpClicker.Web.UseCases.Login;
using FakeItEasy;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CSharpClicker.Web.Tests.UseCases.Tests;

public class LoginCommandHandlerTest
{
    private SignInManager<ApplicationUser> signInManager;
    #pragma warning disable NUnit1032
    private UserManager<ApplicationUser> userManager;
    #pragma warning restore NUnit1032
    private LoginCommandHandler handler;

    [SetUp]
    public void Setup()
    {
        userManager = A.Fake<UserManager<ApplicationUser>>();
        signInManager = A.Fake<SignInManager<ApplicationUser>>();
        handler = new LoginCommandHandler(signInManager, userManager);
    }

    [Test]
    public async Task Handle_ShouldThrowValidationException_WhenUserDoesNotExist()
    {
        var command = new LoginCommand("existinguser", "correctpassword");
        A.CallTo(() => userManager.FindByNameAsync(command.UserName))!.Returns(Task.FromResult<ApplicationUser>(null));

        var act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>().WithMessage("Such user does not exists");
    }

    [Test]
    public async Task Handle_ShouldThrowValidationException_WhenPasswordIsIncorrect()
    {
        var user = new ApplicationUser { UserName = "existinguser" };
        var command = new LoginCommand("existinguser", "correctpassword");
        A.CallTo(() => userManager.FindByNameAsync(command.UserName))!.Returns(Task.FromResult(user));
        A.CallTo(() => signInManager.PasswordSignInAsync(user, command.Password, true, false))
            .Returns(Task.FromResult(SignInResult.Failed));

        var act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>().WithMessage("Password or username is not correct.");
    }

    [Test]
    public async Task Handle_ShouldReturnUnit_WhenLoginIsSuccessful()
    {
        var user = new ApplicationUser { UserName = "existinguser" };
        var command = new LoginCommand("existinguser", "correctpassword");
        A.CallTo(() => userManager.FindByNameAsync(command.UserName))!.Returns(Task.FromResult(user));
        A.CallTo(() => signInManager.PasswordSignInAsync(user, command.Password, true, false))
            .Returns(Task.FromResult(SignInResult.Success));

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
    }
}