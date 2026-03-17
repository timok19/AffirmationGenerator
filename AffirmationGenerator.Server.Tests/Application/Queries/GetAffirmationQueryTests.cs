using AffirmationGenerator.Server.Application.Models;
using AffirmationGenerator.Server.Application.Queries;
using AffirmationGenerator.Server.Application.Services.Affirmation;
using AffirmationGenerator.Server.Application.Services.Mapping;
using AffirmationGenerator.Server.Domain;
using AffirmationGenerator.Server.Infrastructure.DeepL;
using AffirmationGenerator.Server.Tests.Extensions;
using DeepL;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace AffirmationGenerator.Server.Tests.Application.Queries;

[TestFixture]
public sealed class GetAffirmationQueryTests : TestBase
{
    private IDeepLTranslatorClient _translatorClient = null!;
    private IMapper<AffirmationLanguage, string> _affirmationLanguageMapper = null!;
    private IAffirmationService _affirmationService = null!;
    private GetAffirmationQuery _query = null!;

    private const int MaxRequestsPerDay = 10;

    [SetUp]
    public void SetUp()
    {
        _translatorClient = Substitute.For<IDeepLTranslatorClient>();
        _affirmationLanguageMapper = Substitute.For<IMapper<AffirmationLanguage, string>>();
        _affirmationService = Substitute.For<IAffirmationService>();
        _query = new GetAffirmationQuery(_translatorClient, _affirmationService, _affirmationLanguageMapper);
    }

    [Test]
    public async Task Handle_WhenLanguageIsEnglish_ShouldReturnUntranslatedAffirmation()
    {
        // Arrange
        const string affirmationText = "Good day!";

        _affirmationService.GetAffirmation().ReturnsSuccess(affirmationText);

        var getAffirmationRequest = new GetAffirmationRequest { TargetLanguage = AffirmationLanguage.English };

        _affirmationLanguageMapper.Map(getAffirmationRequest.TargetLanguage).ReturnsSuccess(LanguageCode.English);

        // Act
        var result = await _query.Handle(getAffirmationRequest);

        // Assert
        var response = result.ShouldBeSuccess();

        response.TargetLanguage.ShouldBe(AffirmationLanguage.English);
        response.Text.ShouldBe(affirmationText);
        response.RemainingCount.ShouldBeLessThan(MaxRequestsPerDay);

        await _affirmationService.Received(1).GetAffirmation();
        _affirmationLanguageMapper.Received(1).Map(AffirmationLanguage.English);
        await _translatorClient.DidNotReceiveWithAnyArgs().Translate(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Handle_WhenLanguageIsGerman_ShouldReturnTranslatedAffirmationInGerman()
    {
        // Arrange
        const string affirmationText = "Good day!";
        const string affirmationTextInGerman = "Guten Tag!";

        _affirmationService.GetAffirmation().ReturnsSuccess(affirmationText);

        var getAffirmationRequest = new GetAffirmationRequest { TargetLanguage = AffirmationLanguage.German };

        _affirmationLanguageMapper.Map(getAffirmationRequest.TargetLanguage).ReturnsSuccess(LanguageCode.German);
        _translatorClient.Translate(affirmationText, LanguageCode.English, LanguageCode.German).Returns(affirmationTextInGerman);

        // Act
        var result = await _query.Handle(getAffirmationRequest);

        // Assert
        var response = result.ShouldBeSuccess();

        response.TargetLanguage.ShouldBe(AffirmationLanguage.German);
        response.Text.ShouldBe(affirmationTextInGerman);
        response.RemainingCount.ShouldBeLessThan(MaxRequestsPerDay);

        await _affirmationService.Received(1).GetAffirmation();
        _affirmationLanguageMapper.Received(1).Map(AffirmationLanguage.German);
        await _translatorClient.Received(1).Translate(affirmationText, LanguageCode.English, LanguageCode.German);
    }

    [Test]
    public async Task Handle_WhenNoAffirmation_ShouldReturnError()
    {
        // Arrange
        _affirmationService.GetAffirmation().ReturnsError<string, AffirmationNotFound>();

        var getAffirmationRequest = new GetAffirmationRequest { TargetLanguage = AffirmationLanguage.German };

        // Act
        var result = await _query.Handle(getAffirmationRequest);

        // Assert
        result.ShouldBeError().ShouldBeOfType<AffirmationNotFound>();

        await _affirmationService.Received(1).GetAffirmation();
        _affirmationLanguageMapper.DidNotReceiveWithAnyArgs().Map(Arg.Any<AffirmationLanguage>());
        await _translatorClient.DidNotReceiveWithAnyArgs().Translate(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }
}
