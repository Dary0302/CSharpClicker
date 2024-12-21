using System.Security.Claims;
using CSharpClicker.Web.Infrastructure.Implementations;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace CSharpClicker.Web.Tests.Infrastructure.Implementations.Tests
{
    [TestFixture]
    public class CurrentUserAccessorTests
    {
        private IHttpContextAccessor contextAccessor;
        private CurrentUserAccessor currentUserAccessor;

        [SetUp]
        public void Setup()
        {
            contextAccessor = A.Fake<IHttpContextAccessor>();
            currentUserAccessor = new CurrentUserAccessor(contextAccessor);
        }

        [Test]
        public void GetCurrentUserId_ShouldThrowInvalidOperationException_WhenHttpContextIsNull()
        {
            A.CallTo(() => contextAccessor.HttpContext).Returns(null);

            Action act = () => currentUserAccessor.GetCurrentUserId();

            act.Should().Throw<InvalidOperationException>().WithMessage("Cannot get HTTP context.");
        }

        [Test]
        public void GetCurrentUserId_ShouldThrowInvalidOperationException_WhenUserIdCannotBeParsed()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "invalid-guid") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var user = new ClaimsPrincipal(identity);
            var httpContext = A.Fake<HttpContext>();
            A.CallTo(() => httpContext.User).Returns(user);
            A.CallTo(() => contextAccessor.HttpContext).Returns(httpContext);

            Action act = () => currentUserAccessor.GetCurrentUserId();

            act.Should().Throw<InvalidOperationException>().WithMessage("Cannot parse user ID.");
        }

        [Test]
        public void GetCurrentUserId_ShouldReturnUserId_WhenUserIdIsValid()
        {
            var userId = Guid.NewGuid();
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var user = new ClaimsPrincipal(identity);
            var httpContext = A.Fake<HttpContext>();
            A.CallTo(() => httpContext.User).Returns(user);
            A.CallTo(() => contextAccessor.HttpContext).Returns(httpContext);

            var result = currentUserAccessor.GetCurrentUserId();

            result.Should().Be(userId);
        }
    }
}