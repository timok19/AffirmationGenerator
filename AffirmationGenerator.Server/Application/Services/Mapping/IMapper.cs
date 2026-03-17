using AffirmationGenerator.Server.Core;

namespace AffirmationGenerator.Server.Application.Services.Mapping;

public interface IMapper<in TFrom, TTo>
{
    Result<TTo> Map(TFrom value);
}
