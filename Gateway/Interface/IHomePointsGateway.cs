using CasaPopularFamilias.ViewModel;
using Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Interface
{
    public interface IHomePointsGateway
    {
        Task<IEnumerable<PointsViewModel>> GetFamilyPoints();

        Task<PointsViewModel> GetFamilyPointsById(int familyId);

        Task<int> PostFamily(FamilyViewModel family);
    }
}
