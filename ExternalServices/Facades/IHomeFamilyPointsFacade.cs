using Domain.DTOs;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExternalServices.Facades
{
    public interface IHomeFamilyPointsFacade
    {
        [Get("/v1/family/getFamily")]
        Task<ApiResponse<IEnumerable<FamilyInformationExternal>>> GetFamilyInformation([HeaderCollection] IDictionary<string, string> header);

        [Get("/v1/family/{familyId}/getFamilyById")]
        Task<ApiResponse<FamilyInformationExternal>> GetFamilyById([HeaderCollection] IDictionary<string, string> header, int familyId);

        [Post("/v1/family/postFamily")]
        Task<ApiResponse<int>> PostHomeFamilyPoints([HeaderCollection] IDictionary<string, string> header, [Body] FamilyInformationExternal family);

    }
}
