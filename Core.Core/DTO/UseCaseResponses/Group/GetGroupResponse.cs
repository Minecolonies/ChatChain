using System.Collections.Generic;
using Api.Core.Interfaces;

namespace Api.Core.DTO.UseCaseResponses.Group
{
    public class GetGroupResponse : DefaultUseCaseResponseMessage
    {
        public Entities.Group Group { get; }

        public GetGroupResponse(IEnumerable<Error> errors, bool checkedPermissions = false, bool success = false,
            string message = null) :
            base(errors, checkedPermissions, success, message)
        {
        }

        public GetGroupResponse(Entities.Group group, Entities.Organisation organisation,
            Entities.OrganisationUser user, bool checkedPermissions = false, bool success = false,
            string message = null) : base(organisation, user, checkedPermissions, success, message)
        {
            Group = group;
        }
    }
}