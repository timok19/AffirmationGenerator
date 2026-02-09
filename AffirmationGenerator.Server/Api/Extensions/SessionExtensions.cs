namespace AffirmationGenerator.Server.Api.Extensions;

public static class SessionExtensions
{
    extension(ISession session)
    {
        public string RemainingRequestsKey => $"remaining-requests-{session.Id}";
    }
}
