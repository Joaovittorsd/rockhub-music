using Microsoft.AspNetCore.Components.Authorization;
using RockHub.Web.Response;
using System.Net.Http.Json;
using System.Security.Claims;

namespace RockHub.Web.Services;

public class AuthAPI(IHttpClientFactory factory) : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient = factory.CreateClient("API");

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var pessoa = new ClaimsPrincipal();
        var info = await _httpClient.GetFromJsonAsync<InfoPessoaResponse>("auth/manage/info");

        if (info is not null)
        {
            Claim[] dados =
                [
                    new Claim(ClaimTypes.Name, info.Email),
                    new Claim(ClaimTypes.Email, info.Email)
                ];
            var identity = new ClaimsIdentity(dados, "Cookies");
            pessoa = new ClaimsPrincipal(identity);
        }

        return new AuthenticationState(pessoa);
    }

    public async Task<AuthResponse> LoginAsync(string email, string senha)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/login?useCookies=true", new
        {
            Email = email,
            Password = senha
        });

        if (response.IsSuccessStatusCode)
        {
            return new AuthResponse { Sucesso = true };
        }

        return new AuthResponse { Sucesso = false, Erros = ["Login/senha inválidos"] };
    }
}
