using RockHub.Web.Response;
using System.Net.Http.Json;

namespace RockHub.Web.Services;

public class AuthAPI(IHttpClientFactory factory)
{
    private readonly HttpClient _httpClient = factory.CreateClient("API");

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
