using Core.Entity;
using Core.Input;
using Core.Repository;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace DesafioFiapApi.Controllers
{
    #region Rota
    //------------------------------------------------------------------------------------
    [ApiController]
	[Route("/[controller]")]
    //------------------------------------------------------------------------------------
    #endregion

    public class ContatoController : ControllerBase
	{
        #region Interface
        //------------------------------------------------------------------------------------
        private readonly IContatoRepository _contatoRepository;
		private readonly IRegiaoRepository _regiaoRepository;
        //------------------------------------------------------------------------------------
        #endregion

        #region Construtor
        //------------------------------------------------------------------------------------
        public ContatoController(IContatoRepository contatoRepository, IRegiaoRepository regiaoRepository)
		{
			_contatoRepository = contatoRepository;
			_regiaoRepository = regiaoRepository;
		}
        //------------------------------------------------------------------------------------
        #endregion

        #region Listar Todos os Contatos
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Lista todos os contatos cadastrados no sistema.
        /// </summary>
        /// <returns>Retorna uma lista de contatos.</returns>
        /// <response code="200">Lista de contatos retornada com sucesso.</response>
        /// <response code="400">Erro ao listar contatos. Retorna a mensagem de erro detalhada.</response>
        [HttpGet("ListarContatos")]
		public IActionResult Get()
		{
			try
			{
				var contatos = _contatoRepository.ObterTodos();

				var Contato = contatos.Select(C => new ContatoDTO
				{
					NomeContato = C.NomeContato,
					DDD = C.DDD,
					Telefone = C.Telefone,
					Email = C.Email,
					DataCadastro = C.DataCadastro,

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

        #region Listar Contatos Por Telefone
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Busca um contato específico pelo número de telefone.
        /// </summary>
        /// <param name="Telefone">Número de telefone do contato.</param>
        /// <returns>Retorna o contato correspondente ao telefone informado.</returns>
        /// <response code="200">Contato encontrado e retornado com sucesso.</response>
        /// <response code="400">Erro ao buscar contato. Retorna a mensagem de erro detalhada.</response>
        [HttpGet("ListarContatoPorTelefone/{Telefone}")]
		public IActionResult Get(string Telefone)
		{
			try
			{
				return Ok(_contatoRepository.ObterPorTelefone(Telefone));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
        //------------------------------------------------------------------------------------
        #endregion

        #region Listar Contatos por DDD
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Lista todos os contatos de uma determinada região, filtrados pelo DDD.
        /// </summary>
        /// <param name="ddd">Código de DDD da região.</param>
        /// <returns>Retorna uma lista de contatos do DDD especificado.</returns>
        /// <response code="200">Contatos retornados com sucesso.</response>
        /// <response code="404">Nenhum contato encontrado para o DDD informado.</response>
        /// <response code="400">Erro ao listar contatos por DDD. Retorna a mensagem de erro detalhada.</response>
        [HttpGet("ListarContatosPorDDD/{ddd}")]

        public IActionResult ListarContatosPorDDD(int ddd)
        {
            try
            {
                var contatos = _contatoRepository.ObterPorDDD(ddd);

                if (!contatos.Any())
                    return NotFound("Nenhum contato encontrado para o DDD informado");

                return Ok(contatos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //------------------------------------------------------------------------------------
        #endregion

        #region Cadastrar Contato
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Cadastra um novo contato no sistema.
        /// </summary>
        /// <param name="input">Objeto que contém as informações do contato a ser cadastrado.</param>
        /// <returns>Retorna 200 (OK) se o cadastro for bem-sucedido, ou 400 (Bad Request) se ocorrer algum erro.</returns>
        /// <response code="200">Cadastro realizado com sucesso.</response>
        /// <response code="400">Erro no cadastro. Retorna a mensagem de erro detalhada.</response>
        /// //------------------------------------------------------------------------------------
        [HttpPost("CadastrarContato")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public  IActionResult Post([FromBody] ContatoInput input)
		{
			try
			{

				var regiao = _regiaoRepository.ObterPorDDD(input.DDD);

				if (regiao == null)
					return BadRequest("DDD não Existente");

                var contato = new Contato()
				{
                    Telefone = input.Telefone,
                    Email = input.Email,
					DataCadastro = DateTime.Now,
                    NomeContato = input.NomeContato,
					DDD = input.DDD,
					IdRegiao = regiao.IdRegiao

				};

				_contatoRepository.Cadastrar(contato);
				return Ok();
			}catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
        }
        //------------------------------------------------------------------------------------
        #endregion

        #region AlterarContato
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Atualiza as informações de um contato existente. A atualização é realizada com base no nome do contato,
        /// que serve como identificador principal. A partir do nome, você pode alterar as demais informações do contato,
        /// como DDD, telefone e e-mail.
        /// </summary>
        /// <param name="input">Objeto contendo as novas informações do contato a serem atualizadas.</param>
        /// <returns>Retorna 200 (OK) se a atualização for bem-sucedida, ou 404 (Not Found) se o nome do contato não for encontrado.</returns>
        /// <response code="200">Contato atualizado com sucesso.</response>
        /// <response code="404">Contato com o nome fornecido não encontrado.</response>
        /// <response code="400">Erro ao atualizar o contato. Retorna a mensagem de erro detalhada.</response>

        [HttpPut("AlterarContato")]
        public IActionResult Put([FromBody] ContatoUpdateInput input)
        {
            try
            {
                var contato = _contatoRepository.ObterPorNome(input.NomeContato);
                if (contato == null)
                {
                    return NotFound("Nome não encontrado");
                }

                contato.NomeContato = input.NomeContato;
                contato.DDD = input.DDD;
                contato.Telefone = input.Telefone;
                contato.Email = input.Email;

                _contatoRepository.Alterar(contato);
                return Ok("Contato atualizado com sucesso");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //------------------------------------------------------------------------------------
        #endregion

        #region DeletarPorNome
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Exclui um contato específico com base no nome.
        /// </summary>
        /// <param name="nome">Nome do contato a ser excluído.</param>
        /// <returns>Retorna 200 (OK) se o contato for excluído com sucesso, ou 400 (Bad Request) se ocorrer algum erro.</returns>
        /// <response code="200">Contato excluído com sucesso.</response>
        /// <response code="400">Erro ao excluir o contato. Retorna a mensagem de erro detalhada.</response>
        [HttpDelete("{nome}")]
        public async Task<IActionResult> Delete([FromRoute] string nome)
        {
            try
            {
                await _contatoRepository.DeletarPorNome(nome);
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
