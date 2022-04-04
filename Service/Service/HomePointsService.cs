using AutoMapper;
using CasaPopularFamilias.ViewModel;
using Domain.DTOs;
using Domain.ViewModel;
using ExternalServices.Facades;
using Gateway.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Service
{
    public class HomePointsService : IHomePointsGateway
    {
        private readonly IHomeFamilyPointsFacade _facade;
        private readonly ILogger<HomePointsService> _log;
        private readonly IMapper _mapper;
        private Dictionary<string, string> header = new Dictionary<string, string>() { { "autorization", "value" } };

        public HomePointsService(IHomeFamilyPointsFacade facade, ILogger<HomePointsService> log, IMapper mapper)
        {
            _log = log;
            _facade = facade;
            _mapper = mapper;
        }


        /// <summary>
        /// Obtem uma lista de familias ordernada e calcula seus pontos
        /// </summary>
        /// <returns>O Id e os pontos, ordernado por pontos</returns>
        public async Task<IEnumerable<PointsViewModel>> GetFamilyPoints()
        {
            try
            {
                var result = await _facade.GetFamilyInformation(header);

                if (result != null)
                {
                    var familyPoints = TotalPoints(result.Content);

                    return familyPoints.OrderBy(p => p.PointsFamily);
                }
                return new List<PointsViewModel>();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtem um objeto familia através do id e calcula os pontos
        /// </summary>
        /// <param name="familyId">Id da familia</param>
        /// <returns>Objeto com os pontos e Id</returns>
        public async Task<PointsViewModel> GetFamilyPointsById(int familyId)
        {
            try
            {
                var result = await _facade.GetFamilyById(header, familyId);

                if (result.Content != null)
                {
                    var familyPoints = TotalPointsByFamily(result.Content);
                    return familyPoints;
                }
                return new PointsViewModel();

            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Insere as informações da familia através do objeto
        /// </summary>
        /// <param name="family">Informações da familia</param>
        /// <returns>Id</returns>
        public async Task<int> PostFamily(FamilyViewModel family)
        {
            try
            {
                var familyExt = _mapper.Map<FamilyInformationExternal>(family);
                if (familyExt.Income > 0 || familyExt.FamilyMembers.ToList().Count > 0)
                {
                    var fam = new FamilyInformationExternal();
                    var result = await _facade.PostHomeFamilyPoints(header, fam);

                    return result.Content;
                }
                return 0;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return 0;
            }
        }


        /// <summary>
        /// Calcula o total de pontos de uma lista de familias baseado na renda e nos dependentes
        /// Sei que esse método não era necessário, dado que o de baixo faz a mesma coisa só pra um objeto, mas quis colocar um exemplo de foreach separado do método principal
        /// </summary>
        /// <param name="families">Lista de familias</param>
        /// <returns>Lista de Id de familias e pontos</returns>
        private IEnumerable<PointsViewModel> TotalPoints(IEnumerable<FamilyInformationExternal> families)
        {
            List<PointsViewModel> points = new List<PointsViewModel>();

            foreach (var family in families)
            {
                var point = new PointsViewModel();
                point.PointsFamily = GetPointsDependents((List<FamilyMembersExternal>)family.FamilyMembers);
                point.Id = family.Id;
                switch (family.Income)
                {
                    case <= 900:
                        point.PointsFamily = +5;
                        break;
                    case >= 901:
                    case <= 1500:
                        point.PointsFamily = +3;
                        break;
                    default:
                }
                points.Add(point);
            }
            return points;
        }

        /// <summary>
        /// Calcula o total de pontos de uma familia baseado na renda e nos dependentes
        /// </summary>
        /// <param name="family">Objeto preenchido com as infos da famlia</param>
        /// <returns>Id e pontos calculados</returns>
        private PointsViewModel TotalPointsByFamily(FamilyInformationExternal family)
        {
            var point = new PointsViewModel();
            point.PointsFamily = GetPointsDependents((List<FamilyMembersExternal>)family.FamilyMembers);
            point.Id = family.Id;
            switch (family.Income)
            {
                case <= 900:
                    point.PointsFamily = +5;
                    break;
                case >= 901:
                case <= 1500:
                    point.PointsFamily = +3;
                    break;
                default:
            }
            return point;
        }

        /// <summary>
        /// Calcula os pontos por dependentes de uma lista de familias
        /// </summary>
        /// <param name="lstDependents">Lista de membros da familia</param>
        /// <returns>Pontos calulados por uma lista de dependentes</returns>
        private int GetPointsDependents(List<FamilyMembersExternal> lstDependents)
        {
            var numberDependent = lstDependents.Where(d => d.Dependent.Age < 18 && d.IsDependent).Count();
            var points = 0;
            switch (numberDependent)
            {
                case >= 3:
                    points = 3;
                    break;
                case >= 1:
                case <= 2:
                    points = 2;
                    break;
            }
            return points;
        }

    }
}

