using AffirmationGenerator.Server.Application.Queries;
using AffirmationGenerator.Server.Domain;
using AffirmationGenerator.Server.Tests.Extensions;
using NUnit.Framework;
using Shouldly;

namespace AffirmationGenerator.Server.Tests.Application.Queries;

[TestFixture]
public sealed class GetAffirmationLanguagesQueryTests : TestBase
{
    private GetAffirmationLanguagesQuery _query = null!;

    [SetUp]
    public void SetUp()
    {
        _query = new GetAffirmationLanguagesQuery();
    }

    [Test]
    public void Handle_ShouldReturnSortedLanguages()
    {
        // Act
        var result = _query.Handle();

        // Assert
        var response = result.ShouldBeSuccess();

        response.Languages.Count.ShouldBe(Enum.GetValues<AffirmationLanguage>().Length);
        response.Languages.ShouldBeInOrder(SortDirection.Ascending);
    }
}
