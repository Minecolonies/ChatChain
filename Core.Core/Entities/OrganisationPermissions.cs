namespace Api.Core.Entities
{
    public enum OrganisationPermissions
    {
        //EDIT
        EditClients,
        EditGroups,
        EditOrgUsers,
        EditOrg,

        //CREATE
        CreateClients,
        CreateGroups,
        CreateOrgUsers,

        //DELETE
        DeleteClients,
        DeleteGroups,
        DeleteOrgUsers,

        //ALL
        All
    }
}