using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApiBrinquedos.Controllers;
using WebApiBrinquedos.Entity;
using WebApiBrinquedoTest;

namespace WebApiBrinquedosTest
{
    [TestClass]
    public class BrinquedoControllerTests
    {
        private BrinquedoController _controller;
        private TestBrinquedoDbContext _context;
        private ILogger<BrinquedoController> _logger;

        [TestInitialize]
        public void Setup()
        {
            // Configuração do contexto in-memory e do controller para cada teste
            _context = TestBrinquedoDbContext.GetInMemoryContext();
            _logger = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<BrinquedoController>(); // Cria um logger
            _controller = new BrinquedoController(_logger, _context);

            // Popular o banco de dados in-memory com alguns brinquedos para teste
            _context.Brinquedos.AddRange(
                new Brinquedo { Id = 1, Nome = "Carrinho", Cor = "Vermelho" },
                new Brinquedo { Id = 2, Nome = "Boneca", Cor = "Azul" },
                new Brinquedo { Id = 3, Nome = "Quebra-cabeça", Cor = "Verde" }
            );
            _context.SaveChanges();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Limpar o contexto após cada teste
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public void AdicionarBrinquedo_ReturnsCreatedAtAction()
        {
            // Arrange
            var novoBrinquedo = new Brinquedo { Nome = "Jogo de Xadrez", Cor = "Preto/Branco" };

            // Act
            var result = _controller.AdicionarBrinquedo(novoBrinquedo);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.AreEqual(201, createdResult?.StatusCode);
        }

        [TestMethod]
        public void ListarBrinquedos_ComIdExistente_RetornaBrinquedo1()
        {
            // Arrange
            var filtro = new BrinquedoRequest { Id = 2 };

            // Act
            ActionResult<IEnumerable<Brinquedo>> actionResult = _controller.ListarBrinquedos(filtro).Result;

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<IEnumerable<Brinquedo>>));
            var okResult = (actionResult as ActionResult<IEnumerable<Brinquedo>>).Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var result = okResult.Value as List<Brinquedo>;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Boneca", result[0].Nome);
            Assert.AreEqual("Azul", result[0].Cor);
        }

        [TestMethod]
        public void ListarBrinquedos_ComIdInexistente_RetornaNotFound()
        {
            // Arrange
            var filtro = new BrinquedoRequest { Id = 99 };

            // Act
            var result = _controller.ListarBrinquedos(filtro).Result as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public void ListarBrinquedos_ComNomeECor_RetornaListaFiltrada()
        {
            // Arrange
            var filtro = new BrinquedoRequest { Nome = "Carrinho", Cor = "Vermelho" };

            // Act
            ActionResult<IEnumerable<Brinquedo>> actionResult = _controller.ListarBrinquedos(filtro).Result;

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<IEnumerable<Brinquedo>>));
            var okResult = (actionResult as ActionResult<IEnumerable<Brinquedo>>).Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var result = okResult.Value as List<Brinquedo>;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Carrinho", result[0].Nome);
            Assert.AreEqual("Vermelho", result[0].Cor);
        }

        [TestMethod]
        public void ListarBrinquedos_SemFiltros_RetornaBadRequest()
        {
            // Arrange
            var filtro = new BrinquedoRequest();

            // Act
            var result = _controller.ListarBrinquedos(filtro).Result as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Pelo menos um critério de pesquisa (Id, Nome ou Cor) deve ser fornecido.", result.Value);
        }

        [TestMethod]
        public void ListarBrinquedos_ComFiltroAsterisco_RetornaListaCompleta()
        {
            // Arrange
            var filtro = new BrinquedoRequest { Nome = "*" };

            // Act
            ActionResult<IEnumerable<Brinquedo>> actionResult = _controller.ListarBrinquedos(filtro).Result;

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<IEnumerable<Brinquedo>>));
            var okResult = (actionResult as ActionResult<IEnumerable<Brinquedo>>).Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var result = okResult.Value as List<Brinquedo>;
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count); // Verifica se todos os brinquedos foram retornados
        }
    }
}