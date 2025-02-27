using TournamentMS.Application.DTOs.Request;

namespace TournamentMS.Application.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserService> _logger;

        public UserService(HttpClient httpClient, ILogger<UserService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<UserDTO?> GetUserByIdAsync(int idUser)
        {
            try
            {
                var response = await _httpClient.GetAsync($"");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<UserDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user {idUser}: {ex.Message}");
                return null;
            }
        }
    }
}
