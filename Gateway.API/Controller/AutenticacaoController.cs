using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Gateway.API.Controller;

public class AutenticacaoController : ControllerBase
{
	private readonly IConfiguration _configuracao;

	public AutenticacaoController(IConfiguration configuracao)
	{
		_configuracao = configuracao;
	}

	[HttpPost("login")]
	public IActionResult Login([FromBody] Credenciais credenciais)
	{
		if (credenciais.Usuario != "admin" || credenciais.Senha != "fiap")
			return Unauthorized();

		var chave = _configuracao["Jwt:Chave"];
		var emissor = _configuracao["Jwt:Emissor"];
		var audiencia = _configuracao["Jwt:Audiencia"];

		var chaveSegura = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave));
		var credenciaisAssinatura = new SigningCredentials(chaveSegura, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: emissor,
			audience: audiencia,
			claims: new[] { new Claim(ClaimTypes.Name, credenciais.Usuario) },
			expires: DateTime.UtcNow.AddHours(1),
			signingCredentials: credenciaisAssinatura
		);

		var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
		return Ok(new { token = tokenString });
	}
}

public class Credenciais
{
	public string Usuario { get; set; }
	public string Senha { get; set; }
}
