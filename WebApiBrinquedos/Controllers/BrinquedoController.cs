using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiBrinquedos.Context;
using WebApiBrinquedos.Entity;

namespace WebApiBrinquedos.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BrinquedoController(ILogger<BrinquedoController> logger, BrinquedoDbContext context) : ControllerBase
    {
        private readonly BrinquedoDbContext _context = context;
        private readonly ILogger<BrinquedoController> _logger = logger;

        //private static readonly string[] Summaries = new[]
        //{
        //    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        //};


        [HttpPost("Adicionar")] // Use HttpPost para adicionar um brinquedo
        public ActionResult<Brinquedo> AdicionarBrinquedo([FromBody] Brinquedo novoBrinquedo)
        {
            //var novoBrinquedo = new Brinquedo { Nome = "Carrinho", Cor = "Vermelho" };
            _context.Brinquedos.Add(novoBrinquedo);
            _context.SaveChanges(); // Salva as mudanças no banco de dados

            return CreatedAtAction(nameof(AdicionarBrinquedo), new { id = novoBrinquedo.Id }, novoBrinquedo);
        }


        [HttpGet("Listar")]
        public ActionResult<IEnumerable<Brinquedo>> ListarBrinquedos([FromQuery] BrinquedoRequest filtro)
        {
            // Verificar se todos os campos são vazios ou inválidos
            if (filtro == null || (filtro.Id == null && string.IsNullOrEmpty(filtro.Nome) && string.IsNullOrEmpty(filtro.Cor)))
            {
                return BadRequest("Pelo menos um critério de pesquisa (Id, Nome ou Cor) deve ser fornecido.");
            }

            IQueryable<Brinquedo> query = _context.Brinquedos;

            if (filtro.Id ==-1 || filtro.Nome=="*" || filtro.Cor=="*")
            {
                return Ok(query.ToList());
            }

            // 1) Priorizar o filtro por ID
            if (filtro.Id > 0)
            {
                var brinquedoEncontrado = _context.Brinquedos.FirstOrDefault(b => b.Id == filtro.Id);
                if (brinquedoEncontrado != null)
                {
                    return Ok(new List<Brinquedo> { brinquedoEncontrado });
                }
                else
                {
                    return NotFound($"Brinquedo com ID {filtro.Id} não encontrado.");
                }
            }

            // 2) Aplicar filtros de Nome e Cor com cláusula OR
            if (!string.IsNullOrEmpty(filtro.Nome) || !string.IsNullOrEmpty(filtro.Cor))
            {
                query = query.Where(b =>
                    (!string.IsNullOrEmpty(filtro.Nome) && EF.Functions.Like(b.Nome, $"%{filtro.Nome}%")) ||
                    (!string.IsNullOrEmpty(filtro.Cor)  && EF.Functions.Like(b.Cor,  $"%{filtro.Cor }%"))
                );
            }

            var brinquedosEncontrados = query.ToList();

            if (brinquedosEncontrados.Count != 0)
            {
                return Ok(brinquedosEncontrados);
            }
            else
            {
                return NotFound("Nenhum brinquedo encontrado com os critérios especificados.");
            }
        }

        /*
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {


            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        */
    }
}
