using Core.Entity;
using Core.Input;
using Core.Repository;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.RegiaoDTO;

namespace DesafioFiapApi.Controllers
{
    #region Rota
    //------------------------------------------------------------------------------------
    [ApiController]
    [Route("/[controller]")]
    //------------------------------------------------------------------------------------
    #endregion
    public class RegiaoController : ControllerBase
    {
        #region Interface
        //------------------------------------------------------------------------------------
        private readonly IRegiaoRepository _regiaoRepository;
        //------------------------------------------------------------------------------------
        #endregion

        #region Contrutor
        public RegiaoController(IRegiaoRepository regiaoRepository)
        {
            _regiaoRepository = regiaoRepository;
            
        }
        #endregion

        #region ListarTodosDDD
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Lista todos os DDDs e suas respectivas cidades cadastradas.
        /// </summary>
        /// <returns>Retorna uma lista com todos os DDDs e suas cidades.</returns>
        /// <response code="200">Lista de DDDs retornada com sucesso.</response>
        /// <response code="400">Erro ao listar DDDs. Retorna a mensagem de erro detalhada.</response>
        [HttpGet("ListarDDD")]
        public IActionResult Get()
        {
            try
            {
                var contatos = _regiaoRepository.ObterTodos();

                var Contato = contatos.Select(C => new RegiaoDto
                {
                    
                    DDD = C.DDD,
                    Cidade = C.Cidade
                   

                }).ToList();

                return Ok(Contato);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //------------------------------------------------------------------------------------
        #endregion

        #region CadastrarDDD
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Cadastra um novo DDD e sua cidade correspondente no sistema.
        /// </summary>
        /// <param name="input">Objeto que contém as informações do DDD e da cidade a serem cadastrados.</param>
        /// <returns>Retorna 200 (OK) se o cadastro for bem-sucedido, ou 400 (Bad Request) se o DDD já estiver cadastrado ou ocorrer algum erro.</returns>
        /// <response code="200">DDD cadastrado com sucesso.</response>
        /// <response code="400">O DDD informado já está cadastrado ou ocorreu um erro durante o cadastro. Retorna a mensagem de erro detalhada.</response>
        [HttpPost("CadastrarDDD")]
        public IActionResult Post([FromBody] RegiaoInput input)
        {
            try
            {
                var regiaoExistente = _regiaoRepository.ObterPorDDD(input.DDD);

                if (regiaoExistente != null)
                {
                    
                    return BadRequest("O DDD informado já está cadastrado.");
                }
                var contato = new Regiao()
                {
                    DDD = input.DDD,
                    DataCadastro = DateTime.Now,
                    Cidade= input.Cidade,
                };

                _regiaoRepository.Cadastrar(contato);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //------------------------------------------------------------------------------------
        #endregion
    }
}
