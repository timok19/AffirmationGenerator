using AffirmationGenerator.Server.Application.Queries;
using AffirmationGenerator.Server.Application.Services.Affirmation;
using AffirmationGenerator.Server.Tests.Extensions;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace AffirmationGenerator.Server.Tests.Application.Queries;

[TestFixture]
public sealed class GetRemainingAffirmationsQueryTests : TestBase
{
    private IAffirmationService _affirmationService = null!;
    private GetRemainingAffirmationsQuery _query = null!;

    [SetUp]
    public void SetUp()
    {
        _affirmationService = Substitute.For<IAffirmationService>();
        _query = new GetRemainingAffirmationsQuery(_affirmationService);
    }

    [TestCase(255)]
    [TestCase(100)]
    [TestCase(10)]
    [TestCase(0)]
    public async Task Handle_ShouldReturnRemainingAffirmationsCount(int maxRequestsPerDay)
    {
        // Arrange
        _affirmationService.GetRemainingAffirmationsCount().Returns(maxRequestsPerDay);

        // Act
        var result = await _query.Handle();

        // Assert
        var response = result.ShouldBeSuccess();
        response.RemainingCount.ShouldBe(maxRequestsPerDay);
        response.RemainingCount.ShouldBeGreaterThanOrEqualTo(0);

        await _affirmationService.Received(1).GetRemainingAffirmationsCount();
    }
}
