using Api.Core.DTO.UseCaseRequests.Client;
using Api.Core.DTO.UseCaseResponses.Client;

namespace Api.Core.Interfaces.UseCases.Client
{
    public interface IUpdateClientUseCase : IUseCaseRequestHandler<UpdateClientRequest, UpdateClientResponse>
    {
    }
}