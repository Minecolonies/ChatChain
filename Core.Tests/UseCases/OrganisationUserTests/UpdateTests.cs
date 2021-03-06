using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.OrganisationUser;
using Api.Core.DTO.UseCaseResponses.OrganisationUser;
using Api.Core.Entities;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.UseCases.OrganisationUser;
using Api.Tests.MockObjects;
using Moq;
using Xunit;

namespace Api.Tests.UseCases.OrganisationUserTests
{
    public class UpdateTests
    {
        [Fact]
        public async Task UpdateOrganisationUser_True()
        {
            Guid requestedUserId = Guid.NewGuid();

            Guid orgId = Guid.NewGuid();
            //For the first "control" test we'll set all the variables and assert them on the other side.
            IList<OrganisationPermissions> userPermissions = new List<OrganisationPermissions>
                {OrganisationPermissions.All};
            List<OrganisationUser> orgUsers = new List<OrganisationUser>
            {
                new OrganisationUser
                {
                    Id = requestedUserId.ToString(),
                    Permissions = userPermissions
                }
            };

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = orgId,
                Users = orgUsers
            };

            bool updateRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Update(It.Is<Guid>(id => id.Equals(orgId)), It.IsAny<OrganisationDetails>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateOrganisationGatewayResponse(mockOrganisation, true));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            UpdateOrganisationUserUseCase
                useCase = new UpdateOrganisationUserUseCase(mockOrganisationRepository.Object);

            MockOutputPort<UpdateOrganisationUserResponse> mockOutputPort =
                new MockOutputPort<UpdateOrganisationUserResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(
                new UpdateOrganisationUserRequest(orgId, requestedUserId.ToString(), userPermissions), mockOutputPort);

            // Assert \\

            Assert.True(response);
            Assert.True(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Empty(mockOutputPort.Response.Errors);

            Assert.True(updateRan);

            // Assert Value Ids
            Assert.Equal(mockOutputPort.Response.Organisation.Id, orgId);
            Assert.Equal(mockOutputPort.Response.RequestedUser.Id, requestedUserId.ToString());

            // Assert Updated Values
            Assert.Equal(mockOutputPort.Response.RequestedUser.Permissions, userPermissions);
        }

        [Fact]
        public async Task UpdateOrganisationUser_NonExistentOrganisation_False()
        {
            Guid requestedUserId = Guid.NewGuid();

            Guid orgId = Guid.NewGuid();

            bool updateRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Update(It.Is<Guid>(id => id.Equals(orgId)), It.IsAny<OrganisationDetails>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateOrganisationGatewayResponse(new OrganisationDetails(), true));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(null, false,
                    new[] {new Error("404", "Organisation Not Found")}));

            UpdateOrganisationUserUseCase
                useCase = new UpdateOrganisationUserUseCase(mockOrganisationRepository.Object);

            MockOutputPort<UpdateOrganisationUserResponse> mockOutputPort =
                new MockOutputPort<UpdateOrganisationUserResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(
                new UpdateOrganisationUserRequest(orgId, requestedUserId.ToString(),
                    new List<OrganisationPermissions>()), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.RequestedUser);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(updateRan);
        }

        [Fact]
        public async Task UpdateOrganisationUser_NonExistent_False()
        {
            Guid requestedUserId = Guid.NewGuid();

            Guid orgId = Guid.NewGuid();
            //For the first "control" test we'll set all the variables and assert them on the other side.
            string orgOwnerId = Guid.NewGuid().ToString();
            string orgName = Guid.NewGuid().ToString();
            List<OrganisationUser> orgUsers = new List<OrganisationUser>();

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = orgId,
                Owner = orgOwnerId,
                Name = orgName,
                Users = orgUsers
            };

            bool updateRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Update(It.Is<Guid>(id => id.Equals(orgId)), It.IsAny<OrganisationDetails>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateOrganisationGatewayResponse(mockOrganisation, true));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            UpdateOrganisationUserUseCase
                useCase = new UpdateOrganisationUserUseCase(mockOrganisationRepository.Object);

            MockOutputPort<UpdateOrganisationUserResponse> mockOutputPort =
                new MockOutputPort<UpdateOrganisationUserResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(
                new UpdateOrganisationUserRequest(orgId, requestedUserId.ToString(),
                    new List<OrganisationPermissions>()), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.RequestedUser);
            Assert.Contains("404", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation User Not Found", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.False(updateRan);
        }

        [Fact]
        public async Task UpdateOrganisationUser_RepoUpdateErrors_False()
        {
            Guid requestedUserId = Guid.NewGuid();

            Guid orgId = Guid.NewGuid();
            //For the first "control" test we'll set all the variables and assert them on the other side.
            IList<OrganisationPermissions> userPermissions = new List<OrganisationPermissions>
                {OrganisationPermissions.All};
            List<OrganisationUser> orgUsers = new List<OrganisationUser>
            {
                new OrganisationUser
                {
                    Id = requestedUserId.ToString(),
                    Permissions = userPermissions
                }
            };

            OrganisationDetails mockOrganisation = new OrganisationDetails
            {
                Id = orgId,
                Users = orgUsers
            };

            bool updateRan = false;
            Mock<IOrganisationRepository> mockOrganisationRepository = new Mock<IOrganisationRepository>();
            mockOrganisationRepository
                .Setup(repo => repo.Update(It.Is<Guid>(id => id.Equals(orgId)), It.IsAny<OrganisationDetails>()))
                .Callback(() => updateRan = true)
                .ReturnsAsync(new UpdateOrganisationGatewayResponse(null, false,
                    new[] {new Error("500", "Organisation Update Failed")}));
            mockOrganisationRepository
                .Setup(repo => repo.Get(It.Is<Guid>(id => id.Equals(orgId))))
                .ReturnsAsync(new GetOrganisationGatewayResponse(mockOrganisation, true));

            UpdateOrganisationUserUseCase
                useCase = new UpdateOrganisationUserUseCase(mockOrganisationRepository.Object);

            MockOutputPort<UpdateOrganisationUserResponse> mockOutputPort =
                new MockOutputPort<UpdateOrganisationUserResponse>();

            // Act \\

            bool response = await useCase.HandleAsync(
                new UpdateOrganisationUserRequest(orgId, requestedUserId.ToString(),
                    new List<OrganisationPermissions>()), mockOutputPort);

            // Assert \\

            Assert.False(response);
            Assert.False(mockOutputPort.Response.Success);
            Assert.False(mockOutputPort.Response.CheckedPermissions);
            Assert.Null(mockOutputPort.Response.RequestedUser);
            Assert.Contains("500", mockOutputPort.Response.Errors.Select(e => e.Code));
            Assert.Contains("Organisation Update Failed", mockOutputPort.Response.Errors.Select(e => e.Description));

            Assert.True(updateRan);
        }
    }
}