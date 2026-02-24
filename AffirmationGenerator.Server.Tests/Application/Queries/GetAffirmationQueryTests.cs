using AffirmationGenerator.Server.Application;
using AffirmationGenerator.Server.Application.Models;
using AffirmationGenerator.Server.Application.Queries;
using AffirmationGenerator.Server.Application.Services.Affirmation;
using AffirmationGenerator.Server.Application.Services.Language;
using AffirmationGenerator.Server.Core;
using AffirmationGenerator.Server.Domain;
using AffirmationGenerator.Server.Infrastructure.DeepL;
using AffirmationGenerator.Server.Tests.Extensions;
using DeepL;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace AffirmationGenerator.Server.Tests.Application.Queries;

[TestFixture]
public sealed class GetAffirmationQueryTests : TestBase
{
    private IDeepLTranslatorClient _translatorClient = null!;
    private ILanguageCodeMapper<AffirmationLanguage> _languageCodeMapper = null!;
    private IAffirmationService _affirmationService = null!;
    private IOptions<ClientOptions> _apiOptions = null!;
    private GetAffirmationQuery _query = null!;

    [SetUp]
    public void SetUp()
    {
        _translatorClient = Substitute.For<IDeepLTranslatorClient>();
        _languageCodeMapper = Substitute.For<ILanguageCodeMapper<AffirmationLanguage>>();
        _affirmationService = Substitute.For<IAffirmationService>();
        _apiOptions = Options.Create(new ClientOptions { ClientIpHeaderName = "X-Forwarded-For", MaxRequestsPerDay = 10 });
        _query = new GetAffirmationQuery(_translatorClient, _affirmationService, _languageCodeMapper);
    }

    [Test]
    public async Task Handle_WhenLanguageIsEnglish_ShouldReturnUntranslatedAffirmation()
    {
        // Arrange
        const string affirmationText = "Good day!";

        _affirmationService.Get().Returns(affirmationText);

        var getAffirmationRequest = new GetAffirmationRequest { TargetLanguage = AffirmationLanguage.English };

        _languageCodeMapper.Map(getAffirmationRequest.TargetLanguage).Returns(LanguageCode.English);

        // Act
        var result = await _query.Handle(getAffirmationRequest);

        // Assert
        var response = result.ShouldBeSuccess();

        response.TargetLanguage.ShouldBe(AffirmationLanguage.English);
        response.Text.ShouldBe(affirmationText);
        response.RemainingCount.ShouldBeLessThan(_apiOptions.Value.MaxRequestsPerDay);

        await _affirmationService.Received(1).Get();
        _languageCodeMapper.Received(1).Map(AffirmationLanguage.English);
        await _translatorClient.DidNotReceiveWithAnyArgs().Translate(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Handle_WhenLanguageIsGerman_ShouldReturnTranslatedAffirmationInGerman()
    {
        // Arrange
        const string affirmationText = "Good day!";
        const string affirmationTextInGerman = "Guten Tag!";

        _affirmationService.Get().Returns(affirmationText);

        var getAffirmationRequest = new GetAffirmationRequest { TargetLanguage = AffirmationLanguage.German };

        _languageCodeMapper.Map(getAffirmationRequest.TargetLanguage).Returns(LanguageCode.German);
        _translatorClient.Translate(affirmationText, LanguageCode.English, LanguageCode.German).Returns(affirmationTextInGerman);

        // Act
        var result = await _query.Handle(getAffirmationRequest);

        // Assert
        var response = result.ShouldBeSuccess();

        response.TargetLanguage.ShouldBe(AffirmationLanguage.German);
        response.Text.ShouldBe(affirmationTextInGerman);
        response.RemainingCount.ShouldBeLessThan(_apiOptions.Value.MaxRequestsPerDay);

        await _affirmationService.Received(1).Get();
        _languageCodeMapper.Received(1).Map(AffirmationLanguage.German);
        await _translatorClient.Received(1).Translate(affirmationText, LanguageCode.English, LanguageCode.German);
    }

    [Test]
    public async Task Handle_WhenNoAffirmation_ShouldReturnError()
    {
        // Arrange
        _affirmationService.Get().Returns(Result<string>.Error(new AffirmationNotFound()));

        var getAffirmationRequest = new GetAffirmationRequest { TargetLanguage = AffirmationLanguage.German };

        // Act
        var result = await _query.Handle(getAffirmationRequest);

        // Assert
        result.ShouldBeError().ShouldBeOfType<AffirmationNotFound>();

        await _affirmationService.Received(1).Get();
        _languageCodeMapper.DidNotReceiveWithAnyArgs().Map(Arg.Any<AffirmationLanguage>());
        await _translatorClient.DidNotReceiveWithAnyArgs().Translate(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }
}
