using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Gateway.Interface;
using System.Threading.Tasks;
using System.Linq;
using Domain.ViewModel;
using Microsoft.AspNetCore.Http;

namespace CasaPopularFamilias.Controllers
{
    [ApiController]
    [Route("api")]
    public class HomePointsController : ControllerBase
    {
        private readonly ILogger<HomePointsController> _logger;
        private readonly IHomePointsGateway _gateway;


        public HomePointsController(ILogger<HomePointsController> logger, IHomePointsGateway gateway)
        {
            _logger = logger;
            _gateway = gateway;
        }

        /// <summary>
        /// Gera os pontos de uma familia baseada em seus historico de renda e dependentes
        /// </summary>
        /// <returns>Lista de Familias e seus respectivos pontos</returns>
        [HttpGet("points")]
        public async Task<IActionResult> GetPoints()
        {
            var result = await _gateway.GetFamilyPoints();
            if (result == null)
            {
                return NotFound();
            }
            _logger.LogInformation("Controller/GetPoints/Response:", result);
            return Ok(result);
        }

        /// <summary>
        /// Gera os pontos de uma familia baseada em seu Id, historico de renda e dependentes
        /// </summary>
        /// <param name="familyId">Id da familia</param>
        /// <returns>Objeto com as informações da familia e os pontos</returns>
        [HttpGet("pointsById/{familyId}")]
        public async Task<IActionResult> GetPointsByFamilyId(int familyId)
        {
            _logger.LogInformation("Controller/GetPointsByFamilyId/Request:", familyId);

            var result = await _gateway.GetFamilyPointsById(familyId);
            if (result == null)
            {
                return NotFound();
            }
            _logger.LogInformation("Controller/GetPointsByFamilyId/Response:", result);
            return Ok(result);
        }

        /// <summary>
        /// Insere um objeto familia com renda e dependentes
        /// </summary>
        /// <param name="family">Informações da familia</param>
        /// <returns>Retorna o Id do objeto</returns>
        [HttpPost("family")]
        public async Task<IActionResult> PostFamily(FamilyViewModel family)
        {
            try
            {
                _logger.LogInformation("Controller/PostFamily/Request:", family);
                var result = await _gateway.PostFamily(family);
                _logger.LogInformation("Controller/PostFamily/Response:", result);
                return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
